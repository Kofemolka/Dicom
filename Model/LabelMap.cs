using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Model.Annotations;
using Model.Scanners;

namespace Model
{
    public delegate void LabelDataChangedEvent();
    public delegate void LabelPropertiesChangedEvent();

    public enum BuildMethod
    {
        RayCasting,
        EdgeTracing,
        Threshold
    }

    public class LabelLayerCrop
    {
        public int XL { get; set; }
        public int XR { get; set; }
        public int YL { get; set; }
        public int YR { get; set; }

        public bool InCrop(Point2D point)
        {
            return point.X >= XL && point.X <= XR &&
                   point.Y >= YL && point.Y <= YR;
        }
    }

    public interface ILabelMap : INotifyPropertyChanged
    {
        int Id { get; }
        bool Visible { get; set; }
        string Name { get; set; }
        int Volume { get; set; }
        System.Drawing.Color Color { get; set; }
        bool Transparent { get; set; }
       
        IScannerProperties ScannerProperties { get; set; }

        ICropBox Crop { get; set; }
        bool CropVisible { get; set; }
    
        void Add(Point3D point);
        void Add(IEnumerable<Point3D> points);
        void AddCenter(Point3D point);
                
        void Reset();

        IEnumerable<Point3D> GetProjection(Axis axis, int index);
        IEnumerable<Point3D> GetCenters();
        IEnumerable<Point3D> GetAll();

        LabelLayerCrop GetLayerCrop(int zindex);

        void FireUpdate();

        event LabelDataChangedEvent LabelDataChanged;
        event LabelPropertiesChangedEvent LabelPropertiesChanged;

#if DEBUG
        void AddDebugPoint(Point3D point);
        IEnumerable<Point3D> GetDebugProjection(Axis axis, int index);
#endif
    }

    public class LabelMap : ILabelMap
    {
        private readonly List<Point3D> _marks = new List<Point3D>();
        private readonly List<Point3D> _centers = new List<Point3D>();
        private readonly ConcurrentDictionary<int, LabelLayerCrop> _layerCrops = new ConcurrentDictionary<int, LabelLayerCrop>(); 

        private readonly int _id;
        private static int _idGenerator = 0;

        public LabelMap()
        {
            Interlocked.Increment(ref _idGenerator);
            _id = _idGenerator;
        }

        public int Id => _id;

        public IScannerProperties ScannerProperties { get; set; } = new VoidScanner.VoidScannerProperties();

        public ICropBox Crop { get; set; }

        public bool CropVisible
        {
            get { return Crop.Visible; }
            set { Crop.Visible = value; }
        }

        public BuildMethod BuildMethod { get; set; }

        private bool _visible = true;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                LabelPropertiesChanged?.Invoke();
                OnPropertyChanged(nameof(Visible));
            }
        }

        public string Name { get; set; }

        private int _volume = 0;
        public int Volume
        {
            get { return _volume; }
            set { _volume = value;
                OnPropertyChanged(nameof(Volume));
            }
        }

        private System.Drawing.Color _color;

        public System.Drawing.Color Color
        {
            get
            {
                return System.Drawing.Color.FromArgb(Transparent ? 125 : 255, _color.R, _color.G, _color.B);
            }

            set
            {
                _color = value;
                LabelPropertiesChanged?.Invoke();
                OnPropertyChanged(nameof(Color));
            }
        }

        private bool _transparent = false;
        public bool Transparent
        {
            get { return _transparent; }
            set
            {
                _transparent = value;
                LabelPropertiesChanged?.Invoke();
                OnPropertyChanged(nameof(Transparent));
            }
        }

        public void FireUpdate()
        {
            Parallel.ForEach(_layerCrops.Keys, CalculateLayerCrop);

            LabelDataChanged?.Invoke();
        }

        public void Add(Point3D point)
        {
            lock (_marks)
            {
                _marks.Add(point);
            }

            _layerCrops.Keys.Add(point.Z);
        }

        public void Add(IEnumerable<Point3D> points)
        {
            lock (_marks)
            {
                _marks.AddRange(points);
            }

            if (points.Any())
            {
                _layerCrops[points.First().Z] = new LabelLayerCrop();
            }
        }

        public void AddCenter(Point3D point)
        {
            lock (_centers)
            {
                _centers.Add(point);
            }
        }

        public IEnumerable<Point3D> GetProjection(Axis axis, int index)
        {
            lock (_marks)
            {
                foreach (var mark in _marks)
                {
                    switch (axis)
                    {
                        case Axis.X:
                            if (mark.X == index)
                            {
                                yield return mark;
                            }
                            break;

                        case Axis.Y:
                            if (mark.Y == index)
                            {
                                yield return mark;
                            }
                            break;

                        case Axis.Z:
                            if (mark.Z == index)
                            {
                                yield return mark;
                            }
                            break;
                    }
                }
            }
        }

        public IEnumerable<Point3D> GetAll()
        {
            return _marks;
        }

        public IEnumerable<Point3D> GetCenters()
        {
            return _centers;
        }

        public LabelLayerCrop GetLayerCrop(int zindex)
        {
            LabelLayerCrop crop;

            if (_layerCrops.TryGetValue(zindex, out crop)) return crop;

            crop = new LabelLayerCrop();
            _layerCrops[zindex] = crop;

            return crop;
        }

        public void Reset()
        {
            _marks.Clear();
            _centers.Clear();
            _layerCrops.Clear();

#if DEBUG
            _debugs.Clear();
#endif
            //FireUpdate();
        }

        public event LabelDataChangedEvent LabelDataChanged;
        public event LabelPropertiesChangedEvent LabelPropertiesChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CalculateLayerCrop(int zindex)
        {
            var crop = GetLayerCrop(zindex);

            var proj = GetProjection(Axis.Z, zindex);

            crop.XL = Int32.MaxValue;
            crop.YL = Int32.MaxValue;
            crop.XR = Int32.MinValue;
            crop.YR = Int32.MinValue;

            foreach (var point3D in proj)
            {
                if (point3D.X > crop.XR)
                {
                    crop.XR = point3D.X;
                }

                if (point3D.X < crop.XL)
                {
                    crop.XL = point3D.X;
                }

                if (point3D.Y > crop.YR)
                {
                    crop.YR = point3D.Y;
                }

                if (point3D.Y < crop.YL)
                {
                    crop.YL = point3D.Y;
                }
            }
        }

#if DEBUG
        private readonly List<Point3D> _debugs = new List<Point3D>();

       

        public void AddDebugPoint(Point3D point)
        {
            lock (_debugs)
            {
                _debugs.Add(point);
            }
        }

        public IEnumerable<Point3D> GetDebugProjection(Axis axis, int index)
        {
            foreach (var mark in _debugs)
            {
                switch (axis)
                {
                    case Axis.X:
                        if (mark.X == index)
                        {
                            yield return mark;
                        }
                        break;

                    case Axis.Y:
                        if (mark.Y == index)
                        {
                            yield return mark;
                        }
                        break;

                    case Axis.Z:
                        if (mark.Z == index)
                        {
                            yield return mark;
                        }
                        break;
                }
            }
        }
#endif
    }
}

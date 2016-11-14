﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Model.Annotations;

namespace Model
{
    public delegate void LabelDataChangedEvent();
    public delegate void LabelPropertiesChangedEvent();

    public enum BuildMethod
    {
        RayCasting,
        Threshold
    }

    public interface ILabelMap : INotifyPropertyChanged
    {
        bool Visible { get; set; }
        string Name { get; set; }
        int Volume { get; set; }
        System.Drawing.Color Color { get; set; }
        bool Transparent { get; set; }
        BuildMethod BuildMethod { get; set; }

        ICropBox Crop { get; set; }
        bool CropVisible { get; set; }
    
#if DEBUG
        void AddDebugPoint(Point3D point);
        IEnumerable<Point3D> GetDebugProjection(Axis axis, int index);
#endif
        void Add(Point3D point);
        void Add(IEnumerable<Point3D> points);
        void AddCenter(Point3D point);
                
        void Reset();

        IEnumerable<Point3D> GetProjection(Axis axis, int index);
        IEnumerable<Point3D> GetCenters();
        IEnumerable<Point3D> GetAll();

        void FireUpdate();

        event LabelDataChangedEvent LabelDataChanged;
        event LabelPropertiesChangedEvent LabelPropertiesChanged;
    }

    public class LabelMap : ILabelMap
    {
        private readonly List<Point3D> _marks = new List<Point3D>();
        private readonly List<Point3D> _centers = new List<Point3D>();

#if DEBUG
        private readonly List<Point3D> _debugs = new List<Point3D>();

        public ICropBox Crop { get; set; }

        public bool CropVisible
        {
            get { return Crop.Visible; }
            set { Crop.Visible = value; }
        }

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
            LabelDataChanged?.Invoke();
        }

        public void Add(Point3D point)
        {
            lock (_marks)
            {
                _marks.Add(point);
            }
        }

        public void Add(IEnumerable<Point3D> points)
        {
            lock (_marks)
            {
                _marks.AddRange(points);
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

        public IEnumerable<Point3D> GetAll()
        {
            foreach (var mark in _marks)
            {
                yield return mark;
            }
        }

        public IEnumerable<Point3D> GetCenters()
        {
            return _centers;
        }

        public void Reset()
        {
            _marks.Clear();
            _centers.Clear();

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
    }
}

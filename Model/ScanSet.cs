using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model.Utils;

namespace Model
{
    public delegate void DataUpdatedEvent();
    public interface IScanData
    {
        List<string> DicomInfo { get; }
        Projection GetProjection(Axis axis, int index);
        int GetAxisCutCount(Axis axis);
        void MinMaxDencity(out int min, out int max);
        void Resolution(out double X, out double Y, out double Z);

        event DataUpdatedEvent DataUpdated;
    }

    public class ScanSet : IScanData
    {
        private readonly List<string> _dicomInfo = new List<string>();
        private readonly List<Slice> _slices = new List<Slice>();
        private ushort _minDens;
        private ushort _maxDens;

        public ScanSet()
        {
             
        }

        public List<string> DicomInfo => new List<string>(_dicomInfo);


        public void AddSlice(Slice slice)
        {
            _slices.Add(slice);
        }

        public void Reset()
        {
            _slices.Clear();
        }

        public void MinMaxDencity(out int min, out int max)
        {
            min = _minDens;
            max = _maxDens;
        }

        public void Resolution(out double X, out double Y, out double Z)
        {
            X = XRes;
            Y = YRes;
            Z = ZRes;
        }

        public Projection GetProjection(Axis axis, int index)
        {
            switch (axis)
            {
                case Axis.X:
                    return getXProjection(index);
                    
                case Axis.Y:
                    return getYProjection(index);

                case Axis.Z:
                    return getZProjection(index);

                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        public int GetAxisCutCount(Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    return XSize;
                    
                case Axis.Y:
                    return YSize;
                    
                case Axis.Z:
                    return ZSize;
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        public event DataUpdatedEvent DataUpdated;

        private Projection getXProjection(int index)
        {
            if (index < 0 || index >= XSize)
                return new Projection(Axis.X);

            var pixels = new ushort[YSize, ZSize];

            for (int y = 0; y < YSize; y++)                
            {
                for (int z = 0; z < ZSize; z++)
                    pixels[y, z] = Pixels[z, index, y];
            }

            return new Projection(Axis.X, YSize, ZSize, pixels);
        }

        private Projection getYProjection(int index)
        {
            if (index < 0 || index >= YSize)
                return new Projection(Axis.Y);

            var pixels = new ushort[XSize, ZSize];

            for (int x = 0; x < XSize; x++)                
            {
                for (int z = 0; z < ZSize; z++)
                    pixels[x, z] = Pixels[z, x, index];
            }

            return new Projection(Axis.Y, YSize, ZSize, pixels);
        }

        private Projection getZProjection(int index)
        {
            if (index < 0 || index >= ZSize)
                return new Projection(Axis.Z);

            var pixels = new ushort[XSize, YSize];

            for (int x = 0; x < XSize; x++)
            {
                for (int y = 0; y < YSize; y++)
                    pixels[x, y] = Pixels[index, x, y];
            }

            return new Projection(Axis.Y, XSize, YSize, pixels);
        }

        public void Build(IProgress progress)
        {
            if (!_slices.Any())
                return;

            initCommonProperties(_slices.First());

            XSize = _slices.First().width;
            YSize = _slices.First().height;
            ZSize = _slices.Count;

            Pixels = new ushort[_slices.Count, XSize, YSize];

            progress.Min(1);
            progress.Max(ZSize);
            progress.Reset();

            _minDens = ushort.MaxValue;
            _maxDens = ushort.MinValue;

            var minDenses = new ConcurrentBag<ushort>();
            var maxDenses = new ConcurrentBag<ushort>();

            Parallel.ForEach(_slices, (slice, state, z) =>
            {
                var minDens = ushort.MaxValue;
                var maxDens = ushort.MinValue;

                for (int x = 0; x < XSize; x++)
                {
                    for (int y = 0; y < YSize; y++)
                    {
                        Pixels[z, x, y] = _slices[(int) z].pixels.ElementAt(x*XSize + y);

                        if (Pixels[z, x, y] > maxDens)
                        {
                            maxDens = Pixels[z, x, y];
                        }

                        if (Pixels[z, x, y] < minDens)
                        {
                            minDens = Pixels[z, x, y];
                        }
                    }
                }

                minDenses.Add(minDens);
                maxDenses.Add(maxDens);

                progress.Tick();
            });
            
            _minDens = minDenses.Min();
            _maxDens = maxDenses.Max();
            
            _dicomInfo.Clear();
            _dicomInfo.AddRange(_slices.First().DicomInfo);

            _slices.Clear();

            Task.Factory.StartNew(GC.Collect);

            DataUpdated?.Invoke();
        }

        private void initCommonProperties(Slice slice)
        {
            typeOfDicomFile = slice.typeofDicomFile;
            SamplesPerPixel = slice.SamplesPerPixel;
            bitsAllocated = slice.bitsAllocated;
            windowCentre = slice.windowCentre;
            windowWidth = slice.windowWidth;
            signedImage = slice.signedImage;

            XRes = slice.XRes;
            YRes = slice.YRes;
            ZRes = slice.ZRes;
        }

        private ushort[,,] Pixels;

        public int XSize { get; private set; }
        public int YSize { get; private set; }
        public int ZSize { get; private set; }

        private double XRes { get; set; }
        private double YRes { get; set; }
        private double ZRes { get; set; }

        public TypeOfDicomFile typeOfDicomFile { get; private set; }
        public int SamplesPerPixel { get; private set; }
        public int bitsAllocated { get; private set; }
        public double windowCentre { get; private set; }
        public double windowWidth { get; private set; }
        public bool signedImage { get; private set; }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Model.Utils;

namespace Model.Scanners
{
    public class ThresholdScanner
    {
        public class ThresholdScannerProperties : IScannerProperties
        {
            public Point3D LastScanPoint { get; set; }
            public BuildMethod BuildMethod => BuildMethod.Threshold;

            public ushort Threshold { get; set; } = 2;
        }

        private readonly IScanData _scanData;
        private readonly Func<ILabelMap> _labelMap;
        private readonly ILookupTable _lookupTable;
        
        public ThresholdScannerProperties ScannerProperties { get; set; } = new ThresholdScannerProperties();

        public ThresholdScanner(IScanData scanData, ILookupTable lookupTable, Func<ILabelMap> labelMap)
        {
            _scanData = scanData;
            _labelMap = labelMap;
            _lookupTable = lookupTable;
        }

        public void Build(Point3D point, IProgress progress)
        {
            _labelMap().Reset();
            _labelMap().ScannerProperties = new ThresholdScannerProperties()
            {
                LastScanPoint = ScannerProperties.LastScanPoint,
                Threshold = ScannerProperties.Threshold
            };

            var crop = _labelMap().Crop;

            progress.Min(1);
            progress.Max(crop.ZR - crop.ZL);
            progress.Reset();

            var fixProbe = Probe.GetStartingProbe(point, _scanData, _lookupTable, Probe.Method.Average, ScannerProperties.Threshold, ScannerProperties.Threshold);
            
            int volume = 0;
            const int MaxPoints = 10*1000*1000;
            
            Parallel.ForEach(_scanData.GetAllProjections(Axis.Z), (proj, state, z) =>
            {

                if (z < crop.ZL || z > crop.ZR)
                    return;

                for (int x = crop.XL; x < crop.XR; x++)
                {
                    for (int y = crop.YL; y < crop.YR; y++)
                    {
                        if (fixProbe.InRange(_lookupTable.Map(proj.Pixels[x, y])))
                        {
                            _labelMap().Add(new Point3D(x, y, (int)z));
                            Interlocked.Increment(ref volume);

                            if (volume > MaxPoints)
                            {
                                state.Stop();
                                return;
                            }
                        }
                    }
                }

                progress.Tick();
            });

            double xres, yres, zres;
            _scanData.Resolution(out xres, out yres, out zres);

            _labelMap().Volume = (int)(volume * xres * yres * zres);

            _labelMap().FireUpdate();

            Task.Factory.StartNew(GC.Collect);
        }
    }
}

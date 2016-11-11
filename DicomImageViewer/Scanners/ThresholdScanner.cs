using System;
using System.Threading.Tasks;
using Model;
using Model.Utils;
using System.Threading;

namespace DicomImageViewer.Scanners
{
    public class ThresholdScanner
    {
        private readonly IScanData _scanData;
        private readonly Func<ILabelMap> _labelMap;
        private readonly ILookupTable _lookupTable;

        public ushort Threshold { get; set; } = 2;

        public ThresholdScanner(IScanData scanData, ILookupTable lookupTable, Func<ILabelMap> labelMap)
        {
            _scanData = scanData;
            _labelMap = labelMap;
            _lookupTable = lookupTable;
        }

        public void Build(Point3D point, IProgress progress)
        {
            _labelMap().Reset();
            _labelMap().BuildMethod = BuildMethod.Threshold;

            progress.Min(1);
            progress.Max(_scanData.GetAxisCutCount(Axis.Z));
            progress.Reset();

            var fixProbe = Probe.GetStartingProbe(point, _scanData, _lookupTable, Probe.Method.Average, Threshold, Threshold);
            
            int volume = 0;
            const int MaxPoints = 10*1000*1000;
            
            Parallel.ForEach(_scanData.GetAllProjections(Axis.Z), (proj, state, z) =>
            {
                for (int x = 0; x < proj.Width; x++)
                {
                    for (int y = 0; y < proj.Height; y++)
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

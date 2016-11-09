using Model;
using Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomImageViewer.Scanners
{
    public class EdgeFinder
    {
        private readonly IScanData _scanData;
        private readonly Func<ILabelMap> _labelMap;
        private readonly ILookupTable _lookupTable;

        public ushort Threshold { get; set; } = 2;

        public EdgeFinder(IScanData scanData, ILookupTable lookupTable, Func<ILabelMap> labelMap)
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

            var fixProbe = Probe.GetStartingProbe(point, _scanData, _lookupTable, Threshold, Threshold);
        }
    }
}

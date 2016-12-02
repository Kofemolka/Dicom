using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model.Utils;

namespace Model.Scanners
{
    public class EdgeScanner
    {
        public class EdgeScannerProperties : BaseScannerProperties
        {
            public override BuildMethod BuildMethod => BuildMethod.EdgeTracing;

            public ushort Threshold { get; set; } = 2;
        }

        private readonly IScanData _scanData;
        private readonly Func<ILabelMap> _labelMap;
        private readonly ILookupTable _lookupTable;
        private readonly ICrossChecker _crossCheck;

        public EdgeScannerProperties ScannerProperties { get; set; } = new EdgeScannerProperties();

        public EdgeScanner(IScanData scanData, ILookupTable lookupTable, Func<ILabelMap> labelMap, ICrossChecker crossCheck)
        {
            _scanData = scanData;
            _labelMap = labelMap;
            _lookupTable = lookupTable;
            _crossCheck = crossCheck;
        }

        public void Build(Point3D point, IProgress progress)
        {
            _labelMap().Reset();
            _labelMap().ScannerProperties = new EdgeScannerProperties()
            {
                LastScanPoint = ScannerProperties.LastScanPoint,
                Threshold = ScannerProperties.Threshold
            };

            _crossCheck.Prepare(_labelMap().Id);

            var crop = _labelMap().Crop;

            progress.Min(1);
            progress.Max(crop.ZR - crop.ZL);
            progress.Reset();

            var fixProbe = Probe.GetStartingProbe(point, _scanData, _lookupTable, Probe.Method.MinMax, ScannerProperties.Threshold, ScannerProperties.Threshold);

            var tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(() =>
            {
                Point3D center = point;

                //go up
                for (var h = point[Axis.Z]; h < crop.ZR; h++)
                {
                    var p = new Point3D(center)
                    {
                        [Axis.Z] = h
                    };

                    center = ScanProjection(p, fixProbe);

                    progress.Tick();
                }
            }));


            tasks.Add(Task.Factory.StartNew(() =>
            {
                Point3D center = point;

                ////go down
                for (var h = point[Axis.Z] - 1; h >= crop.ZL; h--)
                {
                    var p = new Point3D(center)
                    {
                        [Axis.Z] = h
                    };

                    center = ScanProjection(p, fixProbe);

                    progress.Tick();
                }
            }));

            Task.WaitAll(tasks.ToArray());

            Helpers.CalculateVolume(_labelMap(), _scanData);

            _labelMap().FireUpdate();
        }

        private Point3D ScanProjection(Point3D point, Probe fixProbe)
        {
            var projection = _scanData.GetProjection(Axis.Z, point[Axis.Z]);
            if (projection.Empty)
                return point;

            var scalarPoint = point.To2D(Axis.Z);

            var crop = _labelMap().Crop;

            for (var x = scalarPoint.X; x < crop.XR; x++)
            {
                scalarPoint.X = x;

                var probe = _lookupTable.Map(projection.Pixels[scalarPoint.X, scalarPoint.Y]);

                if (!fixProbe.InRange(probe) || _crossCheck.Check(scalarPoint.To3D(Axis.Z, point.Z), _labelMap().Id))
                {
                    break;
                }
            }

            var layer = Scan(scalarPoint, projection, fixProbe, point[Axis.Z]);
            var point3Ds = layer as Point3D[] ?? layer.ToArray();
            if (point3Ds.Any())
            {
                var center = Helpers.CalculateLayerCenter(point3Ds.ToList());

                _labelMap().Add(point3Ds);

                _labelMap().AddCenter(center);

                return center;
            }

            return point;
        }

        private enum Direction : int
        {
            W = 0,
            NW,
            N,
            NE,
            E,
            SE,
            S,
            SW
        }

        private IEnumerable<Point3D> Scan(Point2D start, Projection proj, Probe fixProbe, int Z)
        {
            var res = new List<Point3D>();
            const int maxPath = 10000;

            Point2D current = start;            
                        
            Direction dir = Direction.E;
            dir = TurnLeft(dir);     

            Point2D test;

            int path = 0;
            do
            {                
                bool edge;
                test = GetNextNeighbor(current, dir, Z, out edge);                

                if(edge || !SafeCheckProbe(fixProbe, proj, test) || _crossCheck.Check(test.To3D(Axis.Z, Z), _labelMap().Id))
                {
                    res.Add(test.To3D(Axis.Z, Z));                    
                    dir = TurnLeft(dir);
                }
                else
                {
                    dir = TurnRight(dir);
                }

                current = test;
            } while (!test.Equals(start) && (path++ < maxPath)); //until got back to start or max path reached

            return res;
        }

        private static Direction TurnLeft(Direction dir)
        {
            return (Direction)(((int)dir + 6) % 8);
        }

        private static Direction TurnRight(Direction dir)
        {
            return (Direction)(((int)dir + 2) % 8);
        }

        private Point2D GetNextNeighbor(Point2D p, Direction dir, int Z, out bool edge)
        {
            edge = false;
            
            var np = new Point2D(p.X + _N[(int)dir].X, p.Y + _N[(int)dir].Y);
            var crop = _labelMap().Crop;

            if (np.X > (crop.XR - 2) ||
               np.X < (crop.XL + 1) ||
               np.Y > (crop.YR - 2) ||
               np.Y < (crop.YL + 1))
            {
                edge = true;
            }

            return np;
        }
        
        private bool SafeCheckProbe(Probe probe, Projection proj, Point2D p)
        {
            if (p.X < 0 || p.X >= proj.Width ||
                p.Y < 0 || p.Y >= proj.Height)
            {
                return false;
            }

            return probe.InRange(_lookupTable.Map(proj.Pixels[p.X, p.Y]));
        }

        private readonly Point2D[] _N = new Point2D[8]
        {
            new Model.Point2D(-1, 0),
            new Model.Point2D(-1, -1),
            new Model.Point2D(0, -1),
            new Model.Point2D(1, -1),
            new Model.Point2D(1, 0),
            new Model.Point2D(1, 1),
            new Model.Point2D(0, 1),
            new Model.Point2D(-1, 1)
        };
    }
}

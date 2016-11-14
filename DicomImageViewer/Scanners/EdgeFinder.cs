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

            var fixProbe = Probe.GetStartingProbe(point, _scanData, _lookupTable, Probe.Method.MinMax, Threshold, Threshold);

            var tasks = new List<Task>();
            //tasks.Add(Task.Factory.StartNew(() =>
            //{
            //    Point3D center = point;

            //    //go up
            //    for (var h = point[Axis.Z]; h < _scanData.GetAxisCutCount(Axis.Z); h++)
            //    {
            //        var p = new Point3D(center)
            //        {
            //            [Axis.Z] = h
            //        };

            //        center = ScanProjection(p, fixProbe);

            //        progress.Tick();
            //    }
            //}));


            tasks.Add(Task.Factory.StartNew(() =>
            {
                Point3D center = point;

                ////go down
                for (var h = point[Axis.Z] - 1; h >= 0; h--)
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

                if (!fixProbe.InRange(probe))
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

                _labelMap().AddCenter(point);

                return center;
            }

            return point;
        }

        private class Neighbor
        {
            public Point2D point { get; set; }
            public bool Edge { get; set; } = false;
        }

        private IEnumerable<Point3D> Scan(Point2D start, Projection proj, Probe fixProbe, int Z)
        {
            var res = new List<Point3D>();
            const int maxPath = 10000;

            Point2D current = start;
            Point2D prev = new Point2D(current.X - 1, current.Y);            
            
            int nN = FindNeighborhood(current, prev, proj); // start from next CW neighbor          
            int nNStart = nN;

            Point2D test = new Point2D(current.X + _N[nN].point.X, current.Y + _N[nN].point.Y); //init first test point

            int path = 0;
            do
            {
                do
                {
                    test = new Point2D(current.X + _N[nN].point.X, current.Y + _N[nN].point.Y);

                    if (_N[nN].Edge || !SafeCheckProbe(fixProbe, proj, test)) //edge found by crop or probe
                    {
                        break;
                    }
                    else
                    {
                        nN++;
                        if (nN >= _N.Length)
                        {
                            nN = 0;
                        }

                        if (nN == nNStart)
                        {
                            break;
                        }
                    }
                } while (true);

                Point3D p3d = test.To3D(Axis.Z, Z);
                Point3D p3dBounded;
                if(!_labelMap().Crop.IsInCrop(p3d, out p3dBounded))
                {
                    test = p3dBounded.To2D(Axis.Z);                    
                }

                res.Add(test.To3D(Axis.Z, Z));

                if (current.X < 0 || current.Y < 0)
                {
                    break;
                }
                prev = current;
                if (test.X < 0 || test.Y < 0)
                {
                    break;
                }
                current = test;
                nN = nNStart = FindNeighborhood(current, prev, proj);

                test = new Point2D(current.X + _N[nN].point.X, current.Y + _N[nN].point.Y);
            } while (!test.Equals(start) && (path++ < maxPath)); //until got back to start or max path reached

            return res;
        }

        private bool SafeCheckProbe(Probe probe, Projection proj, Point2D p)
        {
            if (p.X < 0 || p.X > proj.Width ||
                p.Y < 0 || p.Y > proj.Height)
            {
                return false;
            }

            return probe.InRange(_lookupTable.Map(proj.Pixels[p.X, p.Y]));
        }

        private readonly Neighbor[] _N = new Neighbor[8]
        {
            new Neighbor() { point = new Model.Point2D(-1, 0) },
            new Neighbor() { point = new Model.Point2D(-1, -1) },
            new Neighbor() { point = new Model.Point2D(0, -1) },
            new Neighbor() { point = new Model.Point2D(1, -1) },
            new Neighbor() { point = new Model.Point2D(1, 0) },
            new Neighbor() { point = new Model.Point2D(1, 1) },
            new Neighbor() { point = new Model.Point2D(0, 1) },
            new Neighbor() { point = new Model.Point2D(-1, 1) }
        };

        private int FindNeighborhood(Point2D p, Point2D start, Projection proj)
        {
            var crop = _labelMap().Crop;

            _N[0].Edge = (p.Y < (crop.YL + 2)) || (p.X + _N[0].point.X) < (crop.XL + 2);                 //W
            _N[1].Edge = _N[0].Edge || ((p.Y + _N[1].point.Y) < (crop.YL + 1)); //NW
            _N[2].Edge = (p.X < (crop.XL + 2)) || (p.Y + _N[2].point.Y) < (crop.YL + 2);                 //N
            _N[3].Edge = _N[2].Edge || ((p.X + _N[3].point.X) > (crop.YR - 1)); //NE
            _N[4].Edge = (p.X > (crop.XR - 2)) || (p.X + _N[4].point.X) > (crop.XR - 2);                 //E
            _N[5].Edge = _N[4].Edge || ((p.Y + _N[5].point.Y) > (crop.YR - 1)); //SE
            _N[6].Edge = (p.Y > (crop.YR - 2)) || (p.Y + _N[6].point.Y) > (crop.YR - 2);                 //S
            _N[7].Edge = _N[6].Edge || ((p.X + _N[7].point.X) < (crop.XL + 1)); //SW

            if( _N.Where( (n) => n.Edge).Any())
            {
                bool shit = true;
            }

            int ndx = 0;
            foreach(var n in _N)
            {
                if(start.X == (n.point.X + p.X) &&
                    start.Y == (n.point.Y + p.Y))
                {
                    return ndx == (_N.Length - 1) ? 0 : ++ndx;
                }

                ndx++;
            }

            return0;
        }           
    }
}

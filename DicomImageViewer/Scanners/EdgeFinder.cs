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

            var crop = _labelMap().Crop;

            progress.Min(1);
            progress.Max(crop.ZR - crop.ZL);
            progress.Reset();

            var fixProbe = Probe.GetStartingProbe(point, _scanData, _lookupTable, Probe.Method.MinMax, Threshold, Threshold);

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


            //tasks.Add(Task.Factory.StartNew(() =>
            //{
            //    Point3D center = point;

            //    ////go down
            //    for (var h = point[Axis.Z] - 1; h >= crop.ZL; h--)
            //    {
            //        var p = new Point3D(center)
            //        {
            //            [Axis.Z] = h
            //        };

            //        center = ScanProjection(p, fixProbe);

            //        progress.Tick();
            //    }
            //}));

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
            Point2D prev = new Point2D(current.X - 1, current.Y);

            Direction lastDirection = Direction.E; //to the right
            Direction nN = Direction.NW;
            Direction nNStart = nN;

            Point2D test;

            int path = 0;
            do
            {
                do
                {
                    bool edge;
                    test = GetNextNeighbor(current, nN, Z, out edge);
                    if(edge)
                    {
                        break;
                    }
                    else if(!SafeCheckProbe(fixProbe, proj, test)) //edge found by crop or probe)
                    {
                        break;
                    }
                    else
                    {
                        nN++;
                        if ((int)nN >= _N.Length)
                        {
                            nN = 0;
                        }

                        if (nN == nNStart)
                        {
                            break;
                        }
                    }
                } while (true);

                res.Add(test.To3D(Axis.Z, Z));

                prev = current;               
                current = test;

                Direction tmp = nN;
                nN = (Direction)(((int)lastDirection + 4 + 1) % 7);
                lastDirection = tmp;

                nNStart = nN;              
            } while (!test.Equals(start) && (path++ < maxPath)); //until got back to start or max path reached

            return res;
        }

        private Point2D GetNextNeighbor(Point2D p, Direction ndx, int Z, out bool edge)
        {
            edge = false;

            if (!_labelMap().Crop.IsInCrop(p.To3D(Axis.Z, Z)))
            {
                edge = true;
                return p;
            }

            var np = new Point2D(p.X + _N[(int)ndx].X, p.Y + _N[(int)ndx].Y);
            var crop = _labelMap().Crop;
            
            if(np.X > (crop.XR - 2) ||
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

        //private int FindNeighborhood(Point2D p, Point2D start, Projection proj)
        //{
        //    var crop = _labelMap().Crop;

        //    _N[0].Edge = (p.Y < (crop.YL + 1)) || (p.X + _N[0].point.X) < (crop.XL + 1);                 //W
        //    _N[1].Edge = _N[0].Edge || ((p.Y + _N[1].point.Y) < (crop.YL + 1)); //NW
        //    _N[2].Edge = (p.X < (crop.XL + 1)) || (p.Y + _N[2].point.Y) < (crop.YL + 1);                 //N
        //    _N[3].Edge = _N[2].Edge || ((p.X + _N[3].point.X) > (crop.YR - 1)); //NE
        //    _N[4].Edge = (p.X > (crop.XR - 1)) || (p.X + _N[4].point.X) > (crop.XR - 1);                 //E
        //    _N[5].Edge = _N[4].Edge || ((p.Y + _N[5].point.Y) > (crop.YR - 1)); //SE
        //    _N[6].Edge = (p.Y > (crop.YR - 1)) || (p.Y + _N[6].point.Y) > (crop.YR - 1);                 //S
        //    _N[7].Edge = _N[6].Edge || ((p.X + _N[7].point.X) < (crop.XL + 1)); //SW
                       
        //    int ndx = 0;
        //    foreach(var n in _N)
        //    {
        //        if(start.X == (n.point.X + p.X) &&
        //            start.Y == (n.point.Y + p.Y))
        //        {
        //            return ndx == (_N.Length - 1) ? 0 : ++ndx;
        //        }

        //        ndx++;
        //    }

        //    return 0;
        //}           
    }
}

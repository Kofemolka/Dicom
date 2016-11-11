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
            tasks.Add(Task.Factory.StartNew(() =>
            {
                Point3D center = point;

                //go up
                for (var h = point[Axis.Z]; h < _scanData.GetAxisCutCount(Axis.Z); h++)
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
            if(projection.Empty)
                return point;

            var scalarPoint = point.To2D(Axis.Z);

            for (var x = scalarPoint.X; x < projection.Width; x++)
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

        private IEnumerable<Point3D> Scan(Point2D start, Projection proj, Probe fixProbe, int Z)
        {
            var res = new List<Point3D>();
            const int maxPath = 10000;

            Point2D p = start;
            Point2D b = new Point2D(p.X-1, p.Y);
            Point2D c = new Point2D(p.X-1, p.Y-1);
            IList<Point2D> N = GetNeighborhood(p, b, proj);
            int nN = 0;

            int path = 0;
            do
            {
                if (!fixProbe.InRange(_lookupTable.Map(proj.Pixels[c.X, c.Y])))
                {
                    res.Add(c.To3D(Axis.Z, Z));
                    b = p;
                    p = c;
                    N = GetNeighborhood(p, b, proj);
                    if (N == null)
                    {
                        break;
                    }
                    nN = 0;
                    c = N[nN];
                }
                else
                {
                    nN++;
                    if (nN >= N.Count)
                    {
                        _labelMap().Add(c.To3D(Axis.Z, Z));
                        b = p;
                        p = c;
                        N = GetNeighborhood(p, b, proj);
                        if (N == null)
                        {
                            break;
                        }
                        nN = 0;
                    }

                    c = N[nN];
                }

                if (path++ > maxPath)
                {
                    break;
                }

            } while (!c.Equals(start));

            return res;
        }


        private IList<Point2D> GetNeighborhood(Point2D p, Point2D start, Projection proj)
        {
            var res = new List<Point2D>();

            if (p.X > 0)
            {
                res.Add(new Point2D(p.X - 1, p.Y)); //W

                if (p.Y > 0)
                {
                    res.Add(new Point2D(p.X - 1, p.Y - 1)); //NW
                }
            }

            if (p.Y > 0)
            {
                res.Add(new Point2D(p.X, p.Y - 1)); //N

                if (p.X < proj.Width - 1)
                {
                    res.Add(new Point2D(p.X + 1, p.Y - 1)); //NE
                }
            }


            if (p.X < proj.Width - 1)
            {
                res.Add(new Point2D(p.X + 1, p.Y)); //E

                if (p.Y < proj.Height - 1)
                {
                    res.Add(new Point2D(p.X + 1, p.Y + 1)); //SE
                }
            }

            if (p.Y < proj.Height - 1)
            {
                res.Add(new Point2D(p.X, p.Y + 1)); //S

                if (p.X > 0)
                {
                    res.Add(new Point2D(p.X-1, p.Y + 1)); //SW
                }
            }

            var ndx = res.IndexOf(start);
            if (ndx == -1)
                return null;

            var ordered = new List<Point2D>();

            if (ndx != res.Count - 1)
            {
                ordered.AddRange(res.GetRange(ndx + 1, res.Count - ndx - 1));
            }

            ordered.AddRange(res.GetRange(0, ndx+1));

            return ordered;
        }
    }
}

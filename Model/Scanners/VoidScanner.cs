using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model.Utils;

namespace Model.Scanners
{
    public class VoidScanner
    {
        public class VoidScannerProperties : BaseScannerProperties
        {            
            public override BuildMethod BuildMethod => BuildMethod.RayCasting;

            public int MaxSkip { get; set; } = 4;
            public ushort thUp { get; set; } = 5;
            public ushort thDown { get; set; } = 5;
            public int Rays { get; set; } = 180;
            public bool OptimizePlanes { get; set; } = true;
            public int Smoothness { get; set; } = 10;
        }

        private readonly IScanData _scanData;
        private readonly Func<ILabelMap> _labelMap;
        private readonly ILookupTable _lookupTable;
        private readonly ICrossChecker _crossCheck;

        public VoidScannerProperties ScannerProperties { get; set; } = new VoidScannerProperties();

        public VoidScanner(IScanData scanData, ILookupTable lookupTable, Func<ILabelMap> labelMap, ICrossChecker crossCheck)
        {
            _scanData = scanData;
            _labelMap = labelMap;
            _lookupTable = lookupTable;
            _crossCheck = crossCheck;
        }

        public void Build(Point3D point, Axis axis, IProgress progress)
        {
            _labelMap().Reset();
            _labelMap().ScannerProperties = new VoidScannerProperties()
            {
                LastScanPoint = ScannerProperties.LastScanPoint,
                MaxSkip = ScannerProperties.MaxSkip,
                OptimizePlanes = ScannerProperties.OptimizePlanes,
                Rays = ScannerProperties.Rays,
                Smoothness = ScannerProperties.Smoothness,
                thDown = ScannerProperties.thDown,
                thUp = ScannerProperties.thUp
            };

            _crossCheck.Prepare(_labelMap().Id);

            var fixProbe = Probe.GetStartingProbe(point, _scanData, _lookupTable, Probe.Method.MinMax, ScannerProperties.thUp, ScannerProperties.thDown);

            var heightMap = BuildHeightMap(point, axis, fixProbe);
            var maxHeight = heightMap.Keys.Max();
            var minHeight = heightMap.Keys.Min();

            progress.Min(1);
            progress.Max(maxHeight - minHeight);
            progress.Reset();

            var tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(() =>
            {
                Point3D center = point;

                //go up
                for (var h = point[axis]; h <= maxHeight; h++)
                {
                    var p = new Point3D(center)
                    {
                        [axis] = h
                    };

                    center = ScanProjection(p, axis, fixProbe);

                    progress.Tick();
                }
            }));


            tasks.Add(Task.Factory.StartNew(() =>
            {
                Point3D center = point;

                ////go down
                for (var h = point[axis] - 1; h >= minHeight; h--)
                {
                    var p = new Point3D(center)
                    {
                        [axis] = h
                    };

                    center = ScanProjection(p, axis, fixProbe);

                    progress.Tick();
                }
            }));

            Task.WaitAll(tasks.ToArray());

            if (ScannerProperties.OptimizePlanes)
            {
                RemoveSharpEdges();
            }

            Helpers.CalculateVolume(_labelMap(), _scanData);

            _labelMap().FireUpdate();

            Task.Factory.StartNew(GC.Collect);
        }        

        private Point3D ScanProjection(Point3D point, Axis axis, Probe fixProbe)
        {
#if DEBUG
            _labelMap().AddDebugPoint(point);
#endif
            var projection = _scanData.GetProjection(axis, point[axis]);

            if (projection.Empty)
                return point;

            var scalarPoint = point.To2D(axis);
            ushort probe = _lookupTable.Map(projection.Pixels[scalarPoint.X, scalarPoint.Y]);

            if (fixProbe.InRange(probe))
            {
                var layer = RayCasting(point, projection, fixProbe);
                var point3Ds = layer as Point3D[] ?? layer.ToArray();
                var center = Helpers.CalculateLayerCenter(point3Ds.ToList());
                _labelMap().Add(point3Ds);

                _labelMap().AddCenter(point);

                return center;
            }

            return point;
        }
        
        private IEnumerable<Point3D> RayCasting(Point3D point, Projection projection, Probe probe)
        {
            var res = new List<Point3D>();

            if (projection.Empty)
                return res;

            var scalarPoint = point.To2D(projection.Axis);

            var rayOffset = Math.PI * 2 / ScannerProperties.Rays;

            for (int r = 0; r < ScannerProperties.Rays; r++)
            {
                var angle = r * rayOffset;
                var p2d = Cast(scalarPoint, angle, projection, probe);
                var p3d = p2d.To3D(projection.Axis, point[projection.Axis]);
                p3d.Index = r;
                res.Add(p3d);
            }

            return res;
        }

        private IDictionary<int, Point3D> BuildHeightMap(Point3D point, Axis origianlAxis, Probe probe)
        {
            var map = new Dictionary<int, Point3D>();

            foreach (Axis axis in Enum.GetValues(typeof(Axis)))
            {
                if (axis != origianlAxis)
                {
                    var projection = _scanData.GetProjection(axis, point[axis]);

                    var marks = RayCasting(point, projection, probe);

                    foreach (var mark in marks)
                    {
                        map[mark[origianlAxis]] = mark;
                    }
                }
            }

            return map;
        }

        private Point2D Cast(Point2D point, double angle, Projection projection, Probe refProbe)
        {
            var p2d = new Point2D(point.X, point.Y);
            
            int skipped = 0;

            var crop = _labelMap().Crop;
            
            for (var l = 0; l < Math.Max(projection.Width, projection.Height); l++)
            {
                p2d.X = (int)(point.X + Math.Cos(angle) * l);
                p2d.Y = (int)(point.Y + Math.Sin(angle) * l);

                var p3d = p2d.To3D(projection.Axis, projection.AxisPos);

                Point3D p3dBounded;
                if (!crop.IsInCrop(p3d, out p3dBounded) || _crossCheck.Check(p3d, _labelMap().Id))
                {
                    return p3dBounded.To2D(projection.Axis);
                }

                var probe = _lookupTable.Map(projection.Pixels[p2d.X, p2d.Y]);
                if (!refProbe.InRange(probe))
                {
                    skipped++;
                }
                else
                {
                    skipped = 0;
                }

                if (skipped >= ScannerProperties.MaxSkip)
                {
                    break;
                }
            }

            return p2d;
        }

        private void RemoveSharpEdges()
        {
            for (int iter = 0; iter < 20; iter++)
            {
                bool changes = false;

                Parallel.ForEach(_labelMap().GetCenters(), с =>
                {
                    var proj = _labelMap().GetProjection(Axis.Z, с.Z).ToList();
                    proj.Add(proj.Last());

                    for (int v = 1; v < proj.Count - 1; v++)
                    {
                        var l = Math.Sqrt(Math.Pow(proj[v - 1].X - с.X, 2) + Math.Pow(proj[v - 1].Y - с.Y, 2));
                        var F = Math.Sqrt(Math.Pow(proj[v].X - с.X, 2) + Math.Pow(proj[v].Y - с.Y, 2));
                        var r = Math.Sqrt(Math.Pow(proj[v + 1].X - с.X, 2) + Math.Pow(proj[v + 1].Y - с.Y, 2));

                        var avrg = Math.Abs((l + r) / 2);

                        if (Math.Abs(F - avrg) > (avrg / ScannerProperties.Smoothness))
                        {
                            proj[v].X = (proj[v - 1].X + proj[v + 1].X) / 2;
                            proj[v].Y = (proj[v - 1].Y + proj[v + 1].Y) / 2;

                            changes = true;
                        }
                    }
                });

                if(!changes)
                {
                    break;
                }
            }
         }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model;
using Model.Utils;

namespace DicomImageViewer.Scanners
{
    public class VoidScanner
    {
        private readonly IScanData _scanData;
        private readonly Func<ILabelMap> _labelMap;
        private readonly ILookupTable _lookupTable;

        public int MaxSkip { get; set; } = 6;
        public ushort thUp { get; set; } = 5;
        public ushort thDown { get; set; } = 5;
        public int Rays { get; set; } = 360;
        public bool OptimizePlanes { get; set; } = true;
        public int Smoothness { get; set; } = 10;

        public VoidScanner(IScanData scanData, ILookupTable lookupTable, Func<ILabelMap> labelMap)
        {
            _scanData = scanData;
            _labelMap = labelMap;
            _lookupTable = lookupTable;
        }

        public void Build(Point3D point, Axis axis, IProgress progress)
        {
            _labelMap().Reset();
            _labelMap().BuildMethod = BuildMethod.RayCasting;

            var fixProbe = Probe.GetStartingProbe(point, _scanData, _lookupTable, Probe.Method.MinMax, thUp, thDown);

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

            if (OptimizePlanes)
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

            var rayOffset = Math.PI * 2 / Rays;

            for (int r = 0; r < Rays; r++)
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
            int px = point.X;
            int py = point.Y;

            int skipped = 0;

            var crop = _labelMap().Crop;

            for (var l = 0; l < Math.Max(projection.Width, projection.Height); l++)
            {
                px = (int)(point.X + Math.Cos(angle) * l);
                py = (int)(point.Y + Math.Sin(angle) * l);
                
                var p3d = new Point2D(px, py).To3D(projection.Axis, projection.AxisPos);
                Point3D p3dBounded;
                if (!crop.IsInCrop(p3d, out p3dBounded))
                {
                    return p3dBounded.To2D(projection.Axis);
                }

                var probe = _lookupTable.Map(projection.Pixels[px, py]);
                if (!refProbe.InRange(probe))
                {
                    skipped++;
                }
                else
                {
                    skipped = 0;
                }

                if (skipped >= MaxSkip)
                {
                    break;
                }
            }

            return new Point2D() { X = px, Y = py };
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

                        if (Math.Abs(F - avrg) > (avrg / Smoothness))
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

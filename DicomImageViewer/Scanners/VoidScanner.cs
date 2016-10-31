using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace DicomImageViewer.Scanners
{
    class VoidScanner
    {
        private readonly IScanData _scanData;
        private readonly ILabelMap _labelMap;

        class Probe
        {
            public ushort Min;
            public ushort Max;

            public bool InRange(ushort val)
            {
                return val >= Min && val <= Max;
            }
        }

        public static int MaxSkip { get; set; } = 6;
        public static ushort thUp { get; set; } = 135;
        public static ushort thDown { get; set; } = 370;

        public VoidScanner(IScanData scanData, ILabelMap labelMap)
        {
            _scanData = scanData;
            _labelMap = labelMap;
        }

        public void Build(Point3D point, int rays, Axis axis)
        {
            _labelMap.Reset();

            var fixProbe = GetStartingProbe(point);

            var heightMap = BuildHeightMap(point, rays, axis, fixProbe);
            var maxHeight = heightMap.Keys.Max();
            var minHeight = heightMap.Keys.Min();

            //go up
            for (var h = point[axis]; h <= maxHeight; h++)
            {
                var p = new Point3D(point)
                {
                    [axis] = h
                };

                ScanProjection(p, axis, fixProbe, rays, heightMap);
            }

            ////go down
            for (var h = point[axis] - 1; h >= minHeight; h--)
            {
                var p = new Point3D(point)
                {
                    [axis] = h
                };

                ScanProjection(p, axis, fixProbe, rays, heightMap);
            }

            _labelMap.FireUpdate();
        }

        private void ScanProjection(Point3D point, Axis axis, Probe fixProbe, int rays, IDictionary<int, Point3D> heightMap)
        {
            var projection = _scanData.GetProjection(axis, point[axis]);

            if (projection.Empty)
                return;

            var scalarPoint = point.To2D(axis);
            ushort probe = projection.Pixels[scalarPoint.X, scalarPoint.Y];

            if (fixProbe.InRange(probe))
            {
                _labelMap.Add(RayCasting(point, projection, rays, axis, fixProbe));
            }
            else
            {
                if (heightMap.ContainsKey(point[axis]))
                {
                    var mark = heightMap[point[axis]];

                    _labelMap.Add(RayCasting(mark, projection, rays, axis, fixProbe));
                }
            }
        }

        private Probe GetStartingProbe(Point3D point)
        {
            var projection = _scanData.GetProjection(Axis.Z, point[Axis.Z]);

            if (projection.Empty)
                throw new ArgumentOutOfRangeException();

            var scalarPoint = point.To2D(Axis.Z);
            ushort probe = projection.Pixels[scalarPoint.X, scalarPoint.Y];

            return new Probe()
            {
                Max = (ushort) (probe + thUp),
                Min = (ushort) (probe - thDown)
            };
        }

        private IEnumerable<Point3D> RayCasting(Point3D point, Projection projection, int rays, Axis axis, Probe probe)
        {
            var res = new List<Point3D>();
            
            if (projection.Empty)
                return res;

            var scalarPoint = point.To2D(axis);

            var rayOffset = Math.PI * 2 / rays;
                        
            for (int r = 0; r < rays; r++)
            {
                var angle = r * rayOffset;
                var p3d = Cast(scalarPoint, angle, projection, probe).To3D(axis, point[axis]);
                p3d.Index = r;
                res.Add(p3d);
            }

            return res;
        }

        private IDictionary<int, Point3D> BuildHeightMap(Point3D point, int rays, Axis origianlAxis, Probe probe)
        {
            var map = new Dictionary<int, Point3D>();

            foreach (Axis axis in Enum.GetValues(typeof(Axis)))
            {
                if (axis != origianlAxis)
                {
                    var projection = _scanData.GetProjection(axis, point[axis]);

                    var marks = RayCasting(point, projection, rays, axis, probe);

                    foreach (var mark in marks)
                    {
                        map[mark[origianlAxis]] = mark;
                        
                        //_labelMap.Add(mark);
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

            for (var l = 0; l < Math.Max(projection.Width, projection.Height); l++)
            {
                px = (int)(point.X + Math.Cos(angle) * l);
                py = (int)(point.Y + Math.Sin(angle) * l);

                bool edge = false;

                if (px < 0)
                {
                    px = 0;
                    edge = true;
                }

                if (py < 0)
                {
                    py = 0;
                    edge = true;
                }

                if (px >= projection.Width)
                {
                    px = projection.Width - 1;
                    edge = true;
                }

                if (py >= projection.Height)
                {
                    py = projection.Height - 1;
                    edge = true;
                }

                if (edge)
                {
                    break;
                }

                var probe = projection.Pixels[px, py];
                if (probe >= refProbe.Max || probe <= refProbe.Min)
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
    }
}

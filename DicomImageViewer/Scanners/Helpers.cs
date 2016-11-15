using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using System.Threading.Tasks;

namespace DicomImageViewer.Scanners
{
    static class Helpers
    {
        public static Point3D CalculateLayerCenter(IList<Point3D> layer)
        {
            int x = 0;
            int y = 0;

            foreach (var mark in layer)
            {
                x += mark.X;
                y += mark.Y;
            }

            return new Point3D(x / layer.Count, y / layer.Count, layer.First().Z);
        }

        public static void CalculateVolume(ILabelMap label, IScanData scanData)
        {
            var volume = 0.0d;
            var guard = new object();

            Parallel.ForEach(label.GetCenters(), d =>
            {
                var proj = label.GetProjection(Axis.Z, d.Z).ToList();
                proj.Add(proj.Last());

                var area = 0.0d;

                for (int v = 0; v < proj.Count - 1; v++)
                {
                    var a = Math.Sqrt(Math.Pow(proj[v].X - d.X, 2) + Math.Pow(proj[v].Y - d.Y, 2));
                    var b = Math.Sqrt(Math.Pow(proj[v + 1].X - d.X, 2) + Math.Pow(proj[v + 1].Y - d.Y, 2));
                    var c = Math.Sqrt(Math.Pow(proj[v + 1].X - proj[v].X, 2) + Math.Pow(proj[v + 1].Y - proj[v].Y, 2));

                    var s = (a + b + c) / 2;

                    area += Math.Sqrt(s * Math.Abs(s - a) * Math.Abs(s - b) * Math.Abs(s - c));
                }

                lock (guard)
                {
                    volume += area;
                }
            });

            double xres, yres, zres;
            scanData.Resolution(out xres, out yres, out zres);

            label.Volume = (int)(volume * xres * yres * zres);
        }
    }
}

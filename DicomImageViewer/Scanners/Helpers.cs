using System.Collections.Generic;
using System.Linq;
using Model;

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
    }
}

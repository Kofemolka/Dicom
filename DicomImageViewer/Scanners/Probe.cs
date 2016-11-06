using System;
using Model;

namespace DicomImageViewer.Scanners
{
    class Probe
    {
        public ushort Min;
        public ushort Max;

        public bool InRange(ushort val)
        {
            return val >= Min && val <= Max;
        }

        public static Probe GetStartingProbe(Point3D point, IScanData scanData, ushort thUp = 0, ushort thDown =  0)
        {
            var projection = scanData.GetProjection(Axis.Z, point[Axis.Z]);

            if (projection.Empty)
                throw new ArgumentOutOfRangeException();

            var scalarPoint = point.To2D(Axis.Z);

            int probe = 0;
            const int probeHalfWidth = 3;

            int probeCount = 0;
            for (var x = Math.Max(0, scalarPoint.X - probeHalfWidth);
                x < Math.Min(projection.Width, scalarPoint.X + probeHalfWidth);
                x++)
            {
                for (var y = Math.Max(0, scalarPoint.Y - probeHalfWidth);
                    y < Math.Min(projection.Height, scalarPoint.Y + probeHalfWidth);
                    y++)
                {
                    probe += projection.Pixels[x, y];
                    probeCount++;
                }
            }

            if (probeCount > 0)
            {
                probe /= probeCount;
            }

            return new Probe()
            {
                Max = (ushort)(probe + thUp),
                Min = (ushort)(probe - thDown)
            };
        }
    }
}

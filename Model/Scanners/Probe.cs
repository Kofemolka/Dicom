using System;

namespace Model.Scanners
{
    class Probe
    {
        private ushort _min;
        private ushort _max;

        public enum Method
        {
            Average,
            MinMax
        }

        public bool InRange(ushort val)
        {
            return val >= _min && val <= _max;
        }

        public static Probe GetStartingProbe(Point3D point, IScanData scanData, ILookupTable lookupTable, Method method, ushort thUp = 0, ushort thDown =  0)
        {
            var projection = scanData.GetProjection(Axis.Z, point[Axis.Z]);

            if (projection.Empty)
                throw new ArgumentOutOfRangeException();

            var scalarPoint = point.To2D(Axis.Z);
            
            switch (method)
            {
                case Method.Average:
                    return GetAverageProbe(scalarPoint, projection, lookupTable, thUp, thDown);

                case Method.MinMax:
                    return GetMinMaxProbe(scalarPoint, projection, lookupTable, thUp, thDown);
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }

        private static Probe GetAverageProbe(Point2D scalarPoint, Projection projection, ILookupTable lookupTable, ushort thUp = 0, ushort thDown = 0)
        {
            const int probeHalfWidth = 5;

            int probe = 0;
            int probeCount = 0;
            for (var x = Math.Max(0, scalarPoint.X - probeHalfWidth);
                x < Math.Min(projection.Width, scalarPoint.X + probeHalfWidth);
                x++)
            {
                for (var y = Math.Max(0, scalarPoint.Y - probeHalfWidth);
                    y < Math.Min(projection.Height, scalarPoint.Y + probeHalfWidth);
                    y++)
                {
                    probe += lookupTable.Map(projection.Pixels[x, y]);
                    probeCount++;
                }
            }

            if (probeCount > 0)
            {
                probe /= probeCount;
            }

            return new Probe()
            {
                _max = (ushort)(probe + thUp),
                _min = (ushort)(probe - thDown)
            };
        }

        private static Probe GetMinMaxProbe(Point2D scalarPoint, Projection projection, ILookupTable lookupTable, ushort thUp = 0, ushort thDown = 0)
        {
            const int probeHalfWidth = 5;

            ushort min = ushort.MaxValue;
            ushort max = ushort.MinValue;

            for (var x = Math.Max(0, scalarPoint.X - probeHalfWidth); x < Math.Min(projection.Width, scalarPoint.X + probeHalfWidth); x++)
            {
                for (var y = Math.Max(0, scalarPoint.Y - probeHalfWidth); y < Math.Min(projection.Height, scalarPoint.Y + probeHalfWidth); y++)
                {
                    var mappedValue = lookupTable.Map(projection.Pixels[x, y]);

                    if (mappedValue > max)
                    {
                        max = mappedValue;
                    }

                    if (mappedValue < min)
                    {
                        min = mappedValue;
                    }
                }
            }

            return new Probe()
            {
                _max = (ushort)(max + thUp),
                _min = (ushort)(min - thDown)
            };
        }
    }
}

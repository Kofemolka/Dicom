using System;

namespace Model
{
    public class Point2D
    {
        public int X;
        public int Y;

        public Point2D()
        {

        }

        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point3D To3D(Axis axis, int value)
        {
            switch (axis)
            {
                case Axis.X: return new Point3D(value, X, Y);
                case Axis.Y: return new Point3D(X, value, Y);
                case Axis.Z: return new Point3D(X, Y, value);
            }

            throw new ArgumentOutOfRangeException(nameof(axis));
        }

        public override int GetHashCode()
        {
            return X + Y * 10000;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Point2D;
            if (other == null)
                return false;

            return X == other.X && Y == other.Y;
        }
    }
}

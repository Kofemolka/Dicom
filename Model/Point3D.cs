using System;

namespace Model
{
    public class Point3D
    {
        public int X;
        public int Y;
        public int Z;

        public int Index;

        public Point3D()
        {

        }

        public Point3D(Point3D other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
            Index = other.Index;
        }

        public Point3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int this[Axis axis]
        {
            get
            {
                switch (axis)
                {
                    case Axis.X: return X;
                    case Axis.Y: return Y;
                    case Axis.Z: return Z;
                }

                throw new ArgumentOutOfRangeException(nameof(axis));
            }
            set
            {
                switch (axis)
                {
                    case Axis.X:
                        X = value;
                        return;
                    case Axis.Y:
                        Y = value;
                        return;
                    case Axis.Z:
                        Z = value;
                        return;
                }

                throw new ArgumentOutOfRangeException(nameof(axis));
            }
        }

        public Point2D To2D(Axis axis)
        {
            switch (axis)
            {
                case Axis.X: return new Point2D(Y, Z);
                case Axis.Y: return new Point2D(X, Z);
                case Axis.Z: return new Point2D(X, Y);
            }

            throw new ArgumentOutOfRangeException(nameof(axis));
        }

        public override int GetHashCode()
        {
            return X + Y*1000 + Z*1000*1000;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Point3D;
            if (other == null)
                return false;

            return X == other.X && Y == other.Y && Z == other.Z;
        }
    }
}

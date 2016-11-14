namespace Model
{
    public class Projection
    {        
        public Projection(Axis axis, int axisPos, int width, int height, ushort[,] pixels)
        {
            Axis = axis;
            Width = width;
            Height = height;
            Pixels = pixels;
            AxisPos = axisPos;
        }

        public Projection(Axis axis)
        {
            Axis = axis;
            Width = Height = 0;
            Pixels = null;
        }

        public bool Empty => Pixels == null || Pixels.Length == 0;

        public int AxisPos { get; private set; }

        public Axis Axis { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public ushort[,] Pixels { get; private set; }
    }
}

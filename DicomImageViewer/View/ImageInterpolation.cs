using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Model;
using Color = System.Drawing.Color;

namespace DicomImageViewer.View
{
    public partial class ImageInterpolation : UserControl
    {
        public LookupTable Lookup { get; set; }

        public ImageInterpolation()
        {
            InitializeComponent();
        }

        public void Init()
        {
            Lookup.PropertyChanged += (sender, args) => Invalidate();
        }

        private int minX => dotRadius/2;
        private int maxX => Width - dotRadius/2;
        private int minY => dotRadius/2;
        private int maxY => Height - dotRadius / 2;

        float XScale => (float)(Width - dotRadius) / 65536;
        float YScale => (float)(Height - dotRadius) / 255;
        int LowX => (int)(Lookup.WinMin * XScale) + minX;
        int LowY => Height - minY - (int)(Lookup.IntMin * YScale);
        int HighX => (int)(Lookup.WinMax * XScale) + minX;
        int HighY => Height - minY - (int)(Lookup.IntMax * YScale);

        private void ImageInterpolation_Paint(object sender, PaintEventArgs e)
        {
            if (Lookup == null)
                return;

            Graphics g = Graphics.FromHwnd(Handle);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;

            try
            {
                var path = new GraphicsPath();
            //    path.AddRectangle(new Rectangle(minX, LowY, LowX-minX, Height - LowY));
              //  path.AddRectangle(new Rectangle(HighX, HighY, Width - HighX, Height - HighY));
                path.AddLine(minX, LowY, LowX, LowY);
                path.AddLine(LowX, LowY, HighX, HighY);
                path.AddLine(HighX, HighY, Width-minX, HighY);
                path.AddLine(Width - minX, HighY, Width - minX, maxY);
                path.AddLine(Width - minX, maxY, minX, maxY);
                path.AddLine(minX, maxY, minX, LowY);


                g.FillPath(FillBrush, path);

                g.FillEllipse(LineBrush, LowX - minX, LowY- minY, dotRadius, dotRadius);
                g.FillEllipse(LineBrush, HighX - minX, HighY - minY, dotRadius, dotRadius);

                g.DrawLine(LinePen, LowX, LowY, HighX, HighY);
            }
            catch (Exception ex)
            {

            }

            g.Dispose();
        }

        private Brush LineBrush = new SolidBrush(Color.Black);
        private Brush FillBrush = new SolidBrush(Color.LightGray);
        private Pen LinePen = new Pen(Color.Black);

        private const int dotRadius = 10;

        private void ImageInterpolation_MouseDown(object sender, MouseEventArgs e)
        {
            var newLockPoint = MouseOverPoint(e.X, e.Y);
            if (newLockPoint != LockPoint.None)
            {
                _lockPoint = newLockPoint;
                _lock = new Point(e.X, e.Y);

                _prevWinMin = Lookup.WinMin;
                _prevWinMax = Lookup.WinMax;
                _prevIntMax = Lookup.IntMax;
                _prevIntMin = Lookup.IntMin;
            }
        }

        private void ImageInterpolation_MouseMove(object sender, MouseEventArgs e)
        {
            if (_lockPoint == LockPoint.None)
            {
                var newLockPoint = MouseOverPoint(e.X, e.Y);
                if (newLockPoint != LockPoint.None)
                {
                    Cursor = Cursors.Hand;
                }
                else
                {
                    Cursor = Cursors.Default;
                }
            }
            else
            {
                int x = e.X;
                int y = e.Y;

                if (x < minX) x = minX;
                if (x > maxX) x = maxX;

                if (y < minY) y = minY;
                if (y > maxY) y = maxY;

                var deltaX = x - _lock.X;
                var deltaY = y - _lock.Y;

                var realX = deltaX / XScale;
                var realY = -deltaY / YScale;

                if (_lockPoint == LockPoint.Low)
                {
                    Lookup.WinMin = _prevWinMin + (int)realX;
                    Lookup.IntMin = _prevIntMin + (int)realY;
                }
                else
                {
                    Lookup.WinMax = _prevWinMax + (int)realX;
                    Lookup.IntMax = _prevIntMax + (int)realY;
                }
            }
        }

        private void ImageInterpolation_MouseUp(object sender, MouseEventArgs e)
        {
            _lockPoint = LockPoint.None;
            Cursor = Cursors.Default;
        }

        private void ImageInterpolation_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            _lockPoint = LockPoint.None;
        }

        private LockPoint MouseOverPoint(int x, int y)
        {
            if (Math.Abs(x - LowX) < dotRadius && Math.Abs(y - LowY) < dotRadius)
            {
                return LockPoint.Low;
            }

            if (Math.Abs(x - HighX) < dotRadius && Math.Abs(y - HighY) < dotRadius)
            {
                return LockPoint.High;
            }

            return LockPoint.None;
        }

        enum LockPoint
        {
            None,
            Low,
            High
        }

        private LockPoint _lockPoint = LockPoint.None;
        private Point _lock;

        private int _prevWinMax;
        private int _prevWinMin;
        private int _prevIntMin;
        private int _prevIntMax;
    }
}

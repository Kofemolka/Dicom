using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Model;
using Model.Utils;

namespace DicomImageViewer
{
    public partial class ProjectionView : UserControl
    {
        private readonly Axis _axis;
        private readonly IScanData _scanData;
        private readonly ILookupTable _lookupTable;
        private readonly LabelMapSet _labelMapSet;
        private readonly IProbe _probe;
        private Projection _projection;
        private int CurrentCutIndex => trackCut.Value;

        private Bitmap _bmp;

        public ProjectionView(Axis axis, IScanData scanData, ILookupTable lookupTable, LabelMapSet labelMapSet, IProbe probe)
        {
            _axis = axis;
            _scanData = scanData;
            _lookupTable = lookupTable;
            _labelMapSet = labelMapSet;
            _probe = probe;

            _scanData.DataUpdated += () => this.Invoke(new MethodInvoker(ScanDataOnDataUpdated));
            _labelMapSet.LabelDataChanged += LabelMapOnLabelDataChanged;

            InitializeComponent();

            
        }
        protected override CreateParams CreateParams
        {
            get
            {
                var parms = base.CreateParams;
                parms.Style &= ~0x02000000;  // Turn off WS_CLIPCHILDREN
                return parms;
            }
        }

        private void LabelMapOnLabelDataChanged()
        {
            surface.Invalidate();
        }

        private void ScanDataOnDataUpdated()
        {
            trackCut.Maximum = _scanData.GetAxisCutCount(_axis) - 1;
            trackCut.Value = trackCut.Maximum/2;
            
            Rebuild();
        }

        private void Rebuild()
        {
            _projection = _scanData.GetProjection(_axis, CurrentCutIndex);

            if (_projection.Empty)
                return;

            _bmp = new Bitmap(_projection.Width, _projection.Height, PixelFormat.Format24bppRgb);
            CreateImage16();

            surface.Invalidate();
        }

        private void trackCut_Scroll(object sender, EventArgs e)
        {
            Rebuild();
        }

        private bool DataReady => _bmp != null;

        private void surface_Paint(object sender, PaintEventArgs e)
        {
            if (!DataReady)
                return;

            Graphics g = Graphics.FromHwnd(surface.Handle);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;

            try
            {
                DrawCut(g);
                DrawLabels(g);
                DrawDebugPoints(g);
            }
            catch (Exception ex)
            {
                
            }

            g.Dispose();
        }

        private void DrawCut(Graphics g)
        {
            var sizef = ImageSize();

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(_bmp, (surface.Width - sizef.Width) / 2,
                                (surface.Height - sizef.Height) / 2,
                                sizef.Width, sizef.Height);
        }
        
        private void DrawLabels(Graphics g)
        {
            var imgSize = ImageSize();
            var imgOffset = ImageOffset(imgSize);

            foreach (var label in _labelMapSet.All)
            {
                using (var brush = new SolidBrush(label.Color))
                {
                    foreach (var mark in label.GetProjection(_axis, CurrentCutIndex))
                    {
                        var point = Image2Surface(mark.To2D(_axis), imgOffset, imgSize);
                        g.FillRectangle(brush, point.X, point.Y, 2, 2);
                    }
                }
            }
        }

        private void DrawDebugPoints(Graphics g)
        {
#if DEBUG
            var imgSize = ImageSize();
            var imgOffset = ImageOffset(imgSize);
                        
            foreach (var label in _labelMapSet.All)
            {
                foreach (var mark in label.GetDebugProjection(_axis, CurrentCutIndex))
                {
                    var point = Image2Surface(mark.To2D(_axis), imgOffset, imgSize);
                    g.FillRectangle(Brushes.LimeGreen, point.X, point.Y, 2, 2);
                }
            }
#endif
        }

        private void CreateImage16()
        {
            BitmapData bmd = _bmp.LockBits(new Rectangle(0, 0, _bmp.Width, _bmp.Height),
               ImageLockMode.ReadOnly, _bmp.PixelFormat);

            unsafe
            {
                int pixelSize = 3;
                int i, j, j1, i1;
                byte b;

                for (i = 0; i < bmd.Height; ++i)
                {
                    byte* row = (byte*)bmd.Scan0 + (i * bmd.Stride);
                    i1 = i * bmd.Width;

                    for (j = 0; j < bmd.Width; ++j)
                    {
                        b = _lookupTable.Map(_projection.Pixels[j, i]);
                        j1 = j * pixelSize;
                        row[j1] = b;            // Red
                        row[j1 + 1] = b;        // Green
                        row[j1 + 2] = b;        // Blue
                    }
                }
            }

            _bmp.UnlockBits(bmd);
        }

        private SizeF ImageSize()
        {
            SizeF sizef = new SizeF(_bmp.Width / _bmp.HorizontalResolution,
                                    _bmp.Height / _bmp.VerticalResolution);

            float fScale = Math.Min(surface.Width / sizef.Width,
                                    surface.Height / sizef.Height);

            sizef.Width *= fScale;
            sizef.Height *= fScale;

            return sizef;
        }
        
        private Size ImageOffset(SizeF imgSize)
        {
            return new Size((int)((surface.Width - imgSize.Width) / 2),
                            (int)((surface.Height - imgSize.Height) / 2));
        }

        private Point2D Image2Surface(Point2D p, Size imgOffset, SizeF imgSize)
        {  
            return new Point2D((int)(p.X * imgSize.Width / _projection.Width + imgOffset.Width),
                            (int)(p.Y * imgSize.Height / _projection.Height + imgOffset.Height));
        }

        private Point2D Image2Surface(Point2D p)
        {
            var imgSize = ImageSize();
            var imgOffset = ImageOffset(imgSize);
            return Image2Surface(p, imgOffset, imgSize);
        }

        private Point2D Surface2Image(Point p, out bool outOfImage)
        {
            outOfImage = false;

            var imgSize = ImageSize();
            var imgOffset = ImageOffset(imgSize);

            var normalP = new Point(p.X - imgOffset.Width, p.Y - imgOffset.Height);
            if (normalP.X < 0)
            {
                normalP.X = 0;
                outOfImage = true;
            }
            if (normalP.X >= imgSize.Width)
            {
                outOfImage = true;
                normalP.X = (int)(imgSize.Width - 1);
            }
            if (normalP.Y < 0)
            {
                outOfImage = true;
                normalP.Y = 0;
            }
            if (normalP.Y >= imgSize.Height)
            {
                outOfImage = true;
                normalP.Y = (int)(imgSize.Height - 1);
            }

            var realX = normalP.X * _projection.Width / imgSize.Width;
            var realY = normalP.Y * _projection.Height / imgSize.Height;

            return new Point2D((int)realX, (int)realY);
        }

        private void surface_MouseClick(object sender, MouseEventArgs e)
        {
            if (!DataReady)
                return;

            bool outOfSurface;
            var point2D = Surface2Image(new Point(e.X, e.Y), out outOfSurface);

            if (!outOfSurface)
            {
                _probe.PointSelect(point2D.To3D(_axis, CurrentCutIndex));
            }
        }

        private void surface_MouseMove(object sender, MouseEventArgs e)
        {
            if (!DataReady)
                return;
            
            if (_projection == null || _projection.Empty)
                return;

            bool outOfSurface;
            var point2D = Surface2Image(new Point(e.X, e.Y), out outOfSurface);

            if (!outOfSurface)
            {
                _probe.Dencity(_projection.Pixels[point2D.X, point2D.Y]);
            }
        }

        private void ProjectionView_Resize(object sender, EventArgs e)
        {
            surface.Invalidate();
        }
    }
}

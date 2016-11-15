using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            _labelMapSet.LabelMapUpdated += LabelMapOnLabelDataChanged;
            _labelMapSet.LabelMapDeleted += LabelMapOnLabelDataChanged;
            _labelMapSet.LabelMapSetReset += () => surface.Invalidate();
            _labelMapSet.LabelMapCurrentSelectionChanged += () =>
                {
                    _labelMapSet.Current.Crop.CropChanged += () => surface.Invalidate();
                    surface.Invalidate();
                };

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

        private void LabelMapOnLabelDataChanged(ILabelMap label)
        {
            surface.Invalidate();
        }

        private void ScanDataOnDataUpdated()
        {
            trackCut.Maximum = _scanData.GetAxisCutCount(_axis) - 1;
            if (trackCut.Maximum < 0)
            {
                trackCut.Maximum = 0;
            }
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
                DrawCropBox(g);
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

        private void DrawCropBox(Graphics g)
        {
            var imgSize = ImageSize();
            var imgOffset = ImageOffset(imgSize);

            var crop = _labelMapSet.Current.Crop;

            Point2D tl, tr, bl, br;

            switch (_axis)
            {
                case Axis.X:
                    tl = new Point2D(crop.YL, crop.ZL);
                    tr = new Point2D(crop.YR, crop.ZL);
                    bl = new Point2D(crop.YL, crop.ZR);
                    br = new Point2D(crop.YR, crop.ZR);
                    break;
                case Axis.Y:
                    tl = new Point2D(crop.XL, crop.ZL);
                    tr = new Point2D(crop.XR, crop.ZL);
                    bl = new Point2D(crop.XL, crop.ZR);
                    br = new Point2D(crop.XR, crop.ZR);
                    break;
                case Axis.Z:
                    tl = new Point2D(crop.XL, crop.YL);
                    tr = new Point2D(crop.XR, crop.YL);
                    bl = new Point2D(crop.XL, crop.YR);
                    br = new Point2D(crop.XR, crop.YR);
                    break;
                default:
                    return;
            }

            tl = Image2Surface(tl, imgOffset, imgSize);
            tr = Image2Surface(tr, imgOffset, imgSize);
            bl = Image2Surface(bl, imgOffset, imgSize);
            br = Image2Surface(br, imgOffset, imgSize);

            using (var pen = new Pen(new HatchBrush(HatchStyle.Percent70, Color.White), 1))
            {
                g.DrawLine(pen, tl.X, tl.Y, tr.X, tr.Y);
                g.DrawLine(pen, tr.X , tr.Y, br.X, br.Y);
                g.DrawLine(pen, br.X, br.Y, bl.X, bl.Y);
                g.DrawLine(pen, bl.X, bl.Y, tl.X, tl.Y);
            }
        }

        private void CreateImage16()
        {
            BitmapData bmd = _bmp.LockBits(new Rectangle(0, 0, _bmp.Width, _bmp.Height), ImageLockMode.ReadOnly, _bmp.PixelFormat);

            unsafe
            {
                int pixelSize = 3;
                int i, j, j1, i1;
                byte b;

                for (i = 0; i < bmd.Height; ++i)
                {
                    byte* row = (byte*) bmd.Scan0 + (i*bmd.Stride);
                    i1 = i*bmd.Width;

                    for (j = 0; j < bmd.Width; ++j)
                    {
                        b = _lookupTable.Map(_projection.Pixels[j, i]);
                        j1 = j*pixelSize;
                        row[j1] = b; // Red
                        row[j1 + 1] = b; // Green
                        row[j1 + 2] = b; // Blue
                    }
                }
            }

            _bmp.UnlockBits(bmd);
        }

        private SizeF ImageSize()
        {
            SizeF sizef = new SizeF(_bmp.Width/_bmp.HorizontalResolution, _bmp.Height/_bmp.VerticalResolution);

            float fScale = Math.Min(surface.Width/sizef.Width, surface.Height/sizef.Height);

            sizef.Width *= fScale;
            sizef.Height *= fScale;

            return sizef;
        }

        private Size ImageOffset(SizeF imgSize)
        {
            return new Size((int) ((surface.Width - imgSize.Width)/2), (int) ((surface.Height - imgSize.Height)/2));
        }

        private Point2D Image2Surface(Point2D p, Size imgOffset, SizeF imgSize)
        {
            return new Point2D((int) (p.X*imgSize.Width/_projection.Width + imgOffset.Width), (int) (p.Y*imgSize.Height/_projection.Height + imgOffset.Height));
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
                normalP.X = (int) (imgSize.Width - 1);
            }
            if (normalP.Y < 0)
            {
                outOfImage = true;
                normalP.Y = 0;
            }
            if (normalP.Y >= imgSize.Height)
            {
                outOfImage = true;
                normalP.Y = (int) (imgSize.Height - 1);
            }

            var realX = normalP.X*_projection.Width/imgSize.Width;
            var realY = normalP.Y*_projection.Height/imgSize.Height;

            return new Point2D((int) realX, (int) realY);
        }

        private enum CropLock
        {
            None,
            XL,
            XR,
            YL,
            YR,
            ZL,
            ZR
        }

        private CropLock _cropLock = CropLock.None;

        private void surface_MouseClick(object sender, MouseEventArgs e)
        {
            if (!DataReady)
                return;

            bool outOfSurface;
            var point2D = Surface2Image(new Point(e.X, e.Y), out outOfSurface);

            if (!outOfSurface && _cropLock == CropLock.None)
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

            Debug.Print(point2D.X + ":" + point2D.Y);

            if (!outOfSurface)
            {
                _probe.Dencity(_projection.Pixels[point2D.X, point2D.Y]);
            }

            if (_cropLock == CropLock.None) // not locked
            {
                var cropLock = MouseOverCropBox(point2D);

                if (cropLock != CropLock.None) //can lock on click
                {
                    Cursor = Cursors.Hand;
                }
                else
                {
                    if (!outOfSurface)
                    {
                        Cursor = Cursors.Cross;
                    }
                    else
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }

            var crop = _labelMapSet.Current.Crop;
            switch (_axis)
            {
                case Axis.X:
                    switch (_cropLock)
                    {
                        case CropLock.YL:
                            crop.YL = point2D.X;
                            break;
                        case CropLock.YR:
                            crop.YR = point2D.X;
                            break;
                        case CropLock.ZL:
                            crop.ZL = point2D.Y;
                            break;
                        case CropLock.ZR:
                            crop.ZR = point2D.Y;
                            break;
                    }
                    break;
                case Axis.Y:
                    switch (_cropLock)
                    {
                        case CropLock.XL:
                            crop.XL = point2D.X;
                            break;
                        case CropLock.XR:
                            crop.XR = point2D.X;
                            break;
                        case CropLock.ZL:
                            crop.ZL = point2D.Y;
                            break;
                        case CropLock.ZR:
                            crop.ZR = point2D.Y;
                            break;
                    }
                    break;
                case Axis.Z:
                    switch (_cropLock)
                    {
                        case CropLock.XL:
                            crop.XL = point2D.X;
                            break;
                        case CropLock.XR:
                            crop.XR = point2D.X;
                            break;
                        case CropLock.YL:
                            crop.YL = point2D.Y;
                            break;
                        case CropLock.YR:
                            crop.YR = point2D.Y;
                            break;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool CropEdgeCatch(int a, int b)
        {
            return Math.Abs(a - b) < 2;
        }

        private CropLock MouseOverCropBox(Point2D point)
        {
            var crop = _labelMapSet.Current.Crop;

            switch (_axis)
            {
                case Axis.X:
                    if (CropEdgeCatch(point.X, crop.YL))
                    {
                        return CropLock.YL;
                    }
                    if (CropEdgeCatch(point.X, crop.YR))
                    {
                        return CropLock.YR;
                    }
                    if (CropEdgeCatch(point.Y, crop.ZL))
                    {
                        return CropLock.ZL;
                    }
                    if (CropEdgeCatch(point.Y, crop.ZR))
                    {
                        return CropLock.ZR;
                    }
                    break;
                case Axis.Y:
                    if (CropEdgeCatch(point.X, crop.XL))
                    {
                        return CropLock.XL;
                    }
                    if (CropEdgeCatch(point.X, crop.XR))
                    {
                        return CropLock.XR;
                    }
                    if (CropEdgeCatch(point.Y, crop.ZL))
                    {
                        return CropLock.ZL;
                    }
                    if (CropEdgeCatch(point.Y, crop.ZR))
                    {
                        return CropLock.ZR;
                    }
                    break;
                case Axis.Z:
                    if (CropEdgeCatch(point.X, crop.XL))
                    {
                        return CropLock.XL;
                    }
                    if (CropEdgeCatch(point.X, crop.XR))
                    {
                        return CropLock.XR;
                    }
                    if (CropEdgeCatch(point.Y, crop.YL))
                    {
                        return CropLock.YL;
                    }
                    if (CropEdgeCatch(point.Y, crop.YR))
                    {
                        return CropLock.YR;
                    }

                    break;
            }

            return CropLock.None;
        }

        private void ProjectionView_Resize(object sender, EventArgs e)
        {
            surface.Invalidate();
        }

        private void surface_MouseDown(object sender, MouseEventArgs e)
        {
            if (!DataReady)
                return;

            bool outOfSurface;
            var point2D = Surface2Image(new Point(e.X, e.Y), out outOfSurface);

            var cropLock = MouseOverCropBox(point2D);

            if (cropLock != CropLock.None)
            {
                _cropLock = cropLock;
                Cursor = Cursors.SizeAll;
            }
        }

        private void surface_MouseUp(object sender, MouseEventArgs e)
        {
            _cropLock = CropLock.None;
            Cursor = Cursors.Default;
        }
    }
}

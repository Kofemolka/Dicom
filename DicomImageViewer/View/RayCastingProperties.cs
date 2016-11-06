using DicomImageViewer.Scanners;
using System;
using System.Windows.Forms;
using Model;
using Model.Utils;
using System.Threading.Tasks;

namespace DicomImageViewer.View
{
    public partial class RayCastingProperties : UserControl, IScannerProperties
    {
        private Form Owner { get; set; }
        private IProgress Progress { get; set; }
        private VoidScanner VoidScanner { get; set; }

        private Point3D _lastRayCast;

        public RayCastingProperties()
        {
            InitializeComponent();            
        }

        public void Init(VoidScanner scanner, IProgress progress, Form owner)
        {
            Owner = owner;
            Progress = progress;
            VoidScanner = scanner;

            trackHiThresh.Value = VoidScanner.thUp;
            trackLowThresh.Value = VoidScanner.thDown;
            trackSkippedPixels.Value = VoidScanner.MaxSkip;
            trackRays.Value = VoidScanner.Rays;

            Parent.Tag = this;
        }

        private void trackRays_ValueChanged(object sender, EventArgs e)
        {
            VoidScanner.Rays = trackRays.Value;
            lbRays.Text = "Rays: " + trackRays.Value;
        }

        private void trackHiThresh_ValueChanged(object sender, EventArgs e)
        {
            VoidScanner.thUp = (ushort)trackHiThresh.Value;
            lbHiThresh.Text = "HI threshold: " + VoidScanner.thUp;
        }

        private void trackLowThresh_ValueChanged(object sender, EventArgs e)
        {
            VoidScanner.thDown = (ushort)trackLowThresh.Value;
            lbLowThresh.Text = "LOW threshold: " + VoidScanner.thDown;
        }

        private void trackSkippedPixels_ValueChanged(object sender, EventArgs e)
        {
            VoidScanner.MaxSkip = trackSkippedPixels.Value;
            lbHunger.Text = "Hunger: " + VoidScanner.MaxSkip;
        }

        private void btnRebuild_Click(object sender, EventArgs e)
        {
            Scan(_lastRayCast);
        }

        public void Scan(Point3D point)
        {
            if (point != null)
            {
                _lastRayCast = point;
            }

            if (_lastRayCast == null)
            {
                return;
            }

            Cursor = Cursors.WaitCursor;
            Owner.Enabled = false;

            Task.Factory.StartNew(() =>
            {
                VoidScanner.Build(_lastRayCast, Axis.Z, Progress);
            }).ContinueWith((t) =>
            {
                Owner.Enabled = true;

                Cursor = Cursors.Default;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}

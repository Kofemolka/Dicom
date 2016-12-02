using System;
using System.Windows.Forms;
using Model;
using Model.Utils;
using System.Threading.Tasks;
using Model.Scanners;

namespace DicomImageViewer.View
{
    public partial class RayCastingPropertiesView : UserControl, IScannerPropertiesView
    {
        private Form Owner { get; set; }
        private IProgress Progress { get; set; }
        private VoidScanner VoidScanner { get; set; }

        IScannerProperties IScannerPropertiesView.Properties
        {
            get { return VoidScanner.ScannerProperties; }

            set
            {
                var properties = value as VoidScanner.VoidScannerProperties;
                if (properties != null)
                {
                    VoidScanner.ScannerProperties = properties;
                    UpdateView();
                }
            }
        }

        private void UpdateView()
        {
            trackHiThresh.Value = VoidScanner.ScannerProperties.thUp;
            trackLowThresh.Value = VoidScanner.ScannerProperties.thDown;
            trackSkippedPixels.Value = VoidScanner.ScannerProperties.MaxSkip;
            trackRays.Value = VoidScanner.ScannerProperties.Rays;
            trackSmoothness.Value = VoidScanner.ScannerProperties.Smoothness;
            chkOptimizePlanes.Checked = VoidScanner.ScannerProperties.OptimizePlanes;
        }
        
        public RayCastingPropertiesView()
        {
            InitializeComponent();            
        }

        public void Init(VoidScanner scanner, IProgress progress, Form owner)
        {
            Owner = owner;
            Progress = progress;
            VoidScanner = scanner;

            UpdateView();

            Parent.Tag = this;
        }

        private void trackRays_ValueChanged(object sender, EventArgs e)
        {
            VoidScanner.ScannerProperties.Rays = trackRays.Value;
            lbRays.Text = "Rays: " + trackRays.Value;
        }

        private void trackHiThresh_ValueChanged(object sender, EventArgs e)
        {
            VoidScanner.ScannerProperties.thUp = (ushort)trackHiThresh.Value;
            lbHiThresh.Text = "HI: " + VoidScanner.ScannerProperties.thUp;
        }

        private void trackLowThresh_ValueChanged(object sender, EventArgs e)
        {
            VoidScanner.ScannerProperties.thDown = (ushort)trackLowThresh.Value;
            lbLowThresh.Text = "LOW: " + VoidScanner.ScannerProperties.thDown;
        }

        private void trackSkippedPixels_ValueChanged(object sender, EventArgs e)
        {
            VoidScanner.ScannerProperties.MaxSkip = trackSkippedPixels.Value;
            lbHunger.Text = "Hunger: " + VoidScanner.ScannerProperties.MaxSkip;
        }

        private void btnRebuild_Click(object sender, EventArgs e)
        {
            Scan(VoidScanner.ScannerProperties.LastScanPoint);
        }

        public void Undo()
        {
            if (VoidScanner.ScannerProperties.PrevScanPoint != null)
            {
                Scan(VoidScanner.ScannerProperties.PrevScanPoint);
            }
        }

        public void Scan(Point3D point)
        {
            if (point != null)
            {
                VoidScanner.ScannerProperties.LastScanPoint = point;
            }

            if (VoidScanner.ScannerProperties.LastScanPoint == null)
            {
                return;
            }

            Cursor = Cursors.WaitCursor;
            Owner.Enabled = false;

            Task.Factory.StartNew(() =>
            {
                VoidScanner.Build(VoidScanner.ScannerProperties.LastScanPoint, Axis.Z, Progress);
            }).ContinueWith((t) =>
            {
                Owner.Enabled = true;

                Cursor = Cursors.Default;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void chkOptimizePlanes_CheckedChanged(object sender, EventArgs e)
        {
            VoidScanner.ScannerProperties.OptimizePlanes = chkOptimizePlanes.Checked;
        }

        private void trackSmoothness_ValueChanged(object sender, EventArgs e)
        {
            VoidScanner.ScannerProperties.Smoothness = trackSmoothness.Value;
        }   
    }
}

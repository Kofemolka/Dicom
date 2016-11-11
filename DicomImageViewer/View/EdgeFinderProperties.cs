using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Model.Utils;
using DicomImageViewer.Scanners;
using Model;

namespace DicomImageViewer.View
{
    public partial class EdgeFinderProperties : UserControl, IScannerProperties
    {
        private Form Owner { get; set; }
        private IProgress Progress { get; set; }
        private EdgeFinder Scanner { get; set; }

        private Point3D _lastRayCast;

        public EdgeFinderProperties()
        {
            InitializeComponent();
        }

        public void Init(EdgeFinder scanner, IProgress progress, Form owner)
        {
            Owner = owner;
            Progress = progress;
            Scanner = scanner;

            trackThresh.Value = Scanner.Threshold;

            Parent.Tag = this;
        }

        private void trackThresh_ValueChanged(object sender, EventArgs e)
        {
            Scanner.Threshold = (ushort)trackThresh.Value;
            lbThresh.Text = "Threshold: " + Scanner.Threshold;
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
                Scanner.Build(_lastRayCast, Progress);
            }).ContinueWith((t) =>
            {
                Owner.Enabled = true;

                Cursor = Cursors.Default;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}

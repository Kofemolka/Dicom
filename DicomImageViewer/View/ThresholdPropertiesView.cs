using System;
using System.Windows.Forms;
using Model;
using Model.Utils;
using System.Threading.Tasks;
using Model.Scanners;

namespace DicomImageViewer.View
{
    public partial class ThresholdPropertiesView : UserControl, IScannerPropertiesView
    {
        private Form Owner { get; set; }
        private IProgress Progress { get; set; }
        private ThresholdScanner Scanner { get; set; }

        IScannerProperties IScannerPropertiesView.Properties
        {
            get { return Scanner.ScannerProperties; }

            set
            {
                var properties = value as ThresholdScanner.ThresholdScannerProperties;
                if (properties != null)
                {
                    Scanner.ScannerProperties = properties;

                    UpdateView();
                }
            }
        }

        private void UpdateView()
        {
            trackThresh.Value = Scanner.ScannerProperties.Threshold;
        }

        public ThresholdPropertiesView()
        {
            InitializeComponent();
        }

        public void Init(ThresholdScanner scanner, IProgress progress, Form owner)
        {
            Owner = owner;
            Progress = progress;
            Scanner = scanner;

            UpdateView();

            Parent.Tag = this;
        }

        private void trackThresh_ValueChanged(object sender, EventArgs e)
        {
            Scanner.ScannerProperties.Threshold = (ushort)trackThresh.Value;
            lbThresh.Text = "Threshold: " + Scanner.ScannerProperties.Threshold;
        }

        private void btnRebuild_Click(object sender, EventArgs e)
        {
            Scan(Scanner.ScannerProperties.LastScanPoint);
        }

        public void Scan(Point3D point)
        {
            if (point != null)
            {
                Scanner.ScannerProperties.LastScanPoint = point;
            }

            if (Scanner.ScannerProperties.LastScanPoint == null)
            {
                return;
            }

            Cursor = Cursors.WaitCursor;
            Owner.Enabled = false;

            Task.Factory.StartNew(() =>
            {
                Scanner.Build(Scanner.ScannerProperties.LastScanPoint, Progress);
            }).ContinueWith((t) =>
            {
                Owner.Enabled = true;

                Cursor = Cursors.Default;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}

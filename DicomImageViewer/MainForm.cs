using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using DicomImageViewer.Scanners;
using System.Windows.Forms.Integration;
using DicomImageViewer.Export;
using Model;
using Model.Utils;
using DicomImageViewer.View;

namespace DicomImageViewer
{
    public enum ImageBitsPerPixel { Eight, Sixteen, TwentyFour };
    public enum ViewSettings { Zoom1_1, ZoomToFit };

    public partial class MainForm : Form, IProbe
    {
        private ProjectionView projectionViewX;
        private ProjectionView projectionViewY;
        private ProjectionView projectionViewZ;

        private readonly ScanSet _scanSet = new ScanSet();
        private readonly ILookupTable _lookupTable;
        private readonly LabelMapSet _labelMapSet;
        private readonly VoidScanner _voidScanner;
        private readonly ThresholdScanner _threshScanner;

        private readonly DicomDecoder _dd;

        private View.View3D _3dView;

        private double winCentre;
        private double winWidth;

        private int maxPixelValue; // Updated July 2012
        private int minPixelValue;

        public View.View3D View3D => _3dView;

        public MainForm()
        {
            _labelMapSet = new LabelMapSet(action => this.Invoke(action));
            _lookupTable = new LookupTable(_scanSet);
            _voidScanner = new VoidScanner(_scanSet, _lookupTable, () => _labelMapSet.Current);
            _threshScanner = new ThresholdScanner(_scanSet, _lookupTable, () => _labelMapSet.Current);

            InitializeComponent();

            rayCastingProperties.Init(_voidScanner, new Progress(progBar), this);
            thresholdProperties.Init(_threshScanner, new Progress(progBar), this);

            labelMapView.LabelMapSet = _labelMapSet;
            labelMapView.Init();

            _dd = new DicomDecoder();

            _3dView = new View.View3D(_labelMapSet);
            _3dView.Dock = DockStyle.Fill;

            projectionViewX = new ProjectionView(Axis.X, _scanSet, _lookupTable, _labelMapSet, this);
            projectionViewY = new ProjectionView(Axis.Y, _scanSet, _lookupTable, _labelMapSet, this);
            projectionViewZ = new ProjectionView(Axis.Z, _scanSet, _lookupTable, _labelMapSet, this);

            maxPixelValue = 0;
            minPixelValue = 65535;

            InitUI();
        }

        private void InitUI()
        {

            this.projectionViewX.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.projectionViewX.BackColor = System.Drawing.SystemColors.Control;
            this.projectionViewX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.projectionViewX.CausesValidation = false;
            this.projectionViewX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectionViewX.Name = "projectionViewX";
            this.projectionViewX.TabIndex = 114;
            this.projectionViewX.TabStop = false;

            this.projectionViewY.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.projectionViewY.BackColor = System.Drawing.SystemColors.Control;
            this.projectionViewY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.projectionViewY.CausesValidation = false;
            this.projectionViewY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectionViewY.Name = "projectionViewY";
            this.projectionViewY.TabIndex = 114;
            this.projectionViewY.TabStop = false;

            this.projectionViewZ.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.projectionViewZ.BackColor = System.Drawing.SystemColors.Control;
            this.projectionViewZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.projectionViewZ.CausesValidation = false;
            this.projectionViewZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectionViewZ.Name = "projectionViewZ";
            this.projectionViewZ.TabIndex = 114;
            this.projectionViewZ.TabStop = false;

            this.tableLayoutPanel1.Controls.Add(this.projectionViewX, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.projectionViewY, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.projectionViewZ, 0, 3);
            this.tableLayoutPanel1.Controls.Add(_3dView, 1, 1);
        }

        private void UpdateScanInfo()
        {
            winCentre = _scanSet.windowCentre;
            winWidth = _scanSet.windowWidth;

            //bnSave.Enabled = true;
            bnTags.Enabled = true;

            Text = "DICOM Image Viewer: ";

            _scanSet.MinMaxDencity(out minPixelValue, out maxPixelValue);

            if (_scanSet.signedImage)
            {
                winCentre -= short.MinValue;
            }

            if (Math.Abs(winWidth) < 0.001)
            {
                winWidth = maxPixelValue - minPixelValue;
            }

            if ((winCentre == 0) ||
                (minPixelValue > winCentre) || (maxPixelValue < winCentre))
            {
                winCentre = (maxPixelValue + minPixelValue)/2;
            }
        }

        private void bnTags_Click(object sender, EventArgs e)
        {
            var dtg = new DicomTagsForm();
            dtg.SetString(_scanSet.DicomInfo);
            dtg.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _3dView.Load();
        }

        public void UpdateWindowLevel(int winWidth, int winCentre, ImageBitsPerPixel bpp)
        {
            int winMin = Convert.ToInt32(winCentre - 0.5*winWidth);
            int winMax = winMin + winWidth;
        }

        private void btnOpenSeries_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.SelectedPath = Properties.Settings.Default.LastFolder;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Cursor = Cursors.WaitCursor;
                    this.Enabled = false;

                    var files = Directory.GetFiles(dlg.SelectedPath);

                    var progIndicator = new Progress(progBar);

                    Task.Factory.StartNew(() =>
                    {
                        _scanSet.Reset();

                        LoadSlicesAsync(files, progIndicator);

                        _scanSet.Build(progIndicator);
                    }).ContinueWith((t) =>
                    {
                        this.Enabled = true;

                        _labelMapSet.Reset();

                        Cursor = Cursors.Default;

                        Properties.Settings.Default.LastFolder = dlg.SelectedPath;
                        Properties.Settings.Default.Save();

                        UpdateScanInfo();
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private void LoadSlicesAsync(string[] files, IProgress progress)
        {
            progress.Min(1);
            progress.Max(files.Length);
            progress.Reset();

            Parallel.ForEach(files, file =>
            {
                var decoder = new DicomDecoder();

                var slice = decoder.ReadDicomFile(file);

                _scanSet.AddSlice(slice);

                progress.Tick();
            });
        }

        public void Dencity(ushort density)
        {
            lbDensity.Text = "Density: " + density;
        }

        public void PointSelect(Point3D point)
        {
            var scanProp = (tabsScanners.SelectedTab.Tag as IScannerProperties);

            if (scanProp != null)
            {
                scanProp.Scan(point);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.SelectedPath = Properties.Settings.Default.LastFolder;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    LabelInfoExporter.Export(_labelMapSet, Path.Combine(dlg.SelectedPath, "Labels.csv"));

                    _3dView.Export(dlg.SelectedPath);
                }
            }
        }
    }
}
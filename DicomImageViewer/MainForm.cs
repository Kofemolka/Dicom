using System;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using DicomImageViewer.Export;
using Model;
using Model.Utils;
using DicomImageViewer.View;
using Model.Scanners;

namespace DicomImageViewer
{
    public enum ImageBitsPerPixel { Eight, Sixteen, TwentyFour };
    
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
        private readonly EdgeScanner _edgeScanner;
        private readonly CrossChecker _crossChecker;

        private View.View3D _3dView;

        public View.View3D View3D => _3dView;

        public MainForm()
        {
            _labelMapSet = new LabelMapSet(_scanSet, action => this.Invoke(action));
            _lookupTable = new LookupTable(_scanSet);
            _crossChecker = new CrossChecker(_labelMapSet);
            _voidScanner = new VoidScanner(_scanSet, _lookupTable, () => _labelMapSet.Current, _crossChecker);
            _threshScanner = new ThresholdScanner(_scanSet, _lookupTable, () => _labelMapSet.Current, _crossChecker);
            _edgeScanner = new EdgeScanner(_scanSet, _lookupTable, () => _labelMapSet.Current, _crossChecker);

            InitializeComponent();

            _rayCastingPropertiesView.Init(_voidScanner, new Progress(progBar), this);
            _thresholdPropertiesView.Init(_threshScanner, new Progress(progBar), this);
            edgeFinderProperties1.Init(_edgeScanner, new Progress(progBar), this);

            labelMapView.LabelMapSet = _labelMapSet;
            labelMapView.Init();
            
            _3dView = new View.View3D(_labelMapSet);
            _3dView.Dock = DockStyle.Fill;

            projectionViewX = new ProjectionView(Axis.X, _scanSet, _lookupTable, _labelMapSet, this);
            projectionViewY = new ProjectionView(Axis.Y, _scanSet, _lookupTable, _labelMapSet, this);
            projectionViewZ = new ProjectionView(Axis.Z, _scanSet, _lookupTable, _labelMapSet, this);

            _labelMapSet.LabelMapCurrentSelectionChanged += LabelMapSetOnLabelMapCurrentSelectionChanged;


            InitUI();
        }

        private void LabelMapSetOnLabelMapCurrentSelectionChanged()
        {
            if(_labelMapSet.Current.ScannerProperties is ThresholdScanner.ThresholdScannerProperties)
            {
                var view = FindIScannerPropertiesView<ThresholdPropertiesView>();

                view.Properties = _labelMapSet.Current.ScannerProperties;
            }
            else if (_labelMapSet.Current.ScannerProperties is EdgeScanner.EdgeScannerProperties)
            {
                var view = FindIScannerPropertiesView<EdgeFinderPropertiesView>();

                view.Properties = _labelMapSet.Current.ScannerProperties;
            }
            else if (_labelMapSet.Current.ScannerProperties is VoidScanner.VoidScannerProperties)
            {
                var view = FindIScannerPropertiesView<RayCastingPropertiesView>();

                view.Properties = _labelMapSet.Current.ScannerProperties;
            }
        }

        private IScannerPropertiesView FindIScannerPropertiesView<T>()
        {
            foreach (TabPage tabPage in tabsScanners.TabPages)
            {
                if (tabPage.Tag.GetType() == typeof (T))
                {
                    tabsScanners.SelectedTab = tabPage;
                    return tabPage.Tag as IScannerPropertiesView;
                }
            }

            return null;
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
            var scanProp = (tabsScanners.SelectedTab.Tag as IScannerPropertiesView);

            scanProp?.Scan(point);
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
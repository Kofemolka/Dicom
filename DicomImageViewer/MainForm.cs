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
using Model;
using Model.Utils;

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
        private readonly LabelMap _labelMap = new LabelMap();
        private readonly VoidScanner _voidScanner;

        private readonly DicomDecoder _dd;

        private View.View3D _3dView;
               
        double winCentre;
        double winWidth;
       
        int maxPixelValue;    // Updated July 2012
        int minPixelValue;

        private Point3D _lastRayCast;

        public View.View3D View3D => _3dView;

        public MainForm()
        {
            _lookupTable = new LookupTable(_scanSet);
            _voidScanner = new VoidScanner(_scanSet, _labelMap);

            InitializeComponent();
            _dd = new DicomDecoder();

            _3dView = new View.View3D(_labelMap);
            _3dView.Dock = DockStyle.Fill;

            projectionViewX = new ProjectionView(Axis.X, _scanSet, _lookupTable, _labelMap, this);
            projectionViewY = new ProjectionView(Axis.Y, _scanSet, _lookupTable, _labelMap, this);
            projectionViewZ = new ProjectionView(Axis.Z, _scanSet, _lookupTable, _labelMap, this);           

            maxPixelValue = 0;
            minPixelValue = 65535;
          
            trackHiThresh.Value = VoidScanner.thUp;
            trackLowThresh.Value = VoidScanner.thDown;
            trackSkippedPixels.Value = VoidScanner.MaxSkip;
            trackRays.Value = VoidScanner.Rays;
            
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

            // Bug fix dated 24 Aug 2013 - for proper window/level of signed images
            // Thanks to Matias Montroull from Argentina for pointing this out.
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
                winCentre = (maxPixelValue + minPixelValue) / 2;
            }
        }
        
        private void bnTags_Click(object sender, EventArgs e)
        {
            //if (imageOpened == true)
            //{
            //    List<string> str = slice.DicomInfo;

            //    DicomTagsForm dtg = new DicomTagsForm();
            //    dtg.SetString(ref str);
            //    dtg.ShowDialog();

            //    imagePanelControl.Invalidate();
            //}
            //else
            //    MessageBox.Show("Load a DICOM file before viewing tags!", "Information", 
            //        MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _3dView.Load();
        }              

        public void UpdateWindowLevel(int winWidth, int winCentre, ImageBitsPerPixel bpp)
        {
            int winMin = Convert.ToInt32(winCentre - 0.5 * winWidth);
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
            var filesCount = files.Length;
            var filesPerThread = filesCount/Environment.ProcessorCount;

            progress.Min(1);
            progress.Max(filesCount);
            progress.Reset();

            var tasks = new List<Task<List<Slice>>>();

            for (int t = 0; t < Environment.ProcessorCount; t++)
            {
                var t1 = t;
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var lastFileNdx = t1 == Environment.ProcessorCount
                        ? filesCount
                        : (t1*filesPerThread + filesPerThread);

                    var decoder = new DicomDecoder();

                    var res = new List<Slice>();

                    for (var f = t1*filesPerThread; f < lastFileNdx; f++)
                    {
                        res.Add(decoder.ReadDicomFile(files[f]));

                        progress.Tick();
                    }

                    return res;
                }));
            }

            Task.WaitAll(tasks.ToArray());

            foreach (var task in tasks)
            {
                foreach (var slice in task.Result)
                {
                    _scanSet.AddSlice(slice);
                }
            }
        }
        
        private void trackHiThresh_ValueChanged(object sender, EventArgs e)
        {
            VoidScanner.thUp = (ushort)trackHiThresh.Value;
        }

        private void trackLowThresh_ValueChanged(object sender, EventArgs e)
        {
            VoidScanner.thDown = (ushort)trackLowThresh.Value;
        }

        private void RayCast(Point3D point = null)
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
            this.Enabled = false;
            
            Task.Factory.StartNew(() =>
            {
                _voidScanner.Build(_lastRayCast, Axis.Z);

                var volume = _voidScanner.CalculateVolume();

                return volume;
            }).ContinueWith((t) =>
            {
                if(t != null)
                {
                    lbVolume.Text = "Volume: " + (int)t.Result + " (mm3)";
                }
                this.Enabled = true;

                Cursor = Cursors.Default;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void trackSkippedPixels_ValueChanged(object sender, EventArgs e)
        {
            VoidScanner.MaxSkip = trackSkippedPixels.Value;
        }

        public void Dencity(ushort density)
        {
            lbDensity.Text = "Density: " + density;
        }

        public void PointSelect(Point3D point)
        {
            RayCast(point);
        }

        private void btnRebuild_Click(object sender, EventArgs e)
        {
            RayCast();
        }

        private void trackRays_ValueChanged(object sender, EventArgs e)
        {
            VoidScanner.Rays = trackRays.Value;
        }
    }
}
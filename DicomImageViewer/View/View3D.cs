
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.IO;
using System.Threading.Tasks;
using Model;
using Point3D = Model.Point3D;

namespace DicomImageViewer.View
{
    public partial class View3D : UserControl
    {
        private readonly LabelMapSet _labelMapSet;
        private readonly Wpf3DView.ViewPort _viewPort;

        public View3D(LabelMapSet labelMapSet)
        {
            _labelMapSet = labelMapSet;

            InitializeComponent();

            var host = new ElementHost {Dock = DockStyle.Fill};

            _viewPort = new Wpf3DView.ViewPort();
            host.Child = _viewPort;
            this.Controls.Add(host);

            host.HostContainer.MouseEnter += (a, b) => _viewPort.Focus();

            _labelMapSet.LabelMapAdded += LabelMapUpdated;
            _labelMapSet.LabelMapUpdated += LabelMapUpdated;
            _labelMapSet.LabelMapDeleted += LabelMapDeleted;
            _labelMapSet.LabelMapSetReset += LabelMapSetReset;
        }

        private void LabelMapSetReset()
        {
            Invoke(new MethodInvoker(() => _viewPort.Reset()));
        }

        private void LabelMapUpdated(ILabelMap label)
        {
            Invoke(new MethodInvoker(() => _viewPort.UpdateLabel(label)));
        }

        private void LabelMapDeleted(ILabelMap label)
        {
            Invoke(new MethodInvoker(() => _viewPort.RemoveLabel(label)));
        }
        
        private void Save(IEnumerable<Point3D> points)
        {
            using (StreamWriter writetext = new StreamWriter("points.txt"))
            {
                foreach(var p in points)
                {
                    writetext.WriteLine(p.X + " " + p.Y + " " + p.Z + " " + p.Index);
                }
            }
        }

        public void Load()
        {
            try
            {
                var points = new List<Point3D>();

                string line;
                System.IO.StreamReader file = new System.IO.StreamReader("points.txt");
                while ((line = file.ReadLine()) != null)
                {
                    var coords = line.Split(' ');
                    if (coords.Length < 4)
                    {
                        continue;
                    }

                    var p = new Point3D(Int32.Parse(coords[0]),
                        Int32.Parse(coords[1]),
                        Int32.Parse(coords[2]));
                    p.Index = Int32.Parse(coords[3]);

                    points.Add(p);
                }

                file.Close();

               //this.Invoke(new MethodInvoker(() => _viewPort.SetModel(points)));
            }catch(Exception e)
            {

            }
        }

        
    }
}

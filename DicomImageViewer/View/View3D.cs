
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

            _labelMapSet.LabelMapSetReady += BuildModel;            
        }

        private void BuildModel()
        {
            try
            {
                Invoke(new MethodInvoker(() => _viewPort.Reset()));

                Task.Factory.StartNew(() =>
                {
                    foreach (var label in _labelMapSet.All)
                    {
                        Invoke(
                            new MethodInvoker(() => _viewPort.AddModel(label.GetAll(), new System.Windows.Media.Color()
                            {
                                A = label.Color.A,
                                R = label.Color.R,
                                G = label.Color.G,
                                B = label.Color.B
                            },
                                label.BuildMethod)));
                    }
                }, TaskCreationOptions.LongRunning);

                //Save(_labelMap.GetAll());
            }
            catch (Exception e)
            {
                
            }
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

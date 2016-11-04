using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Wpf3DView
{    
    public partial class ViewPort : UserControl
    {
        private readonly Trackball _trackball;
        private readonly Model3DGroup _modelGroup = new Model3DGroup();
        private readonly ModelVisual3D _modelsVisual = new ModelVisual3D();
        private PerspectiveCamera _camera;        

        private readonly ScaleTransform3D _scaleTransform3D = new ScaleTransform3D(1, 1, 1);
        private readonly RotateTransform3D _rotateTransform3D = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0));
        private readonly TranslateTransform3D _translateTransform3D = new TranslateTransform3D(0, 0, 0);

        public Trackball Trackball => _trackball;

        public ViewPort()
        {
            InitializeComponent();          

            InitScene();

            _trackball = new Trackball();
            _trackball.Attach(this);
            _trackball.Slaves.Add(myViewport);
            _trackball.Enabled = true;            
        }

        public void Reset()
        {
            _modelGroup.Children.Clear();

            DirectionalLight DirLight1 = new DirectionalLight();
            DirLight1.Color = Colors.White;
            DirLight1.Direction = new Vector3D(-1, -1, -1);

            DirectionalLight DirLight2 = new DirectionalLight();
            DirLight2.Color = Colors.White;
            DirLight2.Direction = new Vector3D(1, 1, 1);

            _modelGroup.Children.Add(DirLight1);
            _modelGroup.Children.Add(DirLight2);
        }

        public void AddModel(IEnumerable<Model.Point3D> points, Color color)
        {
            //Point3D center;
            var geo = BuildMesh(points);
            
            var model = new GeometryModel3D();
            model.Material = new DiffuseMaterial(new SolidColorBrush(color));

            model.Geometry = geo;
            _modelGroup.Children.Add(model);

            var center = CalculateCenter();

            _scaleTransform3D.CenterX = _rotateTransform3D.CenterX = center.X;
            _scaleTransform3D.CenterY = _rotateTransform3D.CenterY = center.Y;
            _scaleTransform3D.CenterZ = _rotateTransform3D.CenterZ = center.Z;

            _camera.LookDirection = new Vector3D(center.X, center.Y, center.Z);
            _camera.UpDirection = new Vector3D(0, 0, 1);
                        
           
            myViewport.UpdateLayout();
        }        

        private Point3D CalculateCenter()
        {
            var center = new Point3D();
            long counter = 0;

            foreach (var model in _modelGroup.Children)
            {
                if (model is GeometryModel3D)
                {
                    foreach (var p in ((model as GeometryModel3D).Geometry as MeshGeometry3D).Positions)
                    {
                        center.X += p.X;
                        center.Y += p.Y;
                        center.Z += p.Z;

                        counter++;
                    }
                }
            }

            if(counter > 0)
            {
                center.X /= counter;
                center.Y /= counter;
                center.Z /= counter;
            }

            return center;
        }

        private void InitScene()
        {
            Reset();

            _camera = new PerspectiveCamera();
            _camera.FarPlaneDistance = 1000;
            _camera.NearPlaneDistance = 10;
            _camera.FieldOfView = 60;
            _camera.Position = new Point3D(0, 0, 0);
            _camera.LookDirection = new Vector3D(-2, -2, -3);
            _camera.UpDirection = new Vector3D(0, 1, 0);
            
            _modelsVisual.Content = _modelGroup;
            
            Transform3DGroup t = new Transform3DGroup();
            t.Children.Add(_scaleTransform3D);
            t.Children.Add(_rotateTransform3D);
            t.Children.Add(_translateTransform3D);

            _modelsVisual.Transform = t;

            
            myViewport.Camera = _camera;            
            myViewport.Children.Add(_modelsVisual);
            myViewport.Height = this.Width;
            myViewport.Width = this.Height;
            Canvas.SetTop(myViewport, 0);
            Canvas.SetLeft(myViewport, 0);
            this.Width = myViewport.Width;
            this.Height = myViewport.Height;
        }

        private void AddCubeToMesh(MeshGeometry3D mesh, Point3D center, double size)
        {
            if (mesh != null)
            {
                int offset = mesh.Positions.Count;

                mesh.Positions.Add(new Point3D(center.X - size, center.Y + size, center.Z - size));
                mesh.Positions.Add(new Point3D(center.X + size, center.Y + size, center.Z - size));
                mesh.Positions.Add(new Point3D(center.X + size, center.Y + size, center.Z + size));
                mesh.Positions.Add(new Point3D(center.X - size, center.Y + size, center.Z + size));
                mesh.Positions.Add(new Point3D(center.X - size, center.Y - size, center.Z - size));
                mesh.Positions.Add(new Point3D(center.X + size, center.Y - size, center.Z - size));
                mesh.Positions.Add(new Point3D(center.X + size, center.Y - size, center.Z + size));
                mesh.Positions.Add(new Point3D(center.X - size, center.Y - size, center.Z + size));

                mesh.TriangleIndices.Add(offset + 3);
                mesh.TriangleIndices.Add(offset + 2);
                mesh.TriangleIndices.Add(offset + 6);

                mesh.TriangleIndices.Add(offset + 3);
                mesh.TriangleIndices.Add(offset + 6);
                mesh.TriangleIndices.Add(offset + 7);

                mesh.TriangleIndices.Add(offset + 2);
                mesh.TriangleIndices.Add(offset + 1);
                mesh.TriangleIndices.Add(offset + 5);

                mesh.TriangleIndices.Add(offset + 2);
                mesh.TriangleIndices.Add(offset + 5);
                mesh.TriangleIndices.Add(offset + 6);

                mesh.TriangleIndices.Add(offset + 1);
                mesh.TriangleIndices.Add(offset + 0);
                mesh.TriangleIndices.Add(offset + 4);

                mesh.TriangleIndices.Add(offset + 1);
                mesh.TriangleIndices.Add(offset + 4);
                mesh.TriangleIndices.Add(offset + 5);

                mesh.TriangleIndices.Add(offset + 0);
                mesh.TriangleIndices.Add(offset + 3);
                mesh.TriangleIndices.Add(offset + 7);

                mesh.TriangleIndices.Add(offset + 0);
                mesh.TriangleIndices.Add(offset + 7);
                mesh.TriangleIndices.Add(offset + 4);

                mesh.TriangleIndices.Add(offset + 7);
                mesh.TriangleIndices.Add(offset + 6);
                mesh.TriangleIndices.Add(offset + 5);

                mesh.TriangleIndices.Add(offset + 7);
                mesh.TriangleIndices.Add(offset + 5);
                mesh.TriangleIndices.Add(offset + 4);

                mesh.TriangleIndices.Add(offset + 2);
                mesh.TriangleIndices.Add(offset + 3);
                mesh.TriangleIndices.Add(offset + 0);

                mesh.TriangleIndices.Add(offset + 2);
                mesh.TriangleIndices.Add(offset + 0);
                mesh.TriangleIndices.Add(offset + 1);
            }
        }

        private MeshGeometry3D BuildMesh(IEnumerable<Model.Point3D> pointCloud)
        {
            var zOrderCloud = new Dictionary<int, List<Model.Point3D>>();

            foreach (var point3D in pointCloud)
            {
                List<Model.Point3D> points;
                if (!zOrderCloud.TryGetValue(point3D.Z, out points))
                {
                    points = new List<Model.Point3D>();
                    zOrderCloud.Add(point3D.Z, points);
                }
                
                points.Add(point3D);
            }

            MeshGeometry3D geo = new MeshGeometry3D();

            if (!zOrderCloud.Any())
            {               
                return geo;
            }

            var hashPoint = new Dictionary<Model.Point3D, int>();

            var keys = zOrderCloud.Keys.OrderBy(i => i).ToArray();

            for (var z = 0; z < keys.Length - 1; z++)
            {
                BuildSurface(geo, hashPoint, zOrderCloud[keys[z]], zOrderCloud[keys[z+1]]);
            }

            BuildEndPlanes(geo, hashPoint, zOrderCloud[keys[0]], true);
            BuildEndPlanes(geo, hashPoint, zOrderCloud[keys[keys.Length-1]], false);

            foreach (var source in hashPoint.OrderBy(pair => pair.Value))
            {
                geo.Positions.Add(new Point3D(source.Key.X, source.Key.Y, source.Key.Z));
            }
            
            return geo;
        }

        private void BuildSurface(MeshGeometry3D geo, Dictionary<Model.Point3D, int> hashPoint, List<Model.Point3D> ptUp, List<Model.Point3D> ptDown)
        {
            if (ptUp.Count != ptDown.Count)
            {
                return;
            }

            var ptUpOrd = ptUp.OrderBy(d => d.Index).ToList();
            var ptDownOrd = ptDown.OrderBy(d => d.Index).ToArray();

            ptUpOrd.Add(ptUpOrd.First()); //add first vertex for convinient iteration

            for (var upNdx = 0; upNdx < ptUp.Count; upNdx++)
            {
                AddTriangle(geo, hashPoint, ptUpOrd[upNdx], ptUpOrd[upNdx + 1], ptDownOrd[upNdx]);

                var prevDownNdx = upNdx == 0 ? ptUp.Count - 1 : upNdx - 1;
                AddTriangle(geo, hashPoint, ptUpOrd[upNdx], ptDownOrd[upNdx], ptDownOrd[prevDownNdx]);
            }
        }

        private static int AddPoint(Dictionary<Model.Point3D, int> points, Model.Point3D point)
        {
            int ndx;
            if (points.TryGetValue(point, out ndx))
                return ndx;
            
            points.Add(point, points.Count);
            return points.Count - 1;
        }

        private void AddTriangle(MeshGeometry3D geo, Dictionary<Model.Point3D, int> hashPoint, Model.Point3D p1, Model.Point3D p2, Model.Point3D p3)
        {
            var ndx1 = AddPoint(hashPoint, p1);
            var ndx2 = AddPoint(hashPoint, p2);
            var ndx3 = AddPoint(hashPoint, p3);

            geo.TriangleIndices.Add(ndx1);
            geo.TriangleIndices.Add(ndx2);
            geo.TriangleIndices.Add(ndx3);
        }

        private void BuildEndPlanes(MeshGeometry3D geo, Dictionary<Model.Point3D, int> hashPoint, List<Model.Point3D> points, bool counter)
        {
            if (points.Count < 3)
                return;

            var center = new Model.Point3D();
            
            var ndx = new List<int>();
            foreach (var point3D in points)
            {
                ndx.Add(AddPoint(hashPoint, point3D));

                center.X += point3D.X;
                center.Y += point3D.Y;
                center.Z += point3D.Z;
            }

            ndx.Add(AddPoint(hashPoint, points.First()));

            center.X /= points.Count;
            center.Y /= points.Count;
            center.Z /= points.Count;

            var centerNdx = AddPoint(hashPoint, center);

            for (var i = 0; i < points.Count; i++)
            {
                if (counter)
                {
                    geo.TriangleIndices.Add(ndx[i + 1]);
                    geo.TriangleIndices.Add(ndx[i]);
                    geo.TriangleIndices.Add(centerNdx);
                }
                else
                {
                    geo.TriangleIndices.Add(centerNdx);
                    geo.TriangleIndices.Add(ndx[i]);
                    geo.TriangleIndices.Add(ndx[i + 1]);
                }
            }
        }

        private Point3D CalculateCenter(List<Model.Point3D> topPlane, List<Model.Point3D> bottomPlane)
        {
            var center = new Point3D();

            foreach (var mark in topPlane)
            {
                center.X += mark.X;
                center.Y += mark.Y;
                center.Z += mark.Z;
            }

            foreach (var mark in bottomPlane)
            {
                center.X += mark.X;
                center.Y += mark.Y;
                center.Z += mark.Z;
            }

            center.X /= (topPlane.Count + bottomPlane.Count);
            center.Y /= (topPlane.Count + bottomPlane.Count);
            center.Z /= (topPlane.Count + bottomPlane.Count);

            return center;
        }
    }
}

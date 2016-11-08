using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace DicomImageViewer.Export
{
    public class StlExporter 
    {
        public static void Export(GeometryModel3D model, Stream stream)
        {
            using (var writer = new BinaryWriter(stream))
            {
                var meshGeo = model.Geometry as MeshGeometry3D;
                
                ExportHeader(writer, meshGeo.TriangleIndices.Count/3);

                ExportModel(writer, model, Transform3D.Identity);
            }
        }

        private static void ExportHeader(BinaryWriter writer, int triangleCount)
        {
            ExportHeader(writer);
            writer.Write(triangleCount);
        }
        
        protected static void ExportHeader(BinaryWriter writer)
        {
            writer.Write(new byte[80]);
        }

        protected static void ExportModel(BinaryWriter writer, GeometryModel3D model, Transform3D t)
        {
            var mesh = (MeshGeometry3D) model.Geometry;

            var normals = mesh.Normals;
            if (normals == null || normals.Count != mesh.Positions.Count)
            {
                normals = CalculateNormals(mesh.Positions, mesh.TriangleIndices);
            }

            // TODO: Also handle non-uniform scale
            var matrix = t.Clone().Value;
            matrix.OffsetX = 0;
            matrix.OffsetY = 0;
            matrix.OffsetZ = 0;
            var normalTransform = new MatrixTransform3D();

            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                int i0 = mesh.TriangleIndices[i + 0];
                int i1 = mesh.TriangleIndices[i + 1];
                int i2 = mesh.TriangleIndices[i + 2];

                // Normal
                var faceNormal = normalTransform.Transform(normals[i0] + normals[i1] + normals[i2]);
                faceNormal.Normalize();
                WriteVector(writer, faceNormal);

                // Vertices
                WriteVertex(writer, t.Transform(mesh.Positions[i0]));
                WriteVertex(writer, t.Transform(mesh.Positions[i1]));
                WriteVertex(writer, t.Transform(mesh.Positions[i2]));

                // Attributes
                const ushort attribute = 0;
                writer.Write(attribute);
            }
        }

        private static void WriteVector(BinaryWriter writer, Vector3D normal)
        {
            writer.Write((float)normal.X);
            writer.Write((float)normal.Y);
            writer.Write((float)normal.Z);
        }

        private static void WriteVertex(BinaryWriter writer, Point3D p)
        {
            writer.Write((float)p.X);
            writer.Write((float)p.Y);
            writer.Write((float)p.Z);
        }

        public static Vector3DCollection CalculateNormals(IList<Point3D> positions, IList<int> triangleIndices)
        {
            var normals = new Vector3DCollection(positions.Count);
            for (int i = 0; i < positions.Count; i++)
            {
                normals.Add(new Vector3D());
            }

            for (int i = 0; i < triangleIndices.Count; i += 3)
            {
                int index0 = triangleIndices[i];
                int index1 = triangleIndices[i + 1];
                int index2 = triangleIndices[i + 2];
                var p0 = positions[index0];
                var p1 = positions[index1];
                var p2 = positions[index2];
                Vector3D u = p1 - p0;
                Vector3D v = p2 - p0;
                Vector3D w = Vector3D.CrossProduct(u, v);
                w.Normalize();
                normals[index0] += w;
                normals[index1] += w;
                normals[index2] += w;
            }

            for (int i = 0; i < normals.Count; i++)
            {
                var w = normals[i];
                w.Normalize();
                normals[i] = w;
            }

            return normals;
        }
    }
}

using Model;
using Model.Scanners;

namespace DicomImageViewer.View
{
    public interface IScannerPropertiesView
    {
        IScannerProperties Properties { get; set; }
        void Scan(Point3D point);
        void Undo();
    }
}

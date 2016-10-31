using DicomImageViewer.Model;

namespace DicomImageViewer.Utils
{
    public interface IProbe
    {
        void Dencity(ushort density);
        void PointSelect(Point3D point);
    }
}

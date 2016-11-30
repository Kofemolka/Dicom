using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Scanners
{
    public interface ICrossChecker
    {
        void Prepare(int layerId);
        bool Check(Point3D point, int layerId);
    }

    public class CrossChecker : ICrossChecker
    {
        private readonly LabelMapSet _labelMapSet;
        private List<ILabelMap> _labelMaps = new List<ILabelMap>();
         
        public CrossChecker(LabelMapSet labelMapSet)
        {
            _labelMapSet = labelMapSet;
        }

        public void Prepare(int layerId)
        {
            _labelMaps = _labelMapSet.All.Where(l => l.Id != layerId && l.ScannerProperties.BuildMethod != BuildMethod.Threshold).ToList();
        }

        public bool Check(Point3D point, int layerId)
        {
            foreach (var labelMap in _labelMaps)
            {
                var crop = labelMap.GetLayerCrop(point.Z);

                var p2d = point.To2D(Axis.Z);

                if (crop.InCrop(p2d))
                {
                    var proj = labelMap.GetProjection(Axis.Z, point.Z);
                    if (checkProjection(p2d, proj))
                    {
                        
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool checkProjection(Point2D point, IEnumerable<Point3D> proj)
        {
            var projList = proj.ToList();

            var c = false;
            int i, j = 0;
            for (i = 0, j = projList.Count - 1; i < projList.Count; j = i++)
            {
                if (((projList[i].Y > point.Y) != (projList[j].Y > point.Y)) &&
                    (point.X < (projList[j].X - projList[i].X)*(point.Y - projList[i].Y)/(projList[j].Y - projList[i].Y) + projList[i].X))
                {
                    c = !c;
                }
            }

            return c;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Scanners
{
    public class CrossChecker
    {
        private readonly LabelMapSet _labelMapSet;

        public CrossChecker(LabelMapSet labelMapSet)
        {
            _labelMapSet = labelMapSet;
        }

        public bool Check(Point3D point, int layerId)
        {
            bool res = false;
            Parallel.ForEach(_labelMapSet.All.Where(l => l.Id != layerId), (map, state)=>
            {
                var proj = map.GetProjection(Axis.Z, point.Z);

                if (checkProjection(point.To2D(Axis.Z), proj))
                {
                    state.Stop();
                    res = true;
                }
            });

            return res;
        }

        private static bool checkProjection(Point2D point, IEnumerable<Point3D> proj)
        {
            return false;
        }

        //int pnpoly(int nvert, float* vertx, float* verty, float testx, float testy)
        //{
        //    int i, j, c = 0;
        //    for (i = 0, j = nvert - 1; i < nvert; j = i++)
        //    {
        //        if (((verty[i] > testy) != (verty[j] > testy)) &&
        //         (testx < (vertx[j] - vertx[i]) * (testy - verty[i]) / (verty[j] - verty[i]) + vertx[i]))
        //            c = !c;
        //    }
        //    return c;
        //}
    }
}

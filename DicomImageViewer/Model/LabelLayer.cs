using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomImageViewer.Model
{
    public class LabelLayer
    {
        public Point3D Center { get; set; }
        public List<Point3D> Points { get; set; } 
    }
}

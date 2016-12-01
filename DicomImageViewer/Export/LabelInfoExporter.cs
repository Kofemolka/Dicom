using Model;

namespace DicomImageViewer.Export
{
    class LabelInfoExporter
    {
        public static void Export(LabelMapSet set, string fileName)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName, false))
            {
                file.WriteLine("Label, Volume (mm3)");

                foreach (var labelMap in set.All)
                {
                    file.WriteLine(labelMap.Name + ", " + labelMap.Volume);
                }
            }
        }
    }
}

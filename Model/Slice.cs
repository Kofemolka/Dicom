using System.Collections.Generic;

namespace Model
{
    public class Slice
    {
        public List<string> DicomInfo = new List<string>();

        public TypeOfDicomFile typeofDicomFile = TypeOfDicomFile.NotDicom;
        public int SamplesPerPixel = 1;
        public int bitsAllocated = 0;
        public int width = 1;
        public int height = 1;
        public double windowCentre = 0;
        public double windowWidth = 0;
        public bool signedImage = false;

        public double XRes { get; set; }
        public double YRes { get; set; }
        public double ZRes { get; set; }

        public List<ushort> pixels;

        public void GetPixels(ref List<ushort> pxs)
        {
            pxs.AddRange(pixels);
        }
    }
}

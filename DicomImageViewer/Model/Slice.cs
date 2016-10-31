using System.Collections.Generic;

namespace DicomImageViewer.Model
{
    class Slice
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

        //public List<byte> pixels8;
        //public List<byte> pixels24; // 8 bits bit depth, 3 samples per pixel
        public List<ushort> pixels;


        //public void GetPixels8(ref List<byte> pixels)
        //{
        //    pixels.AddRange(pixels8);
        //}

        public void GetPixels(ref List<ushort> pxs)
        {
            pxs.AddRange(pixels);
        }

        //public void GetPixels24(ref List<byte> pixels)
        //{
        //    pixels.AddRange(pixels24);
        //}
    }
}

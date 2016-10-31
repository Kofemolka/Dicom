namespace DicomImageViewer.Model
{
    public interface ILookupTable
    {
        byte Map(ushort val);
    }

    class LookupTable : ILookupTable
    {
        private readonly byte[] _lut = new byte[65536];
        private readonly IScanData _scanData;

        public LookupTable(IScanData scanData)
        {
            _scanData = scanData;

            _scanData.DataUpdated += Compute;
        }

        public byte Map(ushort val)
        {
            return _lut[val];
        }

        private void Compute()
        {
            int winMin, winMax;
            _scanData.MinMaxDencity(out winMin, out winMax);

            int range = winMax - winMin;
            if (range < 1) range = 1;
            double factor = 255.0 / range;
            int i;

            for (i = 0; i < 65536; ++i)
            {
                if (i <= winMin)
                    _lut[i] = 0;
                else if (i >= winMax)
                    _lut[i] = 255;
                else
                {
                    _lut[i] = (byte)((i - winMin) * factor);
                }
            }
        }
    }
}

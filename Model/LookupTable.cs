using System.ComponentModel;
using System.Runtime.CompilerServices;
using Model.Annotations;

namespace Model
{
    public delegate void MapChangedEvent();

    public interface ILookupTable
    {
        byte Map(ushort val);

        event MapChangedEvent MapChanged;
    }

    public class LookupTable : ILookupTable, INotifyPropertyChanged
    {
        private readonly byte[] _lut = new byte[65536];
        private readonly IScanData _scanData;

        private int _winMin;
        private int _winMax;
        private byte _intMin = 0;
        private byte _intMax = 255;

        public LookupTable(IScanData scanData)
        {
            _scanData = scanData;

            _scanData.DataUpdated += () =>
            {

                int winMin, winMax;
                _scanData.MinMaxDencity(out winMin, out winMax);

                WinMin = winMin;
                WinMax = winMax;

                Compute();
            };

            PropertyChanged += (sender, args) => Compute();
        }


        public int WinMin
        {
            get { return _winMin; }
            set
            {
                if (value >= _winMax)
                {
                    value = _winMax;
                }

                if (value != _winMin)
                {
                    _winMin = value;
                    OnPropertyChanged(nameof(WinMin));
                }
            } 
        }
        public int WinMax
        {
            get { return _winMax; }
            set
            {
                if (value <= _winMin)
                {
                    value = _winMin;
                }

                if (value != _winMax)
                {
                    _winMax = value;
                    OnPropertyChanged(nameof(WinMax));
                }
            }
        }

        public int IntMin
        {
            get { return _intMin; }
            set
            {
                if (value > 255)
                {
                    value = 255;
                }

                if (value >= _intMax)
                {
                    value = _intMax;
                }

                if (value != _intMin)
                {
                    _intMin = (byte) value;
                    OnPropertyChanged(nameof(IntMin));
                }
            }
        }

        public int IntMax
        {
            get { return _intMax; }
            set
            {
                if (value > 255)
                {
                    value = 255;
                }

                if (value <= _intMin)
                {
                    value = _intMin;
                }

                if (value != _intMax)
                {
                    _intMax = (byte) value;
                    OnPropertyChanged(nameof(IntMax));
                }
            }
        }
       
        public byte Map(ushort val)
        {
            return _lut[val];
        }

        public event MapChangedEvent MapChanged;

        private void Compute()
        {
            int range = WinMax - WinMin;
            if (range < 1) range = 1;
            double factor = (float)(IntMax-IntMin) / range;
            int i;

            for (i = 0; i < 65536; ++i)
            {
                if (i <= WinMin)
                    _lut[i] = (byte)IntMin;
                else if (i >= WinMax)
                    _lut[i] = (byte)IntMax;
                else
                {
                    _lut[i] = (byte)(IntMin + (i - WinMin) * factor);
                }
            }

            MapChanged?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

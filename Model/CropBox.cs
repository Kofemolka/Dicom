using System;

namespace Model
{
    public delegate void CropChangedEvent();

    public interface ICropBox
    {
        int XL { get; set; }
        int XR { get; set; }
        int YL { get; set; }
        int YR { get; set; }
        int ZL { get; set; }
        int ZR { get; set; }

        bool Visible { get; set; }

        bool IsInCrop(Point3D point, out Point3D bounded);
        
        event CropChangedEvent CropChanged;
    }

    class CropBox : ICropBox
    {
        private int _xl;
        private int _xr;
        private int _yl;
        private int _yr;
        private int _zl;
        private int _zr;
        private bool _visible = true;

        public bool IsInCrop(Point3D point, out Point3D bounded)
        {
            bounded = new Point3D(point);

            if (point.X < XL)
            {
                bounded.X = XL;
                return false;
            }

            if (point.X > XR)
            {
                bounded.X = XR;
                return false;
            }

            if (point.Y < YL)
            {
                bounded.Y = YL;
                return false;
            }

            if (point.Y > YR)
            {
                bounded.Y = YR;
                return false;
            }

            if (point.Z < ZL)
            {
                bounded.Z = ZL;
                return false;
            }

            if (point.Z > ZR)
            {
                bounded.Z = ZR;
                return false;
            }

            return true;
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                CropChanged?.Invoke();
            }
        }

        public int XL
        {
            get { return _xl; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value > _xr)
                {
                    value = _xr;
                }

                _xl = value;

                CropChanged?.Invoke();
            }
        }

        public int XR
        {
            get { return _xr; }
            set
            {
                if (value < _xl)
                {
                    value = _xl;
                }

                _xr = value;

                CropChanged?.Invoke();
            }
        }

        public int YL
        {
            get { return _yl; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value > _yr)
                {
                    value = _yr;
                }

                _yl = value;

                CropChanged?.Invoke();
            }
        }

        public int YR
        {
            get { return _yr; }
            set
            {
                if (value < _yl)
                {
                    value = _yl;
                }

                _yr = value;

                CropChanged?.Invoke();
            }
        }

        public int ZL
        {
            get { return _zl; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value > _zr)
                {
                    value = _zr;
                }

                _zl = value;

                CropChanged?.Invoke();
            }
        }

        public int ZR
        {
            get { return _zr; }
            set
            {
                if (value < _zl)
                {
                    value = _zl;
                }

                _zr = value;

                CropChanged?.Invoke();
            }
        }

        public event CropChangedEvent CropChanged;
    }
}

namespace Model.Scanners
{
    public interface IScannerProperties
    {
        Point3D LastScanPoint { get; set; }
        BuildMethod BuildMethod { get; }

        Point3D PrevScanPoint { get; set; }
    }

    public abstract class BaseScannerProperties : IScannerProperties
    {
        public abstract BuildMethod BuildMethod { get; }
        public Point3D LastScanPoint
        {
            get
            {
                return _lastScanPoint;
            }
            set
            {
                PrevScanPoint = LastScanPoint;
                _lastScanPoint = value;
            }
        }

        public Point3D PrevScanPoint { get; set; }

        private Point3D _lastScanPoint = null;
    }
}

namespace Model.Scanners
{
    public interface IScannerProperties
    {
        Point3D LastScanPoint { get; set; }
        BuildMethod BuildMethod { get; }
    }
}

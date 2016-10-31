namespace DicomImageViewer.Utils
{
    public interface IProgress
    {
        void Min(int value);
        void Max(int value);
        void Reset();

        void Tick();
    }
}

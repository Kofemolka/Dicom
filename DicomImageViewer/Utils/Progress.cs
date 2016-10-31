using System.Windows.Forms;

namespace DicomImageViewer.Utils
{
    class Progress : IProgress
    {
        private readonly ProgressBar _progress;

        public Progress(ProgressBar progress)
        {
            _progress = progress;
        }

        public void Min(int value)
        {
            _progress.Invoke(new MethodInvoker(() => _progress.Minimum = value));
        }

        public void Max(int value)
        {
            _progress.Invoke(new MethodInvoker(() => _progress.Maximum = value));
        }

        public void Reset()
        {
            _progress.Invoke(new MethodInvoker(() => _progress.Value = _progress.Minimum));
        }

        public void Tick()
        {
            _progress.Invoke(new MethodInvoker(() => _progress.Increment(1)));
        }
    }
}

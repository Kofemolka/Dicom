using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Model
{
    public class LabelMapSet
    {
        private readonly List<LabelMap> _labelMaps = new List<LabelMap>();
        private readonly Color[] _colors =
            {
                Color.OrangeRed,
                Color.LightGreen,
                Color.Pink,
                Color.Violet,
                Color.LightSkyBlue,
                Color.LightYellow,
                Color.Magenta,
                Color.Olive,
                Color.Moccasin,
                Color.PapayaWhip
            };

        public event LabelDataChangedEvent LabelDataChanged;

        public ILabelMap Current()
        {
            if(!_labelMaps.Any())
            {
                Add();
            }

            return _labelMaps.Last();
        }

        public void Add()
        {
            CreateLabelMap();

            LabelDataChanged?.Invoke();
        }

        public void Reset()
        {
            _labelMaps.Clear();

            LabelDataChanged?.Invoke();
        }

        public IEnumerable<ILabelMap> GetAll()
        {
            foreach(var label in _labelMaps)
            {
                yield return label;
            }
        }

        private LabelMap CreateLabelMap()
        {
            var label = new LabelMap();
            label.Color = GetNextColor();
            _labelMaps.Add(label);

            label.LabelDataChanged += () => LabelDataChanged?.Invoke();

            return label;
        }

        private Color GetNextColor()
        {
            if(_labelMaps.Count > _colors.Length)
            {
                return Color.White;
            }

            return _colors[_labelMaps.Count];
        }
    }
}

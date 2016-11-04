using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Drawing;

namespace Model
{
    public class LabelMapSet
    {
        private readonly BindingList<ILabelMap> _labelMaps = new BindingList<ILabelMap>();
        private readonly Action<Action> _syncInvoker;
        public BindingList<ILabelMap> All => _labelMaps;
        private ILabelMap _current = null;

        private readonly Color[] _colors =
        {
            Color.OrangeRed,
            Color.LightGreen,
            Color.Pink,
            Color.LightSkyBlue,
            Color.Violet,
            Color.Moccasin,
            Color.LightYellow,
            Color.Magenta,
            Color.Olive,
            Color.PapayaWhip
        };

        public event LabelDataChangedEvent LabelDataChanged;

        public LabelMapSet(Action<Action>  syncInvoker)
        {
            _syncInvoker = syncInvoker;
        }
        
        public ILabelMap Current
        {
            get
            {
                if (_current == null)
                {
                    if (!_labelMaps.Any())
                    {
                        Add();
                    }

                    _current = _labelMaps.Last();
                }

                return _current;
            }
            set
            {
                _current = value;
               // LabelDataChanged?.Invoke();
            }
        }
        

        public void Add()
        {
            CreateLabelMap();

            LabelDataChanged?.Invoke();
        }

        public void Delete(ILabelMap labelMap)
        {
            _syncInvoker.Invoke(() => _labelMaps.Remove(labelMap));
            Current = null;
        }

        public void Reset()
        {
            _syncInvoker.Invoke(() => _labelMaps.Clear());

            LabelDataChanged?.Invoke();
        }
        
        private LabelMap CreateLabelMap()
        {
            var label = new LabelMap()
            {
                Name = "New Label",
                Color = GetNextColor()
            };
            _syncInvoker.Invoke(() => _labelMaps.Add(label));

            label.LabelDataChanged += () => LabelDataChanged?.Invoke();

            return label;
        }

        private Color GetNextColor()
        {
            if(_labelMaps.Count >= _colors.Length)
            {
                return Color.White;
            }

            return _colors[_labelMaps.Count];
        }
    }
}

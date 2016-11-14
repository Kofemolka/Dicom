using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Drawing;

namespace Model
{
    public delegate void LabelMapAddedEvent(ILabelMap label);
    public delegate void LabelMapUpdatedEvent(ILabelMap label);
    public delegate void LabelMapDeletedEvent(ILabelMap label);
    public delegate void LabelMapSetResetEvent();

    public delegate void LabelMapCurrentSelectionChangedEvent();
    public delegate void LabelMapSetReadyEvent();

    public class LabelMapSet
    {
        private readonly BindingList<ILabelMap> _labelMaps = new BindingList<ILabelMap>();
        private readonly IScanData _scanData;
        private readonly Action<Action> _syncInvoker;
        public BindingList<ILabelMap> All => _labelMaps;
        private ILabelMap _current = null;

        private readonly Color[] _colors =
        {
            Color.OrangeRed,
            Color.LightGreen,
            Color.LightSkyBlue,
            Color.Violet,
            Color.Moccasin,
            Color.Olive,
            Color.Pink,
            Color.LightYellow,
            Color.Magenta,
            Color.PapayaWhip
        };
        
        public event LabelMapAddedEvent LabelMapAdded;
        public event LabelMapUpdatedEvent LabelMapUpdated;
        public event LabelMapDeletedEvent LabelMapDeleted;
        public event LabelMapSetResetEvent LabelMapSetReset;

        public event LabelMapCurrentSelectionChangedEvent LabelMapCurrentSelectionChanged;

        public LabelMapSet(IScanData scanData, Action<Action>  syncInvoker)
        {
            _scanData = scanData;
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

                    LabelMapCurrentSelectionChanged?.Invoke();
                }

                return _current;
            }
            set
            {
                _current = value;

                LabelMapCurrentSelectionChanged?.Invoke();
            }
        }
        

        public void Add()
        {
            var label = CreateLabelMap();

            Current = label;

            LabelMapAdded?.Invoke(label);
        }

        public void Delete(ILabelMap labelMap)
        {
            _syncInvoker.Invoke(() => _labelMaps.Remove(labelMap));
            Current = null;
            
            LabelMapDeleted?.Invoke(labelMap);
        }

        public void Reset()
        {
            _syncInvoker.Invoke(() => _labelMaps.Clear());

            _current = null;

            LabelMapSetReset?.Invoke();
        }

        private static int _newLabelCounter = 1;
        private LabelMap CreateLabelMap()
        {
            var label = new LabelMap
            {
                Name = "New Label" + _newLabelCounter++,
                Color = GetNextColor(),
                Crop = new CropBox()
                {
                    XL = 1,
                    YL = 1,
                    ZL = 1,
                    XR = _scanData.GetAxisCutCount(Axis.X) - 1,
                    YR = _scanData.GetAxisCutCount(Axis.Y) - 1,
                    ZR = _scanData.GetAxisCutCount(Axis.Z) - 1
                }
            };

            _syncInvoker.Invoke(() => _labelMaps.Add(label));

            label.LabelDataChanged += () => LabelMapUpdated?.Invoke(label);

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

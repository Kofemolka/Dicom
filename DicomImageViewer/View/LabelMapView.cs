using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Model;

namespace DicomImageViewer.View
{
    public partial class LabelMapView : UserControl
    {
        public LabelMapSet LabelMapSet;

        public LabelMapView()
        {
            InitializeComponent();
        }

        public void Init()
        {
            grid.AutoGenerateColumns = false;

            grid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Name",
                HeaderText = "Label"
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Volume",
                HeaderText = "Volume",
                ReadOnly = true
            });

            grid.Columns.Add(new DataGridViewColorColumn()
            {
                DataPropertyName = "Color",
                HeaderText = "Color"
            });

            var source = new BindingSource(LabelMapSet.All, null);
            grid.DataSource = source;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            LabelMapSet.Add();

            SelectCurrent();
        }

        private void SelectCurrent()
        {
            for (int i = 0; i < grid.RowCount; i++)
            {
                if (grid.Rows[i].DataBoundItem == LabelMapSet.Current)
                {
                    grid.Rows[i].Selected = true;
                    break;
                }
            }
        }

        public class DataGridViewColorColumn : DataGridViewColumn
        {
            public DataGridViewColorColumn()
            {
                this.CellTemplate = new DataGridViewColorCell();
                this.ReadOnly = true;
            }
        }

        public class DataGridViewColorCell : DataGridViewTextBoxCell
        {
            protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
                DataGridViewElementStates cellState, object value, object formattedValue, string errorText,
                DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
                DataGridViewPaintParts paintParts)
            {
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, "", formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

                if (value != null)
                {
                    var color = (System.Drawing.Color)value;
                    graphics.FillRectangle(new SolidBrush(color), cellBounds);
                }
            }
        }

        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count > 0)
            {
                LabelMapSet.Current = grid.SelectedRows[0].DataBoundItem as ILabelMap;

                btnDelete.Enabled = true;
            }
            else
            {
                btnDelete.Enabled = false;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            LabelMapSet.Delete(LabelMapSet.Current);

            SelectCurrent();
        }
    }
}

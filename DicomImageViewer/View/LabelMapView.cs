using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
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

        private void btnNew_Click(object sender, EventArgs e)
        {
            LabelMapSet.Add();
        }
    }
}

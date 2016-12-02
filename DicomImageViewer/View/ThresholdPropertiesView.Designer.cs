namespace DicomImageViewer.View
{
    partial class ThresholdPropertiesView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbThresh = new System.Windows.Forms.Label();
            this.trackThresh = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.trackThresh)).BeginInit();
            this.SuspendLayout();
            // 
            // lbThresh
            // 
            this.lbThresh.AutoSize = true;
            this.lbThresh.Location = new System.Drawing.Point(3, 7);
            this.lbThresh.Name = "lbThresh";
            this.lbThresh.Size = new System.Drawing.Size(57, 13);
            this.lbThresh.TabIndex = 34;
            this.lbThresh.Text = "Threshold:";
            // 
            // trackThresh
            // 
            this.trackThresh.LargeChange = 20;
            this.trackThresh.Location = new System.Drawing.Point(66, 3);
            this.trackThresh.Maximum = 5;
            this.trackThresh.Name = "trackThresh";
            this.trackThresh.Size = new System.Drawing.Size(179, 45);
            this.trackThresh.TabIndex = 33;
            this.trackThresh.ValueChanged += new System.EventHandler(this.trackThresh_ValueChanged);
            // 
            // ThresholdPropertiesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbThresh);
            this.Controls.Add(this.trackThresh);
            this.Name = "ThresholdPropertiesView";
            this.Size = new System.Drawing.Size(271, 53);
            ((System.ComponentModel.ISupportInitialize)(this.trackThresh)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbThresh;
        private System.Windows.Forms.TrackBar trackThresh;
    }
}

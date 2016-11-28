namespace DicomImageViewer.View
{
    partial class EdgeFinderPropertiesView
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
            this.btnRebuild = new System.Windows.Forms.Button();
            this.lbThresh = new System.Windows.Forms.Label();
            this.trackThresh = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.trackThresh)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRebuild
            // 
            this.btnRebuild.Location = new System.Drawing.Point(167, 67);
            this.btnRebuild.Name = "btnRebuild";
            this.btnRebuild.Size = new System.Drawing.Size(73, 29);
            this.btnRebuild.TabIndex = 38;
            this.btnRebuild.Text = "Rebuild";
            this.btnRebuild.UseVisualStyleBackColor = true;
            this.btnRebuild.Click += new System.EventHandler(this.btnRebuild_Click);
            // 
            // lbThresh
            // 
            this.lbThresh.AutoSize = true;
            this.lbThresh.Location = new System.Drawing.Point(3, 0);
            this.lbThresh.Name = "lbThresh";
            this.lbThresh.Size = new System.Drawing.Size(57, 13);
            this.lbThresh.TabIndex = 37;
            this.lbThresh.Text = "Threshold:";
            // 
            // trackThresh
            // 
            this.trackThresh.LargeChange = 20;
            this.trackThresh.Location = new System.Drawing.Point(0, 16);
            this.trackThresh.Maximum = 15;
            this.trackThresh.Name = "trackThresh";
            this.trackThresh.Size = new System.Drawing.Size(240, 45);
            this.trackThresh.TabIndex = 36;
            this.trackThresh.ValueChanged += new System.EventHandler(this.trackThresh_ValueChanged);
            // 
            // EdgeFinderPropertiesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnRebuild);
            this.Controls.Add(this.lbThresh);
            this.Controls.Add(this.trackThresh);
            this.Name = "EdgeFinderPropertiesView";
            this.Size = new System.Drawing.Size(578, 433);
            ((System.ComponentModel.ISupportInitialize)(this.trackThresh)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRebuild;
        private System.Windows.Forms.Label lbThresh;
        private System.Windows.Forms.TrackBar trackThresh;
    }
}

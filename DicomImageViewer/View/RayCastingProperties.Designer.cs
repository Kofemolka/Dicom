namespace DicomImageViewer.View
{
    partial class RayCastingProperties
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
            this.lbHunger = new System.Windows.Forms.Label();
            this.lbLowThresh = new System.Windows.Forms.Label();
            this.lbHiThresh = new System.Windows.Forms.Label();
            this.lbRays = new System.Windows.Forms.Label();
            this.trackSkippedPixels = new System.Windows.Forms.TrackBar();
            this.trackHiThresh = new System.Windows.Forms.TrackBar();
            this.trackLowThresh = new System.Windows.Forms.TrackBar();
            this.trackRays = new System.Windows.Forms.TrackBar();
            this.btnRebuild = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackSkippedPixels)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackHiThresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackLowThresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackRays)).BeginInit();
            this.SuspendLayout();
            // 
            // lbHunger
            // 
            this.lbHunger.AutoSize = true;
            this.lbHunger.Location = new System.Drawing.Point(3, 192);
            this.lbHunger.Name = "lbHunger";
            this.lbHunger.Size = new System.Drawing.Size(45, 13);
            this.lbHunger.TabIndex = 31;
            this.lbHunger.Text = "Hunger:";
            // 
            // lbLowThresh
            // 
            this.lbLowThresh.AutoSize = true;
            this.lbLowThresh.Location = new System.Drawing.Point(3, 128);
            this.lbLowThresh.Name = "lbLowThresh";
            this.lbLowThresh.Size = new System.Drawing.Size(81, 13);
            this.lbLowThresh.TabIndex = 30;
            this.lbLowThresh.Text = "LOW threshold:";
            // 
            // lbHiThresh
            // 
            this.lbHiThresh.AutoSize = true;
            this.lbHiThresh.Location = new System.Drawing.Point(3, 64);
            this.lbHiThresh.Name = "lbHiThresh";
            this.lbHiThresh.Size = new System.Drawing.Size(67, 13);
            this.lbHiThresh.TabIndex = 29;
            this.lbHiThresh.Text = "HI threshold:";
            // 
            // lbRays
            // 
            this.lbRays.AutoSize = true;
            this.lbRays.Location = new System.Drawing.Point(3, 0);
            this.lbRays.Name = "lbRays";
            this.lbRays.Size = new System.Drawing.Size(34, 13);
            this.lbRays.TabIndex = 28;
            this.lbRays.Text = "Rays:";
            // 
            // trackSkippedPixels
            // 
            this.trackSkippedPixels.LargeChange = 20;
            this.trackSkippedPixels.Location = new System.Drawing.Point(0, 208);
            this.trackSkippedPixels.Name = "trackSkippedPixels";
            this.trackSkippedPixels.Size = new System.Drawing.Size(240, 45);
            this.trackSkippedPixels.TabIndex = 27;
            this.trackSkippedPixels.ValueChanged += new System.EventHandler(this.trackSkippedPixels_ValueChanged);
            // 
            // trackHiThresh
            // 
            this.trackHiThresh.LargeChange = 20;
            this.trackHiThresh.Location = new System.Drawing.Point(0, 80);
            this.trackHiThresh.Maximum = 25;
            this.trackHiThresh.Name = "trackHiThresh";
            this.trackHiThresh.Size = new System.Drawing.Size(240, 45);
            this.trackHiThresh.TabIndex = 26;
            this.trackHiThresh.ValueChanged += new System.EventHandler(this.trackHiThresh_ValueChanged);
            // 
            // trackLowThresh
            // 
            this.trackLowThresh.LargeChange = 20;
            this.trackLowThresh.Location = new System.Drawing.Point(0, 144);
            this.trackLowThresh.Maximum = 25;
            this.trackLowThresh.Name = "trackLowThresh";
            this.trackLowThresh.Size = new System.Drawing.Size(240, 45);
            this.trackLowThresh.TabIndex = 25;
            this.trackLowThresh.ValueChanged += new System.EventHandler(this.trackLowThresh_ValueChanged);
            // 
            // trackRays
            // 
            this.trackRays.LargeChange = 16;
            this.trackRays.Location = new System.Drawing.Point(0, 16);
            this.trackRays.Maximum = 720;
            this.trackRays.Minimum = 16;
            this.trackRays.Name = "trackRays";
            this.trackRays.Size = new System.Drawing.Size(240, 45);
            this.trackRays.SmallChange = 16;
            this.trackRays.TabIndex = 24;
            this.trackRays.TickFrequency = 16;
            this.trackRays.Value = 120;
            this.trackRays.ValueChanged += new System.EventHandler(this.trackRays_ValueChanged);
            // 
            // btnRebuild
            // 
            this.btnRebuild.Location = new System.Drawing.Point(157, 250);
            this.btnRebuild.Name = "btnRebuild";
            this.btnRebuild.Size = new System.Drawing.Size(73, 29);
            this.btnRebuild.TabIndex = 32;
            this.btnRebuild.Text = "Rebuild";
            this.btnRebuild.UseVisualStyleBackColor = true;
            this.btnRebuild.Click += new System.EventHandler(this.btnRebuild_Click);
            // 
            // RayCastingProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnRebuild);
            this.Controls.Add(this.lbHunger);
            this.Controls.Add(this.lbLowThresh);
            this.Controls.Add(this.lbHiThresh);
            this.Controls.Add(this.lbRays);
            this.Controls.Add(this.trackSkippedPixels);
            this.Controls.Add(this.trackHiThresh);
            this.Controls.Add(this.trackLowThresh);
            this.Controls.Add(this.trackRays);
            this.Name = "RayCastingProperties";
            this.Size = new System.Drawing.Size(246, 295);
            ((System.ComponentModel.ISupportInitialize)(this.trackSkippedPixels)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackHiThresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackLowThresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackRays)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbHunger;
        private System.Windows.Forms.Label lbLowThresh;
        private System.Windows.Forms.Label lbHiThresh;
        private System.Windows.Forms.Label lbRays;
        private System.Windows.Forms.TrackBar trackSkippedPixels;
        private System.Windows.Forms.TrackBar trackHiThresh;
        private System.Windows.Forms.TrackBar trackLowThresh;
        private System.Windows.Forms.TrackBar trackRays;
        private System.Windows.Forms.Button btnRebuild;
    }
}

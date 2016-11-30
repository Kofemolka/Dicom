namespace DicomImageViewer
{
    partial class ProjectionView
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
            this.trackCut = new System.Windows.Forms.TrackBar();
            this.surface = new DicomImageViewer.DrawSurface();
            ((System.ComponentModel.ISupportInitialize)(this.trackCut)).BeginInit();
            this.SuspendLayout();
            // 
            // trackCut
            // 
            this.trackCut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackCut.Location = new System.Drawing.Point(0, 400);
            this.trackCut.Name = "trackCut";
            this.trackCut.Size = new System.Drawing.Size(685, 45);
            this.trackCut.TabIndex = 0;
            this.trackCut.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackCut.Scroll += new System.EventHandler(this.trackCut_Scroll);
            // 
            // surface
            // 
            this.surface.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.surface.Location = new System.Drawing.Point(2, 2);
            this.surface.Name = "surface";
            this.surface.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.surface.Size = new System.Drawing.Size(681, 392);
            this.surface.TabIndex = 1;
            this.surface.Paint += new System.Windows.Forms.PaintEventHandler(this.surface_Paint);
            this.surface.MouseClick += new System.Windows.Forms.MouseEventHandler(this.surface_MouseClick);
            this.surface.MouseDown += new System.Windows.Forms.MouseEventHandler(this.surface_MouseDown);
            this.surface.MouseLeave += new System.EventHandler(this.surface_MouseLeave);
            this.surface.MouseMove += new System.Windows.Forms.MouseEventHandler(this.surface_MouseMove);
            this.surface.MouseUp += new System.Windows.Forms.MouseEventHandler(this.surface_MouseUp);
            // 
            // ProjectionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.surface);
            this.Controls.Add(this.trackCut);
            this.Name = "ProjectionView";
            this.Size = new System.Drawing.Size(685, 445);
            this.Resize += new System.EventHandler(this.ProjectionView_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.trackCut)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackCut;
        private DrawSurface surface;
    }
}

using Model;

namespace DicomImageViewer
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.bnTags = new System.Windows.Forms.Button();
            this.btnOpenSeries = new System.Windows.Forms.Button();
            this.progBar = new System.Windows.Forms.ProgressBar();
            this.trackRays = new System.Windows.Forms.TrackBar();
            this.trackLowThresh = new System.Windows.Forms.TrackBar();
            this.trackHiThresh = new System.Windows.Forms.TrackBar();
            this.trackSkippedPixels = new System.Windows.Forms.TrackBar();
            this.lbDensity = new System.Windows.Forms.Label();
            this.btnRebuild = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbVolume = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackRays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackLowThresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackHiThresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSkippedPixels)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 381);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Z axis";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(525, 381);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "X axis";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Y axis";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tableLayoutPanel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(194, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1044, 763);
            this.panel2.TabIndex = 19;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label6, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1044, 763);
            this.tableLayoutPanel1.TabIndex = 18;
            // 
            // bnTags
            // 
            this.bnTags.Location = new System.Drawing.Point(73, 11);
            this.bnTags.Name = "bnTags";
            this.bnTags.Size = new System.Drawing.Size(80, 28);
            this.bnTags.TabIndex = 1;
            this.bnTags.Text = "Tags";
            this.bnTags.UseVisualStyleBackColor = true;
            this.bnTags.Click += new System.EventHandler(this.bnTags_Click);
            // 
            // btnOpenSeries
            // 
            this.btnOpenSeries.Location = new System.Drawing.Point(11, 11);
            this.btnOpenSeries.Name = "btnOpenSeries";
            this.btnOpenSeries.Size = new System.Drawing.Size(56, 28);
            this.btnOpenSeries.TabIndex = 11;
            this.btnOpenSeries.Text = "Open";
            this.btnOpenSeries.UseVisualStyleBackColor = true;
            this.btnOpenSeries.Click += new System.EventHandler(this.btnOpenSeries_Click);
            // 
            // progBar
            // 
            this.progBar.Location = new System.Drawing.Point(11, 525);
            this.progBar.Name = "progBar";
            this.progBar.Size = new System.Drawing.Size(176, 23);
            this.progBar.TabIndex = 12;
            // 
            // trackRays
            // 
            this.trackRays.Location = new System.Drawing.Point(3, 554);
            this.trackRays.Maximum = 360;
            this.trackRays.Minimum = 4;
            this.trackRays.Name = "trackRays";
            this.trackRays.Size = new System.Drawing.Size(184, 45);
            this.trackRays.TabIndex = 13;
            this.trackRays.Value = 120;
            this.trackRays.ValueChanged += new System.EventHandler(this.trackRays_ValueChanged);
            // 
            // trackLowThresh
            // 
            this.trackLowThresh.LargeChange = 20;
            this.trackLowThresh.Location = new System.Drawing.Point(3, 656);
            this.trackLowThresh.Maximum = 1000;
            this.trackLowThresh.Name = "trackLowThresh";
            this.trackLowThresh.Size = new System.Drawing.Size(184, 45);
            this.trackLowThresh.SmallChange = 5;
            this.trackLowThresh.TabIndex = 14;
            this.trackLowThresh.ValueChanged += new System.EventHandler(this.trackLowThresh_ValueChanged);
            // 
            // trackHiThresh
            // 
            this.trackHiThresh.LargeChange = 20;
            this.trackHiThresh.Location = new System.Drawing.Point(3, 605);
            this.trackHiThresh.Maximum = 1000;
            this.trackHiThresh.Name = "trackHiThresh";
            this.trackHiThresh.Size = new System.Drawing.Size(184, 45);
            this.trackHiThresh.SmallChange = 5;
            this.trackHiThresh.TabIndex = 15;
            this.trackHiThresh.ValueChanged += new System.EventHandler(this.trackHiThresh_ValueChanged);
            // 
            // trackSkippedPixels
            // 
            this.trackSkippedPixels.LargeChange = 20;
            this.trackSkippedPixels.Location = new System.Drawing.Point(3, 707);
            this.trackSkippedPixels.Name = "trackSkippedPixels";
            this.trackSkippedPixels.Size = new System.Drawing.Size(184, 45);
            this.trackSkippedPixels.TabIndex = 16;
            this.trackSkippedPixels.ValueChanged += new System.EventHandler(this.trackSkippedPixels_ValueChanged);
            // 
            // lbDensity
            // 
            this.lbDensity.AutoSize = true;
            this.lbDensity.Location = new System.Drawing.Point(19, 57);
            this.lbDensity.Name = "lbDensity";
            this.lbDensity.Size = new System.Drawing.Size(48, 13);
            this.lbDensity.TabIndex = 17;
            this.lbDensity.Text = "Density: ";
            // 
            // btnRebuild
            // 
            this.btnRebuild.Location = new System.Drawing.Point(11, 490);
            this.btnRebuild.Name = "btnRebuild";
            this.btnRebuild.Size = new System.Drawing.Size(176, 29);
            this.btnRebuild.TabIndex = 18;
            this.btnRebuild.Text = "Rebuild";
            this.btnRebuild.UseVisualStyleBackColor = true;
            this.btnRebuild.Click += new System.EventHandler(this.btnRebuild_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lbVolume);
            this.panel1.Controls.Add(this.lbDensity);
            this.panel1.Controls.Add(this.btnRebuild);
            this.panel1.Controls.Add(this.trackSkippedPixels);
            this.panel1.Controls.Add(this.trackHiThresh);
            this.panel1.Controls.Add(this.trackLowThresh);
            this.panel1.Controls.Add(this.trackRays);
            this.panel1.Controls.Add(this.progBar);
            this.panel1.Controls.Add(this.btnOpenSeries);
            this.panel1.Controls.Add(this.bnTags);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(194, 763);
            this.panel1.TabIndex = 18;
            // 
            // lbVolume
            // 
            this.lbVolume.AutoSize = true;
            this.lbVolume.Location = new System.Drawing.Point(19, 84);
            this.lbVolume.Name = "lbVolume";
            this.lbVolume.Size = new System.Drawing.Size(45, 13);
            this.lbVolume.TabIndex = 19;
            this.lbVolume.Text = "Volume:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1238, 763);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DICOM Image Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackRays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackLowThresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackHiThresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSkippedPixels)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button bnTags;
        private System.Windows.Forms.Button btnOpenSeries;
        private System.Windows.Forms.ProgressBar progBar;
        private System.Windows.Forms.TrackBar trackRays;
        private System.Windows.Forms.TrackBar trackLowThresh;
        private System.Windows.Forms.TrackBar trackHiThresh;
        private System.Windows.Forms.TrackBar trackSkippedPixels;
        private System.Windows.Forms.Label lbDensity;
        private System.Windows.Forms.Panel panel1;
       

        private System.Windows.Forms.Button btnRebuild;
        private System.Windows.Forms.Label lbVolume;
    }
}


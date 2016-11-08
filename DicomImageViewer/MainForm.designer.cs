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
            this.lbDensity = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabsScanners = new System.Windows.Forms.TabControl();
            this.tabRayScanner = new System.Windows.Forms.TabPage();
            this.rayCastingProperties = new DicomImageViewer.View.RayCastingProperties();
            this.tabThresh = new System.Windows.Forms.TabPage();
            this.thresholdProperties = new DicomImageViewer.View.ThresholdProperties();
            this.labelMapView = new DicomImageViewer.View.LabelMapView();
            this.btnExport = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabsScanners.SuspendLayout();
            this.tabRayScanner.SuspendLayout();
            this.tabThresh.SuspendLayout();
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
            this.label6.Location = new System.Drawing.Point(482, 381);
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
            this.panel2.Location = new System.Drawing.Point(280, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(958, 763);
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(958, 763);
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
            this.progBar.Location = new System.Drawing.Point(11, 73);
            this.progBar.Name = "progBar";
            this.progBar.Size = new System.Drawing.Size(262, 29);
            this.progBar.TabIndex = 12;
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
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnExport);
            this.panel1.Controls.Add(this.tabsScanners);
            this.panel1.Controls.Add(this.labelMapView);
            this.panel1.Controls.Add(this.lbDensity);
            this.panel1.Controls.Add(this.progBar);
            this.panel1.Controls.Add(this.btnOpenSeries);
            this.panel1.Controls.Add(this.bnTags);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(280, 763);
            this.panel1.TabIndex = 18;
            // 
            // tabsScanners
            // 
            this.tabsScanners.Controls.Add(this.tabRayScanner);
            this.tabsScanners.Controls.Add(this.tabThresh);
            this.tabsScanners.Location = new System.Drawing.Point(11, 108);
            this.tabsScanners.Name = "tabsScanners";
            this.tabsScanners.SelectedIndex = 0;
            this.tabsScanners.Size = new System.Drawing.Size(262, 320);
            this.tabsScanners.TabIndex = 25;
            // 
            // tabRayScanner
            // 
            this.tabRayScanner.Controls.Add(this.rayCastingProperties);
            this.tabRayScanner.Location = new System.Drawing.Point(4, 22);
            this.tabRayScanner.Name = "tabRayScanner";
            this.tabRayScanner.Padding = new System.Windows.Forms.Padding(3);
            this.tabRayScanner.Size = new System.Drawing.Size(254, 294);
            this.tabRayScanner.TabIndex = 0;
            this.tabRayScanner.Text = "Rays";
            this.tabRayScanner.UseVisualStyleBackColor = true;
            // 
            // rayCastingProperties
            // 
            this.rayCastingProperties.BackColor = System.Drawing.SystemColors.Control;
            this.rayCastingProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rayCastingProperties.Location = new System.Drawing.Point(3, 3);
            this.rayCastingProperties.Name = "rayCastingProperties";
            this.rayCastingProperties.Size = new System.Drawing.Size(248, 288);
            this.rayCastingProperties.TabIndex = 0;
            // 
            // tabThresh
            // 
            this.tabThresh.Controls.Add(this.thresholdProperties);
            this.tabThresh.Location = new System.Drawing.Point(4, 22);
            this.tabThresh.Name = "tabThresh";
            this.tabThresh.Padding = new System.Windows.Forms.Padding(3);
            this.tabThresh.Size = new System.Drawing.Size(254, 294);
            this.tabThresh.TabIndex = 1;
            this.tabThresh.Text = "Threashold";
            this.tabThresh.UseVisualStyleBackColor = true;
            // 
            // thresholdProperties
            // 
            this.thresholdProperties.BackColor = System.Drawing.SystemColors.Control;
            this.thresholdProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.thresholdProperties.Location = new System.Drawing.Point(3, 3);
            this.thresholdProperties.Name = "thresholdProperties";
            this.thresholdProperties.Size = new System.Drawing.Size(248, 288);
            this.thresholdProperties.TabIndex = 0;
            // 
            // labelMapView
            // 
            this.labelMapView.Location = new System.Drawing.Point(11, 434);
            this.labelMapView.Name = "labelMapView";
            this.labelMapView.Size = new System.Drawing.Size(255, 257);
            this.labelMapView.TabIndex = 24;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(213, 11);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(56, 28);
            this.btnExport.TabIndex = 26;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
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
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabsScanners.ResumeLayout(false);
            this.tabRayScanner.ResumeLayout(false);
            this.tabThresh.ResumeLayout(false);
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
        private System.Windows.Forms.Label lbDensity;
        private System.Windows.Forms.Panel panel1;
       
        private View.LabelMapView labelMapView;
        private System.Windows.Forms.TabControl tabsScanners;
        private System.Windows.Forms.TabPage tabRayScanner;
        private System.Windows.Forms.TabPage tabThresh;
        private View.RayCastingProperties rayCastingProperties;
        private View.ThresholdProperties thresholdProperties;
        private System.Windows.Forms.Button btnExport;
    }
}


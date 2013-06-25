namespace BetsyNetPDF
{
    partial class BetsyNetPDFCtrlWF
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSaveAs = new System.Windows.Forms.ToolStripButton();
            this.btnPrint = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnFitWidth = new System.Windows.Forms.ToolStripButton();
            this.btnFitWholePage = new System.Windows.Forms.ToolStripButton();
            this.btnZoomOut = new System.Windows.Forms.ToolStripButton();
            this.btnZoomIn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRotateLeft = new System.Windows.Forms.ToolStripButton();
            this.btnRotateRight = new System.Windows.Forms.ToolStripButton();
            this.btnAbout = new System.Windows.Forms.ToolStripButton();
            this.ViewerArea = new System.Windows.Forms.Panel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSaveAs,
            this.btnPrint,
            this.toolStripSeparator1,
            this.btnFitWidth,
            this.btnFitWholePage,
            this.btnZoomOut,
            this.btnZoomIn,
            this.toolStripSeparator2,
            this.btnRotateLeft,
            this.btnRotateRight,
            this.btnAbout});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(772, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSaveAs.Image = global::BetsyNetPDF.Properties.Resources.disk;
            this.btnSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(23, 22);
            this.btnSaveAs.Text = "Save as";
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPrint.Image = global::BetsyNetPDF.Properties.Resources.printer;
            this.btnPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(23, 22);
            this.btnPrint.Text = "Print";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnFitWidth
            // 
            this.btnFitWidth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFitWidth.Image = global::BetsyNetPDF.Properties.Resources.page_width;
            this.btnFitWidth.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFitWidth.Name = "btnFitWidth";
            this.btnFitWidth.Size = new System.Drawing.Size(23, 22);
            this.btnFitWidth.Text = "Fit page width";
            this.btnFitWidth.Click += new System.EventHandler(this.btnFitWidth_Click);
            // 
            // btnFitWholePage
            // 
            this.btnFitWholePage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFitWholePage.Image = global::BetsyNetPDF.Properties.Resources.whole_page;
            this.btnFitWholePage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFitWholePage.Name = "btnFitWholePage";
            this.btnFitWholePage.Size = new System.Drawing.Size(23, 22);
            this.btnFitWholePage.Text = "Fit whole page";
            this.btnFitWholePage.Click += new System.EventHandler(this.btnFitWholePage_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomOut.Image = global::BetsyNetPDF.Properties.Resources.zoom_out;
            this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(23, 22);
            this.btnZoomOut.Text = "Zoom out";
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomIn.Image = global::BetsyNetPDF.Properties.Resources.zoom_in;
            this.btnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(23, 22);
            this.btnZoomIn.Text = "Zoom in";
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnRotateLeft
            // 
            this.btnRotateLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRotateLeft.Image = global::BetsyNetPDF.Properties.Resources.rotate_left;
            this.btnRotateLeft.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRotateLeft.Name = "btnRotateLeft";
            this.btnRotateLeft.Size = new System.Drawing.Size(23, 22);
            this.btnRotateLeft.Text = "Rotate left";
            this.btnRotateLeft.Click += new System.EventHandler(this.btnRotateLeft_Click);
            // 
            // btnRotateRight
            // 
            this.btnRotateRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRotateRight.Image = global::BetsyNetPDF.Properties.Resources.rotate_right;
            this.btnRotateRight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRotateRight.Name = "btnRotateRight";
            this.btnRotateRight.Size = new System.Drawing.Size(23, 22);
            this.btnRotateRight.Text = "Rotate right";
            this.btnRotateRight.Click += new System.EventHandler(this.btnRotateRight_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAbout.Image = global::BetsyNetPDF.Properties.Resources.about;
            this.btnAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(23, 22);
            this.btnAbout.Text = "About";
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // ViewerArea
            // 
            this.ViewerArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ViewerArea.Location = new System.Drawing.Point(0, 25);
            this.ViewerArea.Name = "ViewerArea";
            this.ViewerArea.Size = new System.Drawing.Size(772, 482);
            this.ViewerArea.TabIndex = 1;
            this.ViewerArea.Resize += new System.EventHandler(this.ViewerArea_Resize);
            // 
            // BetsyNetPDFCtrlWF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ViewerArea);
            this.Controls.Add(this.toolStrip1);
            this.Name = "BetsyNetPDFCtrlWF";
            this.Size = new System.Drawing.Size(772, 507);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripButton btnSaveAs;
        private System.Windows.Forms.Panel ViewerArea;
        private System.Windows.Forms.ToolStripButton btnPrint;
        private System.Windows.Forms.ToolStripButton btnFitWidth;
        private System.Windows.Forms.ToolStripButton btnFitWholePage;
        private System.Windows.Forms.ToolStripButton btnZoomOut;
        private System.Windows.Forms.ToolStripButton btnZoomIn;
        private System.Windows.Forms.ToolStripButton btnRotateLeft;
        private System.Windows.Forms.ToolStripButton btnRotateRight;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        protected System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnAbout;
    }
}

namespace BetsyNetPDF
{
    partial class BetsyNetPDFViewerWF
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BetsyNetPDFViewerWF));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.BetsyNetPDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BetsyNetPDFToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // BetsyNetPDFToolStripMenuItem
            // 
            this.BetsyNetPDFToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuOpen,
            this.menuQuit});
            this.BetsyNetPDFToolStripMenuItem.Name = "BetsyNetPDFToolStripMenuItem";
            resources.ApplyResources(this.BetsyNetPDFToolStripMenuItem, "BetsyNetPDFToolStripMenuItem");
            // 
            // menuOpen
            // 
            this.menuOpen.Name = "menuOpen";
            resources.ApplyResources(this.menuOpen, "menuOpen");
            this.menuOpen.Click += new System.EventHandler(this.menuOpen_Click);
            // 
            // menuQuit
            // 
            this.menuQuit.Name = "menuQuit";
            resources.ApplyResources(this.menuQuit, "menuQuit");
            this.menuQuit.Click += new System.EventHandler(this.menuQuit_Click);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // BetsyNetPDFViewerWF
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "BetsyNetPDFViewerWF";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem BetsyNetPDFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuOpen;
        private System.Windows.Forms.ToolStripMenuItem menuQuit;
        private System.Windows.Forms.Panel panel1;
    }
}
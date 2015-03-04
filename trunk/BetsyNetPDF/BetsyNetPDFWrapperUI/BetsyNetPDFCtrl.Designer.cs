namespace BetsyNetPDF
{
    partial class BetsyNetPDFCtrl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BetsyNetPDFCtrl));
            this.BetsyNetPDFArea = new DevExpress.XtraEditors.PanelControl();
            this.dummyBarManager = new DevExpress.XtraBars.BarManager(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.BetsyNetPDFArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dummyBarManager)).BeginInit();
            this.SuspendLayout();
            // 
            // BetsyNetPDFArea
            // 
            resources.ApplyResources(this.BetsyNetPDFArea, "BetsyNetPDFArea");
            this.BetsyNetPDFArea.Name = "BetsyNetPDFArea";
            // 
            // dummyBarManager
            // 
            this.dummyBarManager.DockControls.Add(this.barDockControlTop);
            this.dummyBarManager.DockControls.Add(this.barDockControlBottom);
            this.dummyBarManager.DockControls.Add(this.barDockControlLeft);
            this.dummyBarManager.DockControls.Add(this.barDockControlRight);
            this.dummyBarManager.Form = this;
            this.dummyBarManager.MaxItemId = 0;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
            // 
            // BetsyNetPDFCtrl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BetsyNetPDFArea);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "BetsyNetPDFCtrl";
            this.Resize += new System.EventHandler(this.BetsyNetPDFArea_Resize);
            this.Enter += new System.EventHandler(this.BetsyNetPDFCtrl_Enter);
            ((System.ComponentModel.ISupportInitialize)(this.BetsyNetPDFArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dummyBarManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl BetsyNetPDFArea;
        private DevExpress.XtraBars.BarManager dummyBarManager;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
    }
}

namespace BetsyNetPDF
{
    partial class BetsyNetPDFViewer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BetsyNetPDFViewer));
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.barSubItem1 = new DevExpress.XtraBars.BarSubItem();
            this.barBtnOpen = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnQuit = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem5 = new DevExpress.XtraBars.BarButtonItem();
            this.barCheckItem1 = new DevExpress.XtraBars.BarCheckItem();
            this.barCheckItem2 = new DevExpress.XtraBars.BarCheckItem();
            this.barCheckItem3 = new DevExpress.XtraBars.BarCheckItem();
            this.barCheckAddObject = new DevExpress.XtraBars.BarCheckItem();
            this.barChkWithDimension = new DevExpress.XtraBars.BarCheckItem();
            this.barButtonItem6 = new DevExpress.XtraBars.BarButtonItem();
            this.barChkDeactivateTS = new DevExpress.XtraBars.BarCheckItem();
            this.barBtnTextCoordsLayer = new DevExpress.XtraBars.BarButtonItem();
            this.bar3 = new DevExpress.XtraBars.Bar();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem4 = new DevExpress.XtraBars.BarButtonItem();
            this.barChkShowOverlapping = new DevExpress.XtraBars.BarCheckItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1,
            this.bar2,
            this.bar3});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barSubItem1,
            this.barBtnOpen,
            this.barBtnQuit,
            this.barButtonItem1,
            this.barButtonItem2,
            this.barButtonItem3,
            this.barButtonItem4,
            this.barButtonItem5,
            this.barCheckItem1,
            this.barCheckItem2,
            this.barCheckItem3,
            this.barCheckAddObject,
            this.barChkWithDimension,
            this.barButtonItem6,
            this.barChkDeactivateTS,
            this.barBtnTextCoordsLayer,
            this.barChkShowOverlapping});
            this.barManager1.MainMenu = this.bar2;
            this.barManager1.MaxItemId = 23;
            this.barManager1.StatusBar = this.bar3;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 1;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            resources.ApplyResources(this.bar1, "bar1");
            this.bar1.Visible = false;
            // 
            // bar2
            // 
            this.bar2.BarName = "Main menu";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barSubItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem5),
            new DevExpress.XtraBars.LinkPersistInfo(this.barCheckItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.barCheckItem2),
            new DevExpress.XtraBars.LinkPersistInfo(this.barCheckItem3),
            new DevExpress.XtraBars.LinkPersistInfo(this.barCheckAddObject),
            new DevExpress.XtraBars.LinkPersistInfo(this.barChkWithDimension),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem6),
            new DevExpress.XtraBars.LinkPersistInfo(this.barChkDeactivateTS),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnTextCoordsLayer),
            new DevExpress.XtraBars.LinkPersistInfo(this.barChkShowOverlapping)});
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            resources.ApplyResources(this.bar2, "bar2");
            // 
            // barSubItem1
            // 
            resources.ApplyResources(this.barSubItem1, "barSubItem1");
            this.barSubItem1.Id = 0;
            this.barSubItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnOpen),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnQuit)});
            this.barSubItem1.Name = "barSubItem1";
            // 
            // barBtnOpen
            // 
            resources.ApplyResources(this.barBtnOpen, "barBtnOpen");
            this.barBtnOpen.Id = 1;
            this.barBtnOpen.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O));
            this.barBtnOpen.Name = "barBtnOpen";
            this.barBtnOpen.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnOpen_ItemClick);
            // 
            // barBtnQuit
            // 
            resources.ApplyResources(this.barBtnQuit, "barBtnQuit");
            this.barBtnQuit.Id = 2;
            this.barBtnQuit.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q));
            this.barBtnQuit.Name = "barBtnQuit";
            this.barBtnQuit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnQuit_ItemClick);
            // 
            // barButtonItem1
            // 
            resources.ApplyResources(this.barButtonItem1, "barButtonItem1");
            this.barButtonItem1.Id = 5;
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            // 
            // barButtonItem5
            // 
            resources.ApplyResources(this.barButtonItem5, "barButtonItem5");
            this.barButtonItem5.Id = 9;
            this.barButtonItem5.Name = "barButtonItem5";
            this.barButtonItem5.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem5_ItemClick);
            // 
            // barCheckItem1
            // 
            resources.ApplyResources(this.barCheckItem1, "barCheckItem1");
            this.barCheckItem1.Id = 10;
            this.barCheckItem1.Name = "barCheckItem1";
            this.barCheckItem1.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.barCheckItem1_CheckedChanged);
            // 
            // barCheckItem2
            // 
            resources.ApplyResources(this.barCheckItem2, "barCheckItem2");
            this.barCheckItem2.Id = 13;
            this.barCheckItem2.Name = "barCheckItem2";
            this.barCheckItem2.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.barCheckItem2_CheckedChanged);
            // 
            // barCheckItem3
            // 
            resources.ApplyResources(this.barCheckItem3, "barCheckItem3");
            this.barCheckItem3.Id = 15;
            this.barCheckItem3.Name = "barCheckItem3";
            this.barCheckItem3.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.barCheckItem3_CheckedChanged);
            // 
            // barCheckAddObject
            // 
            resources.ApplyResources(this.barCheckAddObject, "barCheckAddObject");
            this.barCheckAddObject.Id = 16;
            this.barCheckAddObject.Name = "barCheckAddObject";
            // 
            // barChkWithDimension
            // 
            resources.ApplyResources(this.barChkWithDimension, "barChkWithDimension");
            this.barChkWithDimension.Id = 17;
            this.barChkWithDimension.Name = "barChkWithDimension";
            // 
            // barButtonItem6
            // 
            resources.ApplyResources(this.barButtonItem6, "barButtonItem6");
            this.barButtonItem6.Id = 18;
            this.barButtonItem6.Name = "barButtonItem6";
            this.barButtonItem6.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem6_ItemClick);
            // 
            // barChkDeactivateTS
            // 
            resources.ApplyResources(this.barChkDeactivateTS, "barChkDeactivateTS");
            this.barChkDeactivateTS.Id = 19;
            this.barChkDeactivateTS.Name = "barChkDeactivateTS";
            this.barChkDeactivateTS.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.barChkDeactivateTS_CheckedChanged);
            // 
            // barBtnTextCoordsLayer
            // 
            resources.ApplyResources(this.barBtnTextCoordsLayer, "barBtnTextCoordsLayer");
            this.barBtnTextCoordsLayer.Id = 21;
            this.barBtnTextCoordsLayer.Name = "barBtnTextCoordsLayer";
            this.barBtnTextCoordsLayer.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnTextCoordsLayer_ItemClick);
            // 
            // bar3
            // 
            this.bar3.BarName = "Status bar";
            this.bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.bar3.DockCol = 0;
            this.bar3.DockRow = 0;
            this.bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.bar3.OptionsBar.AllowQuickCustomization = false;
            this.bar3.OptionsBar.DrawDragBorder = false;
            this.bar3.OptionsBar.UseWholeRow = true;
            resources.ApplyResources(this.bar3, "bar3");
            this.bar3.Visible = false;
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
            // barButtonItem2
            // 
            resources.ApplyResources(this.barButtonItem2, "barButtonItem2");
            this.barButtonItem2.Id = 6;
            this.barButtonItem2.Name = "barButtonItem2";
            // 
            // barButtonItem3
            // 
            resources.ApplyResources(this.barButtonItem3, "barButtonItem3");
            this.barButtonItem3.Id = 7;
            this.barButtonItem3.Name = "barButtonItem3";
            // 
            // barButtonItem4
            // 
            resources.ApplyResources(this.barButtonItem4, "barButtonItem4");
            this.barButtonItem4.Id = 8;
            this.barButtonItem4.Name = "barButtonItem4";
            // 
            // barChkShowOverlapping
            // 
            resources.ApplyResources(this.barChkShowOverlapping, "barChkShowOverlapping");
            this.barChkShowOverlapping.Id = 22;
            this.barChkShowOverlapping.Name = "barChkShowOverlapping";
            this.barChkShowOverlapping.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.barChkShowOverlapping_CheckedChanged);
            // 
            // BetsyNetPDFViewer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "BetsyNetPDFViewer";
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarSubItem barSubItem1;
        private DevExpress.XtraBars.BarButtonItem barBtnOpen;
        private DevExpress.XtraBars.BarButtonItem barBtnQuit;
        private DevExpress.XtraBars.Bar bar3;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.BarButtonItem barButtonItem3;
        private DevExpress.XtraBars.BarButtonItem barButtonItem4;
        private DevExpress.XtraBars.BarButtonItem barButtonItem5;
        private DevExpress.XtraBars.BarCheckItem barCheckItem1;
        private DevExpress.XtraBars.BarCheckItem barCheckItem2;
        private DevExpress.XtraBars.BarCheckItem barCheckItem3;
        private DevExpress.XtraBars.BarCheckItem barCheckAddObject;
        private DevExpress.XtraBars.BarCheckItem barChkWithDimension;
        private DevExpress.XtraBars.BarButtonItem barButtonItem6;
        private DevExpress.XtraBars.BarCheckItem barChkDeactivateTS;
        private DevExpress.XtraBars.BarButtonItem barBtnTextCoordsLayer;
        private DevExpress.XtraBars.BarCheckItem barChkShowOverlapping;


    }
}

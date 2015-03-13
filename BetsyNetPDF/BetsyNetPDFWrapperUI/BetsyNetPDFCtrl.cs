/*
Copyright 2012-2015 IBB Ehlert&Wolf GbR
Author: Silvio Zimmer

This file is part of BetsyNetPDF.

BetsyNetPDF is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

BetsyNetPDF is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with BetsyNetPDF.  If not, see <http://www.gnu.org/licenses/>.

Diese Datei ist Teil von BetsyNetPDF.

BetsyNetPDF ist Freie Software: Sie können es unter den Bedingungen
der GNU Lesser General Public License, wie von der Free Software Foundation,
Version 3 der Lizenz oder (nach Ihrer Option) jeder späteren
veröffentlichten Version, weiterverbreiten und/oder modifizieren.

BetsyNetPDF wird in der Hoffnung, dass es nützlich sein wird, aber
OHNE JEDE GEWÄHELEISTUNG, bereitgestellt; sogar ohne die implizite
Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK.
Siehe die GNU Lesser General Public License für weitere Details.

Sie sollten eine Kopie der GNU Lesser General Public License zusammen mit diesem
Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraBars;

namespace BetsyNetPDF
{
    public partial class BetsyNetPDFCtrl : DevExpress.XtraEditors.XtraUserControl
    {
        private BarButtonItem barBtnOpenExt;
        private BarManager barManager;
        private Bar toolBar;
        private BarButtonItem barBtnSaveAs;
        private BarButtonItem barBtnPrint;
        private BarButtonItem barBtnFitPageWidth;
        private BarButtonItem barBtnFitWholePage;
        private BarButtonItem barBtnZoomOut;
        private BarButtonItem barBtnZoomIn;
        private BarButtonItem barBtnRotateLeft;
        private BarButtonItem barBtnRotateRight;
        private BarButtonItem barBtnAbout;

        public Bar ToolBar { get { return toolBar; } }

        private BetsyNetPDFWrapper wrapper;
        private bool useExternContextMenu, printOnPlotter;
        private string currentFile, outputFile, layerTitle, printOverlayObjectsQuestion = "Print with overlay objects?";

        public string OutputFile { get { return outputFile; } set { outputFile = value; } }
        public string LayerTitle { get { return layerTitle; } set { layerTitle = value; } }

        public event BetsyNetPDFMouseClickEventHandler BetsyNetPDFMouseClickEvent;
        public event BetsyNetPDFSelectionChangedEventHandler BetsyNetPDFSelectionChangedEvent;
        public event BetsyNetPDFDeleteEventHandler BetsyNetPDFDeleteEvent;
        public event BetsyNetPDFObjectMovedEventHandler BetsyNetPDFObjectMovedEvent;
        public event BetsyNetPDFRequestContextMenuEventHandler BetsyNetPDFRequestContextMenuEvent;
        public event BetsyNetPDFMouseOverObjectEventHandler BetsyNetPDFMouseOverObjectEvent;
        public event BetsyNetPDFDistanceMeasuredEventHandler BetsyNetPDFDistanceMeasuredEvent;
        public event BetsyNetPDFLineDrawnEventHandler BetsyNetPDFLineDrawnEvent;

        public BetsyNetPDFCtrl(BarManager barManager)
        {
            this.barManager = barManager;
            this.useExternContextMenu = false;
            this.printOnPlotter = false;
            InitializeComponent();
            InitializeToolbar();

            currentFile = "";
            wrapper = new BetsyNetPDFWrapper();
            wrapper.MouseClickEvent += new BetsyNetPDFWrapper.MouseClickEventHandler(wrapper_MouseClickEvent);
            wrapper.SelectionChangedEvent += new BetsyNetPDFWrapper.SelectionChangedEventHandler(wrapper_SelectionChangedEvent);
            wrapper.DeleteEvent += new BetsyNetPDFWrapper.DeleteEventHandler(wrapper_DeleteEvent);
            wrapper.ObjectMovedEvent += new BetsyNetPDFWrapper.ObjectMovedEventHandler(wrapper_ObjectMovedEvent);
            wrapper.RequestContextMenuEvent += new BetsyNetPDFWrapper.RequestContextMenuEventHandler(wrapper_RequestContextMenuEvent);
            wrapper.MouseOverObjectEvent += new BetsyNetPDFWrapper.MouseOverObjectEventHandler(wrapper_MouseOverObjectEvent);
            wrapper.DistanceMeasuredEvent += new BetsyNetPDFWrapper.DistanceMeasuredEventHandler(wrapper_DistanceMeasuredEvent);
            wrapper.LineDrawnEvent += new BetsyNetPDFWrapper.LineDrawnEventHandler(wrapper_LineDrawnEvent);
        }

        #region properties
        public string Version { get { if (wrapper != null)return wrapper.Version; return ""; } }
        public string CurrentFile { get { return currentFile; } }
        public bool ShowSaveBtn { set { barBtnSaveAs.Visibility = value ? BarItemVisibility.Always : BarItemVisibility.Never; } }
        public bool ShowPrintBtn { set { barBtnPrint.Visibility = value ? BarItemVisibility.Always : BarItemVisibility.Never; } }
        public bool UseExternContextMenu { get { return this.useExternContextMenu; } set { this.useExternContextMenu = value; } }
        public bool PrintOnPlotter { get { return printOnPlotter; } set { printOnPlotter = value; } }
        public string PrintOverlayObjectsQuestion { set { printOverlayObjectsQuestion = value; } }
        #endregion

        #region delegates
        public delegate void BetsyNetPDFMouseClickEventHandler(double x, double y, string key);
        public delegate void BetsyNetPDFSelectionChangedEventHandler();
        public delegate void BetsyNetPDFDeleteEventHandler();
        public delegate void BetsyNetPDFObjectMovedEventHandler(double deltaX, double deltaY, bool moveLabel);
        public delegate void BetsyNetPDFRequestContextMenuEventHandler(int x, int y, string sid);
        public delegate void BetsyNetPDFMouseOverObjectEventHandler(string id);
        public delegate void BetsyNetPDFDistanceMeasuredEventHandler(double distance);
        public delegate void BetsyNetPDFLineDrawnEventHandler(double p1x, double p1y, double p2x, double p2y);
        #endregion

        #region public methods
        public void OpenPdfFile(string file)
        {
            wrapper.BetsyNetPDFViewer(GetCurrentHWND(), file, UseExternContextMenu);
            currentFile = file;
        }

        public static void DirectPrinting(string file, bool defaultPrinter, string printerName)
        {
            BetsyNetPDFWrapper pwrapper = new BetsyNetPDFWrapper();
            pwrapper.DirectPrinting(file, defaultPrinter, printerName);
        }

        public bool IsDocOpen()
        {
            return wrapper.IsDocOpen();
        }

        public void SetMouseOverEnabled(bool enabled) { wrapper.SetMouseOverEnabled(enabled); }
        public void SetMeasureModeEnabled(bool enabled) { wrapper.SetMeasureModeEnabled(enabled); }
        public void SetLineModeEnabled(bool enabled) { wrapper.SetLineModeEnabled(enabled); }
        public void SetLineStart(double x, double y) { wrapper.SetLineStart(x, y); }
        public PointF GetLineStart() { return wrapper.GetLineStart(); }
        public bool IsLineStart() { return wrapper.IsLineStart(); }
        public void SetFixedAngle(double angle) { wrapper.SetFixedAngle(angle); }
        public void SetDeactivateTextSelection(bool value) { wrapper.SetDeactivateTextSelection(value); }
        public void SetPreventOverlayObjectSelection(bool value) { wrapper.SetPreventOverlayObjectSelection(value); }
        public void SetShowOverlapping(bool value) { wrapper.SetShowOverlapping(value); }
        public void SetHideLabels(bool value) { wrapper.SetHideLabels(value); }
        public void SetTransparantOverlayObjects(bool value) { wrapper.SetTransparantOverlayObjects(value); }
        public PointF CvtScreen2Doc(Point screenCoords) { return wrapper.CvtScreen2Doc(screenCoords); }
        public Point CvtDoc2Screen(PointF docCoords) { return wrapper.CvtDoc2Screen(docCoords); }

        public void FocusViewer() { wrapper.FocusViewer(); }

        public void ProcessOverlayObjects(string objs) { wrapper.ProcessOverlayObjects(objs); }
        public void SetSelectedOverlayObjects(string objectIds) { wrapper.SetSelectedOverlayObjects(objectIds); }

        public void RemoveOverlayObject(string id) { wrapper.RemoveOverlayObject(id); }
        public string GetSelectedOverlayObjectIds() { return wrapper.GetSelectedOverlayObjectIds(); }
        public string GetSelectedOverlayObjects() { return wrapper.GetSelectedOverlayObjects(); }
        public string GetAllOverlayObjects() { return wrapper.GetAllOverlayObjects(); }
        public string GetOverlayObjectAtPosition(double x, double y) { return wrapper.GetOverlayObjectAtPosition(x, y); }
        public void ClearOverlayObjects() { wrapper.ClearOverlayObjectList(); }

        public void RotateSelectedOverlayObjects(double angle)
        {
            List<OverlayObject> objects = OverlayObject.CreateOverlayObjectListFromString(this.GetSelectedOverlayObjects());
            foreach (OverlayObject oo in objects)
                oo.angle += angle;
            this.ProcessOverlayObjects(OverlayObject.CreateStringFromOverlayObjectList(objects));
        }

        public void SetAngle4SelectedOverlayObjects(double angle)
        {
            List<OverlayObject> objects = OverlayObject.CreateOverlayObjectListFromString(this.GetSelectedOverlayObjects());
            foreach (OverlayObject oo in objects)
                oo.angle = angle;
            this.ProcessOverlayObjects(OverlayObject.CreateStringFromOverlayObjectList(objects));
        }

        public void FitWholePage()
        {
            wrapper.FitWholePage();
        }

        public void FitPageWidth()
        {
            wrapper.FitPageWidth();
        }

        public void RotateCounterClockWise(int angle)
        {
            wrapper.RotateCounterClockWise(angle);
        }

        public int GetDocumentRotation()
        {
            return wrapper.GetDocumentRotation();
        }

        // minLength in percentage of the page size
        public List<PointF> GetLinePointsFromPdf(float minLength)
        {
            return BetsyNetPDFEditor.ExtractLinesFromPdf(this.currentFile, minLength);
        }
        #endregion

        #region private methods

        private void InitializeToolbar()
        {
            if (barManager == null)
                barManager = dummyBarManager;

            barManager.ForceInitialize();
            barManager.BeginUpdate();

            toolBar = barManager.Bars["Tools"];
            if (toolBar == null)
            {
                toolBar = new Bar();
                toolBar.BarName = "Tools";
                toolBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
                toolBar.OptionsBar.UseWholeRow = true;
                barManager.Bars.Add(toolBar);
            }

            barBtnSaveAs = new BarButtonItem(barManager, "Save As...");
            barBtnSaveAs.Hint = " ";
            barBtnSaveAs.Glyph = global::BetsyNetPDF.Properties.Resources.disk;
            barBtnSaveAs.Name = "barBtnSaveAs";
            barBtnSaveAs.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSaveAs_ItemClick);
            toolBar.AddItem(barBtnSaveAs).BeginGroup = true;

            barBtnPrint = new BarButtonItem(barManager, "Print");
            barBtnPrint.Hint = " ";
            barBtnPrint.Glyph = global::BetsyNetPDF.Properties.Resources.printer;
            barBtnPrint.Name = "barBtnPrint";
            barBtnPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnPrint_ItemClick);
            toolBar.AddItem(barBtnPrint);

            barBtnOpenExt = new BarButtonItem(barManager, "Open current file external");
            barBtnOpenExt.Hint = " ";
            barBtnOpenExt.Glyph = global::BetsyNetPDF.Properties.Resources.open_ext;
            barBtnOpenExt.Name = "barBtnOpenExt";
            barBtnOpenExt.ItemClick += new ItemClickEventHandler(barBtnOpenExt_ItemClick);
            toolBar.AddItem(barBtnOpenExt);

            barBtnFitPageWidth = new BarButtonItem(barManager, "Zoom to fit page width");
            barBtnFitPageWidth.Hint = " ";
            barBtnFitPageWidth.Glyph = global::BetsyNetPDF.Properties.Resources.page_width;
            barBtnFitPageWidth.Name = "barBtnFitPageWidth";
            barBtnFitPageWidth.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnFitPageWidth_ItemClick);
            toolBar.AddItem(barBtnFitPageWidth).BeginGroup = true;

            barBtnFitWholePage = new BarButtonItem(barManager, "Zoom to fit whole page");
            barBtnFitWholePage.Hint = " ";
            barBtnFitWholePage.Glyph = global::BetsyNetPDF.Properties.Resources.whole_page;
            barBtnFitWholePage.Name = "barBtnFitWholePage";
            barBtnFitWholePage.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnFitWholePage_ItemClick);
            toolBar.AddItem(barBtnFitWholePage);

            barBtnZoomOut = new BarButtonItem(barManager, "Zoom out");
            barBtnZoomOut.Hint = " ";
            barBtnZoomOut.Glyph = global::BetsyNetPDF.Properties.Resources.zoom_out;
            barBtnZoomOut.Name = "barBtnZoomOut";
            barBtnZoomOut.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnZoomOut_ItemClick);
            toolBar.AddItem(barBtnZoomOut);

            barBtnZoomIn = new BarButtonItem(barManager, "Zoom in");
            barBtnZoomIn.Hint = " ";
            barBtnZoomIn.Glyph = global::BetsyNetPDF.Properties.Resources.zoom_in;
            barBtnZoomIn.Name = "barBtnZoomIn";
            barBtnZoomIn.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnZoomIn_ItemClick);
            toolBar.AddItem(barBtnZoomIn);

            barBtnRotateLeft = new BarButtonItem(barManager, "Rotate left");
            barBtnRotateLeft.Hint = " ";
            barBtnRotateLeft.Glyph = global::BetsyNetPDF.Properties.Resources.rotate_left;
            barBtnRotateLeft.Name = "barBtnRotateLeft";
            barBtnRotateLeft.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnRotateLeft_ItemClick);
            toolBar.AddItem(barBtnRotateLeft).BeginGroup = true;

            barBtnRotateRight = new BarButtonItem(barManager, "Rotate right");
            barBtnRotateRight.Hint = " ";
            barBtnRotateRight.Glyph = global::BetsyNetPDF.Properties.Resources.rotate_right;
            barBtnRotateRight.Name = "barBtnRotateRight";
            barBtnRotateRight.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnRotateRight_ItemClick);
            toolBar.AddItem(barBtnRotateRight);

            barBtnAbout = new BarButtonItem(barManager, "About");
            barBtnAbout.Hint = " ";
            barBtnAbout.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            barBtnAbout.Glyph = global::BetsyNetPDF.Properties.Resources.about;
            barBtnAbout.Name = "barBtnAbout";
            barBtnAbout.ItemClick += new ItemClickEventHandler(barBtnAbout_ItemClick);
            toolBar.AddItem(barBtnAbout);

            barManager.EndUpdate();
        }

        private string GetCurrentHWND()
        {
            string hwnd = "";

            if (IntPtr.Size == 4)
                hwnd = BetsyNetPDFArea.Handle.ToInt32().ToString();
            else
                hwnd = BetsyNetPDFArea.Handle.ToInt64().ToString();

            return hwnd;
        }

        private void OnBetsyNetPDFMouseClick(double x, double y, string key)
        {
            if (BetsyNetPDFMouseClickEvent != null)
                BetsyNetPDFMouseClickEvent(x, y, key);
        }

        private void OnBetsyNetPDFSelectionChanged()
        {
            if (BetsyNetPDFSelectionChangedEvent != null)
                BetsyNetPDFSelectionChangedEvent();
        }

        private void OnBetsyNetPDFDelete()
        {
            if (BetsyNetPDFDeleteEvent != null)
                BetsyNetPDFDeleteEvent();
        }

        private void OnBetsyNetPDFObjectMoved(double deltaX, double deltaY, bool moveLabel)
        {
            if (BetsyNetPDFObjectMovedEvent != null)
                BetsyNetPDFObjectMovedEvent(deltaX, deltaY, moveLabel);
        }

        private void OnBetsyNetPDFRequestContextMenu(int x, int y, string sid)
        {
            if (BetsyNetPDFRequestContextMenuEvent != null)
                BetsyNetPDFRequestContextMenuEvent(x, y, sid);
        }

        private void OnBetsyNetPDFMouseOverObject(string id)
        {
            if (BetsyNetPDFMouseOverObjectEvent != null)
                BetsyNetPDFMouseOverObjectEvent(id);
        }

        private void OnBetsyNetPDFDistanceMeasured(double distance)
        {
            if (BetsyNetPDFDistanceMeasuredEvent != null)
                BetsyNetPDFDistanceMeasuredEvent(distance);
        }

        private void OnBetsyNetPDFLineDrawn(double p1x, double p1y, double p2x, double p2y)
        {
            if (BetsyNetPDFLineDrawnEvent != null)
                BetsyNetPDFLineDrawnEvent(p1x, p1y, p2x, p2y);
        }

        private void PrintWithLabels()
        {
            if (string.IsNullOrEmpty(currentFile))
                return;

            string fName = Path.GetFileNameWithoutExtension(currentFile);
            string ext = Path.GetExtension(currentFile);
            string file = fName + "_BetsyNetPDF" + ext;

            string sobjects = this.GetAllOverlayObjects();
            BetsyNetPDFEditor.ExportOverlayObjects2PDF(currentFile, Path.GetTempPath() + file, sobjects, "BetsyNetPDF");

            wrapper.DirectPrinting(Path.GetTempPath() + file, false, "");
        }
        #endregion

        #region events
        void wrapper_SelectionChangedEvent()
        {
            OnBetsyNetPDFSelectionChanged();
        }

        private void wrapper_MouseClickEvent(double x, double y, string key)
        {
            OnBetsyNetPDFMouseClick(x, y, key);
        }

        void wrapper_ObjectMovedEvent(double deltaX, double deltaY, bool moveLabel)
        {
            OnBetsyNetPDFObjectMoved(deltaX, deltaY, moveLabel);
        }

        void wrapper_DeleteEvent()
        {
            OnBetsyNetPDFDelete();
        }

        void wrapper_RequestContextMenuEvent(int x, int y, string sid)
        {
            OnBetsyNetPDFRequestContextMenu(x, y, sid);
        }

        void wrapper_MouseOverObjectEvent(string id)
        {
            OnBetsyNetPDFMouseOverObject(id);
        }

        void wrapper_LineDrawnEvent(double p1x, double p1y, double p2x, double p2y)
        {
            OnBetsyNetPDFLineDrawn(p1x, p1y, p2x, p2y);
        }

        void wrapper_DistanceMeasuredEvent(double distance)
        {
            OnBetsyNetPDFDistanceMeasured(distance);
        }

        private void BetsyNetPDFArea_Resize(object sender, EventArgs e)
        {
            if (wrapper != null)
                wrapper.UpdateViewer(GetCurrentHWND());
        }

        private void barBtnSaveAs_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrEmpty(currentFile))
                return;

            if (string.IsNullOrEmpty(LayerTitle))
                LayerTitle = "BetsyNetPDF";

            if (string.IsNullOrEmpty(OutputFile))
            {
                string path = Path.GetDirectoryName(currentFile);
                string fName = Path.GetFileNameWithoutExtension(currentFile);
                string ext = Path.GetExtension(currentFile);

                OutputFile = path + Path.DirectorySeparatorChar + fName + "_BetsyNetPDF" + ext;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.FileName = Path.GetFileName(outputFile);
                sfd.InitialDirectory = Path.GetDirectoryName(outputFile);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string sobjects = this.GetAllOverlayObjects();
                    BetsyNetPDFEditor.ExportOverlayObjects2PDF(currentFile, sfd.FileName, sobjects, layerTitle);
                }
            }
        }

        private void barBtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult res = MessageBox.Show(printOverlayObjectsQuestion, printOverlayObjectsQuestion, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
                PrintWithLabels();
            else
                wrapper.Print(PrintOnPlotter);
        }

        private void barBtnOpenExt_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (string.IsNullOrEmpty(currentFile))
                return;

            Process.Start(currentFile);
        }

        private void barBtnFitPageWidth_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            wrapper.FitPageWidth();
        }

        private void barBtnFitWholePage_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            wrapper.FitWholePage();
        }

        private void barBtnZoomOut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            wrapper.ZoomOut();
        }

        private void barBtnZoomIn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            wrapper.ZoomIn();
        }

        private void barBtnRotateLeft_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            wrapper.RotateLeft();
        }

        private void barBtnRotateRight_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            wrapper.RotateRight();
        }

        void barBtnAbout_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (AboutDialog about = new AboutDialog("Version: " + this.Version))
            {
                about.ShowDialog();
            }
        }

        private void BetsyNetPDFCtrl_Enter(object sender, EventArgs e)
        {
            wrapper.FocusViewer();
        }
        #endregion
    }
}

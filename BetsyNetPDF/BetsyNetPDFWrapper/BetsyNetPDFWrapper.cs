/*
Copyright 2012, 2013 IBB Ehlert&Wolf GbR
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
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace BetsyNetPDF
{
    public class BetsyNetPDFWrapper
    {
        private IBetsyNetPDFApi api;
        private IntPtr win;

        private CallBackOnSelectionChanged onSelectionChangedDelegate;
        public CallBackOnSelectionChanged OnSelectionChangedDelegate { get { return onSelectionChangedDelegate; } }

        private CallBackOnMouseClick onMouseClickDelegate;
        public CallBackOnMouseClick OnMouseClickDelegate { get { return onMouseClickDelegate; } }

        private CallBackOnDelete onDeleteDelegate;
        public CallBackOnDelete OnDeleteDelegate { get { return onDeleteDelegate; } }

        private CallBackOnObjectMoved onObjectMovedDelegate;
        public CallBackOnObjectMoved OnObjectMovedDelegate { get { return onObjectMovedDelegate; } }

        private CallBackOnRequestContextMenu onRequestContextMenuDelegate;
        public CallBackOnRequestContextMenu OnRequestContextMenuDelegate { get { return onRequestContextMenuDelegate; } }

        private CallBackOnMouseOverObject onMouseOverObjectDelegate;
        public CallBackOnMouseOverObject OnMouseOverObjectDelegate { get { return onMouseOverObjectDelegate; } }

        private CallBackOnDistanceMeasuredDelegate onDistanceMeasuredDelegate;
        public CallBackOnDistanceMeasuredDelegate OnDistanceMeasuredDelegate { get { return onDistanceMeasuredDelegate; } }

        private CallBackOnLineDrawnDelegate onLineDrawnDelegate;
        public CallBackOnLineDrawnDelegate OnLineDrawnDelegate { get { return onLineDrawnDelegate; } }

        public event MouseClickEventHandler MouseClickEvent;
        public event SelectionChangedEventHandler SelectionChangedEvent;
        public event DeleteEventHandler DeleteEvent;
        public event ObjectMovedEventHandler ObjectMovedEvent;
        public event RequestContextMenuEventHandler RequestContextMenuEvent;
        public event MouseOverObjectEventHandler MouseOverObjectEvent;
        public event DistanceMeasuredEventHandler DistanceMeasuredEvent;
        public event LineDrawnEventHandler LineDrawnEvent;

        public BetsyNetPDFWrapper()
        {
            win = IntPtr.Zero;

            onSelectionChangedDelegate = new CallBackOnSelectionChanged(this.OnSelectionChangedCallBack);
            onMouseClickDelegate = new CallBackOnMouseClick(this.OnMouseClickCallBack);
            onDeleteDelegate = new CallBackOnDelete(this.OnDeleteCallBack);
            onObjectMovedDelegate = new CallBackOnObjectMoved(this.OnObjectMovedCallBack);
            onRequestContextMenuDelegate = new CallBackOnRequestContextMenu(this.OnRequestContextMenuCallBack);
            onMouseOverObjectDelegate = new CallBackOnMouseOverObject(this.OnMouseOverObjectCallBack);
            onDistanceMeasuredDelegate = new CallBackOnDistanceMeasuredDelegate(this.OnDistanceMeasuredCallBack);
            onLineDrawnDelegate = new CallBackOnLineDrawnDelegate(this.OnLineDrawnCallBack);

            InitWrapper();
        }

        #region properties
        public string Version { get { return BetsyNetPDF.AssemblyInfo.VERSION; } }
        #endregion

        #region public methods
        public void BetsyNetPDFViewer(string hwnd, string file, bool useExternContextMenu)
        {
            if (api == null)
                return;

            if (win == IntPtr.Zero)
                win = api.BetsyNetPDFViewer(hwnd, file, useExternContextMenu, false, this.onSelectionChangedDelegate, this.onMouseClickDelegate, this.onDeleteDelegate, this.onObjectMovedDelegate, this.OnRequestContextMenuDelegate, this.onMouseOverObjectDelegate, this.OnDistanceMeasuredDelegate, this.OnLineDrawnDelegate);
            else
                api.OpenNewFile(win, file);
        }

        public void DirectPrinting(string file)
        {
            if (api == null)
                return;

            win = api.BetsyNetPDFViewer("", file, false, true, this.onSelectionChangedDelegate, this.onMouseClickDelegate, this.onDeleteDelegate, this.onObjectMovedDelegate, this.OnRequestContextMenuDelegate, this.onMouseOverObjectDelegate, this.OnDistanceMeasuredDelegate, this.OnLineDrawnDelegate);
        }

        public bool IsDocOpen()
        {
            if (api == null || win == IntPtr.Zero)
                return false;

            return api.IsDocOpen(win);
        }

        public void UpdateViewer(string hwnd)
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.UpdateViewer(win, hwnd);
        }

        public void SetMouseOverEnabled(bool enabled)
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.SetMouseOverEnabled(win, enabled);
        }

        public void SetMeasureModeEnabled(bool enabled)
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.SetMeasureModeEnabled(win, enabled);
        }

        public void SetLineModeEnabled(bool enabled)
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.SetLineModeEnabled(win, enabled);
        }

        public void SetDeactivateTextSelection(bool value)
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.SetDeactivateTextSelection(win, value);
        }

        public void SetPreventOverlayObjectSelection(bool value)
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.SetPreventOverlayObjectSelection(win, value);
        }

        public void SetShowOverlapping(bool value)
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.SetShowOverlapping(win, value);
        }

        public void SetHideLabels(bool value)
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.SetHideLabels(win, value);
        }

        public void SetTransparantOverlayObjects(bool value)
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.SetTransparantOverlayObjects(win, value);
        }

        public PointF CvtScreen2Doc(Point screenCoords)
        {
            if (api == null || win == IntPtr.Zero)
                return PointF.Empty;

            return api.CvtScreen2Doc(win, screenCoords);
        }

        public Point CvtDoc2Screen(PointF docCoords)
        {
            if (api == null || win == IntPtr.Zero)
                return Point.Empty;

            return api.CvtDoc2Screen(win, docCoords);
        }

        public void FocusViewer()
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.FocusViewer(win);
        }

        public void SaveAs()
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.SaveAs(win);
        }

        public void Print(bool printOnPlotter)
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.Print(win, printOnPlotter);
        }

        public void FitPageWidth()
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.FitPageWidth(win);
        }

        public void FitWholePage()
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.FitWholePage(win);
        }

        public void ZoomOut()
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.ZoomOut(win);
        }

        public void ZoomIn()
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.ZoomIn(win);
        }

        public void RotateLeft()
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.RotateLeft(win);
        }

        public void RotateRight()
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.RotateRight(win);
        }

        public void RotateCounterClockWise(int angle)
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.RotateCounterClockWise(win, angle);
        }

        public int GetDocumentRotation()
        {
            if (api == null || win == IntPtr.Zero)
                return 0;
            return api.GetDocumentRotation(win);
        }

        //public void ProcessOverlayObject(string id, string label, string font, double x, double y, double dx, double dy, double angle, int fontSize, Color foreColor, Color backColor, bool update)
        //{
        //    if (api == null || win == IntPtr.Zero)
        //        return;
        //    api.ProcessOverlayObject(win, id, label, font, x, y, dx, dy, angle, fontSize, foreColor, backColor, update);
        //}

        // {id|label|x|y|dx|dy|lx|ly|rx|ry|angle|labelAngle|font|fontSize|fgR|fgG|fgB|bgR|bgG|bgB}
        public void ProcessOverlayObjects(string objs)
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.ProcessOverlayObjects(win, objs);
        }

        public void SetSelectedOverlayObjects(string objectIds)
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.SetSelectedOverlayObjects(win, objectIds);
        }

        public void RemoveOverlayObject(string id)
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.RemoveOverlayObject(win, id);
        }

        public string GetSelectedOverlayObjectIds()
        {
            if (api == null || win == IntPtr.Zero)
                return "";

            return api.GetSelectedOverlayObjectIds(win);
        }

        public string GetSelectedOverlayObjects()
        {
            if (api == null || win == IntPtr.Zero)
                return "";
            return api.GetSelectedOverlayObjects(win);
        }

        public string GetAllOverlayObjects()
        {
            if (api == null || win == IntPtr.Zero)
                return "";
            return api.GetAllOverlayObjects(win);
        }

        public string GetOverlayObjectAtPosition(double x, double y)
        {
            if (api == null || win == IntPtr.Zero)
                return "";
            return api.GetOverlayObjectAtPosition(win, x, y);
        }

        public void ClearOverlayObjectList()
        {
            if (api == null || win == IntPtr.Zero)
                return;
            api.ClearOverlayObjectList(win);
        }
        #endregion

        #region public call back methods
        public void OnSelectionChangedCallBack() { OnSelectionChanged(); }
        public void OnMouseClickCallBack(double x, double y) { OnMouseClick(x, y); }
        public void OnDeleteCallBack() { OnDelete(); }
        public void OnObjectMovedCallBack(double deltaX, double deltaY, bool moveLabel) { OnObjectMoved(deltaX, deltaY, moveLabel); }

        public void OnRequestContextMenuCallBack(int x, int y, IntPtr idPtr)
        {
            string sid = Marshal.PtrToStringAnsi(idPtr);
            OnRequestContextMenu(x, y, sid);
        }

        public void OnMouseOverObjectCallBack(IntPtr idPtr)
        {
            string sid = Marshal.PtrToStringAnsi(idPtr);
            if (!string.IsNullOrEmpty(sid))
                OnMouseOverObject(sid);
        }
        public void OnDistanceMeasuredCallBack(double distance) { OnDistanceMeasured(distance); }
        public void OnLineDrawnCallBack(double p1x, double p1y, double p2x, double p2y) { OnLineDrawn(p1x, p1y, p2x, p2y); }
        #endregion

        #region private methods
        private void InitWrapper()
        {
            if (IntPtr.Size == 4)
                api = new ThirtyTwoBitApi();
            else
                api = new SixtyFourBitApi();
        }
        #endregion

        #region event handler methods
        protected virtual void OnMouseClick(double x, double y)
        {
            if (MouseClickEvent != null)
                MouseClickEvent(x, y);
        }

        protected virtual void OnMouseOverObject(string id)
        {
            if (MouseOverObjectEvent != null)
                MouseOverObjectEvent(id);
        }

        protected virtual void OnSelectionChanged()
        {
            if (SelectionChangedEvent != null)
                SelectionChangedEvent();
        }

        protected virtual void OnDelete()
        {
            if (DeleteEvent != null)
                DeleteEvent();
        }

        protected virtual void OnObjectMoved(double deltaX, double deltaY, bool moveLabel)
        {
            if (ObjectMovedEvent != null)
                ObjectMovedEvent(deltaX, deltaY, moveLabel);
        }

        protected virtual void OnRequestContextMenu(int x, int y, string sid)
        {
            if (RequestContextMenuEvent != null)
                RequestContextMenuEvent(x, y, sid);
        }

        protected virtual void OnDistanceMeasured(double distance)
        {
            if (DistanceMeasuredEvent != null)
                DistanceMeasuredEvent(distance);
        }

        protected virtual void OnLineDrawn(double p1x, double p1y, double p2x, double p2y)
        {
            if (LineDrawnEvent != null)
                LineDrawnEvent(p1x, p1y, p2x, p2y);
        }
        #endregion

        #region delegates
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallBackOnSelectionChanged();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallBackOnMouseClick(double x, double y);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallBackOnDelete();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallBackOnObjectMoved(double deltaX, double deltaY, bool moveLabel);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallBackOnRequestContextMenu(int x, int y, IntPtr idPtr);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallBackOnMouseOverObject(IntPtr idPtr);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallBackOnDistanceMeasuredDelegate(double distance);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallBackOnLineDrawnDelegate(double p1x, double p1y, double p2x, double p2y);

        public delegate void MouseClickEventHandler(double x, double y);
        public delegate void SelectionChangedEventHandler();
        public delegate void DeleteEventHandler();
        public delegate void ObjectMovedEventHandler(double deltaX, double deltaY, bool moveLabel);
        public delegate void RequestContextMenuEventHandler(int x, int y, string sid);
        public delegate void MouseOverObjectEventHandler(string id);
        public delegate void DistanceMeasuredEventHandler(double distance);
        public delegate void LineDrawnEventHandler(double p1x, double p1y, double p2x, double p2y);
        #endregion
    }
}

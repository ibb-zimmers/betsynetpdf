/*
Copyright 2012-2016 IBB Ehlert&Wolf GbR
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
    public class SixtyFourBitApi : IBetsyNetPDFApi
    {
        private const string DLL = "BetsyNetPDF-x64.dll";

        #region IBetsyNetPDFApi Member

        [DllImport(DLL, EntryPoint = "CallBetsyNetPDFViewer", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern IntPtr BetsyNetPDFViewer_EXT(string hwnd, string file, bool useExternContextMenu, bool directPrinting, bool defaultPrinter, string printerName, IntPtr win,
            IntPtr onSelectionChangedDelegate,
            IntPtr onMouseClickDelegate,
            IntPtr onDeleteDelegate,
            IntPtr onObjectMovedDelegate,
            IntPtr onRequestContextMenuDelegate,
            IntPtr onMouseOverObjectDelegate,
            IntPtr onDistanceMeasuredDelegate,
            IntPtr onLineDrawnDelegate);

        public IntPtr BetsyNetPDFViewer(string hwnd, string file, bool useExternContextMenu, bool directPrinting, bool defaultPrinter, string printerName, IntPtr win,
            BetsyNetPDFWrapper.CallBackOnSelectionChanged onSelectionChangedDelegate,
            BetsyNetPDFWrapper.CallBackOnMouseClick onMouseClickDelegate,
            BetsyNetPDFWrapper.CallBackOnDelete onDeleteDelegate,
            BetsyNetPDFWrapper.CallBackOnObjectMoved onObjectMovedDelegate,
            BetsyNetPDFWrapper.CallBackOnRequestContextMenu onRequestContextMenuDelegate,
            BetsyNetPDFWrapper.CallBackOnMouseOverObject onMouseOverObjectDelegate,
            BetsyNetPDFWrapper.CallBackOnDistanceMeasuredDelegate onDistanceMeasuredDelegate,
            BetsyNetPDFWrapper.CallBackOnLineDrawnDelegate onLineDrawnDelegate)
        {
            IntPtr onSelectionChangedDelegatePtr = Marshal.GetFunctionPointerForDelegate(onSelectionChangedDelegate);
            IntPtr onMouseClickDelegatePtr = Marshal.GetFunctionPointerForDelegate(onMouseClickDelegate);
            IntPtr onDeleteDelegatePtr = Marshal.GetFunctionPointerForDelegate(onDeleteDelegate);
            IntPtr onObjectMovedDelegatePtr = Marshal.GetFunctionPointerForDelegate(onObjectMovedDelegate);
            IntPtr onReqContextMenuPtr = Marshal.GetFunctionPointerForDelegate(onRequestContextMenuDelegate);
            IntPtr onMouseOverPtr = Marshal.GetFunctionPointerForDelegate(onMouseOverObjectDelegate);
            IntPtr distancePtr = Marshal.GetFunctionPointerForDelegate(onDistanceMeasuredDelegate);
            IntPtr linePtr = Marshal.GetFunctionPointerForDelegate(onLineDrawnDelegate);

            return BetsyNetPDFViewer_EXT(hwnd, file, useExternContextMenu, directPrinting, defaultPrinter, printerName, win, onSelectionChangedDelegatePtr, onMouseClickDelegatePtr, onDeleteDelegatePtr, onObjectMovedDelegatePtr, onReqContextMenuPtr, onMouseOverPtr, distancePtr, linePtr);
        }

        [DllImport(DLL, EntryPoint = "CallOpenNewFile", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void OpenNewFile_EXT(IntPtr obj, string file);

        public void OpenNewFile(IntPtr obj, string file)
        {
            OpenNewFile_EXT(obj, file);
        }

        [DllImport(DLL, EntryPoint = "CallIsDocOpen", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        static private extern bool IsDocOpen_EXT(IntPtr obj);

        public bool IsDocOpen(IntPtr obj)
        {
            return IsDocOpen_EXT(obj);
        }

        [DllImport(DLL, EntryPoint = "CallUpdateViewer", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void UpdateViewer_EXT(IntPtr obj, string hwnd);

        public void UpdateViewer(IntPtr obj, string hwnd)
        {
            UpdateViewer_EXT(obj, hwnd);
        }

        [DllImport(DLL, EntryPoint = "CallFocusViewer", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void FocusViewer_EXT(IntPtr obj);

        public void FocusViewer(IntPtr obj)
        {
            FocusViewer_EXT(obj);
        }

        [DllImport(DLL, EntryPoint = "CallSetMouseOverEnabled", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void SetMouseOverEnabled_EXT(IntPtr obj, bool enabled);

        public void SetMouseOverEnabled(IntPtr obj, bool enabled)
        {
            SetMouseOverEnabled_EXT(obj, enabled);
        }

        [DllImport(DLL, EntryPoint = "CallCvtScreen2Doc", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern IntPtr CvtScreen2Doc_EXT(IntPtr obj, IntPtr screenCoords);

        public PointF CvtScreen2Doc(IntPtr obj, Point screenCoords)
        {
            IntPtr screenCoordsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(screenCoords));
            Marshal.StructureToPtr(screenCoords, screenCoordsPtr, false);

            IntPtr docCoords = CvtScreen2Doc_EXT(obj, screenCoordsPtr);

            return (PointF)Marshal.PtrToStructure(docCoords, typeof(PointF));
        }

        [DllImport(DLL, EntryPoint = "CallCvtDoc2Screen", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern IntPtr CvtDoc2Screen_EXT(IntPtr obj, IntPtr docCoords);

        public Point CvtDoc2Screen(IntPtr obj, PointF docCoords)
        {
            IntPtr docCoordsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(docCoords));
            Marshal.StructureToPtr(docCoords, docCoordsPtr, false);

            IntPtr screenCoords = CvtDoc2Screen_EXT(obj, docCoordsPtr);

            return (Point)Marshal.PtrToStructure(screenCoords, typeof(Point));
        }

        [DllImport(DLL, EntryPoint = "CallSetMeasureModeEnabled", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void SetMeasureModeEnabled_EXT(IntPtr obj, bool enabled);

        public void SetMeasureModeEnabled(IntPtr obj, bool enabled)
        {
            SetMeasureModeEnabled_EXT(obj, enabled);
        }

        [DllImport(DLL, EntryPoint = "CallSetLineModeEnabled", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void SetLineModeEnabled_EXT(IntPtr obj, bool enabled);

        public void SetLineModeEnabled(IntPtr obj, bool enabled)
        {
            SetLineModeEnabled_EXT(obj, enabled);
        }

        [DllImport(DLL, EntryPoint = "CallSetLineStart", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void SetLineStart_EXT(IntPtr obj, double x, double y);

        public void SetLineStart(IntPtr win, double x, double y)
        {
            SetLineStart_EXT(win, x, y);
        }

        [DllImport(DLL, EntryPoint = "CallGetLineStart", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern IntPtr GetLineStart_EXT(IntPtr obj);

        public PointF GetLineStart(IntPtr win)
        {
            IntPtr startCoords = GetLineStart_EXT(win);
            return (PointF)Marshal.PtrToStructure(startCoords, typeof(PointF));
        }

        //http://blogs.msdn.com/b/jaredpar/archive/2008/10/14/pinvoke-and-bool-or-should-i-say-bool.aspx
        [DllImport(DLL, EntryPoint = "CallIsLineStart", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        static private extern bool IsLineStart_EXT(IntPtr obj);

        public bool IsLineStart(IntPtr win)
        {
            return IsLineStart_EXT(win);
        }

        [DllImport(DLL, EntryPoint = "CallSetFixedAngle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void SetFixedAngle_EXT(IntPtr obj, double angle);

        public void SetFixedAngle(IntPtr obj, double angle)
        {
            SetFixedAngle_EXT(obj, angle);
        }

        [DllImport(DLL, EntryPoint = "CallSetDeactivateTextSelection", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void SetDeactivateTextSelection_EXT(IntPtr obj, bool value);

        public void SetDeactivateTextSelection(IntPtr obj, bool value)
        {
            SetDeactivateTextSelection_EXT(obj, value);
        }

        [DllImport(DLL, EntryPoint = "CallSetPreventOverlayObjectSelection", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void SetPreventOverlayObjectSelection_EXT(IntPtr obj, bool value);

        public void SetPreventOverlayObjectSelection(IntPtr obj, bool value)
        {
            SetPreventOverlayObjectSelection_EXT(obj, value);
        }

        [DllImport(DLL, EntryPoint = "CallSetShowOverlapping", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void SetShowOverlapping_EXT(IntPtr obj, bool value);

        public void SetShowOverlapping(IntPtr obj, bool value)
        {
            SetShowOverlapping_EXT(obj, value);
        }

        [DllImport(DLL, EntryPoint = "CallSetHideLabels", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void SetHideLabels_EXT(IntPtr obj, bool value);

        public void SetHideLabels(IntPtr obj, bool value)
        {
            SetHideLabels_EXT(obj, value);
        }

        [DllImport(DLL, EntryPoint = "CallSetTransparantOverlayObjects", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void SetTransparantOverlayObjects_EXT(IntPtr obj, bool value);

        public void SetTransparantOverlayObjects(IntPtr obj, bool value)
        {
            SetTransparantOverlayObjects_EXT(obj, value);
        }

        [DllImport(DLL, EntryPoint = "CallSaveAs", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void SaveAs_EXT(IntPtr obj);

        public void SaveAs(IntPtr obj)
        {
            SaveAs_EXT(obj);
        }

        [DllImport(DLL, EntryPoint = "CallPrint", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void Print_EXT(IntPtr obj, bool printOnPlotter);

        public void Print(IntPtr obj, bool printOnPlotter)
        {
            Print_EXT(obj, printOnPlotter);
        }

        [DllImport(DLL, EntryPoint = "CallFitPageWidth", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void FitPageWidth_EXT(IntPtr obj);

        public void FitPageWidth(IntPtr obj)
        {
            FitPageWidth_EXT(obj);
        }

        [DllImport(DLL, EntryPoint = "CallFitWholePage", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void FitWholePage_EXT(IntPtr obj);

        public void FitWholePage(IntPtr obj)
        {
            FitWholePage_EXT(obj);
        }

        [DllImport(DLL, EntryPoint = "CallZoomOut", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void ZoomOut_EXT(IntPtr obj);

        public void ZoomOut(IntPtr obj)
        {
            ZoomOut_EXT(obj);
        }

        [DllImport(DLL, EntryPoint = "CallZoomIn", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void ZoomIn_EXT(IntPtr obj);

        public void ZoomIn(IntPtr obj)
        {
            ZoomIn_EXT(obj);
        }

        [DllImport(DLL, EntryPoint = "CallRotateLeft", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void RotateLeft_EXT(IntPtr obj);

        public void RotateLeft(IntPtr obj)
        {
            RotateLeft_EXT(obj);
        }

        [DllImport(DLL, EntryPoint = "CallRotateRight", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void RotateRight_EXT(IntPtr obj);

        public void RotateRight(IntPtr obj)
        {
            RotateRight_EXT(obj);
        }

        [DllImport(DLL, EntryPoint = "CallRotateCounterClockWise", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void RotateCounterClockWise_EXT(IntPtr obj, int angle);

        public void RotateCounterClockWise(IntPtr obj, int angle)
        {
            RotateCounterClockWise_EXT(obj, angle);
        }

        [DllImport(DLL, EntryPoint = "CallGetDocumentRotation", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern int GetDocumentRotation_EXT(IntPtr obj);

        public int GetDocumentRotation(IntPtr obj)
        {
            return GetDocumentRotation_EXT(obj);
        }

        //[DllImport(DLL, EntryPoint = "CallProcessOverlayObject", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //static private extern void ProcessOverlayObject_EXT(IntPtr obj, string id, string label, string font, double x, double y, double dx, double dy, double angle, float fontSize, int foreGround, int backGround, bool update);

        //public void ProcessOverlayObject(IntPtr obj, string id, string label, string font, double x, double y, double dx, double dy, double angle, float fontSize, Color foreGround, Color backGround, bool update)
        //{
        //    ProcessOverlayObject_EXT(obj, id, label, font, x, y, dx, dy, angle, fontSize, foreGround.ToArgb(), backGround.ToArgb(), update);
        //}

        [DllImport(DLL, EntryPoint = "CallProcessOverlayObjects", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void ProcessOverlayObjects_EXT(IntPtr obj, string objs);

        public void ProcessOverlayObjects(IntPtr obj, string objs)
        {
            ProcessOverlayObjects_EXT(obj, objs);
        }

        [DllImport(DLL, EntryPoint = "CallRemoveOverlayObject", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void RemoveOverlayObject_EXT(IntPtr obj, string id);

        public void RemoveOverlayObject(IntPtr obj, string id)
        {
            RemoveOverlayObject_EXT(obj, id);
        }

        [DllImport(DLL, EntryPoint = "CallGetSelectedOverlayObjectIds", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern IntPtr GetSelectedOverlayObjectIds_EXT(IntPtr obj);

        public string GetSelectedOverlayObjectIds(IntPtr obj)
        {
            IntPtr ptr = GetSelectedOverlayObjectIds_EXT(obj);
            string value = Marshal.PtrToStringAnsi(ptr);

            return value;
        }

        [DllImport(DLL, EntryPoint = "CallGetSelectedOverlayObjects", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern IntPtr GetSelectedOverlayObjects_EXT(IntPtr obj);

        public string GetSelectedOverlayObjects(IntPtr obj)
        {
            IntPtr ptr = GetSelectedOverlayObjects_EXT(obj);
            string value = Marshal.PtrToStringAnsi(ptr);

            return value;
        }

        [DllImport(DLL, EntryPoint = "CallGetAllOverlayObjects", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern IntPtr GetAllOverlayObjects_EXT(IntPtr obj);

        public string GetAllOverlayObjects(IntPtr obj)
        {
            IntPtr ptr = GetAllOverlayObjects_EXT(obj);
            string value = Marshal.PtrToStringAnsi(ptr);

            return value;
        }

        [DllImport(DLL, EntryPoint = "CallGetOverlayObjectAtPosition", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern IntPtr GetOverlayObjectAtPosition_EXT(IntPtr obj, double x, double y);

        public string GetOverlayObjectAtPosition(IntPtr obj, double x, double y)
        {
            IntPtr ptr = GetOverlayObjectAtPosition_EXT(obj, x, y);
            string value = Marshal.PtrToStringAnsi(ptr);

            return value;
        }

        [DllImport(DLL, EntryPoint = "CallSetSelectedOverlayObjects", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void SetSelectedOverlayObjects_EXT(IntPtr obj, string objectIds);

        public void SetSelectedOverlayObjects(IntPtr obj, string objectIds)
        {
            SetSelectedOverlayObjects_EXT(obj, objectIds);
        }

        [DllImport(DLL, EntryPoint = "CallClearOverlayObjectList", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static private extern void ClearOverlayObjectList_EXT(IntPtr obj);

        public void ClearOverlayObjectList(IntPtr obj)
        {
            ClearOverlayObjectList_EXT(obj);
        }

        #endregion
    }
}

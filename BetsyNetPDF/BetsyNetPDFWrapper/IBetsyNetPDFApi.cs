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
using System.Text;
using System.Drawing;

namespace BetsyNetPDF
{
    interface IBetsyNetPDFApi
    {
        IntPtr BetsyNetPDFViewer(string hwnd, string file, bool useExternContextMenu, bool directPrinting, bool defaultPrinter, string printerName, IntPtr curWin,
            BetsyNetPDFWrapper.CallBackOnSelectionChanged onSelectionChangedDelegate,
            BetsyNetPDFWrapper.CallBackOnMouseClick onMouseClickDelegate,
            BetsyNetPDFWrapper.CallBackOnDelete onDeleteDelegate,
            BetsyNetPDFWrapper.CallBackOnObjectMoved onObjectMovedDelegate,
            BetsyNetPDFWrapper.CallBackOnRequestContextMenu onRequestContextMenuDelegate,
            BetsyNetPDFWrapper.CallBackOnMouseOverObject onMouseOverObjectDelegate,
            BetsyNetPDFWrapper.CallBackOnDistanceMeasuredDelegate onDistanceMeasuredDelegate,
            BetsyNetPDFWrapper.CallBackOnLineDrawnDelegate onLineDrawnDelegate);
        void OpenNewFile(IntPtr win, string file);
        bool IsDocOpen(IntPtr win);
        void UpdateViewer(IntPtr win, string hwnd);
        void FocusViewer(IntPtr win);
        void SetMouseOverEnabled(IntPtr win, bool enabled);
        void SetMeasureModeEnabled(IntPtr win, bool enabled);
        void SetLineModeEnabled(IntPtr win, bool enabled);
        void SetLineStart(IntPtr win, double x, double y);
        PointF GetLineStart(IntPtr win);
        bool IsLineStart(IntPtr win);
        void SetFixedAngle(IntPtr win, double angle);
        void SetDeactivateTextSelection(IntPtr win, bool value);
        void SetPreventOverlayObjectSelection(IntPtr win, bool value);
        void SetShowOverlapping(IntPtr win, bool value);
        void SetHideLabels(IntPtr win, bool value);
        void SetTransparantOverlayObjects(IntPtr win, bool value);
        PointF CvtScreen2Doc(IntPtr win, Point screenCoords);
        Point CvtDoc2Screen(IntPtr win, PointF docCoords);

        void SaveAs(IntPtr win);
        void Print(IntPtr win, bool printOnPlotter);
        void FitPageWidth(IntPtr win);
        void FitWholePage(IntPtr win);
        void ZoomOut(IntPtr win);
        void ZoomIn(IntPtr win);
        void RotateLeft(IntPtr win);
        void RotateRight(IntPtr win);
        void RotateCounterClockWise(IntPtr win, int angle);
        int GetDocumentRotation(IntPtr win);

        //void ProcessOverlayObject(IntPtr win, string id, string label, string font, double x, double y, double dx, double dy, double angle, float fontSize, Color foreGround, Color backGround, bool update);
        void ProcessOverlayObjects(IntPtr win, string objs);
        void SetSelectedOverlayObjects(IntPtr win, string objectIds);
        void RemoveOverlayObject(IntPtr win, string id);
        string GetSelectedOverlayObjectIds(IntPtr win);
        string GetSelectedOverlayObjects(IntPtr win);
        string GetAllOverlayObjects(IntPtr win);
        string GetOverlayObjectAtPosition(IntPtr win, double x, double y);
        void ClearOverlayObjectList(IntPtr win);
    }
}
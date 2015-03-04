/*
Copyright 2012 - 2015 IBB Ehlert&Wolf GbR
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BetsyNetPDF.Parser;
using DevExpress.XtraEditors;
using System.Drawing;


namespace BetsyNetPDF
{
    public partial class BetsyNetPDFViewer : XtraForm
    {
        private BetsyNetPDFCtrl ctrl;
        private string lastID = null;

        public BetsyNetPDFViewer()
        {
            InitializeComponent();
        }

        private void InitPdfCtrl()
        {
            ctrl = new BetsyNetPDFCtrl(null);
            ctrl.Dock = DockStyle.Fill;
            ctrl.BetsyNetPDFMouseClickEvent += new BetsyNetPDFCtrl.BetsyNetPDFMouseClickEventHandler(ctrl_BetsyNetPDFMouseClickEvent);
            ctrl.BetsyNetPDFDistanceMeasuredEvent += new BetsyNetPDFCtrl.BetsyNetPDFDistanceMeasuredEventHandler(ctrl_BetsyNetPDFDistanceMeasuredEvent);
            ctrl.BetsyNetPDFLineDrawnEvent += new BetsyNetPDFCtrl.BetsyNetPDFLineDrawnEventHandler(ctrl_BetsyNetPDFLineDrawnEvent);
            ctrl.BetsyNetPDFMouseOverObjectEvent += new BetsyNetPDFCtrl.BetsyNetPDFMouseOverObjectEventHandler(ctrl_BetsyNetPDFMouseOverObjectEvent);
            this.Controls.Add(ctrl);
        }

        void ctrl_BetsyNetPDFMouseOverObjectEvent(string id)
        {
            MessageBox.Show(id);
            ctrl.FocusViewer();
        }

        void ctrl_BetsyNetPDFLineDrawnEvent(double p1x, double p1y, double p2x, double p2y)
        {
            MessageBox.Show(string.Format("p1: {0} -- {1}" + Environment.NewLine + "p2: {2} -- {3}", p1x, p1y, p2x, p2y));
            barCheckItem2.Checked = false;
            ctrl.FocusViewer();
        }

        void ctrl_BetsyNetPDFDistanceMeasuredEvent(double distance)
        {
            MessageBox.Show(string.Format("distance: {0}", distance));
            barCheckItem3.Checked = false;
            ctrl.FocusViewer();
        }

        void ctrl_BetsyNetPDFMouseClickEvent(double x, double y, string key)
        {
            if (!barCheckAddObject.Checked)
                return;

            string id = DateTime.Now.Ticks.ToString();
            lastID = id;
            //{id|label|x|y|dx|dy|lx|ly|angle|font|fontSize|fgR|fgG|fgB|bgR|bgG|bgB}
            StringBuilder oo = new StringBuilder("{");
            oo.Append("{");
            oo.AppendFormat("{0}|", id);
            oo.AppendFormat("{0}|", id);
            oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", x);
            oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", y);
            if (barChkWithDimension.Checked)
            {
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", 2000);
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", 0);
            }
            else
            {
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", -1);
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", -1);
            }
            oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", -1.0);
            oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", -1.0);
            oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", 0.0);
            oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", 0.0);
            oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", 0.0);
            oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", 0.0);
            oo.AppendFormat("{0}|", "Arial");
            oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", 10.0);
            oo.AppendFormat("{0}|", 0);
            oo.AppendFormat("{0}|", 0);
            oo.AppendFormat("{0}|", 0);
            oo.AppendFormat("{0}|", 255);
            oo.AppendFormat("{0}|", 0);
            oo.AppendFormat("{0}|", 0);
            oo.Append("");
            oo.Append("}");

            ctrl.ProcessOverlayObjects(oo.ToString());

            ctrl.FocusViewer();
        }

        private void barBtnOpen_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string file = "";
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.DefaultExt = ".pdf";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                    file = ofd.FileName;
            }

            if (!string.IsNullOrEmpty(file))
            {
                if (ctrl == null)
                    InitPdfCtrl();
                ctrl.OpenPdfFile(file);
            }

            ctrl.FocusViewer();
        }

        private void barBtnQuit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Process.GetCurrentProcess().CloseMainWindow();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ctrl.RotateSelectedOverlayObjects(10);
            ctrl.FocusViewer();
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<LocatedObject> locObjs = BetsyNetPDFEditor.ExtractTextAndCoordinates(ctrl.CurrentFile, ".*");
            string text = "";
            foreach (LocatedObject obj in locObjs)
                text += string.Format("{0}: {1} -- {2}" + Environment.NewLine, obj.Text, obj.X, obj.Y);

            MessageBox.Show(text);
            ctrl.FocusViewer();
        }

        private void barCheckItem1_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ctrl.SetMouseOverEnabled(barCheckItem1.Checked);
            ctrl.FocusViewer();
        }

        private void barCheckItem2_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ctrl.SetLineModeEnabled(barCheckItem2.Checked);
            ctrl.FocusViewer();
        }

        private void barCheckItem3_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ctrl.SetMeasureModeEnabled(barCheckItem3.Checked);
            ctrl.FocusViewer();
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrEmpty(lastID))
                return;

            ctrl.SetSelectedOverlayObjects(string.Format("|{0}|", lastID));
            ctrl.FocusViewer();
        }

        private void barChkDeactivateTS_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ctrl.SetDeactivateTextSelection(barChkDeactivateTS.Checked);
            ctrl.FocusViewer();
        }

        private void barBtnTextCoordsLayer_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string layer = FormTextInput.ShowInputDialog("Enter layer...", "Layer:", "");
            List<LocatedObject> locObjs = BetsyNetPDFEditor.ExtractTextAndCoordsFromLayer(ctrl.CurrentFile, layer, ".*");
            IOrderedEnumerable<LocatedObject> ordered = locObjs.OrderBy(x => x.Text);
            string text = "";
            foreach (LocatedObject obj in ordered)
                text += string.Format("{0}:\t{1}\t--\t{2}\t{3}" + Environment.NewLine, obj.Text, obj.X, obj.Y, obj.Angle);

            MessageBox.Show(text);
            ctrl.FocusViewer();
        }

        private void barChkShowOverlapping_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ctrl.SetShowOverlapping(barChkShowOverlapping.Checked);
            ctrl.FocusViewer();
        }

        private void barCheckItem4_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ctrl.SetTransparantOverlayObjects(barCheckItem4.Checked);
            ctrl.FocusViewer();
        }

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<string> layers = BetsyNetPDFEditor.GetLayers(ctrl.CurrentFile);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Available layers:");
            foreach (string l in layers)
                sb.AppendLine(l);

            MessageBox.Show(sb.ToString(), "Layers", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ctrl.FocusViewer();
        }

        private void barButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InitPdfCtrl();
        }

        private void barButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            String[] pdffiles = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "*.pdf");
            string files = "";
            foreach (string f in pdffiles)
            {
                if (string.IsNullOrEmpty(files))
                {
                    files += f;
                    continue;
                }

                files += "\" \"" + f;
            }

            DialogResult res = MessageBox.Show("Print to default printer?", "Print to default printer?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            BetsyNetPDFCtrl.DirectPrinting(files, res == DialogResult.Yes, "");
            ctrl.FocusViewer();
        }

        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            String[] pdffiles = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "*.pdf");
            string files = "";
            foreach (string f in pdffiles)
            {
                if (string.IsNullOrEmpty(files))
                {
                    files += f;
                    continue;
                }

                files += "\" \"" + f;
            }

            BetsyNetPDFCtrl.DirectPrinting(files, false, "PDFCreator");
            ctrl.FocusViewer();
        }

        private void barButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<PointF> linePoints = ctrl.GetLinePointsFromPdf(0.01f);
            //{id|label|x|y|dx|dy|lx|ly|angle|font|fontSize|fgR|fgG|fgB|bgR|bgG|bgB}
            StringBuilder oo = new StringBuilder("");
            PointF p1, p2;
            for (int i = 0; i < linePoints.Count; i = i + 2)
            {
                p1 = linePoints[i];
                p2 = linePoints[i + 1];

                oo.Append("{");
                oo.AppendFormat("{0}|", i.ToString());
                oo.AppendFormat("{0}|", "foo");
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", p1.X);
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", p1.Y);
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", CalcLength(p1.X, p1.Y, p2.X, p2.Y));
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", 0.0);
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", -1.0);
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", -1.0);
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", 0.0);
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", 0.0);
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", CalcAngle(p1.X, p1.Y, p2.X, p2.Y));
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", 0.0);
                oo.AppendFormat("{0}|", "Arial");
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", 10.0);
                oo.AppendFormat("{0}|", 0);
                oo.AppendFormat("{0}|", 0);
                oo.AppendFormat("{0}|", 0);
                oo.AppendFormat("{0}|", 255);
                oo.AppendFormat("{0}|", 0);
                oo.AppendFormat("{0}|", 0);
                oo.Append("");
                oo.Append("}");
            }

            ctrl.ProcessOverlayObjects(oo.ToString());

            ctrl.FocusViewer();
        }

        private static double CalcAngle(double p1x, double p1y, double p2x, double p2y)
        {
            double radians = Math.Atan2(p2y - p1y, p2x - p1x);
            double degree = radians * 180 / Math.PI;

            while (degree < 0.0)
                degree += 360.0;

            while (degree > 360.0)
                degree -= 360.0;

            return degree;
        }

        private static double CalcLength(double p1x, double p1y, double p2x, double p2y)
        {
            double pdfA = p2x - p1x;
            double pdfB = p2y - p1y;
            double pdfLength = (pdfA * pdfA) + (pdfB * pdfB);

            return Math.Sqrt(pdfLength);
        }

        private void barCheckItem5_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ctrl.SetHideLabels(barCheckItem5.Checked);
            ctrl.FocusViewer();
        }

        private void barButtonItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ctrl.SetLineModeEnabled(true);
            ctrl.SetFixedAngle(45);
        }

        private void barButtonItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool islinestart = ctrl.IsLineStart();
            MessageBox.Show(islinestart.ToString());
        }

        private void barButtonItem14_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            PointF pointstart = ctrl.GetLineStart();
            MessageBox.Show(string.Format("x: {0}\ty:{1}", pointstart.X, pointstart.Y));
        }
    }
}
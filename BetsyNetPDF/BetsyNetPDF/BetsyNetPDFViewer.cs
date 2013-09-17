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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BetsyNetPDF.Parser;
using DevExpress.XtraEditors;


namespace BetsyNetPDF
{
    public partial class BetsyNetPDFViewer : XtraForm
    {
        private BetsyNetPDFCtrl ctrl;
        private string lastID = null;

        public BetsyNetPDFViewer()
        {
            InitializeComponent();

            ctrl = new BetsyNetPDFCtrl(null);
            ctrl.Dock = DockStyle.Fill;
            ctrl.BetsyNetPDFMouseClickEvent += new BetsyNetPDFCtrl.BetsyNetPDFMouseClickEventHandler(ctrl_BetsyNetPDFMouseClickEvent);
            ctrl.BetsyNetPDFSelectionChangedEvent += new BetsyNetPDFCtrl.BetsyNetPDFSelectionChangedEventHandler(ctrl_BetsyNetPDFSelectionChangedEvent);
            ctrl.BetsyNetPDFDistanceMeasuredEvent += new BetsyNetPDFCtrl.BetsyNetPDFDistanceMeasuredEventHandler(ctrl_BetsyNetPDFDistanceMeasuredEvent);
            ctrl.BetsyNetPDFLineDrawnEvent += new BetsyNetPDFCtrl.BetsyNetPDFLineDrawnEventHandler(ctrl_BetsyNetPDFLineDrawnEvent);
            ctrl.BetsyNetPDFMouseOverObjectEvent += new BetsyNetPDFCtrl.BetsyNetPDFMouseOverObjectEventHandler(ctrl_BetsyNetPDFMouseOverObjectEvent);
            this.Controls.Add(ctrl);
        }

        void ctrl_BetsyNetPDFMouseOverObjectEvent(string id)
        {
            MessageBox.Show(id);
        }

        void ctrl_BetsyNetPDFLineDrawnEvent(double p1x, double p1y, double p2x, double p2y)
        {
            MessageBox.Show(string.Format("p1: {0} -- {1}" + Environment.NewLine + "p2: {2} -- {3}", p1x, p1y, p2x, p2y));
            barCheckItem2.Checked = false;
        }

        void ctrl_BetsyNetPDFDistanceMeasuredEvent(double distance)
        {
            MessageBox.Show(string.Format("distance: {0}", distance));
            barCheckItem3.Checked = false;
        }

        void ctrl_BetsyNetPDFSelectionChangedEvent()
        {
            string selectedObjs = ctrl.GetSelectedOverlayObjectIds();
            Console.WriteLine(selectedObjs);
        }

        void ctrl_BetsyNetPDFMouseClickEvent(double x, double y)
        {
            if (!barCheckAddObject.Checked)
                return;

            string id = DateTime.Now.Ticks.ToString();
            lastID = id;
            //{id|label|x|y|dx|dy|lx|ly|angle|font|fontSize|fgR|fgG|fgB|bgR|bgG|bgB}
            StringBuilder oo = new StringBuilder("{");
            oo.AppendFormat("{0}|", id);
            oo.AppendFormat("{0}|", id);
            oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", x);
            oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", y);
            if (barChkWithDimension.Checked)
            {
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", 1000);
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", 1000);
            }
            else
            {
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", -1);
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", -1);
            }
            oo.Append("-1|-1|");
            if (barChkWithDimension.Checked)
            {
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", 500);
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", -500);
            }
            else
            {
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", 0.0);
                oo.AppendFormat(CultureInfo.InvariantCulture, "{0}|", 0.0);
            }
            oo.Append(ctrl.GetDocumentRotation());
            oo.Append("|Arial|10|255|255|255|0|0|0}");
            ctrl.ProcessOverlayObjects(oo.ToString());
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
                ctrl.OpenPdfFile(file);
        }

        private void barBtnQuit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Process.GetCurrentProcess().CloseMainWindow();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ctrl.RotateSelectedOverlayObjects(10);
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<LocatedObject> locObjs = BetsyNetPDFEditor.ExtractTextAndCoordinates(ctrl.CurrentFile, ".*");
            string text = "";
            foreach (LocatedObject obj in locObjs)
                text += string.Format("{0}: {1} -- {2}" + Environment.NewLine, obj.Text, obj.X, obj.Y);

            MessageBox.Show(text);
        }

        private void barCheckItem1_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ctrl.SetMouseOverEnabled(barCheckItem1.Checked);
        }

        private void barCheckItem2_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ctrl.SetLineModeEnabled(barCheckItem2.Checked);
        }

        private void barCheckItem3_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ctrl.SetMeasureModeEnabled(barCheckItem3.Checked);
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrEmpty(lastID))
                return;

            ctrl.SetSelectedOverlayObjects(string.Format("|{0}|", lastID));
        }

        private void barChkDeactivateTS_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ctrl.SetDeactivateTextSelection(barChkDeactivateTS.Checked);
        }

        private void barBtnTextCoordsLayer_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<LocatedObject> locObjs = BetsyNetPDFEditor.ExtractTextAndCoordsFromLayer(ctrl.CurrentFile, "BETSY", ".*");
            IOrderedEnumerable<LocatedObject> ordered = locObjs.OrderBy(x => x.Text);
            string text = "";
            foreach (LocatedObject obj in ordered)
                text += string.Format("{0}:\t{1}\t--\t{2}\t{3}" + Environment.NewLine, obj.Text, obj.X, obj.Y, obj.Angle);

            MessageBox.Show(text);
        }
    }
}
/*
Copyright 2012-2014 IBB Ehlert&Wolf GbR
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BetsyNetPDF
{
    public partial class BetsyNetPDFCtrlWF : UserControl
    {
        private BetsyNetPDFWrapper wrapper;
        private bool useExternContextMenu, printOnPlotter;

        public string Version { get { if (wrapper != null)return wrapper.Version; return ""; } }
        public bool UseExternContextMenu { get { return useExternContextMenu; } set { this.useExternContextMenu = value; } }
        public bool PrintOnPlotter { get { return printOnPlotter; } set { printOnPlotter = value; } }

        public BetsyNetPDFCtrlWF()
        {
            InitializeComponent();
            this.useExternContextMenu = false;
            this.printOnPlotter = false;

            wrapper = new BetsyNetPDFWrapper();
        }

        #region public methods
        public void OpenPdfFile(string file)
        {
            wrapper.BetsyNetPDFViewer(GetCurrentHWND(), file, UseExternContextMenu);
        }
        #endregion

        #region private methods
        private string GetCurrentHWND()
        {
            string hwnd = "";

            if (IntPtr.Size == 4)
                hwnd = ViewerArea.Handle.ToInt32().ToString();
            else
                hwnd = ViewerArea.Handle.ToInt64().ToString();

            return hwnd;
        }
        #endregion

        #region events
        private void ViewerArea_Resize(object sender, EventArgs e)
        {
            wrapper.UpdateViewer(GetCurrentHWND());
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            wrapper.SaveAs();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            wrapper.Print(PrintOnPlotter);
        }

        private void btnFitWidth_Click(object sender, EventArgs e)
        {
            wrapper.FitPageWidth();
        }

        private void btnFitWholePage_Click(object sender, EventArgs e)
        {
            wrapper.FitWholePage();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            wrapper.ZoomOut();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            wrapper.ZoomIn();
        }

        private void btnRotateLeft_Click(object sender, EventArgs e)
        {
            wrapper.RotateLeft();
        }

        private void btnRotateRight_Click(object sender, EventArgs e)
        {
            wrapper.RotateRight();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            using (AboutDialog about = new AboutDialog("rev. " + this.Version))
            {
                about.ShowDialog();
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace BetsyNetPDF
{
    public partial class FormTextInput : XtraForm
    {
        private FormTextInput()
        {
            InitializeComponent();
        }

        public static string ShowInputDialog(string dialogTitle, string labelText, string stdInput)
        {
            string input = string.Empty;

            using (FormTextInput fti = new FormTextInput())
            {
                fti.Text = dialogTitle;
                fti.labelControl1.Text = labelText;
                fti.textEdit1.Text = stdInput;

                if (fti.ShowDialog() == DialogResult.OK)
                    input = fti.textEdit1.Text;
            }

            return input;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATD_ID4P.UIComponent
{
    public partial class frmWaiting : Form
    {
        public frmWaiting()
        {
            InitializeComponent();

            // Subscribe to the FormClosing event.
            this.FormClosing += frmWaiting_FormClosing;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }


        private void frmWaiting_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Cancel the close operation if the user clicked the close button
            // or the form is being closed programmatically.
            if (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.ApplicationExitCall)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        public void SetMessage(string Title = "Processing", string Msg = "Please wait...")
        {
            if (lblTitle.InvokeRequired)
                lblTitle.Invoke((MethodInvoker)delegate { lblTitle.Text = Title; });
            else
                lblTitle.Text = Title;

            Msg = (string.IsNullOrEmpty(Msg) ? "Please wait..." : Msg);
            if (lblMessage.InvokeRequired)
                lblMessage.Invoke((MethodInvoker)delegate { lblMessage.Text = Msg; });
            else
                lblMessage.Text = Msg;

        }
    }
}

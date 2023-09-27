using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATD_ID4P
{
    public partial class frmNewOrderDialog : Form
    {
        public string Material_SO = "";
        public string SONo = "";

        public frmNewOrderDialog()
        {
            InitializeComponent();
        }

        private bool Dark_DisplayMode = Properties.Settings.Default.UI_DarkMode;

        private void FrmNewOrderDialog_Load(object sender, EventArgs e)
        {
            SetDisplayMode();
            lblNotice.Text = "";

        }

        private void SetDisplayMode()
        {
            if (Dark_DisplayMode == false)
            {
                this.BackColor = Color.LightGray;
                label1.ForeColor = Color.Black;
                lblMatNoTopic.ForeColor = Color.Green;
                label3.ForeColor = Color.Black;
                btnOK.BackColor = Color.SteelBlue;
                btnCancel.BackColor = Color.SteelBlue;
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Material_SO = txbMatNo.Text.Trim();
            if (string.IsNullOrEmpty(Material_SO))
            {
                lblNotice.Text = "Invalid Material No. or SO No.";
                return;
            }

            if (Properties.Settings.Default.Print_Label == true)
            {
                SONo = txbSO.Text.Trim();
                if (Material_SO.StartsWith("Z") && string.IsNullOrEmpty(SONo))
                {
                    lblNotice.Text = "Please insert SO No.";
                    return;
                }
            }

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txbMatNo_KeyDown(object sender, KeyEventArgs e)
        {
            //Check Language
            //var Language = System.Windows.Forms.InputLanguage.CurrentInputLanguage.Culture.Name;
            //if (!string.IsNullOrEmpty(Language))
            //{
            //    if (!Language.ToLower().StartsWith("en"))
            //    {
            //        MessageBox.Show("Please check language to Engligh.", "Language Invalid!!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        txbMatNo.Text = "";
            //        return;
            //    }
            //}

            if (e.KeyCode == Keys.Enter)
            {
                Material_SO = txbMatNo.Text.Trim();
                if (string.IsNullOrEmpty(Material_SO))
                {
                    lblNotice.Text = "Invalid Material No. or SO No.";
                    return;
                }

                if (Properties.Settings.Default.Print_Label == true)
                {
                    SONo = txbSO.Text.Trim();
                    if (Material_SO.StartsWith("Z") && string.IsNullOrEmpty(SONo))
                    {
                        lblNotice.Text = "Please insert SO No.";
                        return;
                    }
                }

                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void TxbMatNo_TextChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.Print_Label == true)
            {
                Material_SO = txbMatNo.Text.Trim();
                if (!string.IsNullOrEmpty(Material_SO) && Material_SO.StartsWith("Z"))
                {
                    lblSOTopic.Visible = true;
                    txbSO.Visible = true;
                }
                else
                {
                    lblSOTopic.Visible = false;
                    txbSO.Visible = false;
                }
            }
        }
    }
}

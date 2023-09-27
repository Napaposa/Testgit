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
    public partial class frmSettingLogin : Form
    {
        public string AdminCode = "";
        public frmSettingLogin()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            AdminCode = txbAdminCode.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void txbAdminCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AdminCode = txbAdminCode.Text;
                this.DialogResult = DialogResult.OK;
            }
        }

        private void frmSettingLogin_Load(object sender, EventArgs e)
        {
            SetDisplayMode();
        }
        private void SetDisplayMode()
        {
            if (Properties.Settings.Default.UI_DarkMode == false)
            {
                this.BackColor = Color.LightGray;
                lblPWTopic.ForeColor = Color.Black;
                btnSubmit.BackColor = Color.SteelBlue;
                btnClose.BackColor = Color.SteelBlue;
            }

        }
    }
}

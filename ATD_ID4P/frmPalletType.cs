using ATD_ID4P.Class;
using ATD_ID4P.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace ATD_ID4P
{
    public partial class frmPalletType : Form
    {
        public frmPalletType()
        {
            InitializeComponent();
            pallets = new palletsCls();

        }

        private Class.LogCls Log = new Class.LogCls();
        public Model.ClientDataModel ClientDataPallet = new Model.ClientDataModel();
        public palletsCls pallets { get; set; }
        int loc_palWidth;
        int loc_palLength;
        int loc_palHeight;
        string loc_palName;

        private void frmPalletType_Load(object sender, EventArgs e)
        {

            loc_palWidth= pallets.Width;
            loc_palLength = pallets.Length;
            loc_palHeight = pallets.Height;
            if (string.IsNullOrEmpty(pallets.Name))
            {
                loc_palName = "Wooden";
                string buf_Msg = "Pallet Type has never been assigned for this Order.\n" +
                                 "The system will set Pallet Type to Wooden by default.";
                string buf_Topic = "Pallet Type data not found.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                loc_palName = pallets.Name;

            numPalletWidth.Value = loc_palWidth;
            numPalletLength.Value = loc_palLength;
            numPalletHeight.Value = loc_palHeight;

            rdoWoodenPallet.Checked = (loc_palName == "Wooden");
            rdoPlasticPallet.Checked = (loc_palName == "Plastic");
            rdoNoPallet.Checked = (loc_palName == "NoPallet");

            if (rdoNoPallet.Checked)
            {
                numPalletHeight.Value = 0;
                numPalletHeight.Enabled = true;
            }
            else
            {
                numPalletHeight.Enabled = true;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rdoWoodenPallet.Checked == true)
            {
                pallets.Name = "Wooden";
            }
            else if (rdoPlasticPallet.Checked)
            {
                pallets.Name = "Plastic";
            }
            else if (rdoNoPallet.Checked)
            {
                pallets.Name = "NoPallet";
            }
            pallets.Width = (int)numPalletWidth.Value;
            pallets.Length = (int)numPalletLength.Value;
            pallets.Height = (int)numPalletHeight.Value;

            this.Close();
        }

        private void rdoNoPallet_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoNoPallet.Checked)
            {
                numPalletHeight.Value = 0;
                numPalletHeight.Enabled = false;
            }
            else
            {
                numPalletHeight.Enabled = true;
            }
        }

        private void rdoWoodenPallet_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoWoodenPallet.Checked)
            {
                numPalletWidth.Value = (numPalletWidth.Value == 0) ? 1000 : numPalletWidth.Value;
                numPalletLength.Value = (numPalletLength.Value == 0) ? 1200 : numPalletWidth.Value;
                numPalletHeight.Value = (numPalletHeight.Value == 0) ? 150 : numPalletHeight.Value;
            }
        }

        private void rdoPlasticPallet_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoPlasticPallet.Checked)
            {
                numPalletWidth.Value = (numPalletWidth.Value == 0) ? 1000 : numPalletWidth.Value;
                numPalletLength.Value = (numPalletLength.Value == 0) ? 1200 : numPalletWidth.Value;
                numPalletHeight.Value = (numPalletHeight.Value == 0) ? 180 : numPalletHeight.Value;
            }

        }
    }


}

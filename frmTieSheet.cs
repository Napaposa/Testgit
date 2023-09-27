using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Adapters;
using ATD_ID4P.Class;

namespace ATD_ID4P
{
    public partial class frmTiesheet : Form
    {
        public Model.ClientDataModel ClientDataTS = new Model.ClientDataModel();
        private decimal[] TSL_opt = new decimal[5];
        private decimal[] TSW_opt = new decimal[5];

        public frmTiesheet()
        {
            InitializeComponent();
        }

        private void frmTiesheet_Load(object sender, EventArgs e)
        {

            numTSL_opt1.Minimum = Convert.ToDecimal(Properties.Settings.Default.TS_Min_Length);
            numTSL_opt1.Maximum = Convert.ToDecimal(Properties.Settings.Default.TS_Max_Length);
            numTSW_opt1.Minimum = Convert.ToDecimal(Properties.Settings.Default.TS_Min_Width);
            numTSW_opt1.Maximum = Convert.ToDecimal(Properties.Settings.Default.TS_Max_Width);

            numTSL_opt1.Minimum = numTSL_opt2.Minimum;
            numTSL_opt1.Maximum = numTSL_opt2.Maximum;
            numTSW_opt1.Minimum = numTSW_opt2.Minimum;
            numTSW_opt1.Maximum = numTSW_opt2.Maximum;

            numTSL_opt1.Minimum = numTSL_opt3.Minimum;
            numTSL_opt1.Maximum = numTSL_opt3.Maximum;
            numTSW_opt1.Minimum = numTSW_opt3.Minimum;
            numTSW_opt1.Maximum = numTSW_opt3.Maximum;

            numTSL_opt1.Minimum = numTSL_opt4.Minimum;
            numTSL_opt1.Maximum = numTSL_opt4.Maximum;
            numTSW_opt1.Minimum = numTSW_opt4.Minimum;
            numTSW_opt1.Maximum = numTSW_opt4.Maximum;

            numTSL_opt1.Minimum = numTSL_opt5.Minimum;
            numTSL_opt1.Maximum = numTSL_opt5.Maximum;
            numTSW_opt1.Minimum = numTSW_opt5.Minimum;
            numTSW_opt1.Maximum = numTSW_opt5.Maximum;

            SetDisplayMode();
            if (!LoadData())
            {
                this.Close();
                this.Dispose();
            }
        }

        private bool LoadData()
        {
            ClientDataTS.LoadTSSizeOptions();
            for (int i = 0; i < 5; i++)
            {
                if (ClientDataTS.TSSizeOption[i].Length < Properties.Settings.Default.TS_Min_Length ||
                    ClientDataTS.TSSizeOption[i].Length > Properties.Settings.Default.TS_Max_Length ||
                    ClientDataTS.TSSizeOption[i].Width < Properties.Settings.Default.TS_Min_Width ||
                    ClientDataTS.TSSizeOption[i].Width > Properties.Settings.Default.TS_Max_Width )
                {
                    string buf_Msg = "Some value in the TSSizeOption.json is out of limit\n"+
                                        "Please contact administrator, If you don't know how to modify that file.\n\n"+
                                        "Minimum Value ( W x L ): "+ Properties.Settings.Default.TS_Min_Width+" x "+ Properties.Settings.Default.TS_Min_Length+"\n"+
                                        "Maximum Value ( W x L ): " + Properties.Settings.Default.TS_Max_Width + " x " + Properties.Settings.Default.TS_Max_Length;
                    string buf_Topic = "Error! - Data options' values out of limit.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            rdoTSOpt1.Checked = ClientDataTS.TSSizeOption[0].Selected;
            numTSW_opt1.Value = ClientDataTS.TSSizeOption[0].Width;
            numTSL_opt1.Value = ClientDataTS.TSSizeOption[0].Length;
            rdoTSOpt2.Checked = ClientDataTS.TSSizeOption[1].Selected;
            numTSW_opt2.Value = ClientDataTS.TSSizeOption[1].Width;
            numTSL_opt2.Value = ClientDataTS.TSSizeOption[1].Length;
            rdoTSOpt3.Checked = ClientDataTS.TSSizeOption[2].Selected;
            numTSW_opt3.Value = ClientDataTS.TSSizeOption[2].Width;
            numTSL_opt3.Value = ClientDataTS.TSSizeOption[2].Length;
            rdoTSOpt4.Checked = ClientDataTS.TSSizeOption[3].Selected;
            numTSW_opt4.Value = ClientDataTS.TSSizeOption[3].Width;
            numTSL_opt4.Value = ClientDataTS.TSSizeOption[3].Length;
            rdoTSOpt5.Checked = ClientDataTS.TSSizeOption[4].Selected;
            numTSW_opt5.Value = ClientDataTS.TSSizeOption[4].Width;
            numTSL_opt5.Value = ClientDataTS.TSSizeOption[4].Length;
            for (int i = 0; i < 5; i++)
            {
                TSW_opt[i] = ClientDataTS.TSSizeOption[i].Width;
                TSL_opt[i] = ClientDataTS.TSSizeOption[i].Length;

            }
            lblOKAlarm.Text = "";
            btnTSOpt_OK.Enabled = true;
            return true;
        }
        private void SetDisplayMode()
        {
            if (Properties.Settings.Default.UI_DarkMode == false)
            {
                this.BackColor = Color.LightGray;
                lblTSWidth.ForeColor = Color.Black;
                lblTSLength.ForeColor = Color.Black;
                rdoTSOpt1.ForeColor = Color.Black;
                rdoTSOpt2.ForeColor = Color.Black;
                rdoTSOpt3.ForeColor = Color.Black;
                rdoTSOpt4.ForeColor = Color.Black;
                rdoTSOpt5.ForeColor = Color.Black;
                lblOKAlarm.ForeColor = Color.Black;
            }

        }

        private void btnTSOpt_Edit_Click(object sender, EventArgs e)
        {
            numTSL_opt1.Enabled = true;
            numTSW_opt1.Enabled = true;
            numTSL_opt2.Enabled = true;
            numTSW_opt2.Enabled = true;
            numTSL_opt3.Enabled = true;
            numTSW_opt3.Enabled = true;
            numTSL_opt4.Enabled = true;
            numTSW_opt4.Enabled = true;
            numTSL_opt5.Enabled = true;
            numTSW_opt5.Enabled = true;
            btnTSOpt_OK.Enabled = false;
            lblOKAlarm.Text = "Disable until LOCKED.";
        }

        private void btnTSOpt_Lock_Click(object sender, EventArgs e)
        {
            numTSL_opt1.Enabled = false;
            numTSW_opt1.Enabled = false;
            numTSL_opt2.Enabled = false;
            numTSW_opt2.Enabled = false;
            numTSL_opt3.Enabled = false;
            numTSW_opt3.Enabled = false;
            numTSL_opt4.Enabled = false;
            numTSW_opt4.Enabled = false;
            numTSL_opt5.Enabled = false;
            numTSW_opt5.Enabled = false;
            btnTSOpt_OK.Enabled = true;
            lblOKAlarm.Text = "";
            string[] buf_Msg = new string[6];
            if (numTSW_opt1.Value != TSW_opt[0] || numTSL_opt1.Value != TSL_opt[0])
            {
                buf_Msg[0] = "Option 1 has been changed:\n" +
                    "[ " + TSW_opt[0] + " , " + TSL_opt[0] + " ] > [ " + numTSW_opt1.Value + " , " + numTSL_opt1.Value + " ]\n\n";
            }
            else
                buf_Msg[0] = "";
            if (numTSW_opt2.Value != TSW_opt[1] || numTSL_opt2.Value != TSL_opt[1])
            {
                buf_Msg[1] = "Option 2 has been changed:\n" +
                    "[ " + TSW_opt[1] + " , " + TSL_opt[1] + " ] > [ " + numTSW_opt2.Value + " , " + numTSL_opt2.Value + " ]\n\n";
            }
            else
                buf_Msg[1] = "";
            if (numTSW_opt3.Value != TSW_opt[2] || numTSL_opt3.Value != TSL_opt[2])
            {
                buf_Msg[2] = "Option 3 has been changed:\n" +
                    "[ " + TSW_opt[2] + " , " + TSL_opt[1] + " ] > [ " + numTSW_opt3.Value + " , " + numTSL_opt3.Value + " ]\n\n";
            }
            else
                buf_Msg[2] = "";
            if (numTSW_opt4.Value != TSW_opt[3] || numTSL_opt4.Value != TSL_opt[3])
            {
                buf_Msg[3] = "Option 4 has been changed:\n" +
                    "[ " + TSW_opt[3] + " , " + TSL_opt[3] + " ] > [ " + numTSW_opt4.Value + " , " + numTSL_opt4.Value + " ]\n\n";
            }
            else
                buf_Msg[3] = "";
            if (numTSW_opt5.Value != TSW_opt[4] || numTSL_opt5.Value != TSL_opt[4])
            {
                buf_Msg[4] = "Option 5 has been changed:\n" +
                    "[ " + TSW_opt[4] + " , " + TSL_opt[4] + " ] > [ " + numTSW_opt5.Value + " , " + numTSL_opt5.Value + " ]\n\n";
            }
            else
                buf_Msg[4] = "";

            if (string.IsNullOrEmpty(buf_Msg[0] + buf_Msg[1] + buf_Msg[2] + buf_Msg[3] + buf_Msg[4]))
            {
                buf_Msg[5] = "No Option Values have been modified";
            }
            else
            {
                buf_Msg[5] = "Option Values have been modified as below\n\n" +
                                buf_Msg[0] + buf_Msg[1] + buf_Msg[2] + buf_Msg[3] + buf_Msg[4] + "\n\n" +
                                "Modified value will be save to the system when you press OK.\n" +
                                "You can press RESET to return to previous value.";
            }
            TSW_opt[0] = numTSW_opt1.Value;
            TSL_opt[0] = numTSL_opt1.Value;
            TSW_opt[1] = numTSW_opt2.Value;
            TSL_opt[1] = numTSL_opt2.Value;
            TSW_opt[2] = numTSW_opt3.Value;
            TSL_opt[2] = numTSL_opt3.Value;
            TSW_opt[3] = numTSW_opt4.Value;
            TSL_opt[3] = numTSL_opt4.Value;
            TSW_opt[4] = numTSW_opt5.Value;
            TSL_opt[4] = numTSL_opt5.Value;
            string buf_Topic = "Option Value is Locked!";
            MessageBox.Show(buf_Msg[5], buf_Topic,MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void btnTSOpt_Reset_Click(object sender, EventArgs e)
        {
            if (!LoadData())
            {
                this.Close();
                return;
            }
            string buf_Msg = "All TieSheet Option have been reset to previous value.";
            string buf_Topic = "Option Values Reset!";
            MessageBox.Show(buf_Msg,buf_Topic,MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void UpdateClientBufferData()
        {
            ClientDataTS.TSSizeOption[0].Selected = rdoTSOpt1.Checked;
            ClientDataTS.TSSizeOption[1].Selected = rdoTSOpt2.Checked;
            ClientDataTS.TSSizeOption[2].Selected = rdoTSOpt3.Checked;
            ClientDataTS.TSSizeOption[3].Selected = rdoTSOpt4.Checked;
            ClientDataTS.TSSizeOption[4].Selected = rdoTSOpt5.Checked;
            for (int i = 0; i < 5; i++)
            {
                ClientDataTS.TSSizeOption[i].Width = Convert.ToInt32(TSW_opt[i]);
                ClientDataTS.TSSizeOption[i].Length = Convert.ToInt32(TSL_opt[i]);
                }
        }
        private void btnTSOpt_OK_Click(object sender, EventArgs e)
        {
            if (rdoTSOpt1.Checked)
            {
                ClientDataTS.Selected_TSWidth = Convert.ToInt32(TSW_opt[0]);
                ClientDataTS.Selected_TSLength = Convert.ToInt32(TSL_opt[0]);
                UpdateClientBufferData();
                ClientDataTS.SaveTieSheetOption();
                this.Close();
            }
            else if (rdoTSOpt2.Checked)
            {
                ClientDataTS.Selected_TSWidth = Convert.ToInt32(TSW_opt[1]);
                ClientDataTS.Selected_TSLength = Convert.ToInt32(TSL_opt[1]);
                UpdateClientBufferData();
                ClientDataTS.SaveTieSheetOption();
                this.Close();
            }
            else if (rdoTSOpt3.Checked)
            {
                ClientDataTS.Selected_TSWidth = Convert.ToInt32(TSW_opt[2]);
                ClientDataTS.Selected_TSLength = Convert.ToInt32(TSL_opt[2]);
                UpdateClientBufferData();
                ClientDataTS.SaveTieSheetOption();
                this.Close();
            }
            else if (rdoTSOpt4.Checked)
            {
                ClientDataTS.Selected_TSWidth = Convert.ToInt32(TSW_opt[3]);
                ClientDataTS.Selected_TSLength = Convert.ToInt32(TSL_opt[3]);
                UpdateClientBufferData();
                ClientDataTS.SaveTieSheetOption();
                this.Close();
            }
            else if (rdoTSOpt5.Checked)
            {
                ClientDataTS.Selected_TSWidth = Convert.ToInt32(TSW_opt[4]);
                ClientDataTS.Selected_TSLength = Convert.ToInt32(TSL_opt[4]);
                UpdateClientBufferData();
                ClientDataTS.SaveTieSheetOption();
                this.Close();
            }
            else
            {
                string buf_Msg = "No Option has been selected.\n" +
                                    "Please selected any option and try again.";
                string buf_Topic = "Error! - No option has been selected.";
                MessageBox.Show(buf_Msg,buf_Topic,MessageBoxButtons.OK,MessageBoxIcon.Error);
            }


        }
    }
}

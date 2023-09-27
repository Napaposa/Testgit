using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ATD_ID4P.UIComponent;

namespace ATD_ID4P.Class
{
    public static class UICls
    {
        private static Thread WaitThread;
        private static frmWaiting _fWait = new frmWaiting();

        //ChatGPT Suggestion
        private static bool _isClosing = false;
        private static CancellationTokenSource _cancellationTokenSource;
        private static Task _waitTask;
        //private static frmWaiting _fWait = new frmWaiting();

        public static void Btn_MouseHover(object sender, EventArgs e, Color ForeColor, Color? BackColor = null)
        {
            Button btn = (Button)sender;
            btn.ForeColor = ForeColor;
            if (BackColor.HasValue && BackColor != null)
                btn.BackColor = BackColor.Value;
            btn.Cursor = Cursors.Hand;
        }

        public static void Tsm_MouseHover(object sender, EventArgs e, Color ForeColor, Color? BackColor = null)
        {
            ToolStripMenuItem tsm = (ToolStripMenuItem)sender;
            tsm.ForeColor = ForeColor;
            if (BackColor.HasValue && BackColor != null)
                tsm.BackColor = BackColor.Value;
        }

        public static object FindFormInPanel(Panel pn, string FormName)
        {
            object result = null;

            foreach (Control ctrl in pn.Controls)
            {
                if (ctrl.Name == FormName)
                {
                    result = ctrl;
                    break;
                }
            }

            return result;
        }

        public static object FindOpenForm(string FormName)
        {
            object result = null;
            foreach (Form f in Application.OpenForms)
            {
                if (f.Name == FormName)
                {
                    result = f;
                    break;
                }
            }
            return result;
        }

        #region WaitingScreen
        public static void ShowWaiting(bool IsShow = true, string Title = "", string Message = "", string Position = "", int Duration = 0)
        {
            try
            {
                if (IsShow == true)
                {
                    _fWait.TopMost = true;
                    if (WaitThread != null)
                    {
                        if (WaitThread.IsAlive == true)
                        {
                            if (!string.IsNullOrEmpty(Title) || !string.IsNullOrEmpty(Message))
                            {
                                _fWait.SetMessage(Title, Message);
                            }

                            return;
                        }
                    }

                    WaitThread = new Thread(new ThreadStart(Splash));
                    WaitThread.IsBackground = true;
                    if (!string.IsNullOrEmpty(Title) || !string.IsNullOrEmpty(Message))
                    {
                        _fWait.SetMessage(Title, Message);
                    }
                    WaitThread.Start();
                }
                else
                {
                    CloseSplash();
                }
            }
            catch (Exception)
            {
                //_Log.WriteLog(e.Message, "Fail", this.ToString());
            }
        }

        private static void Splash()
        {
            try
            {
                _fWait = new frmWaiting();
                Application.Run(_fWait);

                // Cleanup code goes here...
            }
            catch (Exception ex)
            {
                string buf_Topic = "Error! - Splash Method.";
                string buf_Msg = "Abnormal Error occured.\n" +
                            "Please capture this screen and send to the developer.\n\n" +
                            "Error Detail:\n" + ex;
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Set the _isClosing flag to true to signal the thread to terminate itself.
                _isClosing = true;
            }
        }

        private static void CloseSplash()
        {
            if (_fWait.InvokeRequired)
            {
                _fWait.Invoke(new MethodInvoker(CloseSplash));
            }
            else
            {
                try
                {
                    if (WaitThread == null || !WaitThread.IsAlive)
                    {
                        // The WaitThread has already terminated or was never started.
                        return;
                    }

                    // Signal the frmWaiting form to close itself.
                    _fWait.Invoke(new MethodInvoker(_fWait.Dispose));

                    // Reset the mouse cursor to the default value.
                    //Cursor.Current = Cursors.Default;

                    // Abort the WaitThread.
                    //WaitThread.Abort();
                    _isClosing = true;
                    // Wait for the WaitThread to terminate.
                    //WaitThread.Join();
                }
                catch (System.Threading.ThreadAbortException)
                {
                    _fWait.Close();
                    Thread.ResetAbort();
                }
            }

        }

        static int GetControlThreadId(Control control)
        {
            int threadId = -1;
            control.Invoke(new Action(() =>
            {
                threadId = Thread.CurrentThread.ManagedThreadId;
            }));
            return threadId;
        }
        #endregion

        #region Grid
        public class DeleteColumn : DataGridViewButtonColumn
        {
            public DeleteColumn()
            {
                this.CellTemplate = new DeleteCell();
                this.Width = 10;
                this.Resizable = DataGridViewTriState.False;
                this.Name = "ColDelete";
                this.HeaderText = "";
                //set other options here 
            }
        }

        public class DeleteCell : DataGridViewButtonCell
        {
            private string ImageFolder = Application.StartupPath + @"\Images\";       

            protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
            {
                Image img = Image.FromFile(ImageFolder + "DeleteIcon.png");
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
                graphics.DrawImage(img, cellBounds);
            }
        }

        public class EditColumn : DataGridViewButtonColumn
        {
            public EditColumn()
            {
                this.CellTemplate = new EditCell();
                this.Width = 40;
                this.Resizable = DataGridViewTriState.False;
                //set other options here 
            }
        }

        public class EditCell : DataGridViewButtonCell
        {
            private string ImageFolder = Application.StartupPath + @"\Images\";

            protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
            {
                Image img = Image.FromFile(ImageFolder + "EditIcon.png");
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
                graphics.DrawImage(img, cellBounds);
            }
        }
        #endregion
    }
}

namespace ATD_ID4P.Report
{
    partial class frmOrderReport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOrderReport));
            this.dgvMain = new System.Windows.Forms.DataGridView();
            this.pnlMenu = new System.Windows.Forms.Panel();
            this.lblRowAmount = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblWorkTimeWarning = new System.Windows.Forms.Label();
            this.lblWorkTimeTopic = new System.Windows.Forms.Label();
            this.btnExportToExcel = new System.Windows.Forms.Button();
            this.btnGetData = new System.Windows.Forms.Button();
            this.lblOrderDateTopicDash = new System.Windows.Forms.Label();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.lblOrderStateTopic = new System.Windows.Forms.Label();
            this.lblOrderDateTopic = new System.Windows.Forms.Label();
            this.grpbxWorkTime = new System.Windows.Forms.GroupBox();
            this.rdoNightTime = new System.Windows.Forms.RadioButton();
            this.rdoDayTime = new System.Windows.Forms.RadioButton();
            this.rdoAllTime = new System.Windows.Forms.RadioButton();
            this.grpbxOrderState = new System.Windows.Forms.GroupBox();
            this.rdoUncompleteState = new System.Windows.Forms.RadioButton();
            this.rdoCompleteState = new System.Windows.Forms.RadioButton();
            this.rdoAllState = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).BeginInit();
            this.pnlMenu.SuspendLayout();
            this.grpbxWorkTime.SuspendLayout();
            this.grpbxOrderState.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvMain
            // 
            this.dgvMain.AllowUserToAddRows = false;
            this.dgvMain.AllowUserToDeleteRows = false;
            this.dgvMain.AllowUserToOrderColumns = true;
            this.dgvMain.AllowUserToResizeColumns = false;
            this.dgvMain.AllowUserToResizeRows = false;
            this.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMain.Location = new System.Drawing.Point(0, 176);
            this.dgvMain.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.dgvMain.Name = "dgvMain";
            this.dgvMain.ReadOnly = true;
            this.dgvMain.RowHeadersWidth = 51;
            this.dgvMain.Size = new System.Drawing.Size(1223, 501);
            this.dgvMain.TabIndex = 3;
            // 
            // pnlMenu
            // 
            this.pnlMenu.Controls.Add(this.lblRowAmount);
            this.pnlMenu.Controls.Add(this.btnClose);
            this.pnlMenu.Controls.Add(this.lblWorkTimeWarning);
            this.pnlMenu.Controls.Add(this.lblWorkTimeTopic);
            this.pnlMenu.Controls.Add(this.btnExportToExcel);
            this.pnlMenu.Controls.Add(this.btnGetData);
            this.pnlMenu.Controls.Add(this.lblOrderDateTopicDash);
            this.pnlMenu.Controls.Add(this.dtpEnd);
            this.pnlMenu.Controls.Add(this.dtpStart);
            this.pnlMenu.Controls.Add(this.lblOrderStateTopic);
            this.pnlMenu.Controls.Add(this.lblOrderDateTopic);
            this.pnlMenu.Controls.Add(this.grpbxWorkTime);
            this.pnlMenu.Controls.Add(this.grpbxOrderState);
            this.pnlMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMenu.Location = new System.Drawing.Point(0, 0);
            this.pnlMenu.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.pnlMenu.Name = "pnlMenu";
            this.pnlMenu.Size = new System.Drawing.Size(1223, 176);
            this.pnlMenu.TabIndex = 2;
            // 
            // lblRowAmount
            // 
            this.lblRowAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRowAmount.AutoSize = true;
            this.lblRowAmount.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblRowAmount.ForeColor = System.Drawing.SystemColors.Control;
            this.lblRowAmount.Location = new System.Drawing.Point(1123, 154);
            this.lblRowAmount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRowAmount.Name = "lblRowAmount";
            this.lblRowAmount.Size = new System.Drawing.Size(40, 16);
            this.lblRowAmount.TabIndex = 74;
            this.lblRowAmount.Text = "Row :";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnClose.ForeColor = System.Drawing.Color.LightGray;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnClose.Location = new System.Drawing.Point(1073, 9);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(135, 75);
            this.btnClose.TabIndex = 73;
            this.btnClose.Tag = "";
            this.btnClose.Text = "Close";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblWorkTimeWarning
            // 
            this.lblWorkTimeWarning.AutoSize = true;
            this.lblWorkTimeWarning.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWorkTimeWarning.ForeColor = System.Drawing.Color.OrangeRed;
            this.lblWorkTimeWarning.Location = new System.Drawing.Point(612, 119);
            this.lblWorkTimeWarning.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblWorkTimeWarning.Name = "lblWorkTimeWarning";
            this.lblWorkTimeWarning.Size = new System.Drawing.Size(564, 24);
            this.lblWorkTimeWarning.TabIndex = 13;
            this.lblWorkTimeWarning.Text = "WorkTime in Day and Night working only on Same OrderDate ";
            this.lblWorkTimeWarning.Visible = false;
            // 
            // lblWorkTimeTopic
            // 
            this.lblWorkTimeTopic.AutoSize = true;
            this.lblWorkTimeTopic.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lblWorkTimeTopic.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblWorkTimeTopic.Location = new System.Drawing.Point(24, 65);
            this.lblWorkTimeTopic.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblWorkTimeTopic.Name = "lblWorkTimeTopic";
            this.lblWorkTimeTopic.Size = new System.Drawing.Size(99, 20);
            this.lblWorkTimeTopic.TabIndex = 11;
            this.lblWorkTimeTopic.Text = "WorkTime:";
            // 
            // btnExportToExcel
            // 
            this.btnExportToExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportToExcel.BackColor = System.Drawing.Color.SeaGreen;
            this.btnExportToExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportToExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnExportToExcel.ForeColor = System.Drawing.SystemColors.Control;
            this.btnExportToExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnExportToExcel.Image")));
            this.btnExportToExcel.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnExportToExcel.Location = new System.Drawing.Point(927, 9);
            this.btnExportToExcel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnExportToExcel.Name = "btnExportToExcel";
            this.btnExportToExcel.Size = new System.Drawing.Size(140, 75);
            this.btnExportToExcel.TabIndex = 10;
            this.btnExportToExcel.Text = "Export Excel";
            this.btnExportToExcel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnExportToExcel.UseVisualStyleBackColor = false;
            this.btnExportToExcel.Click += new System.EventHandler(this.btnExportToExcel_Click);
            // 
            // btnGetData
            // 
            this.btnGetData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetData.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnGetData.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnGetData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGetData.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnGetData.ForeColor = System.Drawing.SystemColors.Control;
            this.btnGetData.Image = ((System.Drawing.Image)(resources.GetObject("btnGetData.Image")));
            this.btnGetData.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnGetData.Location = new System.Drawing.Point(776, 9);
            this.btnGetData.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnGetData.Name = "btnGetData";
            this.btnGetData.Size = new System.Drawing.Size(143, 75);
            this.btnGetData.TabIndex = 8;
            this.btnGetData.Text = "Get Data";
            this.btnGetData.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnGetData.UseVisualStyleBackColor = false;
            this.btnGetData.Click += new System.EventHandler(this.btnGetData_Click);
            // 
            // lblOrderDateTopicDash
            // 
            this.lblOrderDateTopicDash.AutoSize = true;
            this.lblOrderDateTopicDash.Location = new System.Drawing.Point(372, 18);
            this.lblOrderDateTopicDash.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblOrderDateTopicDash.Name = "lblOrderDateTopicDash";
            this.lblOrderDateTopicDash.Size = new System.Drawing.Size(11, 16);
            this.lblOrderDateTopicDash.TabIndex = 4;
            this.lblOrderDateTopicDash.Text = "-";
            // 
            // dtpEnd
            // 
            this.dtpEnd.CustomFormat = "dd/MM/yyyy";
            this.dtpEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEnd.Location = new System.Drawing.Point(405, 11);
            this.dtpEnd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.Size = new System.Drawing.Size(199, 30);
            this.dtpEnd.TabIndex = 3;
            // 
            // dtpStart
            // 
            this.dtpStart.CustomFormat = "dd/MM/yyyy";
            this.dtpStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStart.Location = new System.Drawing.Point(161, 11);
            this.dtpStart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpStart.Name = "dtpStart";
            this.dtpStart.Size = new System.Drawing.Size(199, 30);
            this.dtpStart.TabIndex = 2;
            // 
            // lblOrderStateTopic
            // 
            this.lblOrderStateTopic.AutoSize = true;
            this.lblOrderStateTopic.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lblOrderStateTopic.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblOrderStateTopic.Location = new System.Drawing.Point(12, 123);
            this.lblOrderStateTopic.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblOrderStateTopic.Name = "lblOrderStateTopic";
            this.lblOrderStateTopic.Size = new System.Drawing.Size(107, 20);
            this.lblOrderStateTopic.TabIndex = 1;
            this.lblOrderStateTopic.Text = "OrderState:";
            // 
            // lblOrderDateTopic
            // 
            this.lblOrderDateTopic.AutoSize = true;
            this.lblOrderDateTopic.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lblOrderDateTopic.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblOrderDateTopic.Location = new System.Drawing.Point(17, 18);
            this.lblOrderDateTopic.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblOrderDateTopic.Name = "lblOrderDateTopic";
            this.lblOrderDateTopic.Size = new System.Drawing.Size(103, 20);
            this.lblOrderDateTopic.TabIndex = 0;
            this.lblOrderDateTopic.Text = "OrderDate:";
            // 
            // grpbxWorkTime
            // 
            this.grpbxWorkTime.Controls.Add(this.rdoNightTime);
            this.grpbxWorkTime.Controls.Add(this.rdoDayTime);
            this.grpbxWorkTime.Controls.Add(this.rdoAllTime);
            this.grpbxWorkTime.Location = new System.Drawing.Point(161, 41);
            this.grpbxWorkTime.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpbxWorkTime.Name = "grpbxWorkTime";
            this.grpbxWorkTime.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpbxWorkTime.Size = new System.Drawing.Size(444, 64);
            this.grpbxWorkTime.TabIndex = 12;
            this.grpbxWorkTime.TabStop = false;
            // 
            // rdoNightTime
            // 
            this.rdoNightTime.AutoSize = true;
            this.rdoNightTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.rdoNightTime.ForeColor = System.Drawing.SystemColors.Control;
            this.rdoNightTime.Location = new System.Drawing.Point(244, 22);
            this.rdoNightTime.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoNightTime.Name = "rdoNightTime";
            this.rdoNightTime.Size = new System.Drawing.Size(74, 24);
            this.rdoNightTime.TabIndex = 2;
            this.rdoNightTime.TabStop = true;
            this.rdoNightTime.Text = "Night";
            this.rdoNightTime.UseVisualStyleBackColor = true;
            // 
            // rdoDayTime
            // 
            this.rdoDayTime.AutoSize = true;
            this.rdoDayTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.rdoDayTime.ForeColor = System.Drawing.SystemColors.Control;
            this.rdoDayTime.Location = new System.Drawing.Point(97, 22);
            this.rdoDayTime.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoDayTime.Name = "rdoDayTime";
            this.rdoDayTime.Size = new System.Drawing.Size(63, 24);
            this.rdoDayTime.TabIndex = 1;
            this.rdoDayTime.TabStop = true;
            this.rdoDayTime.Text = "Day";
            this.rdoDayTime.UseVisualStyleBackColor = true;
            // 
            // rdoAllTime
            // 
            this.rdoAllTime.AutoSize = true;
            this.rdoAllTime.Checked = true;
            this.rdoAllTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.rdoAllTime.ForeColor = System.Drawing.SystemColors.Control;
            this.rdoAllTime.Location = new System.Drawing.Point(8, 22);
            this.rdoAllTime.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoAllTime.Name = "rdoAllTime";
            this.rdoAllTime.Size = new System.Drawing.Size(52, 24);
            this.rdoAllTime.TabIndex = 0;
            this.rdoAllTime.TabStop = true;
            this.rdoAllTime.Text = "All";
            this.rdoAllTime.UseVisualStyleBackColor = true;
            // 
            // grpbxOrderState
            // 
            this.grpbxOrderState.Controls.Add(this.rdoUncompleteState);
            this.grpbxOrderState.Controls.Add(this.rdoCompleteState);
            this.grpbxOrderState.Controls.Add(this.rdoAllState);
            this.grpbxOrderState.Location = new System.Drawing.Point(161, 98);
            this.grpbxOrderState.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpbxOrderState.Name = "grpbxOrderState";
            this.grpbxOrderState.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpbxOrderState.Size = new System.Drawing.Size(444, 64);
            this.grpbxOrderState.TabIndex = 9;
            this.grpbxOrderState.TabStop = false;
            // 
            // rdoUncompleteState
            // 
            this.rdoUncompleteState.AutoSize = true;
            this.rdoUncompleteState.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.rdoUncompleteState.ForeColor = System.Drawing.SystemColors.Control;
            this.rdoUncompleteState.Location = new System.Drawing.Point(244, 23);
            this.rdoUncompleteState.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoUncompleteState.Name = "rdoUncompleteState";
            this.rdoUncompleteState.Size = new System.Drawing.Size(129, 24);
            this.rdoUncompleteState.TabIndex = 2;
            this.rdoUncompleteState.TabStop = true;
            this.rdoUncompleteState.Text = "Uncomplete";
            this.rdoUncompleteState.UseVisualStyleBackColor = true;
            // 
            // rdoCompleteState
            // 
            this.rdoCompleteState.AutoSize = true;
            this.rdoCompleteState.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.rdoCompleteState.ForeColor = System.Drawing.SystemColors.Control;
            this.rdoCompleteState.Location = new System.Drawing.Point(97, 22);
            this.rdoCompleteState.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoCompleteState.Name = "rdoCompleteState";
            this.rdoCompleteState.Size = new System.Drawing.Size(109, 24);
            this.rdoCompleteState.TabIndex = 1;
            this.rdoCompleteState.TabStop = true;
            this.rdoCompleteState.Text = "Complete";
            this.rdoCompleteState.UseVisualStyleBackColor = true;
            // 
            // rdoAllState
            // 
            this.rdoAllState.AutoSize = true;
            this.rdoAllState.Checked = true;
            this.rdoAllState.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.rdoAllState.ForeColor = System.Drawing.SystemColors.Control;
            this.rdoAllState.Location = new System.Drawing.Point(8, 22);
            this.rdoAllState.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoAllState.Name = "rdoAllState";
            this.rdoAllState.Size = new System.Drawing.Size(52, 24);
            this.rdoAllState.TabIndex = 0;
            this.rdoAllState.TabStop = true;
            this.rdoAllState.Text = "All";
            this.rdoAllState.UseVisualStyleBackColor = true;
            // 
            // frmOrderReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(1223, 677);
            this.Controls.Add(this.dgvMain);
            this.Controls.Add(this.pnlMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmOrderReport";
            this.Text = "frmOrderRrport";
            this.Load += new System.EventHandler(this.frmOrderRrport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).EndInit();
            this.pnlMenu.ResumeLayout(false);
            this.pnlMenu.PerformLayout();
            this.grpbxWorkTime.ResumeLayout(false);
            this.grpbxWorkTime.PerformLayout();
            this.grpbxOrderState.ResumeLayout(false);
            this.grpbxOrderState.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvMain;
        private System.Windows.Forms.Panel pnlMenu;
        private System.Windows.Forms.Label lblWorkTimeWarning;
        private System.Windows.Forms.Label lblWorkTimeTopic;
        private System.Windows.Forms.Button btnExportToExcel;
        private System.Windows.Forms.Button btnGetData;
        private System.Windows.Forms.Label lblOrderDateTopicDash;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.Label lblOrderStateTopic;
        private System.Windows.Forms.Label lblOrderDateTopic;
        private System.Windows.Forms.GroupBox grpbxWorkTime;
        private System.Windows.Forms.RadioButton rdoNightTime;
        private System.Windows.Forms.RadioButton rdoDayTime;
        private System.Windows.Forms.RadioButton rdoAllTime;
        private System.Windows.Forms.GroupBox grpbxOrderState;
        private System.Windows.Forms.RadioButton rdoUncompleteState;
        private System.Windows.Forms.RadioButton rdoCompleteState;
        private System.Windows.Forms.RadioButton rdoAllState;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblRowAmount;
    }
}
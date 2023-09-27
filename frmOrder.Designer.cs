namespace ATD_ID4P
{
    partial class frmOrder
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOrder));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnNewOrder = new System.Windows.Forms.Button();
            this.btnSendData = new System.Windows.Forms.Button();
            this.dgvOrderList = new System.Windows.Forms.DataGridView();
            this.pnlRobSpeed = new System.Windows.Forms.Panel();
            this.lblRobotSpeed_Title = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.pnlPLCStatus = new System.Windows.Forms.Panel();
            this.lblPLCIP = new System.Windows.Forms.Label();
            this.lblPLCActive = new System.Windows.Forms.Label();
            this.picPLCCom = new System.Windows.Forms.PictureBox();
            this.pnlRobotStatus = new System.Windows.Forms.Panel();
            this.lblRobotIP = new System.Windows.Forms.Label();
            this.lblRobotActive = new System.Windows.Forms.Label();
            this.picRobotCom = new System.Windows.Forms.PictureBox();
            this.lblSim3DMode = new System.Windows.Forms.Label();
            this.pnlSimRun = new System.Windows.Forms.Panel();
            this.lblUDHAlarm = new System.Windows.Forms.Label();
            this.lblDispMatNo = new System.Windows.Forms.Label();
            this.pgbPlacedBundle = new System.Windows.Forms.ProgressBar();
            this.lblPalletPercent = new System.Windows.Forms.Label();
            this.lblDispBDCount = new System.Windows.Forms.Label();
            this.lblDispLYCount = new System.Windows.Forms.Label();
            this.lblDispBDPerGrip = new System.Windows.Forms.Label();
            this.lblDispTSLayer = new System.Windows.Forms.Label();
            this.pnlSimulate = new System.Windows.Forms.Panel();
            this.pic3DOrder = new System.Windows.Forms.PictureBox();
            this.imlMain = new System.Windows.Forms.ImageList(this.components);
            this.lblTieSheetSize_Title = new System.Windows.Forms.Label();
            this.lblTieSheetSize = new System.Windows.Forms.Label();
            this.pnlTieSheetSize = new System.Windows.Forms.Panel();
            this.btnEditTS = new System.Windows.Forms.Button();
            this.lblDBClicktoEdit = new System.Windows.Forms.Label();
            this.txbDispMatNo = new System.Windows.Forms.TextBox();
            this.txbDispBDPerGrip = new System.Windows.Forms.TextBox();
            this.txbTSLY1_10 = new System.Windows.Forms.TextBox();
            this.txbDispBDCount = new System.Windows.Forms.TextBox();
            this.txbDispLYCount = new System.Windows.Forms.TextBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.rdoPressSquaring = new System.Windows.Forms.RadioButton();
            this.rdoOpenSquaring = new System.Windows.Forms.RadioButton();
            this.tmrLotEnd = new System.Windows.Forms.Timer(this.components);
            this.tmrConfirmStart = new System.Windows.Forms.Timer(this.components);
            this.tmrRobotFeed = new System.Windows.Forms.Timer(this.components);
            this.pnlGripperFinger = new System.Windows.Forms.Panel();
            this.lblFingerRequire = new System.Windows.Forms.Label();
            this.lblFinger_Title = new System.Windows.Forms.Label();
            this.picGripperFinger = new System.Windows.Forms.PictureBox();
            this.pnlPallet = new System.Windows.Forms.Panel();
            this.lblSizePallet = new System.Windows.Forms.Label();
            this.lblPallet_Title = new System.Windows.Forms.Label();
            this.lblPalletType = new System.Windows.Forms.Label();
            this.btnResetData = new System.Windows.Forms.Button();
            this.btnChangeToCurOrder = new System.Windows.Forms.Button();
            this.lblDispPatSeq = new System.Windows.Forms.Label();
            this.txbTSLY11_20 = new System.Windows.Forms.TextBox();
            this.txbTSLY21_30 = new System.Windows.Forms.TextBox();
            this.txbDispSO = new System.Windows.Forms.TextBox();
            this.lblDispSO = new System.Windows.Forms.Label();
            this.lblDispSOBD = new System.Windows.Forms.Label();
            this.txbDispSOBD = new System.Windows.Forms.TextBox();
            this.chkbxDispPat1 = new System.Windows.Forms.CheckBox();
            this.chkbxDispPat2 = new System.Windows.Forms.CheckBox();
            this.chkbxDispPat4 = new System.Windows.Forms.CheckBox();
            this.chkbxDispPat3 = new System.Windows.Forms.CheckBox();
            this.lblDispTSSize = new System.Windows.Forms.Label();
            this.txbDispTSW = new System.Windows.Forms.TextBox();
            this.txbDispTSL = new System.Windows.Forms.TextBox();
            this.lblDispTSW = new System.Windows.Forms.Label();
            this.lblDispTSL = new System.Windows.Forms.Label();
            this.chbRealTime = new System.Windows.Forms.CheckBox();
            this.tltipRobotConnection = new System.Windows.Forms.ToolTip(this.components);
            this.tltipPLCConnection = new System.Windows.Forms.ToolTip(this.components);
            this.lblDoorStatusDisp = new System.Windows.Forms.Label();
            this.lblDoorStatus = new System.Windows.Forms.Label();
            this.lblDoorCmd = new System.Windows.Forms.Label();
            this.btnDoorUnlock = new System.Windows.Forms.Button();
            this.btnDoorLock = new System.Windows.Forms.Button();
            this.lblSystemStatusDisp = new System.Windows.Forms.Label();
            this.lblSystemCmd = new System.Windows.Forms.Label();
            this.lblSystemStatus = new System.Windows.Forms.Label();
            this.btnHalfFinish = new System.Windows.Forms.Button();
            this.btnConfirmStart = new System.Windows.Forms.Button();
            this.btnLotEnd = new System.Windows.Forms.Button();
            this.grpbxControlPanel = new System.Windows.Forms.GroupBox();
            this.grpbxFeedData = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderList)).BeginInit();
            this.pnlRobSpeed.SuspendLayout();
            this.pnlPLCStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPLCCom)).BeginInit();
            this.pnlRobotStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRobotCom)).BeginInit();
            this.pnlSimRun.SuspendLayout();
            this.pnlSimulate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic3DOrder)).BeginInit();
            this.pnlTieSheetSize.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.pnlGripperFinger.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGripperFinger)).BeginInit();
            this.pnlPallet.SuspendLayout();
            this.grpbxControlPanel.SuspendLayout();
            this.grpbxFeedData.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.Black;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.ForeColor = System.Drawing.Color.LightGray;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.Location = new System.Drawing.Point(141, 9);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(131, 59);
            this.btnRefresh.TabIndex = 22;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnNewOrder
            // 
            this.btnNewOrder.BackColor = System.Drawing.Color.Black;
            this.btnNewOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewOrder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewOrder.ForeColor = System.Drawing.Color.LightGray;
            this.btnNewOrder.Image = ((System.Drawing.Image)(resources.GetObject("btnNewOrder.Image")));
            this.btnNewOrder.Location = new System.Drawing.Point(9, 9);
            this.btnNewOrder.Margin = new System.Windows.Forms.Padding(4);
            this.btnNewOrder.Name = "btnNewOrder";
            this.btnNewOrder.Size = new System.Drawing.Size(131, 59);
            this.btnNewOrder.TabIndex = 20;
            this.btnNewOrder.Text = "New Order";
            this.btnNewOrder.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnNewOrder.UseVisualStyleBackColor = false;
            this.btnNewOrder.Click += new System.EventHandler(this.btnNewOrder_Click);
            // 
            // btnSendData
            // 
            this.btnSendData.BackColor = System.Drawing.Color.Black;
            this.btnSendData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendData.ForeColor = System.Drawing.Color.LightGray;
            this.btnSendData.Image = ((System.Drawing.Image)(resources.GetObject("btnSendData.Image")));
            this.btnSendData.Location = new System.Drawing.Point(273, 9);
            this.btnSendData.Margin = new System.Windows.Forms.Padding(4);
            this.btnSendData.Name = "btnSendData";
            this.btnSendData.Size = new System.Drawing.Size(131, 59);
            this.btnSendData.TabIndex = 19;
            this.btnSendData.Text = "Send Data";
            this.btnSendData.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSendData.UseVisualStyleBackColor = false;
            this.btnSendData.Click += new System.EventHandler(this.btnSendData_Click);
            // 
            // dgvOrderList
            // 
            this.dgvOrderList.AllowUserToAddRows = false;
            this.dgvOrderList.AllowUserToDeleteRows = false;
            this.dgvOrderList.AllowUserToResizeColumns = false;
            this.dgvOrderList.AllowUserToResizeRows = false;
            this.dgvOrderList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Desktop;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvOrderList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvOrderList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOrderList.Location = new System.Drawing.Point(9, 75);
            this.dgvOrderList.Margin = new System.Windows.Forms.Padding(4);
            this.dgvOrderList.MultiSelect = false;
            this.dgvOrderList.Name = "dgvOrderList";
            this.dgvOrderList.ReadOnly = true;
            this.dgvOrderList.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Desktop;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvOrderList.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvOrderList.RowHeadersWidth = 30;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Desktop;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.LightGray;
            this.dgvOrderList.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvOrderList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOrderList.Size = new System.Drawing.Size(777, 377);
            this.dgvOrderList.TabIndex = 18;
            this.dgvOrderList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvOrderList_CellContentClick);
            this.dgvOrderList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvOrderList_CellDoubleClick);
            this.dgvOrderList.SelectionChanged += new System.EventHandler(this.dgvOrderList_SelectionChanged);
            // 
            // pnlRobSpeed
            // 
            this.pnlRobSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlRobSpeed.BackColor = System.Drawing.SystemColors.Desktop;
            this.pnlRobSpeed.Controls.Add(this.lblRobotSpeed_Title);
            this.pnlRobSpeed.Controls.Add(this.lblSpeed);
            this.pnlRobSpeed.Location = new System.Drawing.Point(750, 7);
            this.pnlRobSpeed.Margin = new System.Windows.Forms.Padding(4);
            this.pnlRobSpeed.Name = "pnlRobSpeed";
            this.pnlRobSpeed.Size = new System.Drawing.Size(137, 60);
            this.pnlRobSpeed.TabIndex = 15;
            // 
            // lblRobotSpeed_Title
            // 
            this.lblRobotSpeed_Title.AutoSize = true;
            this.lblRobotSpeed_Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRobotSpeed_Title.ForeColor = System.Drawing.Color.LightGray;
            this.lblRobotSpeed_Title.Location = new System.Drawing.Point(-1, 0);
            this.lblRobotSpeed_Title.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRobotSpeed_Title.Name = "lblRobotSpeed_Title";
            this.lblRobotSpeed_Title.Size = new System.Drawing.Size(50, 18);
            this.lblRobotSpeed_Title.TabIndex = 3;
            this.lblRobotSpeed_Title.Text = "Speed";
            // 
            // lblSpeed
            // 
            this.lblSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpeed.ForeColor = System.Drawing.Color.Red;
            this.lblSpeed.Location = new System.Drawing.Point(32, 17);
            this.lblSpeed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(77, 39);
            this.lblSpeed.TabIndex = 2;
            this.lblSpeed.Text = "220";
            // 
            // pnlPLCStatus
            // 
            this.pnlPLCStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPLCStatus.BackColor = System.Drawing.SystemColors.Desktop;
            this.pnlPLCStatus.Controls.Add(this.lblPLCIP);
            this.pnlPLCStatus.Controls.Add(this.lblPLCActive);
            this.pnlPLCStatus.Controls.Add(this.picPLCCom);
            this.pnlPLCStatus.Location = new System.Drawing.Point(1044, 7);
            this.pnlPLCStatus.Margin = new System.Windows.Forms.Padding(4);
            this.pnlPLCStatus.Name = "pnlPLCStatus";
            this.pnlPLCStatus.Size = new System.Drawing.Size(147, 60);
            this.pnlPLCStatus.TabIndex = 14;
            // 
            // lblPLCIP
            // 
            this.lblPLCIP.AutoSize = true;
            this.lblPLCIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPLCIP.ForeColor = System.Drawing.Color.LightGray;
            this.lblPLCIP.Location = new System.Drawing.Point(5, 39);
            this.lblPLCIP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPLCIP.Name = "lblPLCIP";
            this.lblPLCIP.Size = new System.Drawing.Size(42, 15);
            this.lblPLCIP.TabIndex = 3;
            this.lblPLCIP.Text = "Offline";
            // 
            // lblPLCActive
            // 
            this.lblPLCActive.AutoSize = true;
            this.lblPLCActive.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPLCActive.ForeColor = System.Drawing.Color.Red;
            this.lblPLCActive.Location = new System.Drawing.Point(4, 6);
            this.lblPLCActive.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPLCActive.Name = "lblPLCActive";
            this.lblPLCActive.Size = new System.Drawing.Size(54, 25);
            this.lblPLCActive.TabIndex = 2;
            this.lblPLCActive.Text = "PLC";
            // 
            // picPLCCom
            // 
            this.picPLCCom.Image = ((System.Drawing.Image)(resources.GetObject("picPLCCom.Image")));
            this.picPLCCom.Location = new System.Drawing.Point(92, 14);
            this.picPLCCom.Margin = new System.Windows.Forms.Padding(4);
            this.picPLCCom.Name = "picPLCCom";
            this.picPLCCom.Size = new System.Drawing.Size(24, 24);
            this.picPLCCom.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picPLCCom.TabIndex = 7;
            this.picPLCCom.TabStop = false;
            // 
            // pnlRobotStatus
            // 
            this.pnlRobotStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlRobotStatus.BackColor = System.Drawing.SystemColors.Desktop;
            this.pnlRobotStatus.Controls.Add(this.lblRobotIP);
            this.pnlRobotStatus.Controls.Add(this.lblRobotActive);
            this.pnlRobotStatus.Controls.Add(this.picRobotCom);
            this.pnlRobotStatus.Location = new System.Drawing.Point(892, 7);
            this.pnlRobotStatus.Margin = new System.Windows.Forms.Padding(4);
            this.pnlRobotStatus.Name = "pnlRobotStatus";
            this.pnlRobotStatus.Size = new System.Drawing.Size(147, 60);
            this.pnlRobotStatus.TabIndex = 13;
            // 
            // lblRobotIP
            // 
            this.lblRobotIP.AutoSize = true;
            this.lblRobotIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRobotIP.ForeColor = System.Drawing.Color.LightGray;
            this.lblRobotIP.Location = new System.Drawing.Point(8, 39);
            this.lblRobotIP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRobotIP.Name = "lblRobotIP";
            this.lblRobotIP.Size = new System.Drawing.Size(86, 15);
            this.lblRobotIP.TabIndex = 3;
            this.lblRobotIP.Text = "172.31.204.82";
            // 
            // lblRobotActive
            // 
            this.lblRobotActive.AutoSize = true;
            this.lblRobotActive.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRobotActive.ForeColor = System.Drawing.Color.SpringGreen;
            this.lblRobotActive.Location = new System.Drawing.Point(7, 6);
            this.lblRobotActive.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRobotActive.Name = "lblRobotActive";
            this.lblRobotActive.Size = new System.Drawing.Size(68, 25);
            this.lblRobotActive.TabIndex = 2;
            this.lblRobotActive.Text = "Robot";
            // 
            // picRobotCom
            // 
            this.picRobotCom.Image = ((System.Drawing.Image)(resources.GetObject("picRobotCom.Image")));
            this.picRobotCom.Location = new System.Drawing.Point(100, 14);
            this.picRobotCom.Margin = new System.Windows.Forms.Padding(4);
            this.picRobotCom.Name = "picRobotCom";
            this.picRobotCom.Size = new System.Drawing.Size(24, 24);
            this.picRobotCom.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picRobotCom.TabIndex = 6;
            this.picRobotCom.TabStop = false;
            // 
            // lblSim3DMode
            // 
            this.lblSim3DMode.AutoSize = true;
            this.lblSim3DMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSim3DMode.ForeColor = System.Drawing.Color.Black;
            this.lblSim3DMode.Location = new System.Drawing.Point(4, 10);
            this.lblSim3DMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSim3DMode.Name = "lblSim3DMode";
            this.lblSim3DMode.Size = new System.Drawing.Size(126, 25);
            this.lblSim3DMode.TabIndex = 3;
            this.lblSim3DMode.Text = "Current Job";
            // 
            // pnlSimRun
            // 
            this.pnlSimRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSimRun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pnlSimRun.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSimRun.Controls.Add(this.lblUDHAlarm);
            this.pnlSimRun.Location = new System.Drawing.Point(-94, 460);
            this.pnlSimRun.Margin = new System.Windows.Forms.Padding(4);
            this.pnlSimRun.Name = "pnlSimRun";
            this.pnlSimRun.Size = new System.Drawing.Size(373, 344);
            this.pnlSimRun.TabIndex = 11;
            // 
            // lblUDHAlarm
            // 
            this.lblUDHAlarm.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblUDHAlarm.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUDHAlarm.ForeColor = System.Drawing.Color.Yellow;
            this.lblUDHAlarm.Location = new System.Drawing.Point(3, 1);
            this.lblUDHAlarm.Name = "lblUDHAlarm";
            this.lblUDHAlarm.Size = new System.Drawing.Size(453, 59);
            this.lblUDHAlarm.TabIndex = 101;
            this.lblUDHAlarm.Text = "UnderHang Alarm";
            // 
            // lblDispMatNo
            // 
            this.lblDispMatNo.AutoSize = true;
            this.lblDispMatNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispMatNo.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDispMatNo.Location = new System.Drawing.Point(17, 32);
            this.lblDispMatNo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDispMatNo.Name = "lblDispMatNo";
            this.lblDispMatNo.Size = new System.Drawing.Size(106, 20);
            this.lblDispMatNo.TabIndex = 12;
            this.lblDispMatNo.Text = "Material No";
            this.lblDispMatNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pgbPlacedBundle
            // 
            this.pgbPlacedBundle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pgbPlacedBundle.Location = new System.Drawing.Point(29, 686);
            this.pgbPlacedBundle.Margin = new System.Windows.Forms.Padding(4);
            this.pgbPlacedBundle.Name = "pgbPlacedBundle";
            this.pgbPlacedBundle.Size = new System.Drawing.Size(347, 37);
            this.pgbPlacedBundle.TabIndex = 13;
            this.pgbPlacedBundle.Value = 50;
            // 
            // lblPalletPercent
            // 
            this.lblPalletPercent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPalletPercent.AutoSize = true;
            this.lblPalletPercent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPalletPercent.ForeColor = System.Drawing.Color.Black;
            this.lblPalletPercent.Location = new System.Drawing.Point(29, 729);
            this.lblPalletPercent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPalletPercent.Name = "lblPalletPercent";
            this.lblPalletPercent.Size = new System.Drawing.Size(120, 17);
            this.lblPalletPercent.TabIndex = 14;
            this.lblPalletPercent.Text = "Pallet:1 (100%)";
            this.lblPalletPercent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDispBDCount
            // 
            this.lblDispBDCount.AutoSize = true;
            this.lblDispBDCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispBDCount.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDispBDCount.Location = new System.Drawing.Point(17, 85);
            this.lblDispBDCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDispBDCount.Name = "lblDispBDCount";
            this.lblDispBDCount.Size = new System.Drawing.Size(122, 20);
            this.lblDispBDCount.TabIndex = 15;
            this.lblDispBDCount.Text = "Bundle Count";
            this.lblDispBDCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblDispBDCount.Click += new System.EventHandler(this.lblCurBDCount_Click);
            // 
            // lblDispLYCount
            // 
            this.lblDispLYCount.AutoSize = true;
            this.lblDispLYCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispLYCount.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDispLYCount.Location = new System.Drawing.Point(17, 135);
            this.lblDispLYCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDispLYCount.Name = "lblDispLYCount";
            this.lblDispLYCount.Size = new System.Drawing.Size(125, 20);
            this.lblDispLYCount.TabIndex = 16;
            this.lblDispLYCount.Text = "Current Layer";
            this.lblDispLYCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDispBDPerGrip
            // 
            this.lblDispBDPerGrip.AutoSize = true;
            this.lblDispBDPerGrip.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispBDPerGrip.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDispBDPerGrip.Location = new System.Drawing.Point(160, 135);
            this.lblDispBDPerGrip.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDispBDPerGrip.Name = "lblDispBDPerGrip";
            this.lblDispBDPerGrip.Size = new System.Drawing.Size(144, 20);
            this.lblDispBDPerGrip.TabIndex = 19;
            this.lblDispBDPerGrip.Text = "Bundle Per Grip";
            this.lblDispBDPerGrip.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDispTSLayer
            // 
            this.lblDispTSLayer.AutoSize = true;
            this.lblDispTSLayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispTSLayer.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDispTSLayer.Location = new System.Drawing.Point(160, 191);
            this.lblDispTSLayer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDispTSLayer.Name = "lblDispTSLayer";
            this.lblDispTSLayer.Size = new System.Drawing.Size(136, 20);
            this.lblDispTSLayer.TabIndex = 20;
            this.lblDispTSLayer.Text = "TieSheet Layer";
            this.lblDispTSLayer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlSimulate
            // 
            this.pnlSimulate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSimulate.AutoSize = true;
            this.pnlSimulate.BackColor = System.Drawing.Color.LightGreen;
            this.pnlSimulate.Controls.Add(this.pic3DOrder);
            this.pnlSimulate.Controls.Add(this.lblPalletPercent);
            this.pnlSimulate.Controls.Add(this.pgbPlacedBundle);
            this.pnlSimulate.Controls.Add(this.lblSim3DMode);
            this.pnlSimulate.Location = new System.Drawing.Point(793, 74);
            this.pnlSimulate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlSimulate.Name = "pnlSimulate";
            this.pnlSimulate.Size = new System.Drawing.Size(399, 785);
            this.pnlSimulate.TabIndex = 16;
            // 
            // pic3DOrder
            // 
            this.pic3DOrder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pic3DOrder.Location = new System.Drawing.Point(13, 38);
            this.pic3DOrder.Margin = new System.Windows.Forms.Padding(4);
            this.pic3DOrder.Name = "pic3DOrder";
            this.pic3DOrder.Size = new System.Drawing.Size(373, 634);
            this.pic3DOrder.TabIndex = 21;
            this.pic3DOrder.TabStop = false;
            // 
            // imlMain
            // 
            this.imlMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlMain.ImageStream")));
            this.imlMain.TransparentColor = System.Drawing.Color.Transparent;
            this.imlMain.Images.SetKeyName(0, "Pallet_TopView.PNG");
            // 
            // lblTieSheetSize_Title
            // 
            this.lblTieSheetSize_Title.AutoSize = true;
            this.lblTieSheetSize_Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTieSheetSize_Title.ForeColor = System.Drawing.Color.LightGray;
            this.lblTieSheetSize_Title.Location = new System.Drawing.Point(0, 0);
            this.lblTieSheetSize_Title.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTieSheetSize_Title.Name = "lblTieSheetSize_Title";
            this.lblTieSheetSize_Title.Size = new System.Drawing.Size(99, 18);
            this.lblTieSheetSize_Title.TabIndex = 3;
            this.lblTieSheetSize_Title.Text = "TieSheet Size";
            // 
            // lblTieSheetSize
            // 
            this.lblTieSheetSize.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblTieSheetSize.AutoSize = true;
            this.lblTieSheetSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTieSheetSize.ForeColor = System.Drawing.Color.Aquamarine;
            this.lblTieSheetSize.Location = new System.Drawing.Point(1, 25);
            this.lblTieSheetSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTieSheetSize.Name = "lblTieSheetSize";
            this.lblTieSheetSize.Size = new System.Drawing.Size(156, 31);
            this.lblTieSheetSize.TabIndex = 2;
            this.lblTieSheetSize.Text = "1000x1000";
            this.lblTieSheetSize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlTieSheetSize
            // 
            this.pnlTieSheetSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTieSheetSize.BackColor = System.Drawing.SystemColors.Desktop;
            this.pnlTieSheetSize.Controls.Add(this.btnEditTS);
            this.pnlTieSheetSize.Controls.Add(this.lblTieSheetSize_Title);
            this.pnlTieSheetSize.Controls.Add(this.lblTieSheetSize);
            this.pnlTieSheetSize.Location = new System.Drawing.Point(248, 7);
            this.pnlTieSheetSize.Margin = new System.Windows.Forms.Padding(4);
            this.pnlTieSheetSize.Name = "pnlTieSheetSize";
            this.pnlTieSheetSize.Size = new System.Drawing.Size(177, 60);
            this.pnlTieSheetSize.TabIndex = 23;
            // 
            // btnEditTS
            // 
            this.btnEditTS.BackColor = System.Drawing.Color.Gold;
            this.btnEditTS.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEditTS.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnEditTS.ForeColor = System.Drawing.Color.Black;
            this.btnEditTS.Location = new System.Drawing.Point(127, 0);
            this.btnEditTS.Margin = new System.Windows.Forms.Padding(4);
            this.btnEditTS.Name = "btnEditTS";
            this.btnEditTS.Size = new System.Drawing.Size(51, 25);
            this.btnEditTS.TabIndex = 72;
            this.btnEditTS.Text = "Edit";
            this.btnEditTS.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnEditTS.UseVisualStyleBackColor = false;
            this.btnEditTS.Click += new System.EventHandler(this.btnEditTS_Click);
            // 
            // lblDBClicktoEdit
            // 
            this.lblDBClicktoEdit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDBClicktoEdit.ForeColor = System.Drawing.Color.LightSalmon;
            this.lblDBClicktoEdit.Location = new System.Drawing.Point(544, 22);
            this.lblDBClicktoEdit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDBClicktoEdit.Name = "lblDBClicktoEdit";
            this.lblDBClicktoEdit.Size = new System.Drawing.Size(201, 46);
            this.lblDBClicktoEdit.TabIndex = 24;
            this.lblDBClicktoEdit.Text = "*Double Click Data Row to open Edit Window";
            this.lblDBClicktoEdit.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txbDispMatNo
            // 
            this.txbDispMatNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbDispMatNo.Location = new System.Drawing.Point(33, 54);
            this.txbDispMatNo.Margin = new System.Windows.Forms.Padding(4);
            this.txbDispMatNo.Name = "txbDispMatNo";
            this.txbDispMatNo.ReadOnly = true;
            this.txbDispMatNo.Size = new System.Drawing.Size(119, 26);
            this.txbDispMatNo.TabIndex = 25;
            this.txbDispMatNo.Text = "Z033129066";
            this.txbDispMatNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txbDispBDPerGrip
            // 
            this.txbDispBDPerGrip.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbDispBDPerGrip.Location = new System.Drawing.Point(176, 158);
            this.txbDispBDPerGrip.Margin = new System.Windows.Forms.Padding(4);
            this.txbDispBDPerGrip.Name = "txbDispBDPerGrip";
            this.txbDispBDPerGrip.ReadOnly = true;
            this.txbDispBDPerGrip.Size = new System.Drawing.Size(72, 26);
            this.txbDispBDPerGrip.TabIndex = 26;
            this.txbDispBDPerGrip.Text = "9";
            this.txbDispBDPerGrip.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txbTSLY1_10
            // 
            this.txbTSLY1_10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbTSLY1_10.Location = new System.Drawing.Point(176, 214);
            this.txbTSLY1_10.Margin = new System.Windows.Forms.Padding(4);
            this.txbTSLY1_10.Name = "txbTSLY1_10";
            this.txbTSLY1_10.ReadOnly = true;
            this.txbTSLY1_10.Size = new System.Drawing.Size(281, 26);
            this.txbTSLY1_10.TabIndex = 27;
            this.txbTSLY1_10.Text = "1,2,3,4,5,6,7,8,9,10";
            // 
            // txbDispBDCount
            // 
            this.txbDispBDCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbDispBDCount.Location = new System.Drawing.Point(33, 107);
            this.txbDispBDCount.Margin = new System.Windows.Forms.Padding(4);
            this.txbDispBDCount.Name = "txbDispBDCount";
            this.txbDispBDCount.ReadOnly = true;
            this.txbDispBDCount.Size = new System.Drawing.Size(72, 26);
            this.txbDispBDCount.TabIndex = 28;
            this.txbDispBDCount.Text = "999999";
            this.txbDispBDCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txbDispBDCount.TextChanged += new System.EventHandler(this.txbCurBDCount_TextChanged);
            // 
            // txbDispLYCount
            // 
            this.txbDispLYCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbDispLYCount.Location = new System.Drawing.Point(33, 158);
            this.txbDispLYCount.Margin = new System.Windows.Forms.Padding(4);
            this.txbDispLYCount.Name = "txbDispLYCount";
            this.txbDispLYCount.ReadOnly = true;
            this.txbDispLYCount.Size = new System.Drawing.Size(72, 26);
            this.txbDispLYCount.TabIndex = 29;
            this.txbDispLYCount.Text = "99";
            this.txbDispLYCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox11
            // 
            this.groupBox11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox11.Controls.Add(this.rdoPressSquaring);
            this.groupBox11.Controls.Add(this.rdoOpenSquaring);
            this.groupBox11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox11.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox11.Location = new System.Drawing.Point(-94, 810);
            this.groupBox11.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox11.Size = new System.Drawing.Size(373, 49);
            this.groupBox11.TabIndex = 81;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Squaring display";
            // 
            // rdoPressSquaring
            // 
            this.rdoPressSquaring.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rdoPressSquaring.AutoSize = true;
            this.rdoPressSquaring.Location = new System.Drawing.Point(202, 21);
            this.rdoPressSquaring.Margin = new System.Windows.Forms.Padding(4);
            this.rdoPressSquaring.Name = "rdoPressSquaring";
            this.rdoPressSquaring.Size = new System.Drawing.Size(149, 24);
            this.rdoPressSquaring.TabIndex = 15;
            this.rdoPressSquaring.Text = "Pressed Bundle";
            this.rdoPressSquaring.UseVisualStyleBackColor = true;
            this.rdoPressSquaring.CheckedChanged += new System.EventHandler(this.rdoPressSquaring_CheckedChanged);
            // 
            // rdoOpenSquaring
            // 
            this.rdoOpenSquaring.AutoSize = true;
            this.rdoOpenSquaring.Checked = true;
            this.rdoOpenSquaring.Location = new System.Drawing.Point(31, 21);
            this.rdoOpenSquaring.Margin = new System.Windows.Forms.Padding(4);
            this.rdoOpenSquaring.Name = "rdoOpenSquaring";
            this.rdoOpenSquaring.Size = new System.Drawing.Size(141, 24);
            this.rdoOpenSquaring.TabIndex = 14;
            this.rdoOpenSquaring.TabStop = true;
            this.rdoOpenSquaring.Text = "Open Squaring";
            this.rdoOpenSquaring.UseVisualStyleBackColor = true;
            this.rdoOpenSquaring.CheckedChanged += new System.EventHandler(this.rdoOpenSquaring_CheckedChanged);
            // 
            // tmrLotEnd
            // 
            this.tmrLotEnd.Interval = 5000;
            this.tmrLotEnd.Tick += new System.EventHandler(this.TmrLotEnd_Tick);
            // 
            // tmrConfirmStart
            // 
            this.tmrConfirmStart.Interval = 5000;
            this.tmrConfirmStart.Tick += new System.EventHandler(this.TmrConfirmStart_Tick);
            // 
            // tmrRobotFeed
            // 
            this.tmrRobotFeed.Interval = 1000;
            this.tmrRobotFeed.Tick += new System.EventHandler(this.TmrRobotFeed_Tick);
            // 
            // pnlGripperFinger
            // 
            this.pnlGripperFinger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlGripperFinger.BackColor = System.Drawing.Color.Black;
            this.pnlGripperFinger.Controls.Add(this.lblFingerRequire);
            this.pnlGripperFinger.Controls.Add(this.lblFinger_Title);
            this.pnlGripperFinger.Controls.Add(this.picGripperFinger);
            this.pnlGripperFinger.Location = new System.Drawing.Point(612, 7);
            this.pnlGripperFinger.Margin = new System.Windows.Forms.Padding(4);
            this.pnlGripperFinger.Name = "pnlGripperFinger";
            this.pnlGripperFinger.Size = new System.Drawing.Size(133, 60);
            this.pnlGripperFinger.TabIndex = 32;
            // 
            // lblFingerRequire
            // 
            this.lblFingerRequire.AutoSize = true;
            this.lblFingerRequire.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFingerRequire.ForeColor = System.Drawing.Color.SpringGreen;
            this.lblFingerRequire.Location = new System.Drawing.Point(59, 25);
            this.lblFingerRequire.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFingerRequire.Name = "lblFingerRequire";
            this.lblFingerRequire.Size = new System.Drawing.Size(57, 31);
            this.lblFingerRequire.TabIndex = 7;
            this.lblFingerRequire.Text = "ON";
            this.lblFingerRequire.DoubleClick += new System.EventHandler(this.lblFingerRequire_DoubleClick);
            // 
            // lblFinger_Title
            // 
            this.lblFinger_Title.AutoSize = true;
            this.lblFinger_Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFinger_Title.ForeColor = System.Drawing.Color.LightGray;
            this.lblFinger_Title.Location = new System.Drawing.Point(0, 0);
            this.lblFinger_Title.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFinger_Title.Name = "lblFinger_Title";
            this.lblFinger_Title.Size = new System.Drawing.Size(49, 18);
            this.lblFinger_Title.TabIndex = 7;
            this.lblFinger_Title.Text = "Finger";
            this.lblFinger_Title.DoubleClick += new System.EventHandler(this.lblFinger_Title_DoubleClick);
            // 
            // picGripperFinger
            // 
            this.picGripperFinger.Image = ((System.Drawing.Image)(resources.GetObject("picGripperFinger.Image")));
            this.picGripperFinger.Location = new System.Drawing.Point(19, 23);
            this.picGripperFinger.Margin = new System.Windows.Forms.Padding(4);
            this.picGripperFinger.Name = "picGripperFinger";
            this.picGripperFinger.Size = new System.Drawing.Size(32, 30);
            this.picGripperFinger.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picGripperFinger.TabIndex = 6;
            this.picGripperFinger.TabStop = false;
            this.picGripperFinger.DoubleClick += new System.EventHandler(this.picGripperFinger_DoubleClick);
            // 
            // pnlPallet
            // 
            this.pnlPallet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPallet.BackColor = System.Drawing.SystemColors.Desktop;
            this.pnlPallet.Controls.Add(this.lblSizePallet);
            this.pnlPallet.Controls.Add(this.lblPallet_Title);
            this.pnlPallet.Controls.Add(this.lblPalletType);
            this.pnlPallet.Location = new System.Drawing.Point(430, 7);
            this.pnlPallet.Margin = new System.Windows.Forms.Padding(4);
            this.pnlPallet.Name = "pnlPallet";
            this.pnlPallet.Size = new System.Drawing.Size(177, 60);
            this.pnlPallet.TabIndex = 24;
            // 
            // lblSizePallet
            // 
            this.lblSizePallet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSizePallet.AutoSize = true;
            this.lblSizePallet.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSizePallet.ForeColor = System.Drawing.Color.Gold;
            this.lblSizePallet.Location = new System.Drawing.Point(15, 37);
            this.lblSizePallet.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSizePallet.Name = "lblSizePallet";
            this.lblSizePallet.Size = new System.Drawing.Size(46, 20);
            this.lblSizePallet.TabIndex = 10;
            this.lblSizePallet.Text = "Size";
            this.lblSizePallet.DoubleClick += new System.EventHandler(this.lblSizePallet_DoubleClick);
            // 
            // lblPallet_Title
            // 
            this.lblPallet_Title.AutoSize = true;
            this.lblPallet_Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPallet_Title.ForeColor = System.Drawing.Color.LightGray;
            this.lblPallet_Title.Location = new System.Drawing.Point(0, 0);
            this.lblPallet_Title.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPallet_Title.Name = "lblPallet_Title";
            this.lblPallet_Title.Size = new System.Drawing.Size(80, 18);
            this.lblPallet_Title.TabIndex = 3;
            this.lblPallet_Title.Text = "Pallet Type";
            this.lblPallet_Title.DoubleClick += new System.EventHandler(this.lblPallet_Title_DoubleClick);
            // 
            // lblPalletType
            // 
            this.lblPalletType.AutoSize = true;
            this.lblPalletType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPalletType.ForeColor = System.Drawing.Color.Aquamarine;
            this.lblPalletType.Location = new System.Drawing.Point(35, 16);
            this.lblPalletType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPalletType.Name = "lblPalletType";
            this.lblPalletType.Size = new System.Drawing.Size(93, 25);
            this.lblPalletType.TabIndex = 2;
            this.lblPalletType.Text = "Wooden";
            this.lblPalletType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPalletType.DoubleClick += new System.EventHandler(this.lblPalletType_DoubleClick);
            // 
            // btnResetData
            // 
            this.btnResetData.BackColor = System.Drawing.Color.Black;
            this.btnResetData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResetData.ForeColor = System.Drawing.Color.LightGray;
            this.btnResetData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnResetData.Location = new System.Drawing.Point(405, 9);
            this.btnResetData.Margin = new System.Windows.Forms.Padding(4);
            this.btnResetData.Name = "btnResetData";
            this.btnResetData.Size = new System.Drawing.Size(131, 59);
            this.btnResetData.TabIndex = 19;
            this.btnResetData.Text = "Reset Data";
            this.btnResetData.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnResetData.UseVisualStyleBackColor = false;
            this.btnResetData.Click += new System.EventHandler(this.btnResetData_Click);
            // 
            // btnChangeToCurOrder
            // 
            this.btnChangeToCurOrder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChangeToCurOrder.ForeColor = System.Drawing.Color.Black;
            this.btnChangeToCurOrder.Location = new System.Drawing.Point(161, 54);
            this.btnChangeToCurOrder.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnChangeToCurOrder.Name = "btnChangeToCurOrder";
            this.btnChangeToCurOrder.Size = new System.Drawing.Size(153, 27);
            this.btnChangeToCurOrder.TabIndex = 84;
            this.btnChangeToCurOrder.Text = "Select Current Order";
            this.btnChangeToCurOrder.UseVisualStyleBackColor = true;
            this.btnChangeToCurOrder.Click += new System.EventHandler(this.btnChangeToCurOrder_Click);
            // 
            // lblDispPatSeq
            // 
            this.lblDispPatSeq.AutoSize = true;
            this.lblDispPatSeq.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispPatSeq.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDispPatSeq.Location = new System.Drawing.Point(25, 295);
            this.lblDispPatSeq.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDispPatSeq.Name = "lblDispPatSeq";
            this.lblDispPatSeq.Size = new System.Drawing.Size(158, 20);
            this.lblDispPatSeq.TabIndex = 89;
            this.lblDispPatSeq.Text = "Pattern Sequence";
            this.lblDispPatSeq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txbTSLY11_20
            // 
            this.txbTSLY11_20.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbTSLY11_20.Location = new System.Drawing.Point(176, 239);
            this.txbTSLY11_20.Margin = new System.Windows.Forms.Padding(4);
            this.txbTSLY11_20.Name = "txbTSLY11_20";
            this.txbTSLY11_20.ReadOnly = true;
            this.txbTSLY11_20.Size = new System.Drawing.Size(281, 26);
            this.txbTSLY11_20.TabIndex = 27;
            this.txbTSLY11_20.Text = "11,12,13,14,15,16,17,18,19,20";
            // 
            // txbTSLY21_30
            // 
            this.txbTSLY21_30.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbTSLY21_30.Location = new System.Drawing.Point(176, 266);
            this.txbTSLY21_30.Margin = new System.Windows.Forms.Padding(4);
            this.txbTSLY21_30.Name = "txbTSLY21_30";
            this.txbTSLY21_30.ReadOnly = true;
            this.txbTSLY21_30.Size = new System.Drawing.Size(281, 26);
            this.txbTSLY21_30.TabIndex = 27;
            this.txbTSLY21_30.Text = "21,22,23,24,25,26,27,28,29,30";
            // 
            // txbDispSO
            // 
            this.txbDispSO.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbDispSO.Location = new System.Drawing.Point(176, 107);
            this.txbDispSO.Margin = new System.Windows.Forms.Padding(4);
            this.txbDispSO.Name = "txbDispSO";
            this.txbDispSO.ReadOnly = true;
            this.txbDispSO.Size = new System.Drawing.Size(72, 26);
            this.txbDispSO.TabIndex = 91;
            this.txbDispSO.Text = "99 / 99";
            this.txbDispSO.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblDispSO
            // 
            this.lblDispSO.AutoSize = true;
            this.lblDispSO.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispSO.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDispSO.Location = new System.Drawing.Point(160, 85);
            this.lblDispSO.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDispSO.Name = "lblDispSO";
            this.lblDispSO.Size = new System.Drawing.Size(45, 20);
            this.lblDispSO.TabIndex = 90;
            this.lblDispSO.Text = "#SO";
            this.lblDispSO.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDispSOBD
            // 
            this.lblDispSOBD.AutoSize = true;
            this.lblDispSOBD.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispSOBD.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDispSOBD.Location = new System.Drawing.Point(281, 85);
            this.lblDispSOBD.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDispSOBD.Name = "lblDispSOBD";
            this.lblDispSOBD.Size = new System.Drawing.Size(154, 20);
            this.lblDispSOBD.TabIndex = 15;
            this.lblDispSOBD.Text = "SO Bundle Count";
            this.lblDispSOBD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblDispSOBD.Click += new System.EventHandler(this.lblCurBDCount_Click);
            // 
            // txbDispSOBD
            // 
            this.txbDispSOBD.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbDispSOBD.Location = new System.Drawing.Point(297, 107);
            this.txbDispSOBD.Margin = new System.Windows.Forms.Padding(4);
            this.txbDispSOBD.Name = "txbDispSOBD";
            this.txbDispSOBD.ReadOnly = true;
            this.txbDispSOBD.Size = new System.Drawing.Size(160, 26);
            this.txbDispSOBD.TabIndex = 28;
            this.txbDispSOBD.Text = "999999 / 999999";
            this.txbDispSOBD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txbDispSOBD.TextChanged += new System.EventHandler(this.txbCurBDCount_TextChanged);
            // 
            // chkbxDispPat1
            // 
            this.chkbxDispPat1.AutoSize = true;
            this.chkbxDispPat1.Enabled = false;
            this.chkbxDispPat1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkbxDispPat1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.chkbxDispPat1.Location = new System.Drawing.Point(43, 324);
            this.chkbxDispPat1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkbxDispPat1.Name = "chkbxDispPat1";
            this.chkbxDispPat1.Size = new System.Drawing.Size(48, 21);
            this.chkbxDispPat1.TabIndex = 92;
            this.chkbxDispPat1.Text = "#1";
            this.chkbxDispPat1.UseVisualStyleBackColor = true;
            // 
            // chkbxDispPat2
            // 
            this.chkbxDispPat2.AutoSize = true;
            this.chkbxDispPat2.Enabled = false;
            this.chkbxDispPat2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkbxDispPat2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.chkbxDispPat2.Location = new System.Drawing.Point(99, 324);
            this.chkbxDispPat2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkbxDispPat2.Name = "chkbxDispPat2";
            this.chkbxDispPat2.Size = new System.Drawing.Size(48, 21);
            this.chkbxDispPat2.TabIndex = 92;
            this.chkbxDispPat2.Text = "#2";
            this.chkbxDispPat2.UseVisualStyleBackColor = true;
            // 
            // chkbxDispPat4
            // 
            this.chkbxDispPat4.AutoSize = true;
            this.chkbxDispPat4.Enabled = false;
            this.chkbxDispPat4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkbxDispPat4.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.chkbxDispPat4.Location = new System.Drawing.Point(203, 324);
            this.chkbxDispPat4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkbxDispPat4.Name = "chkbxDispPat4";
            this.chkbxDispPat4.Size = new System.Drawing.Size(48, 21);
            this.chkbxDispPat4.TabIndex = 93;
            this.chkbxDispPat4.Text = "#4";
            this.chkbxDispPat4.UseVisualStyleBackColor = true;
            // 
            // chkbxDispPat3
            // 
            this.chkbxDispPat3.AutoSize = true;
            this.chkbxDispPat3.Enabled = false;
            this.chkbxDispPat3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkbxDispPat3.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.chkbxDispPat3.Location = new System.Drawing.Point(149, 324);
            this.chkbxDispPat3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkbxDispPat3.Name = "chkbxDispPat3";
            this.chkbxDispPat3.Size = new System.Drawing.Size(48, 21);
            this.chkbxDispPat3.TabIndex = 94;
            this.chkbxDispPat3.Text = "#3";
            this.chkbxDispPat3.UseVisualStyleBackColor = true;
            // 
            // lblDispTSSize
            // 
            this.lblDispTSSize.AutoSize = true;
            this.lblDispTSSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispTSSize.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDispTSSize.Location = new System.Drawing.Point(17, 191);
            this.lblDispTSSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDispTSSize.Name = "lblDispTSSize";
            this.lblDispTSSize.Size = new System.Drawing.Size(126, 20);
            this.lblDispTSSize.TabIndex = 20;
            this.lblDispTSSize.Text = "TieSheet Size";
            this.lblDispTSSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txbDispTSW
            // 
            this.txbDispTSW.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbDispTSW.Location = new System.Drawing.Point(64, 214);
            this.txbDispTSW.Margin = new System.Windows.Forms.Padding(4);
            this.txbDispTSW.Name = "txbDispTSW";
            this.txbDispTSW.ReadOnly = true;
            this.txbDispTSW.Size = new System.Drawing.Size(52, 26);
            this.txbDispTSW.TabIndex = 27;
            this.txbDispTSW.Text = "9999";
            this.txbDispTSW.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txbDispTSL
            // 
            this.txbDispTSL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbDispTSL.Location = new System.Drawing.Point(64, 239);
            this.txbDispTSL.Margin = new System.Windows.Forms.Padding(4);
            this.txbDispTSL.Name = "txbDispTSL";
            this.txbDispTSL.ReadOnly = true;
            this.txbDispTSL.Size = new System.Drawing.Size(52, 26);
            this.txbDispTSL.TabIndex = 27;
            this.txbDispTSL.Text = "9999";
            this.txbDispTSL.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblDispTSW
            // 
            this.lblDispTSW.AutoSize = true;
            this.lblDispTSW.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispTSW.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDispTSW.Location = new System.Drawing.Point(33, 217);
            this.lblDispTSW.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDispTSW.Name = "lblDispTSW";
            this.lblDispTSW.Size = new System.Drawing.Size(26, 20);
            this.lblDispTSW.TabIndex = 20;
            this.lblDispTSW.Text = "W";
            this.lblDispTSW.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDispTSL
            // 
            this.lblDispTSL.AutoSize = true;
            this.lblDispTSL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispTSL.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDispTSL.Location = new System.Drawing.Point(33, 242);
            this.lblDispTSL.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDispTSL.Name = "lblDispTSL";
            this.lblDispTSL.Size = new System.Drawing.Size(20, 20);
            this.lblDispTSL.TabIndex = 95;
            this.lblDispTSL.Text = "L";
            this.lblDispTSL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chbRealTime
            // 
            this.chbRealTime.AutoSize = true;
            this.chbRealTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chbRealTime.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.chbRealTime.Location = new System.Drawing.Point(320, 58);
            this.chbRealTime.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chbRealTime.Name = "chbRealTime";
            this.chbRealTime.Size = new System.Drawing.Size(156, 21);
            this.chbRealTime.TabIndex = 93;
            this.chbRealTime.Text = "Display RealTime";
            this.chbRealTime.UseVisualStyleBackColor = true;
            // 
            // tltipRobotConnection
            // 
            this.tltipRobotConnection.AutoPopDelay = 5000;
            this.tltipRobotConnection.InitialDelay = 0;
            this.tltipRobotConnection.IsBalloon = true;
            this.tltipRobotConnection.ReshowDelay = 100;
            // 
            // tltipPLCConnection
            // 
            this.tltipPLCConnection.AutoPopDelay = 5000;
            this.tltipPLCConnection.InitialDelay = 0;
            this.tltipPLCConnection.IsBalloon = true;
            this.tltipPLCConnection.ReshowDelay = 100;
            // 
            // lblDoorStatusDisp
            // 
            this.lblDoorStatusDisp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDoorStatusDisp.Location = new System.Drawing.Point(25, 73);
            this.lblDoorStatusDisp.Name = "lblDoorStatusDisp";
            this.lblDoorStatusDisp.Size = new System.Drawing.Size(309, 25);
            this.lblDoorStatusDisp.TabIndex = 4;
            this.lblDoorStatusDisp.Text = "PLC Recieved Cmd (Unlocked)";
            this.lblDoorStatusDisp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDoorStatus
            // 
            this.lblDoorStatus.AutoSize = true;
            this.lblDoorStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDoorStatus.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblDoorStatus.Location = new System.Drawing.Point(13, 36);
            this.lblDoorStatus.Name = "lblDoorStatus";
            this.lblDoorStatus.Size = new System.Drawing.Size(110, 20);
            this.lblDoorStatus.TabIndex = 3;
            this.lblDoorStatus.Text = "Door Status";
            // 
            // lblDoorCmd
            // 
            this.lblDoorCmd.AutoSize = true;
            this.lblDoorCmd.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDoorCmd.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblDoorCmd.Location = new System.Drawing.Point(13, 118);
            this.lblDoorCmd.Name = "lblDoorCmd";
            this.lblDoorCmd.Size = new System.Drawing.Size(139, 20);
            this.lblDoorCmd.TabIndex = 2;
            this.lblDoorCmd.Text = "Door Command";
            // 
            // btnDoorUnlock
            // 
            this.btnDoorUnlock.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDoorUnlock.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnDoorUnlock.Location = new System.Drawing.Point(17, 142);
            this.btnDoorUnlock.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDoorUnlock.Name = "btnDoorUnlock";
            this.btnDoorUnlock.Size = new System.Drawing.Size(101, 33);
            this.btnDoorUnlock.TabIndex = 1;
            this.btnDoorUnlock.Text = "Unlock";
            this.btnDoorUnlock.UseVisualStyleBackColor = true;
            this.btnDoorUnlock.Click += new System.EventHandler(this.btnDoorUnlock_Click);
            // 
            // btnDoorLock
            // 
            this.btnDoorLock.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDoorLock.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnDoorLock.Location = new System.Drawing.Point(125, 142);
            this.btnDoorLock.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDoorLock.Name = "btnDoorLock";
            this.btnDoorLock.Size = new System.Drawing.Size(101, 33);
            this.btnDoorLock.TabIndex = 0;
            this.btnDoorLock.Text = "Lock";
            this.btnDoorLock.UseVisualStyleBackColor = true;
            this.btnDoorLock.Click += new System.EventHandler(this.btnDoorLock_Click);
            // 
            // lblSystemStatusDisp
            // 
            this.lblSystemStatusDisp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSystemStatusDisp.Location = new System.Drawing.Point(25, 231);
            this.lblSystemStatusDisp.Name = "lblSystemStatusDisp";
            this.lblSystemStatusDisp.Size = new System.Drawing.Size(309, 25);
            this.lblSystemStatusDisp.TabIndex = 4;
            this.lblSystemStatusDisp.Text = "PLC Recieved Cmd (Half-Finish)";
            this.lblSystemStatusDisp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSystemCmd
            // 
            this.lblSystemCmd.AutoSize = true;
            this.lblSystemCmd.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSystemCmd.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblSystemCmd.Location = new System.Drawing.Point(13, 276);
            this.lblSystemCmd.Name = "lblSystemCmd";
            this.lblSystemCmd.Size = new System.Drawing.Size(160, 20);
            this.lblSystemCmd.TabIndex = 3;
            this.lblSystemCmd.Text = "System Command";
            // 
            // lblSystemStatus
            // 
            this.lblSystemStatus.AutoSize = true;
            this.lblSystemStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSystemStatus.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblSystemStatus.Location = new System.Drawing.Point(13, 194);
            this.lblSystemStatus.Name = "lblSystemStatus";
            this.lblSystemStatus.Size = new System.Drawing.Size(131, 20);
            this.lblSystemStatus.TabIndex = 3;
            this.lblSystemStatus.Text = "System Status";
            // 
            // btnHalfFinish
            // 
            this.btnHalfFinish.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHalfFinish.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnHalfFinish.Location = new System.Drawing.Point(125, 299);
            this.btnHalfFinish.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnHalfFinish.Name = "btnHalfFinish";
            this.btnHalfFinish.Size = new System.Drawing.Size(101, 33);
            this.btnHalfFinish.TabIndex = 2;
            this.btnHalfFinish.Text = "Half-Finish";
            this.btnHalfFinish.UseVisualStyleBackColor = true;
            this.btnHalfFinish.Click += new System.EventHandler(this.btnHalfFinish_Click);
            // 
            // btnConfirmStart
            // 
            this.btnConfirmStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirmStart.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnConfirmStart.Location = new System.Drawing.Point(17, 299);
            this.btnConfirmStart.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnConfirmStart.Name = "btnConfirmStart";
            this.btnConfirmStart.Size = new System.Drawing.Size(101, 33);
            this.btnConfirmStart.TabIndex = 1;
            this.btnConfirmStart.Text = "Start";
            this.btnConfirmStart.UseVisualStyleBackColor = true;
            this.btnConfirmStart.Click += new System.EventHandler(this.btnConfirmStart_Click);
            // 
            // btnLotEnd
            // 
            this.btnLotEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLotEnd.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnLotEnd.Location = new System.Drawing.Point(233, 299);
            this.btnLotEnd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLotEnd.Name = "btnLotEnd";
            this.btnLotEnd.Size = new System.Drawing.Size(101, 33);
            this.btnLotEnd.TabIndex = 0;
            this.btnLotEnd.Text = "Lot-End";
            this.btnLotEnd.UseVisualStyleBackColor = true;
            this.btnLotEnd.Click += new System.EventHandler(this.btnLotEnd_Click);
            // 
            // grpbxControlPanel
            // 
            this.grpbxControlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grpbxControlPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grpbxControlPanel.Controls.Add(this.lblDoorStatusDisp);
            this.grpbxControlPanel.Controls.Add(this.lblSystemStatusDisp);
            this.grpbxControlPanel.Controls.Add(this.lblDoorStatus);
            this.grpbxControlPanel.Controls.Add(this.lblDoorCmd);
            this.grpbxControlPanel.Controls.Add(this.lblSystemStatus);
            this.grpbxControlPanel.Controls.Add(this.btnDoorUnlock);
            this.grpbxControlPanel.Controls.Add(this.lblSystemCmd);
            this.grpbxControlPanel.Controls.Add(this.btnDoorLock);
            this.grpbxControlPanel.Controls.Add(this.btnLotEnd);
            this.grpbxControlPanel.Controls.Add(this.btnConfirmStart);
            this.grpbxControlPanel.Controls.Add(this.btnHalfFinish);
            this.grpbxControlPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpbxControlPanel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.grpbxControlPanel.Location = new System.Drawing.Point(9, 459);
            this.grpbxControlPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpbxControlPanel.Name = "grpbxControlPanel";
            this.grpbxControlPanel.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpbxControlPanel.Size = new System.Drawing.Size(351, 345);
            this.grpbxControlPanel.TabIndex = 99;
            this.grpbxControlPanel.TabStop = false;
            this.grpbxControlPanel.Text = "Robot Control Panel";
            // 
            // grpbxFeedData
            // 
            this.grpbxFeedData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.grpbxFeedData.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grpbxFeedData.Controls.Add(this.txbDispMatNo);
            this.grpbxFeedData.Controls.Add(this.lblDispTSL);
            this.grpbxFeedData.Controls.Add(this.lblDispMatNo);
            this.grpbxFeedData.Controls.Add(this.chbRealTime);
            this.grpbxFeedData.Controls.Add(this.lblDispBDPerGrip);
            this.grpbxFeedData.Controls.Add(this.chkbxDispPat4);
            this.grpbxFeedData.Controls.Add(this.lblDispBDCount);
            this.grpbxFeedData.Controls.Add(this.chkbxDispPat3);
            this.grpbxFeedData.Controls.Add(this.lblDispSOBD);
            this.grpbxFeedData.Controls.Add(this.chkbxDispPat2);
            this.grpbxFeedData.Controls.Add(this.lblDispLYCount);
            this.grpbxFeedData.Controls.Add(this.chkbxDispPat1);
            this.grpbxFeedData.Controls.Add(this.lblDispTSLayer);
            this.grpbxFeedData.Controls.Add(this.txbDispSO);
            this.grpbxFeedData.Controls.Add(this.lblDispTSSize);
            this.grpbxFeedData.Controls.Add(this.lblDispSO);
            this.grpbxFeedData.Controls.Add(this.lblDispTSW);
            this.grpbxFeedData.Controls.Add(this.lblDispPatSeq);
            this.grpbxFeedData.Controls.Add(this.txbDispBDPerGrip);
            this.grpbxFeedData.Controls.Add(this.btnChangeToCurOrder);
            this.grpbxFeedData.Controls.Add(this.txbTSLY1_10);
            this.grpbxFeedData.Controls.Add(this.txbDispTSW);
            this.grpbxFeedData.Controls.Add(this.txbTSLY11_20);
            this.grpbxFeedData.Controls.Add(this.txbDispTSL);
            this.grpbxFeedData.Controls.Add(this.txbDispLYCount);
            this.grpbxFeedData.Controls.Add(this.txbTSLY21_30);
            this.grpbxFeedData.Controls.Add(this.txbDispSOBD);
            this.grpbxFeedData.Controls.Add(this.txbDispBDCount);
            this.grpbxFeedData.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpbxFeedData.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.grpbxFeedData.Location = new System.Drawing.Point(296, 459);
            this.grpbxFeedData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpbxFeedData.Name = "grpbxFeedData";
            this.grpbxFeedData.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpbxFeedData.Size = new System.Drawing.Size(491, 400);
            this.grpbxFeedData.TabIndex = 100;
            this.grpbxFeedData.TabStop = false;
            this.grpbxFeedData.Text = "Current Order";
            // 
            // frmOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(1200, 874);
            this.Controls.Add(this.grpbxControlPanel);
            this.Controls.Add(this.pnlPallet);
            this.Controls.Add(this.pnlGripperFinger);
            this.Controls.Add(this.groupBox11);
            this.Controls.Add(this.lblDBClicktoEdit);
            this.Controls.Add(this.pnlTieSheetSize);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnNewOrder);
            this.Controls.Add(this.btnResetData);
            this.Controls.Add(this.btnSendData);
            this.Controls.Add(this.dgvOrderList);
            this.Controls.Add(this.pnlSimulate);
            this.Controls.Add(this.pnlRobSpeed);
            this.Controls.Add(this.pnlPLCStatus);
            this.Controls.Add(this.pnlRobotStatus);
            this.Controls.Add(this.pnlSimRun);
            this.Controls.Add(this.grpbxFeedData);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmOrder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmOrder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmOrder_FormClosing);
            this.Load += new System.EventHandler(this.FrmOrder_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderList)).EndInit();
            this.pnlRobSpeed.ResumeLayout(false);
            this.pnlRobSpeed.PerformLayout();
            this.pnlPLCStatus.ResumeLayout(false);
            this.pnlPLCStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPLCCom)).EndInit();
            this.pnlRobotStatus.ResumeLayout(false);
            this.pnlRobotStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRobotCom)).EndInit();
            this.pnlSimRun.ResumeLayout(false);
            this.pnlSimulate.ResumeLayout(false);
            this.pnlSimulate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic3DOrder)).EndInit();
            this.pnlTieSheetSize.ResumeLayout(false);
            this.pnlTieSheetSize.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.pnlGripperFinger.ResumeLayout(false);
            this.pnlGripperFinger.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGripperFinger)).EndInit();
            this.pnlPallet.ResumeLayout(false);
            this.pnlPallet.PerformLayout();
            this.grpbxControlPanel.ResumeLayout(false);
            this.grpbxControlPanel.PerformLayout();
            this.grpbxFeedData.ResumeLayout(false);
            this.grpbxFeedData.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnNewOrder;
        private System.Windows.Forms.Button btnSendData;
        private System.Windows.Forms.DataGridView dgvOrderList;
        private System.Windows.Forms.Panel pnlRobSpeed;
        private System.Windows.Forms.Label lblRobotSpeed_Title;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Panel pnlPLCStatus;
        private System.Windows.Forms.Label lblPLCIP;
        private System.Windows.Forms.Label lblPLCActive;
        private System.Windows.Forms.PictureBox picPLCCom;
        private System.Windows.Forms.Panel pnlRobotStatus;
        private System.Windows.Forms.Label lblRobotIP;
        private System.Windows.Forms.Label lblRobotActive;
        private System.Windows.Forms.PictureBox picRobotCom;
        private System.Windows.Forms.Label lblSim3DMode;
        private System.Windows.Forms.Panel pnlSimRun;
        private System.Windows.Forms.Label lblDispMatNo;
        private System.Windows.Forms.ProgressBar pgbPlacedBundle;
        private System.Windows.Forms.Label lblPalletPercent;
        private System.Windows.Forms.Label lblDispBDCount;
        private System.Windows.Forms.Label lblDispLYCount;
        private System.Windows.Forms.Label lblDispBDPerGrip;
        private System.Windows.Forms.Label lblDispTSLayer;
        private System.Windows.Forms.Panel pnlSimulate;
        private System.Windows.Forms.PictureBox pic3DOrder;
        private System.Windows.Forms.Label lblTieSheetSize_Title;
        private System.Windows.Forms.Label lblTieSheetSize;
        private System.Windows.Forms.Panel pnlTieSheetSize;
        private System.Windows.Forms.Label lblDBClicktoEdit;
        private System.Windows.Forms.TextBox txbDispMatNo;
        private System.Windows.Forms.TextBox txbDispBDPerGrip;
        private System.Windows.Forms.TextBox txbTSLY1_10;
        private System.Windows.Forms.TextBox txbDispBDCount;
        private System.Windows.Forms.TextBox txbDispLYCount;
        public System.Windows.Forms.ImageList imlMain;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.RadioButton rdoPressSquaring;
        private System.Windows.Forms.RadioButton rdoOpenSquaring;
        private System.Windows.Forms.Timer tmrLotEnd;
        private System.Windows.Forms.Timer tmrConfirmStart;
        private System.Windows.Forms.Timer tmrRobotFeed;
        private System.Windows.Forms.Panel pnlGripperFinger;
        private System.Windows.Forms.Label lblFingerRequire;
        private System.Windows.Forms.Label lblFinger_Title;
        private System.Windows.Forms.PictureBox picGripperFinger;
        private System.Windows.Forms.Panel pnlPallet;
        private System.Windows.Forms.Label lblSizePallet;
        private System.Windows.Forms.Label lblPallet_Title;
        private System.Windows.Forms.Label lblPalletType;
        private System.Windows.Forms.Button btnEditTS;
        private System.Windows.Forms.Button btnResetData;
        private System.Windows.Forms.Button btnChangeToCurOrder;
        private System.Windows.Forms.Label lblDispPatSeq;
        private System.Windows.Forms.TextBox txbTSLY11_20;
        private System.Windows.Forms.TextBox txbTSLY21_30;
        private System.Windows.Forms.TextBox txbDispSO;
        private System.Windows.Forms.Label lblDispSO;
        private System.Windows.Forms.Label lblDispSOBD;
        private System.Windows.Forms.TextBox txbDispSOBD;
        private System.Windows.Forms.CheckBox chkbxDispPat1;
        private System.Windows.Forms.CheckBox chkbxDispPat2;
        private System.Windows.Forms.CheckBox chkbxDispPat4;
        private System.Windows.Forms.CheckBox chkbxDispPat3;
        private System.Windows.Forms.Label lblDispTSSize;
        private System.Windows.Forms.TextBox txbDispTSW;
        private System.Windows.Forms.TextBox txbDispTSL;
        private System.Windows.Forms.Label lblDispTSW;
        private System.Windows.Forms.Label lblDispTSL;
        private System.Windows.Forms.CheckBox chbRealTime;
        private System.Windows.Forms.ToolTip tltipRobotConnection;
        private System.Windows.Forms.ToolTip tltipPLCConnection;
        private System.Windows.Forms.Label lblDoorStatusDisp;
        private System.Windows.Forms.Label lblDoorStatus;
        private System.Windows.Forms.Label lblDoorCmd;
        private System.Windows.Forms.Button btnDoorUnlock;
        private System.Windows.Forms.Button btnDoorLock;
        private System.Windows.Forms.Label lblSystemStatusDisp;
        private System.Windows.Forms.Label lblSystemCmd;
        private System.Windows.Forms.Label lblSystemStatus;
        private System.Windows.Forms.Button btnHalfFinish;
        private System.Windows.Forms.Button btnConfirmStart;
        private System.Windows.Forms.Button btnLotEnd;
        private System.Windows.Forms.GroupBox grpbxControlPanel;
        private System.Windows.Forms.GroupBox grpbxFeedData;
        private System.Windows.Forms.Label lblUDHAlarm;
    }
}
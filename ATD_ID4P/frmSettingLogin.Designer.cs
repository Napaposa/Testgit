namespace ATD_ID4P
{
    partial class frmSettingLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettingLogin));
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblPWTopic = new System.Windows.Forms.Label();
            this.txbAdminCode = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnSubmit
            // 
            this.btnSubmit.BackColor = System.Drawing.Color.Black;
            this.btnSubmit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubmit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.LightGray;
            this.btnSubmit.Image = ((System.Drawing.Image)(resources.GetObject("btnSubmit.Image")));
            this.btnSubmit.Location = new System.Drawing.Point(109, 98);
            this.btnSubmit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(129, 57);
            this.btnSubmit.TabIndex = 23;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Black;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.LightGray;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Location = new System.Drawing.Point(244, 98);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(120, 57);
            this.btnClose.TabIndex = 70;
            this.btnClose.Tag = "";
            this.btnClose.Text = "Close";
            this.btnClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblPWTopic
            // 
            this.lblPWTopic.AutoSize = true;
            this.lblPWTopic.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblPWTopic.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblPWTopic.Location = new System.Drawing.Point(27, 30);
            this.lblPWTopic.Name = "lblPWTopic";
            this.lblPWTopic.Size = new System.Drawing.Size(149, 29);
            this.lblPWTopic.TabIndex = 72;
            this.lblPWTopic.Text = "AdminCode";
            // 
            // txbAdminCode
            // 
            this.txbAdminCode.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txbAdminCode.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.RecentlyUsedList;
            this.txbAdminCode.BackColor = System.Drawing.Color.DarkGray;
            this.txbAdminCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.txbAdminCode.ForeColor = System.Drawing.Color.Black;
            this.txbAdminCode.Location = new System.Drawing.Point(192, 26);
            this.txbAdminCode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txbAdminCode.MaxLength = 25;
            this.txbAdminCode.Name = "txbAdminCode";
            this.txbAdminCode.Size = new System.Drawing.Size(265, 34);
            this.txbAdminCode.TabIndex = 71;
            this.txbAdminCode.UseSystemPasswordChar = true;
            this.txbAdminCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txbAdminCode_KeyDown);
            // 
            // frmSettingLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(485, 176);
            this.Controls.Add(this.lblPWTopic);
            this.Controls.Add(this.txbAdminCode);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSubmit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmSettingLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.Load += new System.EventHandler(this.frmSettingLogin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblPWTopic;
        public System.Windows.Forms.TextBox txbAdminCode;
    }
}
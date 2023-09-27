namespace ATD_ID4P
{
    partial class frmAboutID4P
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAboutID4P));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.picAppLogo = new System.Windows.Forms.PictureBox();
            this.lblProductName = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblCompanyName = new System.Windows.Forms.Label();
            this.txbDescription = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAppLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67F));
            this.tableLayoutPanel.Controls.Add(this.picAppLogo, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.lblProductName, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.lblVersion, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.lblCopyright, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.lblCompanyName, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.txbDescription, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.btnOK, 1, 5);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(11, 10);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(810, 326);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // picAppLogo
            // 
            this.picAppLogo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picAppLogo.Image = ((System.Drawing.Image)(resources.GetObject("picAppLogo.Image")));
            this.picAppLogo.Location = new System.Drawing.Point(5, 4);
            this.picAppLogo.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.picAppLogo.Name = "picAppLogo";
            this.tableLayoutPanel.SetRowSpan(this.picAppLogo, 6);
            this.picAppLogo.Size = new System.Drawing.Size(257, 318);
            this.picAppLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAppLogo.TabIndex = 12;
            this.picAppLogo.TabStop = false;
            // 
            // lblProductName
            // 
            this.lblProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProductName.Location = new System.Drawing.Point(275, 0);
            this.lblProductName.Margin = new System.Windows.Forms.Padding(8, 0, 5, 0);
            this.lblProductName.MaximumSize = new System.Drawing.Size(0, 22);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(530, 22);
            this.lblProductName.TabIndex = 19;
            this.lblProductName.Text = "Product Name";
            this.lblProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblVersion
            // 
            this.lblVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblVersion.Location = new System.Drawing.Point(275, 32);
            this.lblVersion.Margin = new System.Windows.Forms.Padding(8, 0, 5, 0);
            this.lblVersion.MaximumSize = new System.Drawing.Size(0, 22);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(530, 22);
            this.lblVersion.TabIndex = 0;
            this.lblVersion.Text = "Version";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCopyright
            // 
            this.lblCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCopyright.Location = new System.Drawing.Point(275, 64);
            this.lblCopyright.Margin = new System.Windows.Forms.Padding(8, 0, 5, 0);
            this.lblCopyright.MaximumSize = new System.Drawing.Size(0, 22);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(530, 22);
            this.lblCopyright.TabIndex = 21;
            this.lblCopyright.Text = "Copyright";
            this.lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCompanyName
            // 
            this.lblCompanyName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCompanyName.Location = new System.Drawing.Point(275, 96);
            this.lblCompanyName.Margin = new System.Windows.Forms.Padding(8, 0, 5, 0);
            this.lblCompanyName.MaximumSize = new System.Drawing.Size(0, 22);
            this.lblCompanyName.Name = "lblCompanyName";
            this.lblCompanyName.Size = new System.Drawing.Size(530, 22);
            this.lblCompanyName.TabIndex = 22;
            this.lblCompanyName.Text = "Company Name";
            this.lblCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txbDescription
            // 
            this.txbDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txbDescription.Location = new System.Drawing.Point(275, 132);
            this.txbDescription.Margin = new System.Windows.Forms.Padding(8, 4, 5, 4);
            this.txbDescription.Multiline = true;
            this.txbDescription.Name = "txbDescription";
            this.txbDescription.ReadOnly = true;
            this.txbDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txbDescription.Size = new System.Drawing.Size(530, 155);
            this.txbDescription.TabIndex = 23;
            this.txbDescription.TabStop = false;
            this.txbDescription.Text = "ID4P (Industrial 4.0 Palletizer) with Kawasaki Robot";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Location = new System.Drawing.Point(704, 296);
            this.btnOK.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(101, 26);
            this.btnOK.TabIndex = 24;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // frmAboutID4P
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 346);
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAboutID4P";
            this.Padding = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AboutID4P";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAppLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.PictureBox picAppLogo;
        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.Label lblCompanyName;
        private System.Windows.Forms.TextBox txbDescription;
        private System.Windows.Forms.Button btnOK;
    }
}
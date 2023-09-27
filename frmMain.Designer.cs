using ATD_ID4P.Properties;

namespace ATD_ID4P
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.pnlMenuBar = new System.Windows.Forms.Panel();
            this.btnAboutMenu = new System.Windows.Forms.Button();
            this.btnAdminMenu = new System.Windows.Forms.Button();
            this.btnMenuReport = new System.Windows.Forms.Button();
            this.btnProductMenu = new System.Windows.Forms.Button();
            this.btnMinimizeMenu = new System.Windows.Forms.Button();
            this.btnCloseMenu = new System.Windows.Forms.Button();
            this.picAppLogo = new System.Windows.Forms.PictureBox();
            this.btnOrderMenu = new System.Windows.Forms.Button();
            this.cmsReport = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.orderReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dailyReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlMenuBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAppLogo)).BeginInit();
            this.cmsReport.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMenuBar
            // 
            this.pnlMenuBar.BackColor = System.Drawing.SystemColors.Desktop;
            this.pnlMenuBar.Controls.Add(this.btnAboutMenu);
            this.pnlMenuBar.Controls.Add(this.btnAdminMenu);
            this.pnlMenuBar.Controls.Add(this.btnMenuReport);
            this.pnlMenuBar.Controls.Add(this.btnProductMenu);
            this.pnlMenuBar.Controls.Add(this.btnMinimizeMenu);
            this.pnlMenuBar.Controls.Add(this.btnCloseMenu);
            this.pnlMenuBar.Controls.Add(this.picAppLogo);
            this.pnlMenuBar.Controls.Add(this.btnOrderMenu);
            this.pnlMenuBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMenuBar.Location = new System.Drawing.Point(0, 0);
            this.pnlMenuBar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlMenuBar.Name = "pnlMenuBar";
            this.pnlMenuBar.Size = new System.Drawing.Size(1141, 55);
            this.pnlMenuBar.TabIndex = 0;
            // 
            // btnAboutMenu
            // 
            this.btnAboutMenu.BackColor = System.Drawing.Color.Black;
            this.btnAboutMenu.FlatAppearance.BorderSize = 0;
            this.btnAboutMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAboutMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnAboutMenu.ForeColor = System.Drawing.Color.LightGray;
            this.btnAboutMenu.Location = new System.Drawing.Point(720, 6);
            this.btnAboutMenu.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnAboutMenu.Name = "btnAboutMenu";
            this.btnAboutMenu.Size = new System.Drawing.Size(140, 44);
            this.btnAboutMenu.TabIndex = 12;
            this.btnAboutMenu.Text = "About";
            this.btnAboutMenu.UseVisualStyleBackColor = false;
            this.btnAboutMenu.Click += new System.EventHandler(this.btnAboutMenu_Click);
            // 
            // btnAdminMenu
            // 
            this.btnAdminMenu.BackColor = System.Drawing.Color.Black;
            this.btnAdminMenu.FlatAppearance.BorderSize = 0;
            this.btnAdminMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdminMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnAdminMenu.ForeColor = System.Drawing.Color.LightGray;
            this.btnAdminMenu.Location = new System.Drawing.Point(560, 6);
            this.btnAdminMenu.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnAdminMenu.Name = "btnAdminMenu";
            this.btnAdminMenu.Size = new System.Drawing.Size(140, 44);
            this.btnAdminMenu.TabIndex = 11;
            this.btnAdminMenu.Text = "Setting";
            this.btnAdminMenu.UseVisualStyleBackColor = false;
            this.btnAdminMenu.Click += new System.EventHandler(this.btnAdminMenu_Click);
            // 
            // btnMenuReport
            // 
            this.btnMenuReport.BackColor = System.Drawing.Color.Black;
            this.btnMenuReport.FlatAppearance.BorderSize = 0;
            this.btnMenuReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMenuReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnMenuReport.ForeColor = System.Drawing.Color.LightGray;
            this.btnMenuReport.Location = new System.Drawing.Point(400, 6);
            this.btnMenuReport.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnMenuReport.Name = "btnMenuReport";
            this.btnMenuReport.Size = new System.Drawing.Size(140, 44);
            this.btnMenuReport.TabIndex = 10;
            this.btnMenuReport.Text = "Report";
            this.btnMenuReport.UseVisualStyleBackColor = false;
            this.btnMenuReport.Click += new System.EventHandler(this.btnMenuReport_Click);
            // 
            // btnProductMenu
            // 
            this.btnProductMenu.BackColor = System.Drawing.Color.Black;
            this.btnProductMenu.FlatAppearance.BorderSize = 0;
            this.btnProductMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProductMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnProductMenu.ForeColor = System.Drawing.Color.LightGray;
            this.btnProductMenu.Location = new System.Drawing.Point(240, 6);
            this.btnProductMenu.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnProductMenu.Name = "btnProductMenu";
            this.btnProductMenu.Size = new System.Drawing.Size(140, 44);
            this.btnProductMenu.TabIndex = 8;
            this.btnProductMenu.Text = "Product";
            this.btnProductMenu.UseVisualStyleBackColor = false;
            this.btnProductMenu.Click += new System.EventHandler(this.btnProductMenu_Click);
            // 
            // btnMinimizeMenu
            // 
            this.btnMinimizeMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMinimizeMenu.BackColor = System.Drawing.Color.Black;
            this.btnMinimizeMenu.FlatAppearance.BorderSize = 0;
            this.btnMinimizeMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimizeMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnMinimizeMenu.ForeColor = System.Drawing.Color.LightGray;
            this.btnMinimizeMenu.Location = new System.Drawing.Point(1011, 7);
            this.btnMinimizeMenu.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnMinimizeMenu.Name = "btnMinimizeMenu";
            this.btnMinimizeMenu.Size = new System.Drawing.Size(49, 44);
            this.btnMinimizeMenu.TabIndex = 7;
            this.btnMinimizeMenu.Text = "-";
            this.btnMinimizeMenu.UseVisualStyleBackColor = false;
            this.btnMinimizeMenu.Click += new System.EventHandler(this.btnMinimizeMenu_Click);
            // 
            // btnCloseMenu
            // 
            this.btnCloseMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseMenu.BackColor = System.Drawing.Color.Black;
            this.btnCloseMenu.FlatAppearance.BorderSize = 0;
            this.btnCloseMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCloseMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnCloseMenu.ForeColor = System.Drawing.Color.LightGray;
            this.btnCloseMenu.Location = new System.Drawing.Point(1085, 7);
            this.btnCloseMenu.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnCloseMenu.Name = "btnCloseMenu";
            this.btnCloseMenu.Size = new System.Drawing.Size(49, 44);
            this.btnCloseMenu.TabIndex = 6;
            this.btnCloseMenu.Text = "X";
            this.btnCloseMenu.UseVisualStyleBackColor = false;
            this.btnCloseMenu.Click += new System.EventHandler(this.btnCloseMenu_Click);
            // 
            // picAppLogo
            // 
            this.picAppLogo.Image = ((System.Drawing.Image)(resources.GetObject("picAppLogo.Image")));
            this.picAppLogo.Location = new System.Drawing.Point(24, 6);
            this.picAppLogo.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.picAppLogo.Name = "picAppLogo";
            this.picAppLogo.Size = new System.Drawing.Size(49, 46);
            this.picAppLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picAppLogo.TabIndex = 1;
            this.picAppLogo.TabStop = false;
            // 
            // btnOrderMenu
            // 
            this.btnOrderMenu.BackColor = System.Drawing.Color.Black;
            this.btnOrderMenu.Enabled = false;
            this.btnOrderMenu.FlatAppearance.BorderSize = 0;
            this.btnOrderMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOrderMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnOrderMenu.ForeColor = System.Drawing.Color.LightGray;
            this.btnOrderMenu.Location = new System.Drawing.Point(80, 6);
            this.btnOrderMenu.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnOrderMenu.Name = "btnOrderMenu";
            this.btnOrderMenu.Size = new System.Drawing.Size(140, 44);
            this.btnOrderMenu.TabIndex = 1;
            this.btnOrderMenu.Text = "Order";
            this.btnOrderMenu.UseVisualStyleBackColor = false;
            this.btnOrderMenu.Click += new System.EventHandler(this.btnOrderMenu_Click);
            // 
            // cmsReport
            // 
            this.cmsReport.BackColor = System.Drawing.SystemColors.Desktop;
            this.cmsReport.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsReport.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.orderReportToolStripMenuItem,
            this.dailyReportToolStripMenuItem});
            this.cmsReport.Name = "cmsReport";
            this.cmsReport.ShowImageMargin = false;
            this.cmsReport.Size = new System.Drawing.Size(164, 52);
            // 
            // orderReportToolStripMenuItem
            // 
            this.orderReportToolStripMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.orderReportToolStripMenuItem.ForeColor = System.Drawing.Color.LightGray;
            this.orderReportToolStripMenuItem.Name = "orderReportToolStripMenuItem";
            this.orderReportToolStripMenuItem.Size = new System.Drawing.Size(163, 24);
            this.orderReportToolStripMenuItem.Text = "Order Report";
            this.orderReportToolStripMenuItem.Click += new System.EventHandler(this.OrderReportToolStripMenuItem_Click);
            // 
            // dailyReportToolStripMenuItem
            // 
            this.dailyReportToolStripMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.dailyReportToolStripMenuItem.ForeColor = System.Drawing.Color.LightGray;
            this.dailyReportToolStripMenuItem.Name = "dailyReportToolStripMenuItem";
            this.dailyReportToolStripMenuItem.Size = new System.Drawing.Size(163, 24);
            this.dailyReportToolStripMenuItem.Text = "Daily Report";
            this.dailyReportToolStripMenuItem.Click += new System.EventHandler(this.DailyReportToolStripMenuItem_Click);
            // 
            // pnlContent
            // 
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 55);
            this.pnlContent.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(1141, 1037);
            this.pnlContent.TabIndex = 1;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(1141, 1092);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlMenuBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Palletizer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.pnlMenuBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picAppLogo)).EndInit();
            this.cmsReport.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMenuBar;
        private System.Windows.Forms.Button btnOrderMenu;
        private System.Windows.Forms.PictureBox picAppLogo;
        private System.Windows.Forms.Button btnCloseMenu;
        private System.Windows.Forms.Button btnMinimizeMenu;
        private System.Windows.Forms.Button btnAboutMenu;
        private System.Windows.Forms.Button btnAdminMenu;
        private System.Windows.Forms.Button btnMenuReport;
        private System.Windows.Forms.Button btnProductMenu;
        private System.Windows.Forms.ContextMenuStrip cmsReport;
        private System.Windows.Forms.ToolStripMenuItem orderReportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dailyReportToolStripMenuItem;
        private System.Windows.Forms.Panel pnlContent;
    }
}
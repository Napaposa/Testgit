namespace ATD_ID4P
{
    partial class frmSplitSheet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSplitSheet));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblJobDetail = new System.Windows.Forms.Label();
            this.dgvSplitSheet = new System.Windows.Forms.DataGridView();
            this.bdsSplitSheet = new System.Windows.Forms.BindingSource(this.components);
            this.lblTotalSheet = new System.Windows.Forms.Label();
            this.chkbxAutoEnd = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSplitSheet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsSplitSheet)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.ForestGreen;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnSave.ForeColor = System.Drawing.Color.LightGray;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.Location = new System.Drawing.Point(463, 609);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(135, 53);
            this.btnSave.TabIndex = 74;
            this.btnSave.Tag = "";
            this.btnSave.Text = "Save";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblJobDetail
            // 
            this.lblJobDetail.AutoSize = true;
            this.lblJobDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lblJobDetail.Location = new System.Drawing.Point(16, 11);
            this.lblJobDetail.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblJobDetail.Name = "lblJobDetail";
            this.lblJobDetail.Size = new System.Drawing.Size(204, 20);
            this.lblJobDetail.TabIndex = 75;
            this.lblJobDetail.Text = "Material No.: xxxxxxxxx";
            // 
            // dgvSplitSheet
            // 
            this.dgvSplitSheet.AllowUserToAddRows = false;
            this.dgvSplitSheet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSplitSheet.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSplitSheet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSplitSheet.Location = new System.Drawing.Point(15, 34);
            this.dgvSplitSheet.Margin = new System.Windows.Forms.Padding(4);
            this.dgvSplitSheet.Name = "dgvSplitSheet";
            this.dgvSplitSheet.RowHeadersWidth = 51;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvSplitSheet.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSplitSheet.Size = new System.Drawing.Size(583, 561);
            this.dgvSplitSheet.TabIndex = 73;
            this.dgvSplitSheet.ColumnAdded += new System.Windows.Forms.DataGridViewColumnEventHandler(this.DgvSplitSheet_ColumnAdded);
            this.dgvSplitSheet.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvSplitSheet_EditingControlShowing);
            this.dgvSplitSheet.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.DgvSplitSheet_RowPrePaint);
            this.dgvSplitSheet.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DgvSplitSheet_KeyDown);
            // 
            // bdsSplitSheet
            // 
            this.bdsSplitSheet.AddingNew += new System.ComponentModel.AddingNewEventHandler(this.BindingSource1_AddingNew);
            this.bdsSplitSheet.CurrentItemChanged += new System.EventHandler(this.BindingSource1_CurrentItemChanged);
            // 
            // lblTotalSheet
            // 
            this.lblTotalSheet.AutoSize = true;
            this.lblTotalSheet.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lblTotalSheet.Location = new System.Drawing.Point(16, 611);
            this.lblTotalSheet.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotalSheet.Name = "lblTotalSheet";
            this.lblTotalSheet.Size = new System.Drawing.Size(111, 20);
            this.lblTotalSheet.TabIndex = 76;
            this.lblTotalSheet.Text = "Total Sheet:";
            // 
            // chkbxAutoEnd
            // 
            this.chkbxAutoEnd.AutoSize = true;
            this.chkbxAutoEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkbxAutoEnd.Location = new System.Drawing.Point(20, 638);
            this.chkbxAutoEnd.Name = "chkbxAutoEnd";
            this.chkbxAutoEnd.Size = new System.Drawing.Size(128, 24);
            this.chkbxAutoEnd.TabIndex = 77;
            this.chkbxAutoEnd.Text = "Auto End Job";
            this.chkbxAutoEnd.UseVisualStyleBackColor = true;
            // 
            // frmSplitSheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(612, 668);
            this.Controls.Add(this.chkbxAutoEnd);
            this.Controls.Add(this.lblTotalSheet);
            this.Controls.Add(this.dgvSplitSheet);
            this.Controls.Add(this.lblJobDetail);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmSplitSheet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Mutilple Order with common Material";
            this.Load += new System.EventHandler(this.frmSplitSheet_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSplitSheet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsSplitSheet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblJobDetail;
        private System.Windows.Forms.DataGridView dgvSplitSheet;
        private System.Windows.Forms.BindingSource bdsSplitSheet;
        private System.Windows.Forms.Label lblTotalSheet;
        private System.Windows.Forms.CheckBox chkbxAutoEnd;
    }
}
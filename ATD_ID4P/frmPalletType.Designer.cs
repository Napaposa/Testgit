namespace ATD_ID4P
{
    partial class frmPalletType
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPalletType));
            this.grpbxPalletType = new System.Windows.Forms.GroupBox();
            this.rdoNoPallet = new System.Windows.Forms.RadioButton();
            this.rdoPlasticPallet = new System.Windows.Forms.RadioButton();
            this.rdoWoodenPallet = new System.Windows.Forms.RadioButton();
            this.numPalletLength = new System.Windows.Forms.NumericUpDown();
            this.numPalletWidth = new System.Windows.Forms.NumericUpDown();
            this.lblPalletLength = new System.Windows.Forms.Label();
            this.lblPalletWidth = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.picPalletDim = new System.Windows.Forms.PictureBox();
            this.grpbxPalletDim = new System.Windows.Forms.GroupBox();
            this.numPalletHeight = new System.Windows.Forms.NumericUpDown();
            this.lblPalletHeight = new System.Windows.Forms.Label();
            this.grpbxPalletType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPalletLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPalletWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPalletDim)).BeginInit();
            this.grpbxPalletDim.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPalletHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // grpbxPalletType
            // 
            this.grpbxPalletType.Controls.Add(this.rdoNoPallet);
            this.grpbxPalletType.Controls.Add(this.rdoPlasticPallet);
            this.grpbxPalletType.Controls.Add(this.rdoWoodenPallet);
            this.grpbxPalletType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.grpbxPalletType.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.grpbxPalletType.Location = new System.Drawing.Point(16, 15);
            this.grpbxPalletType.Margin = new System.Windows.Forms.Padding(4);
            this.grpbxPalletType.Name = "grpbxPalletType";
            this.grpbxPalletType.Padding = new System.Windows.Forms.Padding(4);
            this.grpbxPalletType.Size = new System.Drawing.Size(554, 79);
            this.grpbxPalletType.TabIndex = 0;
            this.grpbxPalletType.TabStop = false;
            this.grpbxPalletType.Text = "Pallet Type";
            // 
            // rdoNoPallet
            // 
            this.rdoNoPallet.AutoSize = true;
            this.rdoNoPallet.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.rdoNoPallet.Location = new System.Drawing.Point(384, 34);
            this.rdoNoPallet.Margin = new System.Windows.Forms.Padding(4);
            this.rdoNoPallet.Name = "rdoNoPallet";
            this.rdoNoPallet.Size = new System.Drawing.Size(107, 24);
            this.rdoNoPallet.TabIndex = 2;
            this.rdoNoPallet.Text = "No Pallet";
            this.rdoNoPallet.UseVisualStyleBackColor = true;
            this.rdoNoPallet.CheckedChanged += new System.EventHandler(this.rdoNoPallet_CheckedChanged);
            // 
            // rdoPlasticPallet
            // 
            this.rdoPlasticPallet.AutoSize = true;
            this.rdoPlasticPallet.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.rdoPlasticPallet.Location = new System.Drawing.Point(206, 34);
            this.rdoPlasticPallet.Margin = new System.Windows.Forms.Padding(4);
            this.rdoPlasticPallet.Name = "rdoPlasticPallet";
            this.rdoPlasticPallet.Size = new System.Drawing.Size(88, 24);
            this.rdoPlasticPallet.TabIndex = 1;
            this.rdoPlasticPallet.Text = "Plastic";
            this.rdoPlasticPallet.UseVisualStyleBackColor = true;
            this.rdoPlasticPallet.CheckedChanged += new System.EventHandler(this.rdoPlasticPallet_CheckedChanged);
            // 
            // rdoWoodenPallet
            // 
            this.rdoWoodenPallet.AutoSize = true;
            this.rdoWoodenPallet.Checked = true;
            this.rdoWoodenPallet.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.rdoWoodenPallet.Location = new System.Drawing.Point(39, 34);
            this.rdoWoodenPallet.Margin = new System.Windows.Forms.Padding(4);
            this.rdoWoodenPallet.Name = "rdoWoodenPallet";
            this.rdoWoodenPallet.Size = new System.Drawing.Size(97, 24);
            this.rdoWoodenPallet.TabIndex = 0;
            this.rdoWoodenPallet.TabStop = true;
            this.rdoWoodenPallet.Text = "Wooden";
            this.rdoWoodenPallet.UseVisualStyleBackColor = true;
            this.rdoWoodenPallet.CheckedChanged += new System.EventHandler(this.rdoWoodenPallet_CheckedChanged);
            // 
            // numPalletLength
            // 
            this.numPalletLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.numPalletLength.Location = new System.Drawing.Point(269, 34);
            this.numPalletLength.Margin = new System.Windows.Forms.Padding(4);
            this.numPalletLength.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numPalletLength.Name = "numPalletLength";
            this.numPalletLength.Size = new System.Drawing.Size(77, 26);
            this.numPalletLength.TabIndex = 7;
            this.numPalletLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numPalletLength.Value = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            // 
            // numPalletWidth
            // 
            this.numPalletWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.numPalletWidth.Location = new System.Drawing.Point(95, 34);
            this.numPalletWidth.Margin = new System.Windows.Forms.Padding(4);
            this.numPalletWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numPalletWidth.Name = "numPalletWidth";
            this.numPalletWidth.Size = new System.Drawing.Size(77, 26);
            this.numPalletWidth.TabIndex = 9;
            this.numPalletWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numPalletWidth.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // lblPalletLength
            // 
            this.lblPalletLength.AutoSize = true;
            this.lblPalletLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lblPalletLength.Location = new System.Drawing.Point(201, 38);
            this.lblPalletLength.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPalletLength.Name = "lblPalletLength";
            this.lblPalletLength.Size = new System.Drawing.Size(66, 20);
            this.lblPalletLength.TabIndex = 5;
            this.lblPalletLength.Text = "Length";
            // 
            // lblPalletWidth
            // 
            this.lblPalletWidth.AutoSize = true;
            this.lblPalletWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lblPalletWidth.Location = new System.Drawing.Point(35, 38);
            this.lblPalletWidth.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPalletWidth.Name = "lblPalletWidth";
            this.lblPalletWidth.Size = new System.Drawing.Size(57, 20);
            this.lblPalletWidth.TabIndex = 3;
            this.lblPalletWidth.Text = "Width";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.ForestGreen;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnSave.ForeColor = System.Drawing.Color.LightGray;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.Location = new System.Drawing.Point(462, 203);
            this.btnSave.Margin = new System.Windows.Forms.Padding(2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(108, 52);
            this.btnSave.TabIndex = 70;
            this.btnSave.Tag = "";
            this.btnSave.Text = "SAVE";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // picPalletDim
            // 
            this.picPalletDim.Image = ((System.Drawing.Image)(resources.GetObject("picPalletDim.Image")));
            this.picPalletDim.Location = new System.Drawing.Point(16, 272);
            this.picPalletDim.Name = "picPalletDim";
            this.picPalletDim.Size = new System.Drawing.Size(554, 464);
            this.picPalletDim.TabIndex = 71;
            this.picPalletDim.TabStop = false;
            // 
            // grpbxPalletDim
            // 
            this.grpbxPalletDim.Controls.Add(this.numPalletLength);
            this.grpbxPalletDim.Controls.Add(this.numPalletHeight);
            this.grpbxPalletDim.Controls.Add(this.lblPalletWidth);
            this.grpbxPalletDim.Controls.Add(this.lblPalletLength);
            this.grpbxPalletDim.Controls.Add(this.lblPalletHeight);
            this.grpbxPalletDim.Controls.Add(this.numPalletWidth);
            this.grpbxPalletDim.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.grpbxPalletDim.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.grpbxPalletDim.Location = new System.Drawing.Point(17, 104);
            this.grpbxPalletDim.Margin = new System.Windows.Forms.Padding(4);
            this.grpbxPalletDim.Name = "grpbxPalletDim";
            this.grpbxPalletDim.Padding = new System.Windows.Forms.Padding(4);
            this.grpbxPalletDim.Size = new System.Drawing.Size(554, 79);
            this.grpbxPalletDim.TabIndex = 3;
            this.grpbxPalletDim.TabStop = false;
            this.grpbxPalletDim.Text = "Pallet Dimension";
            // 
            // numPalletHeight
            // 
            this.numPalletHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.numPalletHeight.Location = new System.Drawing.Point(445, 34);
            this.numPalletHeight.Margin = new System.Windows.Forms.Padding(4);
            this.numPalletHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numPalletHeight.Name = "numPalletHeight";
            this.numPalletHeight.Size = new System.Drawing.Size(77, 26);
            this.numPalletHeight.TabIndex = 8;
            this.numPalletHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numPalletHeight.Value = new decimal(new int[] {
            155,
            0,
            0,
            0});
            // 
            // lblPalletHeight
            // 
            this.lblPalletHeight.AutoSize = true;
            this.lblPalletHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lblPalletHeight.Location = new System.Drawing.Point(379, 36);
            this.lblPalletHeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPalletHeight.Name = "lblPalletHeight";
            this.lblPalletHeight.Size = new System.Drawing.Size(64, 20);
            this.lblPalletHeight.TabIndex = 5;
            this.lblPalletHeight.Text = "Height";
            // 
            // frmPalletType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(585, 751);
            this.Controls.Add(this.grpbxPalletDim);
            this.Controls.Add(this.picPalletDim);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpbxPalletType);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmPalletType";
            this.Text = "Pallet Type Setting";
            this.Load += new System.EventHandler(this.frmPalletType_Load);
            this.grpbxPalletType.ResumeLayout(false);
            this.grpbxPalletType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPalletLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPalletWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPalletDim)).EndInit();
            this.grpbxPalletDim.ResumeLayout(false);
            this.grpbxPalletDim.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPalletHeight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpbxPalletType;
        private System.Windows.Forms.Label lblPalletLength;
        private System.Windows.Forms.Label lblPalletWidth;
        private System.Windows.Forms.RadioButton rdoPlasticPallet;
        private System.Windows.Forms.RadioButton rdoWoodenPallet;
        private System.Windows.Forms.NumericUpDown numPalletLength;
        private System.Windows.Forms.NumericUpDown numPalletWidth;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.RadioButton rdoNoPallet;
        private System.Windows.Forms.PictureBox picPalletDim;
        private System.Windows.Forms.GroupBox grpbxPalletDim;
        private System.Windows.Forms.NumericUpDown numPalletHeight;
        private System.Windows.Forms.Label lblPalletHeight;
    }
}
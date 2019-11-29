namespace Vendo
{
    partial class frmCargaMasivaProd
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblCantReg = new System.Windows.Forms.Label();
            this.btnCargaraBD = new System.Windows.Forms.Button();
            this.btnCargar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 114);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1294, 563);
            this.dataGridView1.TabIndex = 1;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 85);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1294, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // lblCantReg
            // 
            this.lblCantReg.AutoSize = true;
            this.lblCantReg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(20)))), ((int)(((byte)(133)))));
            this.lblCantReg.Location = new System.Drawing.Point(26, 40);
            this.lblCantReg.Name = "lblCantReg";
            this.lblCantReg.Size = new System.Drawing.Size(0, 13);
            this.lblCantReg.TabIndex = 3;
            // 
            // btnCargaraBD
            // 
            this.btnCargaraBD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(135)))), ((int)(((byte)(214)))));
            this.btnCargaraBD.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnCargaraBD.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCargaraBD.FlatAppearance.BorderColor = System.Drawing.Color.Navy;
            this.btnCargaraBD.FlatAppearance.BorderSize = 2;
            this.btnCargaraBD.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Navy;
            this.btnCargaraBD.Image = global::Vendo.Properties.Resources.if_kservices_18068;
            this.btnCargaraBD.Location = new System.Drawing.Point(1233, 14);
            this.btnCargaraBD.Name = "btnCargaraBD";
            this.btnCargaraBD.Size = new System.Drawing.Size(73, 65);
            this.btnCargaraBD.TabIndex = 0;
            this.btnCargaraBD.UseVisualStyleBackColor = false;
            this.btnCargaraBD.Click += new System.EventHandler(this.btnCargaraBD_Click);
            // 
            // btnCargar
            // 
            this.btnCargar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(135)))), ((int)(((byte)(214)))));
            this.btnCargar.FlatAppearance.BorderColor = System.Drawing.Color.Navy;
            this.btnCargar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Navy;
            this.btnCargar.Image = global::Vendo.Properties.Resources.if_application_vnd_ms_excel_24744;
            this.btnCargar.Location = new System.Drawing.Point(1152, 14);
            this.btnCargar.Name = "btnCargar";
            this.btnCargar.Size = new System.Drawing.Size(75, 65);
            this.btnCargar.TabIndex = 0;
            this.btnCargar.UseVisualStyleBackColor = false;
            this.btnCargar.Click += new System.EventHandler(this.btnCargar_Click);
            // 
            // frmCargaMasivaProd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(178)))), ((int)(((byte)(229)))));
            this.ClientSize = new System.Drawing.Size(1318, 689);
            this.Controls.Add(this.lblCantReg);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnCargaraBD);
            this.Controls.Add(this.btnCargar);
            this.Name = "frmCargaMasivaProd";
            this.Text = "Carga Masiva de Productos";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCargar;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblCantReg;
        private System.Windows.Forms.Button btnCargaraBD;
    }
}
namespace Mitgliederverwaltung
{
    partial class FrmMain
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.Grid = new System.Windows.Forms.DataGridView();
            this.LboTelefon = new System.Windows.Forms.ListBox();
            this.LblNotiz = new System.Windows.Forms.Label();
            this.PnlNotiz = new System.Windows.Forms.Panel();
            this.LblCaptionName = new System.Windows.Forms.Label();
            this.LblCaptionNotiz = new System.Windows.Forms.Label();
            this.LblCaptionTelefon = new System.Windows.Forms.Label();
            this.LblInfo = new System.Windows.Forms.Label();
            this.LblInfoCount = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Grid)).BeginInit();
            this.PnlNotiz.SuspendLayout();
            this.SuspendLayout();
            // 
            // Grid
            // 
            this.Grid.AllowUserToAddRows = false;
            this.Grid.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Grid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.Grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.Grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Grid.DefaultCellStyle = dataGridViewCellStyle3;
            this.Grid.Location = new System.Drawing.Point(22, 55);
            this.Grid.MultiSelect = false;
            this.Grid.Name = "Grid";
            this.Grid.ReadOnly = true;
            this.Grid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.PeachPuff;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Grid.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.Grid.RowHeadersWidth = 24;
            this.Grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.Grid.Size = new System.Drawing.Size(789, 376);
            this.Grid.TabIndex = 2;
            // 
            // LboTelefon
            // 
            this.LboTelefon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LboTelefon.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LboTelefon.FormattingEnabled = true;
            this.LboTelefon.IntegralHeight = false;
            this.LboTelefon.ItemHeight = 16;
            this.LboTelefon.Location = new System.Drawing.Point(648, 460);
            this.LboTelefon.Name = "LboTelefon";
            this.LboTelefon.Size = new System.Drawing.Size(163, 84);
            this.LboTelefon.TabIndex = 3;
            // 
            // LblNotiz
            // 
            this.LblNotiz.AutoSize = true;
            this.LblNotiz.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblNotiz.Location = new System.Drawing.Point(0, 0);
            this.LblNotiz.Name = "LblNotiz";
            this.LblNotiz.Size = new System.Drawing.Size(0, 16);
            this.LblNotiz.TabIndex = 13;
            // 
            // PnlNotiz
            // 
            this.PnlNotiz.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PnlNotiz.AutoScroll = true;
            this.PnlNotiz.BackColor = System.Drawing.Color.White;
            this.PnlNotiz.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PnlNotiz.Controls.Add(this.LblNotiz);
            this.PnlNotiz.Location = new System.Drawing.Point(22, 461);
            this.PnlNotiz.Name = "PnlNotiz";
            this.PnlNotiz.Size = new System.Drawing.Size(620, 83);
            this.PnlNotiz.TabIndex = 14;
            // 
            // LblCaptionName
            // 
            this.LblCaptionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblCaptionName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCaptionName.Location = new System.Drawing.Point(22, 29);
            this.LblCaptionName.Name = "LblCaptionName";
            this.LblCaptionName.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.LblCaptionName.Size = new System.Drawing.Size(789, 24);
            this.LblCaptionName.TabIndex = 15;
            this.LblCaptionName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblCaptionNotiz
            // 
            this.LblCaptionNotiz.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblCaptionNotiz.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCaptionNotiz.Location = new System.Drawing.Point(22, 435);
            this.LblCaptionNotiz.Name = "LblCaptionNotiz";
            this.LblCaptionNotiz.Size = new System.Drawing.Size(620, 24);
            this.LblCaptionNotiz.TabIndex = 16;
            this.LblCaptionNotiz.Text = "Notiz:";
            this.LblCaptionNotiz.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblCaptionTelefon
            // 
            this.LblCaptionTelefon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LblCaptionTelefon.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCaptionTelefon.Location = new System.Drawing.Point(648, 434);
            this.LblCaptionTelefon.Name = "LblCaptionTelefon";
            this.LblCaptionTelefon.Size = new System.Drawing.Size(163, 24);
            this.LblCaptionTelefon.TabIndex = 17;
            this.LblCaptionTelefon.Text = "Telefon";
            this.LblCaptionTelefon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblInfo
            // 
            this.LblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblInfo.Location = new System.Drawing.Point(22, 547);
            this.LblInfo.Name = "LblInfo";
            this.LblInfo.Size = new System.Drawing.Size(620, 24);
            this.LblInfo.TabIndex = 18;
            this.LblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblInfoCount
            // 
            this.LblInfoCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LblInfoCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblInfoCount.Location = new System.Drawing.Point(648, 547);
            this.LblInfoCount.Name = "LblInfoCount";
            this.LblInfoCount.Size = new System.Drawing.Size(163, 24);
            this.LblInfoCount.TabIndex = 19;
            this.LblInfoCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(834, 585);
            this.Controls.Add(this.LblInfoCount);
            this.Controls.Add(this.LblInfo);
            this.Controls.Add(this.LblCaptionTelefon);
            this.Controls.Add(this.LblCaptionNotiz);
            this.Controls.Add(this.LblCaptionName);
            this.Controls.Add(this.PnlNotiz);
            this.Controls.Add(this.LboTelefon);
            this.Controls.Add(this.Grid);
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "FrmMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mitgliederverwaltung";
            ((System.ComponentModel.ISupportInitialize)(this.Grid)).EndInit();
            this.PnlNotiz.ResumeLayout(false);
            this.PnlNotiz.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView Grid;
        private System.Windows.Forms.ListBox LboTelefon;
        private System.Windows.Forms.Label LblNotiz;
        private System.Windows.Forms.Panel PnlNotiz;
        private System.Windows.Forms.Label LblCaptionName;
        private System.Windows.Forms.Label LblCaptionNotiz;
        private System.Windows.Forms.Label LblCaptionTelefon;
        private System.Windows.Forms.Label LblInfo;
        private System.Windows.Forms.Label LblInfoCount;
    }
}


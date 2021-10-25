
namespace FarCry6Czech
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.bSelectExe = new System.Windows.Forms.Button();
            this.tbGameExe = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bInstall = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bUninstall = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // bSelectExe
            // 
            this.bSelectExe.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bSelectExe.Location = new System.Drawing.Point(326, 51);
            this.bSelectExe.Name = "bSelectExe";
            this.bSelectExe.Size = new System.Drawing.Size(125, 32);
            this.bSelectExe.TabIndex = 0;
            this.bSelectExe.Text = "Procházet...";
            this.bSelectExe.UseVisualStyleBackColor = true;
            this.bSelectExe.Click += new System.EventHandler(this.bSelectExe_Click);
            // 
            // tbGameExe
            // 
            this.tbGameExe.Location = new System.Drawing.Point(6, 22);
            this.tbGameExe.Name = "tbGameExe";
            this.tbGameExe.Size = new System.Drawing.Size(445, 23);
            this.tbGameExe.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(453, 255);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(138, 279);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(205, 32);
            this.label1.TabIndex = 3;
            this.label1.Text = "Far Cry 6 Čeština";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(432, 60);
            this.label2.TabIndex = 4;
            this.label2.Text = "Příklady:\r\nC:\\Program Files (x86)\\Far Cry 6\\bin\\FarCry6.exe\r\nC:\\Program Files (x8" +
    "6)\\Ubisoft Game Launcher\\games\\Far Cry 6\\bin\\FarCry6.exe\r\nD:\\Games\\Far Cry 6\\bin" +
    "\\FarCry6.exe";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbGameExe);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.bSelectExe);
            this.groupBox1.Location = new System.Drawing.Point(483, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(457, 159);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "1. Krok - Vyber složku s hrou";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bInstall);
            this.groupBox2.Location = new System.Drawing.Point(483, 177);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(457, 100);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "2. Krok - Instalace";
            // 
            // bInstall
            // 
            this.bInstall.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bInstall.Location = new System.Drawing.Point(164, 36);
            this.bInstall.Name = "bInstall";
            this.bInstall.Size = new System.Drawing.Size(125, 32);
            this.bInstall.TabIndex = 7;
            this.bInstall.Text = "Instalovat";
            this.bInstall.UseVisualStyleBackColor = true;
            this.bInstall.Click += new System.EventHandler(this.bInstall_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.bUninstall);
            this.groupBox3.Location = new System.Drawing.Point(483, 283);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(457, 100);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "V případě oddinstalace";
            // 
            // bUninstall
            // 
            this.bUninstall.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bUninstall.Location = new System.Drawing.Point(164, 38);
            this.bUninstall.Name = "bUninstall";
            this.bUninstall.Size = new System.Drawing.Size(125, 32);
            this.bUninstall.TabIndex = 8;
            this.bUninstall.Text = "Odinstalovat";
            this.bUninstall.UseVisualStyleBackColor = true;
            this.bUninstall.Click += new System.EventHandler(this.bUninstall_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 321);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(443, 105);
            this.label3.TabIndex = 5;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(958, 465);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Far Cry 6 Čeština";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bSelectExe;
        private System.Windows.Forms.TextBox tbGameExe;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button bInstall;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button bUninstall;
        private System.Windows.Forms.Label label3;
    }
}


/* 
 * Far Cry 6 Czech
 * Copyright (C) 2020  Jakub Mareček (info@jakubmarecek.cz)
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Far Cry 6 Czech.  If not, see <https://www.gnu.org/licenses/>.
 */


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
            this.label4 = new System.Windows.Forms.Label();
            this.bInstall = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.bUninstall = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // bSelectExe
            // 
            this.bSelectExe.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bSelectExe.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.bSelectExe.Location = new System.Drawing.Point(340, 51);
            this.bSelectExe.Name = "bSelectExe";
            this.bSelectExe.Size = new System.Drawing.Size(111, 28);
            this.bSelectExe.TabIndex = 0;
            this.bSelectExe.Text = "Procházet...";
            this.bSelectExe.UseVisualStyleBackColor = true;
            this.bSelectExe.Click += new System.EventHandler(this.bSelectExe_Click);
            // 
            // tbGameExe
            // 
            this.tbGameExe.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tbGameExe.Location = new System.Drawing.Point(6, 22);
            this.tbGameExe.Name = "tbGameExe";
            this.tbGameExe.ReadOnly = true;
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
            this.label1.Location = new System.Drawing.Point(103, 278);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(274, 32);
            this.label1.TabIndex = 3;
            this.label1.Text = "Far Cry 6 Čeština v1.00";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(6, 82);
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
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.groupBox1.Location = new System.Drawing.Point(483, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(457, 154);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "1. Krok - Vyberte složku s hrou";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.bInstall);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.groupBox2.Location = new System.Drawing.Point(483, 172);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(457, 95);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "2. Krok - Instalace";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(6, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(284, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "Nyní stačí kliknout na tlačítko níže pro nainstalování.";
            // 
            // bInstall
            // 
            this.bInstall.Enabled = false;
            this.bInstall.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bInstall.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.bInstall.Location = new System.Drawing.Point(164, 49);
            this.bInstall.Name = "bInstall";
            this.bInstall.Size = new System.Drawing.Size(125, 32);
            this.bInstall.TabIndex = 7;
            this.bInstall.Text = "Instalovat";
            this.bInstall.UseVisualStyleBackColor = true;
            this.bInstall.Click += new System.EventHandler(this.bInstall_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.bUninstall);
            this.groupBox3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.groupBox3.Location = new System.Drawing.Point(483, 422);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(457, 100);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "V případě oddinstalace";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(6, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(410, 30);
            this.label5.TabIndex = 9;
            this.label5.Text = "Kliknutím na tlačítko níže oddinstalujete češtinu ze hry, herní soubory budou\r\nvr" +
    "áceny na svůj původní stav.";
            // 
            // bUninstall
            // 
            this.bUninstall.Enabled = false;
            this.bUninstall.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bUninstall.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.bUninstall.Location = new System.Drawing.Point(164, 52);
            this.bUninstall.Name = "bUninstall";
            this.bUninstall.Size = new System.Drawing.Size(125, 32);
            this.bUninstall.TabIndex = 8;
            this.bUninstall.Text = "Odinstalovat";
            this.bUninstall.UseVisualStyleBackColor = true;
            this.bUninstall.Click += new System.EventHandler(this.bUninstall_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.groupBox4.Location = new System.Drawing.Point(483, 273);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(457, 50);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "3. Krok - Hrát";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(6, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(230, 15);
            this.label6.TabIndex = 9;
            this.label6.Text = "Nyní je již vše připravené, můžete hrát hru.";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(109, 316);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(258, 15);
            this.linkLabel1.TabIndex = 9;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "https://prekladyher.eu/preklady/far-cry-6.1132/";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBox1.Location = new System.Drawing.Point(12, 342);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(453, 180);
            this.textBox1.TabIndex = 10;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(952, 534);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.groupBox4);
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
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
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
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TextBox textBox1;
    }
}


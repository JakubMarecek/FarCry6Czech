using System;
using System.IO;
using System.Windows.Forms;

namespace FarCry6Czech
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void bSelectExe_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Title = "Vyber složku s hrou";
            openFileDialog.Filter = "Far Cry 6|farcry6.exe";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                tbGameExe.Text = openFileDialog.FileName;
            }
        }

        private void bInstall_Click(object sender, EventArgs e)
        {
            if (tbGameExe.Text == "")
            {
                MessageBox.Show(this, "", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(tbGameExe.Text))
            {
                MessageBox.Show(this, "", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string patchPath = tbGameExe.Text.ToLower().Replace("bin\\farcry6.exe", "data_final\\pc\\patch");
            string patchFat = patchPath + ".fat";
            string patchDat = patchPath + ".dat";
            string patchRtroBak = patchPath + ".fat.rtrobak";

            if (!File.Exists(patchFat) && !File.Exists(patchDat))
            {
                // todo create clean patch files
            }

            if (File.Exists(patchRtroBak))
            {
                MessageBox.Show(this, "", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }







        }

        private void bUninstall_Click(object sender, EventArgs e)
        {

        }
    }
}

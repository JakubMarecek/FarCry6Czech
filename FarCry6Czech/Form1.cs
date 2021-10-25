using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Gibbed.IO;

namespace FarCry6Czech
{
    public partial class Form1 : Form
    {
        public void CreateFatBak(string datFilePath, string fatFilePath, string fatBakFilePath)
        {
            long datLen = new FileInfo(datFilePath).Length;
            byte[] fatOrig = File.ReadAllBytes(fatFilePath);

            FileStream bin = new FileStream(fatBakFilePath, FileMode.CreateNew);
            bin.WriteValueU64((ulong)datLen);
            bin.WriteValueU64((ulong)fatOrig.Length);
            bin.Write(fatOrig, 0, fatOrig.Length);
            bin.Close();
        }

        public void RestoreFatBak(string FatBak, string dat, string fat)
        {
            FileStream TFATStream = new FileStream(FatBak, FileMode.Open);

            ulong InstallpkgDatLength = TFATStream.ReadValueU64();
            ulong InstallpkgFatLength = TFATStream.ReadValueU64();

            byte[] origInstallpkgFat = TFATStream.ReadBytes((int)InstallpkgFatLength);

            FileStream outputInstallpkgDat = File.Open(dat, FileMode.Open);
            outputInstallpkgDat.Seek((long)InstallpkgDatLength, SeekOrigin.Begin);
            outputInstallpkgDat.SetLength((long)InstallpkgDatLength);
            outputInstallpkgDat.Flush();
            outputInstallpkgDat.Close();

            TFATStream.Close();

            File.Delete(fat);
            File.Delete(FatBak);
            File.WriteAllBytes(fat, origInstallpkgFat);
        }

        public void CreateNewFat(string fatFilePath, FileStream fatStream, SortedDictionary<ulong, FatEntry> Entries)
        {
            if (fatFilePath != null && File.Exists(fatFilePath))
                File.Delete(fatFilePath);

            int fatVer = 11;

            FileStream output;
            if (fatFilePath == null)
                output = fatStream;
            else
                output = File.Create(fatFilePath);

            output.WriteValueU32(0x46415432, 0);
            output.WriteValueS32(fatVer, 0);

            output.WriteByte(1);

            output.WriteByte(0);

            output.WriteValueU16(0);

            output.WriteValueS32(0, 0); // dwSubfatTotalEntryCount
            output.WriteValueS32(0, 0); // dwSubfatCount
            output.WriteValueS32(Entries.Count, 0);

            foreach (ulong entryE in Entries.Keys)
            {
                var fatEntry = Entries[entryE];

                if (fatVer == 11)
                {
                    uint dwHash = (uint)((fatEntry.NameHash & 0xFFFFFFFF00000000ul) >> 32);
                    uint dwHash2 = (uint)((fatEntry.NameHash & 0x00000000FFFFFFFFul) >> 0);

                    uint dwUncompressedSize = 0u;
                    dwUncompressedSize = (uint)((int)dwUncompressedSize | ((int)(fatEntry.UncompressedSize << 2) & -4));
                    dwUncompressedSize = (uint)((int)dwUncompressedSize | (int)((int)fatEntry.CompressionScheme & 3L));

                    uint dwUnresolvedOffset = (uint)(((fatEntry.Offset >> 4) & 0x7FFFFFFF8) >> 3);

                    uint dwCompressedSize = 0u;
                    dwCompressedSize = (uint)((int)dwCompressedSize | (int)((fatEntry.Offset >> 4) << 29));
                    dwCompressedSize |= (fatEntry.CompressedSize & 0x1FFFFFFF);

                    output.WriteValueU32(dwHash, 0);
                    output.WriteValueU32(dwHash2, 0);
                    output.WriteValueU32(dwUncompressedSize, 0);
                    output.WriteValueU32(dwUnresolvedOffset, 0);
                    output.WriteValueU32(dwCompressedSize, 0);
                }
            }

            output.WriteValueU32(0, 0);
            output.WriteValueU32(0, 0);
            output.Flush();
            output.Close();
        }

        public SortedDictionary<ulong, FatEntry> GetFatEntries(string fatFile, bool isRtroBak, out long datLength)
        {
            FileStream TFATStream = new FileStream(fatFile, FileMode.Open, FileAccess.Read);
            return GetFatEntriesStream(TFATStream, isRtroBak, out datLength);
        }
        public SortedDictionary<ulong, FatEntry> GetFatEntriesStream(FileStream TFATStream, bool isRtroBak, out long datLength)
        {
            datLength = 0;

            SortedDictionary<ulong, FatEntry> Entries = new SortedDictionary<ulong, FatEntry>();

            if (isRtroBak)
            {
                int bakMagic = TFATStream.ReadValueS32();
                datLength = TFATStream.ReadValueS64();
                long fatLength = TFATStream.ReadValueS64();

                if (bakMagic != fatBakHeader)
                {
                    Logging.Write("Wrong version of patch.fat.rtroBak.");
                    TFATStream.Dispose();
                    TFATStream.Close();
                    return null;
                }
            }

            int dwMagic = TFATStream.ReadValueS32();
            int dwVersion = TFATStream.ReadValueS32();
            int dwUnknown = TFATStream.ReadValueS32();

            int dwSubfatTotalEntryCount = 0;
            int dwSubfatCount = 0;
            if (dwVersion >= 9)
            {
                dwSubfatTotalEntryCount = TFATStream.ReadValueS32();
                dwSubfatCount = TFATStream.ReadValueS32();
            }

            int dwTotalFiles = TFATStream.ReadValueS32();

            if (dwMagic != 0x46415432)
            {
                Logging.Write("Invalid FAT Index file!");
                TFATStream.Dispose();
                TFATStream.Close();
                return null;
            }

            if (((CurrentGame == GameType.FarCry6) && dwVersion != 11) || ((CurrentGame == GameType.FarCry5 || CurrentGame == GameType.FarCryNewDawn) && dwVersion != 10) || ((CurrentGame == GameType.FarCry3 || CurrentGame == GameType.FarCry4) && dwVersion != 9))
            {
                Logging.Write("Invalid version of FAT Index file!");
                TFATStream.Dispose();
                TFATStream.Close();
                return null;
            }

            for (int i = 0; i < dwTotalFiles; i++)
            {
                FatEntry entry = GetFatEntriesDeserialize(TFATStream, dwVersion);
                Entries[entry.NameHash] = entry;
            }

            uint unknown1Count = TFATStream.ReadValueU32();
            if (unknown1Count > 0)
                throw new NotSupportedException();
            /*for (uint i = 0; i < unknown1Count; i++)
            {
                TFATReader.ReadBytes(16);
            }*/

            uint unknown2Count = TFATStream.ReadValueU32();
            for (uint i = 0; i < unknown2Count; i++)
            {
                TFATStream.ReadBytes(16);
            }

            // we support sub fats
            for (int i = 0; i < dwSubfatCount; i++)
            {
                uint subfatEntryCount = TFATStream.ReadValueU32();
                for (uint j = 0; j < subfatEntryCount; j++)
                {
                    FatEntry entry = GetFatEntriesDeserialize(TFATStream, dwVersion);
                    Entries[entry.NameHash] = entry;
                }
            }

            TFATStream.Dispose();
            TFATStream.Close();

            return Entries;
        }

        static FatEntry GetFatEntriesDeserialize(FileStream TFATStream, int dwVersion)
        {
            ulong dwHash = TFATStream.ReadValueU64();
            dwHash = (dwHash << 32) + (dwHash >> 32);

            uint dwUncompressedSize = TFATStream.ReadValueU32();
            uint dwUnresolvedOffset = TFATStream.ReadValueU32();
            uint dwCompressedSize = TFATStream.ReadValueU32();

            uint dwFlag = 0;
            ulong dwOffset = 0;

            if (dwVersion == 11)
            {
                dwFlag = dwUncompressedSize & 3;
                dwOffset = ((ulong)dwCompressedSize >> 29 | (ulong)dwUnresolvedOffset << 3) << 4; // thx to ミルクティー (miru)
                dwCompressedSize = (dwCompressedSize & 0x1FFFFFFF);
                dwUncompressedSize = (dwUncompressedSize >> 2);
            }
            if (dwVersion == 10)
            {
                dwFlag = dwUncompressedSize & 3;
                dwOffset = dwCompressedSize >> 29 | 8ul * dwUnresolvedOffset;
                dwCompressedSize = (dwCompressedSize & 0x1FFFFFFF);
                dwUncompressedSize = (dwUncompressedSize >> 2);
            }
            if (dwVersion == 9)
            {
                dwFlag = (dwUncompressedSize & 0x00000003u) >> 0;
                dwOffset = (ulong)dwUnresolvedOffset << 2;
                dwOffset |= (dwCompressedSize & 0xC0000000u) >> 30;
                dwCompressedSize = (uint)((dwCompressedSize & 0x3FFFFFFFul) >> 0);
                dwUncompressedSize = (dwUncompressedSize & 0xFFFFFFFCu) >> 2;
            }

            var entry = new FatEntry();
            entry.NameHash = dwHash;
            entry.UncompressedSize = dwUncompressedSize;
            entry.Offset = (long)dwOffset;
            entry.CompressedSize = dwCompressedSize;
            entry.CompressionScheme = (CompressionScheme)dwFlag;

            return entry;
        }











        string bakFatFile = ".fat.bak";

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
            string patchBak = patchPath + bakFatFile;

            if (!File.Exists(patchFat) && !File.Exists(patchDat))
            {
                // todo create clean patch files
            }

            if (File.Exists(patchRtroBak))
            {
                MessageBox.Show(this, "", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (File.Exists(patchBak))
            {
                RestoreFatBak(patchBak, patchDat, patchFat);
            }
            else
            {
                CreateFatBak(patchDat, patchFat, patchBak);
            }

            // load fat here


            // write new files here


            // create new fat here



        }

        private void bUninstall_Click(object sender, EventArgs e)
        {

        }
    }
}

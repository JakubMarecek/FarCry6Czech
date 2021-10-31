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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Gibbed.Dunia2.FileFormats;
using Gibbed.IO;

namespace FarCry6Czech
{
    public partial class Form1 : Form
    {
        bool allow = false;
        const string ver = "1.00";
        const string appVer = "20211031-2200";
        const string logFile = "FarCry6Czech.log";
        string BaseDir = "";
        const string baseFile = "FarCry6Czech.zip";
        const string bakFatFile = ".fat.bak";
        string patchPath = "";
        string patchFat = "";
        string patchDat = "";
        string patchRtroBak = "";
        string patchBak = "";
        const string identifStr = "Far Cry 6 Czech ends here.";
        string readVer = "";
        FileToPack[] filesToPack = new FileToPack[]
        {
            new() { Source = "oasisstrings.oasis.bin", Target = @"languages\english\oasisstrings.oasis.bin" },
            new() { Source = "oasisstrings_subtitles.oasis.bin", Target = @"languages\english\oasisstrings_subtitles.oasis.bin" },
            new() { Source = "oasisstrings_subtitles_male.oasis.bin", Target = @"languages\english\oasisstrings_subtitles_male.oasis.bin" }
        };
        const string desc = @"Ubisoft se rozhodl Far Cry 6 nepřekládat, a tak nám nezbývá nic než hrát hru v angličině.
Proto jsme se rozhodli vytvořit vlastní český překlad.

= INSTALACE =
=============
Instalováním češtiny bude přepsán anglický jazyk ve hře, takže zvolte v Uplay / Epic jazyk English a ve hře zvolte anglické titulky.

Pokud instalujete update češtiny, stačí opět vybrat složku s hrou a poté kliknout na ""Instalovat"".


= KDO HRU PŘEKLÁDAL =
=====================
Johnny Cash
Jarek459
Sary
Reloader158CZ
mlekocze12

= INSTALÁTOR =
==============
ArmanIII

= TESTEŘI PŘEKLADU =
====================
Ajper
Fellras
Paras
Hugozlata
SpillCZ-Ripple
BoB
";

        public void CreateFatBak(string datFilePath, string fatFilePath, string fatBakFilePath)
        {
            long datLen = new FileInfo(datFilePath).Length;
            byte[] fatOrig = File.ReadAllBytes(fatFilePath);

            FileStream bin = new FileStream(fatBakFilePath, FileMode.CreateNew);
            bin.WriteValueS64(datLen);
            bin.WriteValueS64(fatOrig.Length);
            bin.Write(fatOrig, 0, fatOrig.Length);
            bin.Close();
        }

        public void RestoreFatBak(string FatBak, string dat, string fat)
        {
            FileStream TFATStream = new FileStream(FatBak, FileMode.Open);

            long InstallpkgDatLength = TFATStream.ReadValueS64();
            long InstallpkgFatLength = TFATStream.ReadValueS64();

            byte[] origInstallpkgFat = TFATStream.ReadBytes((int)InstallpkgFatLength);

            FileStream outputInstallpkgDat = File.Open(dat, FileMode.Open);
            outputInstallpkgDat.Seek(InstallpkgDatLength, SeekOrigin.Begin);
            outputInstallpkgDat.SetLength(InstallpkgDatLength);
            outputInstallpkgDat.Flush();
            outputInstallpkgDat.Close();

            TFATStream.Close();

            File.Delete(fat);
            File.Delete(FatBak);
            File.WriteAllBytes(fat, origInstallpkgFat);
        }

        public void CreateNewFat(string fatFilePath, SortedDictionary<ulong, FatEntry> Entries)
        {
            if (fatFilePath != null && File.Exists(fatFilePath))
                File.Delete(fatFilePath);

            FileStream output = File.Create(fatFilePath);

            output.WriteValueU32(0x46415432, 0);
            output.WriteValueS32(11, 0);

            output.WriteByte(1);

            output.WriteByte(0);

            output.WriteValueU16(0);

            output.WriteValueS32(0, 0); // dwSubfatTotalEntryCount
            output.WriteValueS32(0, 0); // dwSubfatCount
            output.WriteValueS32(Entries.Count, 0);

            foreach (ulong entryE in Entries.Keys)
            {
                var fatEntry = Entries[entryE];

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

            output.WriteValueU32(0, 0);
            output.WriteValueU32(0, 0);
            output.Flush();
            output.Close();
        }

        public SortedDictionary<ulong, FatEntry> GetFatEntries(string fatFile)
        {
            FileStream TFATStream = new FileStream(fatFile, FileMode.Open, FileAccess.Read);

            SortedDictionary<ulong, FatEntry> Entries = new SortedDictionary<ulong, FatEntry>();

            int dwMagic = TFATStream.ReadValueS32();
            int dwVersion = TFATStream.ReadValueS32();
            int dwUnknown = TFATStream.ReadValueS32();

            int dwSubfatTotalEntryCount = TFATStream.ReadValueS32();
            int dwSubfatCount = TFATStream.ReadValueS32();

            int dwTotalFiles = TFATStream.ReadValueS32();

            if (dwMagic != 0x46415432)
            {
                TFATStream.Dispose();
                TFATStream.Close();
                return null;
            }

            if (dwVersion != 11)
            {
                TFATStream.Dispose();
                TFATStream.Close();
                return null;
            }

            for (int i = 0; i < dwTotalFiles; i++)
            {
                FatEntry entry = GetFatEntriesDeserialize(TFATStream);
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
                    FatEntry entry = GetFatEntriesDeserialize(TFATStream);
                    Entries[entry.NameHash] = entry;
                }
            }

            TFATStream.Dispose();
            TFATStream.Close();

            return Entries;
        }

        static FatEntry GetFatEntriesDeserialize(FileStream TFATStream)
        {
            ulong dwHash = TFATStream.ReadValueU64();
            dwHash = (dwHash << 32) + (dwHash >> 32);

            uint dwUncompressedSize = TFATStream.ReadValueU32();
            uint dwUnresolvedOffset = TFATStream.ReadValueU32();
            uint dwCompressedSize = TFATStream.ReadValueU32();

            uint dwFlag;
            ulong dwOffset;

            dwFlag = dwUncompressedSize & 3;
            dwOffset = ((ulong)dwCompressedSize >> 29 | (ulong)dwUnresolvedOffset << 3) << 4; // thx to ミルクティー (miru)
            dwCompressedSize = (dwCompressedSize & 0x1FFFFFFF);
            dwUncompressedSize = (dwUncompressedSize >> 2);

            var entry = new FatEntry();
            entry.NameHash = dwHash;
            entry.UncompressedSize = dwUncompressedSize;
            entry.Offset = (long)dwOffset;
            entry.CompressedSize = dwCompressedSize;
            entry.CompressionScheme = (CompressionScheme)dwFlag;

            return entry;
        }

        private void SelectGameExe()
        {
            tbGameExe.Text = "";
            allow = false;
            bInstall.Enabled = false;
            bUninstall.Enabled = false;

            OpenFileDialog openFileDialog = new();
            openFileDialog.Title = "Vyber složku s hrou";
            openFileDialog.Filter = "Far Cry 6|farcry6.exe";
            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sel = openFileDialog.FileName;

                if (sel == "")
                {
                    Write("Bad path: " + sel);
                    MessageBox.Show(this, "Vybraná cesta není správná.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SelectGameExe();
                    return;
                }

                if (!File.Exists(sel) || !sel.ToLower().EndsWith("bin\\farcry6.exe"))
                {
                    Write("Bad game path: " + sel);
                    MessageBox.Show(this, "Soubor s hrou neexistuje.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SelectGameExe();
                    return;
                }

                tbGameExe.Text = sel;
                patchPath = sel.ToLower().Replace("bin\\farcry6.exe", "data_final\\pc\\patch");
                patchFat = patchPath + ".fat";
                patchDat = patchPath + ".dat";
                patchRtroBak = patchPath + ".fat.rtrobak";
                patchBak = patchPath + bakFatFile;

                if (File.Exists(patchRtroBak))
                {
                    Write("Used Mod Installer: " + sel);
                    MessageBox.Show(this, "Vypadá to, že máte nainstalované módy pomocí Mod Installeru. Pokud chcete nainstalovat češtinu společně s módy, použijte přímo A3 balíček s češtinou v Mod Installeru.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                allow = true;
                bInstall.Enabled = true;
                bUninstall.Enabled = true;

                Write("Selected path: " + sel);

                if (File.Exists(patchDat))
                {
                    FileStream outputDat = File.Open(patchDat, FileMode.Open);
                    outputDat.Seek(outputDat.Length, SeekOrigin.Begin);

                    if (outputDat.Length > 30)
                    {
                        outputDat.Seek(outputDat.Length - 32, SeekOrigin.Begin);
                        string identif = outputDat.ReadStringZ(Encoding.ASCII);
                        readVer = outputDat.ReadStringZ(Encoding.ASCII);

                        InstallStatus(identif == identifStr);
                    }
                    else
                        InstallStatus(false);

                    outputDat.Close();
                }
                else
                    InstallStatus(false);
            }
        }

        void InstallStatus(bool stat)
        {
            if (stat)
            {
                if (readVer != "")
                    lStatus.Text = $"Čeština verze {readVer} je nainstalována.";
                else
                    lStatus.Text = "Čeština je nainstalována.";
                lStatus.ForeColor = Color.Green;
            }
            else
            {
                lStatus.Text = "Čeština není nainstalována.";
                lStatus.ForeColor = Color.Red;
            }
        }

        private void Write(string data)
        {
            File.AppendAllText(logFile, DateTime.Now.ToString("HH:mm:ss") + " -> " + data + Environment.NewLine);
        }

        private ZipArchive GetBaseFromResourceData()
        {
            Assembly assembly = GetType().Assembly;
            Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + baseFile);

            ZipArchive zipArchive = new(stream);

            return zipArchive;
        }

        struct FileToPack
        {
            public string Source { set; get; }

            public string Target { set; get; }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using var processModule = Process.GetCurrentProcess().MainModule;
            BaseDir = Path.GetDirectoryName(processModule?.FileName) + "\\";
            Write("Far Cry 6 Czech v" + ver);
            Write("Started.");
            Text += " v" + ver;
            label1.Text = "Far Cry 6 Čeština v" + ver;
            textBox1.Text = desc;
            lAppVer.Text = "Verze app: " + appVer;
            lStatus.Text = "";
        }

        private void bSelectExe_Click(object sender, EventArgs e)
        {
            SelectGameExe();
        }

        private void bInstall_Click(object sender, EventArgs e)
        {
            if (!allow)
                return;

            try
            {
                Write("Starting install...");

                if (!File.Exists(patchFat) && !File.Exists(patchDat))
                {
                    File.WriteAllBytes(patchDat, new byte[] { });

                    byte[] emptyFat = new byte[]
                    {
                    0x32, 0x54, 0x41, 0x46, 0x0B, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                    };

                    File.WriteAllBytes(patchFat, emptyFat);

                    Write("Patch files didn't exist. Created.");
                }

                FileStream outputDat = File.Open(patchDat, FileMode.Open);
                outputDat.Seek(outputDat.Length, SeekOrigin.Begin);

                if (File.Exists(patchBak) && outputDat.Length > 30)
                {
                    outputDat.Seek(outputDat.Length - 32, SeekOrigin.Begin);
                    string identif = outputDat.ReadStringZ(Encoding.ASCII);
                    outputDat.Close();

                    if (identif == identifStr)
                    {
                        Write("Restoring FAT and DAT...");
                        RestoreFatBak(patchBak, patchDat, patchFat);
                        Write("FAT and DAT restored");
                    }
                    else
                        Write("Note in DAT doesn't exist.");
                }

                outputDat.Close();

                Write("Creating FAT bak...");
                CreateFatBak(patchDat, patchFat, patchBak);
                Write("FAT bak created.");

                Write("Loading FAT...");
                SortedDictionary<ulong, FatEntry> Entries = GetFatEntries(patchFat);
                Write("FAT loaded.");

                outputDat = File.Open(patchDat, FileMode.Open);
                outputDat.Seek(outputDat.Length, SeekOrigin.Begin);

                outputDat.WriteStringZ("Far Cry 6 Czech starts here.", Encoding.ASCII);
                outputDat.Seek(outputDat.Position.Align(16), SeekOrigin.Begin);

                Write("DAT opened, seek, begin to write...");

                foreach (FileToPack fileToPack in filesToPack)
                {
                    Write("Preparing to write " + fileToPack.Source + "...");

                    ZipArchive archive = null;
                    if (File.Exists(BaseDir + baseFile))
                        archive = ZipFile.OpenRead(BaseDir + baseFile);
                    else
                        archive = GetBaseFromResourceData();

                    ZipArchiveEntry zipEntry = archive.Entries.Where(e => e.Name.ToLowerInvariant() == fileToPack.Source.ToLowerInvariant()).FirstOrDefault();

                    Stream zipInput = zipEntry.Open();
                    var ms = new MemoryStream();
                    zipInput.CopyTo(ms);
                    byte[] bytes = ms.ToArray();

                    Write("Got " + fileToPack.Source + " from zip.");

                    ulong fileHash = CRC64.Hash(fileToPack.Target);

                    if (!Entries.TryGetValue(fileHash, out FatEntry entry))
                    {
                        entry = new FatEntry
                        {
                            NameHash = fileHash
                        };
                    }

                    entry.CompressionScheme = CompressionScheme.None;
                    entry.UncompressedSize = (uint)bytes.Length;
                    entry.CompressedSize = (uint)bytes.Length;

                    entry.Offset = outputDat.Position;
                    Entries[entry.NameHash] = entry;

                    outputDat.Write(bytes);
                    outputDat.Seek(outputDat.Position.Align(16), SeekOrigin.Begin);

                    Write("Wrote " + fileToPack.Source + ".");
                }

                outputDat.WriteStringZ(identifStr, Encoding.ASCII);
                outputDat.WriteStringZ(ver, Encoding.ASCII);

                Write("Flushing DAT...");
                outputDat.Flush();
                outputDat.Close();
                Write("DAT saved.");

                Write("Creating FAT...");
                CreateNewFat(patchFat, Entries);
                Write("FAT created.");

                Write("Install done.");

                readVer = ver;
                InstallStatus(true);
                MessageBox.Show(this, "Čeština byla úspěšně nainstalována.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                Write("Install error:");
                Write(ex.ToString());
                InstallStatus(false);
                bool yes = MessageBox.Show(this, $"Aj, někde se stala chyba. Zkuste to znovu, pokud chyba přetrvává, zeptejte se na fóru.{Environment.NewLine}Chcete přejít na fórum?{Environment.NewLine}{Environment.NewLine}{ex.Message}", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes;
                if (yes)
                    linkLabel1_LinkClicked(null, null);
            }
        }

        private void bUninstall_Click(object sender, EventArgs e)
        {
            if (!allow)
                return;

            bool res = MessageBox.Show(this, "Opravdu?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            if (res)
            {
                Write("Uninstalling...");

                FileStream outputDat = File.Open(patchDat, FileMode.Open);
                outputDat.Seek(outputDat.Length, SeekOrigin.Begin);

                if (File.Exists(patchBak) && outputDat.Length > 30)
                {
                    outputDat.Seek(outputDat.Length - 32, SeekOrigin.Begin);
                    string identif = outputDat.ReadStringZ(Encoding.ASCII);
                    outputDat.Close();

                    if (identif == identifStr)
                    {
                        Write("Restoring FAT and DAT...");
                        RestoreFatBak(patchBak, patchDat, patchFat);
                        Write("FAT and DAT restored");
                    }
                    else
                        Write("Note in DAT doesn't exist.");
                }

                outputDat.Close();

                Write("Uninstall done.");
                InstallStatus(false);
                MessageBox.Show(this, "Čeština byla úspěšně odinstalována.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var ps = new ProcessStartInfo("https://prekladyher.eu/preklady/far-cry-6.1132/")
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }
    }
}

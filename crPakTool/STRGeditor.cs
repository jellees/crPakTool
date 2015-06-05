using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using EndianLib;

namespace crPakTool
{
    public partial class STRGeditor : Form
    {
        public static byte[] finalCompressedStrg;

        public STRGeditor()
        {
            InitializeComponent();
        }

        public byte[] fixByteString(byte[] byteString)
        {
            int lastByte = byteString[byteString.Length - 1];

            if (lastByte != 0x00)
            {
                byte[] fixedByteString = new byte[byteString.Length + 1];
                Buffer.BlockCopy(byteString, 0, fixedByteString, 0, byteString.Length);

                return fixedByteString;
            }
            else
            {
                return byteString;
            }
        }

        public void LoadData(MemoryStream strgStream)
        {
            strgDGR.Rows.Clear();
            strgDGR.Columns.Clear();

            BinaryReader fileSTRG = new BinaryReader(strgStream);

            // Get the magic
            fileSTRG.BaseStream.Position = 0x0;
            uint magicStarter = Endian.SwapEndianU32(fileSTRG.ReadUInt32());

            if (magicStarter == 0x87654321)
            {
                // Get field count
                fileSTRG.BaseStream.Position = 0xC;
                uint fieldCount = Endian.SwapEndianU32(fileSTRG.ReadUInt32());

                if (fieldCount > 0)
                {
                    // Making arrays to use later on
                    uint[][] fieldNameInformation= new uint[fieldCount][];
                    string[] fieldNames = new string[fieldCount];
                    uint[,] stringInformation = new uint[10, 2];
                    string[,] strings = new string[fieldCount, 10];

                    if (fieldCount > 1)
                    {
                        MessageBox.Show("Multi-Field STRG's are not supported yet");
                        /*
                        // First we want the pointers and lengths of the names of the fields
                        for (int i = 0; i < fieldCount; i++)
                        {
                            fieldNameInformation[i][0] = Endian.SwapEndianU32(fileSTRG.ReadUInt32());
                            fieldNameInformation[i][1] = Endian.SwapEndianU32(fileSTRG.ReadUInt32());
                        }

                        // Then we loop through the names
                        for (int i = 0; i < fieldCount; i++)
                        {
                            // The names dont have a counter, thats why I use a loop
                            List<byte> stringCatcher = new List<byte>();
                            while (fileSTRG.ReadByte() != 0)
                            {
                                fileSTRG.BaseStream.Position -= 1;
                                stringCatcher.Add(fileSTRG.ReadByte());
                            }
                            fieldNames[i] = Encoding.ASCII.GetString(stringCatcher.ToArray());
                        }
                        
                        // skip the language magics
                        fileSTRG.BaseStream.Position += 0x28;*/
                    }
                    else
                    {
                        // skip the language magics
                        fileSTRG.BaseStream.Position += 0x30;

                        // Now get the string information
                        for (int i = 0; i < 10; i++)
                        {
                            stringInformation[i, 0] = Endian.SwapEndianU32(fileSTRG.ReadUInt32()); // String count
                            stringInformation[i, 1] = Endian.SwapEndianU32(fileSTRG.ReadUInt32()); // Relative offset
                        }

                        // Get strings
                        for (int i = 0; i < 10; i++)
                        {
                            fileSTRG.BaseStream.Position = 0x90 + stringInformation[i, 1];

                            uint stringLength = Endian.SwapEndianU32(fileSTRG.ReadUInt32());
                            byte[] stringbytes = fileSTRG.ReadBytes((int)stringLength);

                            // strings[0] <= is always 0 because there is only one field
                            strings[0, i] = Encoding.UTF8.GetString(stringbytes);
                        }

                        strgDGR.ColumnCount = 11;
                        strgDGR.Columns[0].Name = "Name";
                        strgDGR.Columns[1].Name = "English";
                        strgDGR.Columns[2].Name = "Japanese";
                        strgDGR.Columns[3].Name = "German";
                        strgDGR.Columns[4].Name = "French";
                        strgDGR.Columns[5].Name = "Spanish";
                        strgDGR.Columns[6].Name = "Italian";
                        strgDGR.Columns[7].Name = "Dutch";
                        strgDGR.Columns[8].Name = "Korean";
                        strgDGR.Columns[9].Name = "NA French";
                        strgDGR.Columns[10].Name = "NA Spanish";

                        strgDGR.Rows.Add("", strings[0, 0], strings[0, 1], strings[0, 2], strings[0, 3], strings[0, 4], strings[0, 5], strings[0, 6], strings[0, 7], strings[0, 8], strings[0, 9]);
                    }
                }
                else
                {
                    MessageBox.Show("No fields found");
                }
            }
            else
            {
                MessageBox.Show("Wrong STRG version. Got 0x" + magicStarter.ToString() + ", expected 0x87654321");
            }
        }

        // Cell painting thing that I do not understand but sure
        private void strgDGR_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.Value == null) return;
            var s = e.Graphics.MeasureString(e.Value.ToString(), strgDGR.Font);
            if (s.Width == strgDGR.Columns[e.ColumnIndex].Width)
            {
                using (Brush gridBrush = new SolidBrush(this.strgDGR.GridColor), backColorBrush = new SolidBrush(e.CellStyle.BackColor))
                {
                    using (Pen gridLinePen = new Pen(gridBrush))
                    {
                        e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
                        e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
                        e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
                        e.Graphics.DrawString(e.Value.ToString(), strgDGR.Font, Brushes.Black, e.CellBounds, StringFormat.GenericDefault);
                        strgDGR.Rows[e.RowIndex].Height = (int)(s.Height * Math.Ceiling(s.Width / strgDGR.Columns[e.ColumnIndex].Width));
                        e.Handled = true;
                    }
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Writing data into pakstream...";
            strgStatusStrip.Refresh();

            string[,] stringData = new string[strgDGR.Rows.Count, 11];

            for (int rows = 0; rows < strgDGR.Rows.Count; rows++)
            {
                for (int col = 0; col < 11; col++)
                {
                    stringData[rows, col] = strgDGR.Rows[rows].Cells[col].Value.ToString();
                }
            }

            if (stringData.Length < 12)
            {
                byte[] headerData = new byte[]{
                    0x87, 0x65, 0x43, 0x21, //Magic
                    0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x01, //HeaderData
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, //Name data unused
                    0x45, 0x4E, 0x47, 0x4C, 0x4A, 0x41, 0x50, 0x4E, 0x47, 0x45, 0x52, 0x4D, 0x46,
                    0x52, 0x45, 0x4E, 0x53, 0x50, 0x41, 0x4E, 0x49, 0x54, 0x41, 0x4C, 0x55, 0x4B, 
                    0x45, 0x4E, 0x4B, 0x4F, 0x52, 0x45, 0x4E, 0x41, 0x46, 0x52, 0x4E, 0x41, 0x53, 0x50 //Language Table
                };

                int bufferSize = 64;
                byte[][][] pointerTable = new byte[11][][];
                byte[][][] stringSection = new byte[11][][];
                
                // Write stringdata to byte array's
                for (int i = 1; i < stringData.Length; i++ )
                {
                    stringSection[i] = new byte[2][];
                    stringSection[i][0] = fixByteString(System.Text.Encoding.UTF8.GetBytes(stringData[0, i]));
                    stringSection[i][1] = BitConverter.GetBytes(Endian.SwapEndian32(stringSection[i][0].Length));
                    bufferSize += stringSection[i][0].Length + 4;
                }

                int pointerCounter = Endian.SwapEndian32(stringSection[1][0].Length);

                // write pointertable array
                for (int i = 1; i < stringData.Length; i++)
                {
                    pointerTable[i] = new byte[2][];
                    pointerTable[i][0] = BitConverter.GetBytes(Endian.SwapEndian32(stringSection[i][0].Length));
                    if (i == 1)
                    {
                        pointerCounter = 0;
                    }
                    else
                    {
                        pointerCounter += stringSection[i - 1][0].Length + 4;
                    }

                    pointerTable[i][1] = BitConverter.GetBytes(Endian.SwapEndian32(pointerCounter));
                    bufferSize += 8;
                }

                byte[] strgFile = new byte[bufferSize];
                int offsetCounter = 0;

                //making final file
                Buffer.BlockCopy(headerData, 0, strgFile, offsetCounter, headerData.Length);
                offsetCounter += headerData.Length;
                for (int i = 1; i < 11; i++)
                {
                    Buffer.BlockCopy(pointerTable[i][0], 0, strgFile, offsetCounter, 4);
                    offsetCounter += 4;
                    Buffer.BlockCopy(pointerTable[i][1], 0, strgFile, offsetCounter, 4);
                    offsetCounter += 4;
                }
                for (int i = 1; i < 11; i++)
                {
                    Buffer.BlockCopy(stringSection[i][1], 0, strgFile, offsetCounter, 4);
                    offsetCounter += 4;
                    Buffer.BlockCopy(stringSection[i][0], 0, strgFile, offsetCounter, stringSection[i][0].Length);
                    offsetCounter += stringSection[i][0].Length;
                }
                
                Form1 formClass = new Form1();
                ZlibCompression compress = new ZlibCompression();
                MemoryStream strgFileStr = new MemoryStream(strgFile);
                BinaryReader strgFileBin = new BinaryReader(strgFileStr);

                finalCompressedStrg = compress.Compress(strgFileBin);

                formClass.replaceFile(finalCompressedStrg);
                formClass.renewFloatingData();
                formClass.loadRSHDData();

                lblStatus.Text = "File Saved";
                strgStatusStrip.Refresh();

                /////////////////
                // For testing //
                /////////////////
                /*
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "STRG file|*.STRG";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(saveFileDialog1.FileName, strgFile);
                }*/
            }
        }

        // when changed something imidiatly update when mouse enters it
        private void strgDGR_MouseEnter(object sender, EventArgs e)
        {
            if (strgDGR.ColumnCount > 0)
            {
                if (englishToolStripMenuItem.Checked == true) strgDGR.Columns[1].Visible = true; else strgDGR.Columns[1].Visible = false;
                if (japaneseToolStripMenuItem.Checked == true) strgDGR.Columns[2].Visible = true; else strgDGR.Columns[2].Visible = false;
                if (germanToolStripMenuItem.Checked == true) strgDGR.Columns[3].Visible = true; else strgDGR.Columns[3].Visible = false;
                if (frenchToolStripMenuItem.Checked == true) strgDGR.Columns[4].Visible = true; else strgDGR.Columns[4].Visible = false;
                if (spanishToolStripMenuItem.Checked == true) strgDGR.Columns[5].Visible = true; else strgDGR.Columns[5].Visible = false;
                if (italianToolStripMenuItem.Checked == true) strgDGR.Columns[6].Visible = true; else strgDGR.Columns[6].Visible = false;
                if (uKEnglishToolStripMenuItem.Checked == true) strgDGR.Columns[7].Visible = true; else strgDGR.Columns[7].Visible = false;
                if (koreanToolStripMenuItem.Checked == true) strgDGR.Columns[8].Visible = true; else strgDGR.Columns[8].Visible = false;
                if (nAFrenchToolStripMenuItem.Checked == true) strgDGR.Columns[9].Visible = true; else strgDGR.Columns[9].Visible = false;
                if (nASpanishToolStripMenuItem.Checked == true) strgDGR.Columns[10].Visible = true; else strgDGR.Columns[10].Visible = false;
                strgDGR.Columns[0].Width = 50;
            }
        }

        // checks everything at the beginning
        private void STRGeditor_Load(object sender, EventArgs e)
        {
            if (strgDGR.ColumnCount > 0)
            {
                if (englishToolStripMenuItem.Checked == true) strgDGR.Columns[1].Visible = true; else strgDGR.Columns[1].Visible = false;
                if (japaneseToolStripMenuItem.Checked == true) strgDGR.Columns[2].Visible = true; else strgDGR.Columns[2].Visible = false;
                if (germanToolStripMenuItem.Checked == true) strgDGR.Columns[3].Visible = true; else strgDGR.Columns[3].Visible = false;
                if (frenchToolStripMenuItem.Checked == true) strgDGR.Columns[4].Visible = true; else strgDGR.Columns[4].Visible = false;
                if (spanishToolStripMenuItem.Checked == true) strgDGR.Columns[5].Visible = true; else strgDGR.Columns[5].Visible = false;
                if (italianToolStripMenuItem.Checked == true) strgDGR.Columns[6].Visible = true; else strgDGR.Columns[6].Visible = false;
                if (uKEnglishToolStripMenuItem.Checked == true) strgDGR.Columns[7].Visible = true; else strgDGR.Columns[7].Visible = false;
                if (koreanToolStripMenuItem.Checked == true) strgDGR.Columns[8].Visible = true; else strgDGR.Columns[8].Visible = false;
                if (nAFrenchToolStripMenuItem.Checked == true) strgDGR.Columns[9].Visible = true; else strgDGR.Columns[9].Visible = false;
                if (nASpanishToolStripMenuItem.Checked == true) strgDGR.Columns[10].Visible = true; else strgDGR.Columns[10].Visible = false;
                strgDGR.Columns[0].Width = 50;
            }
        }

        private void lblStatus_Click(object sender, EventArgs e)
        {

        }
    }
}

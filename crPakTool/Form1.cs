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
    public partial class Form1 : Form
    {
        FileStream readFileStream;
        static MemoryStream readMemoryStream;
        static List<List<uint>> rshdData = new List<List<uint>>();
        static uint pakVersion;
        static uint strgLength;
        static uint strgCount;
        static uint rshdLength;
        static uint rshdCount;
        static uint dataLength;
        static long hash;

        ZlibCompression Compress = new ZlibCompression(); //Zlib class reference

        static BinaryReader pakfile;
        static byte[] fileArray; //Changed data, Write this

        string filePath = null; //FileName litteraly
        string fileName = null; //FilePath litteraly

        static int reference; //Reference for node selected
        static int nodeLevel; //Which Level node is selected

        // For rshdData reference
        uint compressed;
        uint magic = 0;
        uint fileId1 = 0;
        uint fileId2 = 0;
        uint fileDataLength;
        uint pointer;

        static uint SwapEndianU32(uint unsignedInteger)
        {
            return ((unsignedInteger & 0x000000ff) << 24) +
                   ((unsignedInteger & 0x0000ff00) << 8) +
                   ((unsignedInteger & 0x00ff0000) >> 8) +
                   ((unsignedInteger & 0xff000000) >> 24);
        }

        static ulong SwapEndianU64(ulong unsignedInteger)
        {
            return (ulong)(((SwapEndianU32((uint)unsignedInteger) & 0xffffffffL) << 0x20) |
                            (SwapEndianU32((uint)(unsignedInteger >> 0x20)) & 0xffffffffL));

        }

        static string ConvertToAscii(uint input) //door peter
        {
            return Convert.ToChar(input >> 24) + "" + Convert.ToChar((input >> 16) & 0xFF) + Convert.ToChar((input >> 8) & 0xFF) + Convert.ToChar(input & 0xFF);
        }

        public Form1()
        {
            InitializeComponent();
        }

        // USE THIS TO REPLACE A FILE
        public void replaceFile(byte[] fileFinal)
        {
            int Position = reference;

            int oldFileLength = (int)rshdData[reference][4];
            int oldFileAddress = (int)(rshdData[reference][5] + 0x80 + strgLength + rshdLength);
            
            //Making First part of the main file
            pakfile.BaseStream.Position = 0x00;
            byte[] firstPart = pakfile.ReadBytes(oldFileAddress);

            //Making the second part of the main file, without the overwriting file of course
            pakfile.BaseStream.Position = oldFileAddress + oldFileLength;
            byte[] secondPart = pakfile.ReadBytes(((int)pakfile.BaseStream.Length) - (oldFileAddress + oldFileLength));

            fileArray = new byte[firstPart.Length + secondPart.Length + fileFinal.Length];

            if (fileFinal.Length == oldFileLength)
            {
                Buffer.BlockCopy(firstPart, 0, fileArray, 0, firstPart.Length);
                Buffer.BlockCopy(fileFinal, 0, fileArray, firstPart.Length, fileFinal.Length);
                Buffer.BlockCopy(secondPart, 0, fileArray, firstPart.Length + fileFinal.Length, secondPart.Length);
            }
            else if (fileFinal.Length < oldFileLength)
            {
                uint som = (uint)(oldFileLength - fileFinal.Length);

                //Making the array
                Buffer.BlockCopy(firstPart, 0, fileArray, 0, firstPart.Length);
                Buffer.BlockCopy(fileFinal, 0, fileArray, firstPart.Length, fileFinal.Length);
                Buffer.BlockCopy(secondPart, 0, fileArray, firstPart.Length + fileFinal.Length, secondPart.Length);

                //changing the DATA length integer
                uint DataLengthChange = BitConverter.ToUInt32(fileArray, 88);
                DataLengthChange = SwapEndianU32(DataLengthChange) - som;
                byte[] DataLengthChangeArray = BitConverter.GetBytes(DataLengthChange);
                Array.Reverse(DataLengthChangeArray);
                for (int i = 0; i < 4; i++)
                {
                    fileArray[i + 88] = DataLengthChangeArray[i];
                }

                //changing filesize of file
                uint fileSize = BitConverter.ToUInt32(fileArray, (int)(0x7C + strgLength) + (24 * (Position + 1)));
                fileSize = SwapEndianU32(fileSize) - som;
                byte[] fileSizeArray = BitConverter.GetBytes(fileSize);
                Array.Reverse(fileSizeArray);
                for (int i = 0; i < 4; i++)
                {
                    fileArray[i + ((int)(0x7C + strgLength) + (24 * (Position + 1)))] = fileSizeArray[i];
                }

                Position++;

                //Changing all the pointers
                for (int i = Position; i < rshdCount; i++)
                {
                    uint oldPointer = BitConverter.ToUInt32(fileArray, (int)(0x80 + strgLength) + (24 * (Position + 1)));
                    oldPointer = SwapEndianU32(oldPointer) - som;
                    byte[] pointerChange = BitConverter.GetBytes(oldPointer);
                    Array.Reverse(pointerChange);

                    fileArray[(int)(0x80 + strgLength) + (24 * (Position + 1))] = pointerChange[0];
                    fileArray[(int)(0x80 + strgLength) + (24 * (Position + 1)) + 1] = pointerChange[1];
                    fileArray[(int)(0x80 + strgLength) + (24 * (Position + 1)) + 2] = pointerChange[2];
                    fileArray[(int)(0x80 + strgLength) + (24 * (Position + 1)) + 3] = pointerChange[3];

                    Position++;
                }
            }
            else if (fileFinal.Length > oldFileLength)
            {
                uint som = (uint)(fileFinal.Length - oldFileLength);

                //Making the array
                Buffer.BlockCopy(firstPart, 0, fileArray, 0, firstPart.Length);
                Buffer.BlockCopy(fileFinal, 0, fileArray, firstPart.Length, fileFinal.Length);
                Buffer.BlockCopy(secondPart, 0, fileArray, firstPart.Length + fileFinal.Length, secondPart.Length);

                //changing the DATA length integer at the beginning of the pak
                uint DataLengthChange = BitConverter.ToUInt32(fileArray, 88);
                DataLengthChange = SwapEndianU32(DataLengthChange) + som;
                byte[] DataLengthChangeArray = BitConverter.GetBytes(DataLengthChange);
                Array.Reverse(DataLengthChangeArray);
                for (int i = 0; i < 4; i++)
                {
                    fileArray[i + 88] = DataLengthChangeArray[i];
                }

                //changing filesize of replaced file in the strg list
                uint fileSize = BitConverter.ToUInt32(fileArray, (int)(0x7C + strgLength) + (24 * (Position + 1)));
                fileSize = SwapEndianU32(fileSize) + som;
                byte[] fileSizeArray = BitConverter.GetBytes(fileSize);
                Array.Reverse(fileSizeArray);
                for (int i = 0; i < 4; i++)
                {
                    fileArray[i + ((int)(0x7C + strgLength) + (24 * (Position + 1)))] = fileSizeArray[i];
                }

                Position++;

                //Changing all the pointers
                for (int i = Position; i < rshdCount; i++)
                {
                    //int thing = (int)(0x80 + strgLength) + (24 * (Position + 1));
                    uint oldPointer = BitConverter.ToUInt32(fileArray, (int)(0x80 + strgLength) + (24 * (Position + 1)));
                    oldPointer = SwapEndianU32(oldPointer) + som;
                    byte[] pointerChange = BitConverter.GetBytes(oldPointer);
                    Array.Reverse(pointerChange);

                    fileArray[(int)(0x80 + strgLength) + (24 * (Position + 1))] = pointerChange[0];
                    fileArray[(int)(0x80 + strgLength) + (24 * (Position + 1)) + 1] = pointerChange[1];
                    fileArray[(int)(0x80 + strgLength) + (24 * (Position + 1)) + 2] = pointerChange[2];
                    fileArray[(int)(0x80 + strgLength) + (24 * (Position + 1)) + 3] = pointerChange[3];

                    Position++;
                }
            }

            pakfile.Close();
        }

        // USE THIS TO LOAD RSHDDATA FROM BINARYREADER PAKFILE
        public void loadRSHDData()
        {
            rshdData.Clear();

            pakfile.BaseStream.Position = 0x0;
            pakVersion = SwapEndianU32(pakfile.ReadUInt32());
            pakfile.BaseStream.Position = 0x8;
            hash = Endian.SwapEndian64(pakfile.ReadInt64());

            pakfile.BaseStream.Position = 0x48;
            strgLength = SwapEndianU32(pakfile.ReadUInt32());
            pakfile.BaseStream.Position = 0x50;
            rshdLength = SwapEndianU32(pakfile.ReadUInt32());
            pakfile.BaseStream.Position = 0x58;
            dataLength = SwapEndianU32(pakfile.ReadUInt32());

            pakfile.BaseStream.Position = 0x80;
            strgCount = SwapEndianU32(pakfile.ReadUInt32());

            pakfile.BaseStream.Position = 0x80 + strgLength;
            rshdCount = SwapEndianU32(pakfile.ReadUInt32());

            txtPakversion.Text = pakVersion.ToString("X2");
            txtRSHDsize.Text = rshdCount.ToString(); //File Count
            txtDATAsize.Text = "0x" + dataLength.ToString("X2");

            for (int i = 0; i < rshdCount; i++)
            {
                compressed = SwapEndianU32(pakfile.ReadUInt32());
                magic = SwapEndianU32(pakfile.ReadUInt32());
                fileId1 = SwapEndianU32(pakfile.ReadUInt32());
                fileId2 = SwapEndianU32(pakfile.ReadUInt32());
                fileDataLength = SwapEndianU32(pakfile.ReadUInt32());
                pointer = SwapEndianU32(pakfile.ReadUInt32());

                rshdData.Add(new List<uint> { compressed, magic, fileId1, fileId2, fileDataLength, pointer });
            }
        }

        // USE THIS TO RENEW THE FLOATING DATA
        public void renewFloatingData()
        {
            readMemoryStream = new MemoryStream(fileArray);
            pakfile = new BinaryReader(readMemoryStream);
        }

        // USE THIS TO LOAD A PAK FILE 
        public void fileLoad()
        {
            ObjectName objectNameClass = new ObjectName();

            //clearing stuff
            rshdData.Clear();
            treeView1.Nodes.Clear();

            // For user to know that it is loading
            toolStripStatusLabel1.Text = string.Format("Loading...");
            statusStrip1.Refresh();

            loadRSHDData();

            objectNameClass.loadObjectFile(hash);

            TreeNode treeNode;
            treeNode = treeView1.Nodes.Add("Root", fileName);

            //Booleans for knowing if the folder already exist
            bool txtr = true,
                misc = true,
                strg = true,
                fsmc = true,
                part = true,
                cmdl = true,
                anim = true,
                cinf = true,
                caud = true,
                csmp = true,
                swhc = true,
                ggar = true,
                cskr = true,
                dcln = true,
                frme = true,
                mlvl = true,
                savw = true,
                rule = true,
                font = true;

            #region Creating Folders
            for (int i = 0; i < rshdCount; i++)
            {
                uint magicCheck = rshdData[i][1];

                if (magicCheck == 0x54585452 && txtr == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("TXTRnode", "TXTR" + " - Texture Files");
                    txtr = false;
                }
                else if (magicCheck == 0x53545247 && strg == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("STRGnode", "STRG" + " - String text Files");
                    strg = false;
                }
                else if (magicCheck == 0x46534D43 && fsmc == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("FSMCnode", "FSMC");
                    fsmc = false;
                }
                else if (magicCheck == 0x50415254 && part == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("PARTnode", "PART" + " - Particle Script Files");
                    part = false;
                }
                else if (magicCheck == 0x434D444C && cmdl == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("CMDLnode", "CMDL" + " - Model Files");
                    cmdl = false;
                }
                else if (magicCheck == 0x414E494D && anim == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("ANIMnode", "ANIM" + " - Animation Files (Animations)");
                    anim = false;
                }
                else if (magicCheck == 0x43494E46 && cinf == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("CINFnode", "CINF" + " - Skeleton Files (Animations)");
                    cinf = false;
                }
                else if (magicCheck == 0x43415544 && caud == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("CAUDnode", "CAUD");
                    caud = false;
                }
                else if (magicCheck == 0x43534D50 && csmp == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("CSMPnode", "CSMP" + " - Audio Files");
                    csmp = false;
                }
                else if (magicCheck == 0x53574843 && swhc == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("SWHCnode", "SWHC" + " - Swoosh Particle Data Files");
                    swhc = false;
                }
                else if (magicCheck == 0x43484152 && ggar == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("CHARnode", "CHAR");
                    ggar = false;
                }
                else if (magicCheck == 0x43534B52 && cskr == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("CSKRnode", "CSKR" + " - Skin Binding Files (Animations)");
                    cskr = false;
                }
                else if (magicCheck == 0x44434C4E && dcln == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("DCLNnode", "DCLN");
                    dcln = false;
                }
                else if (magicCheck == 0x46524D45 && frme == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("FRMEnode", "FRME" + " - Frame Data Files");
                    frme = false;
                }
                else if (magicCheck == 0x4D4C564C && mlvl == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("MLVLnode", "MLVL" + "  ");
                    mlvl = false;
                }
                else if (magicCheck == 0x53415657 && savw == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("SAVWnode", "SAVW" + "  ");
                    savw = false;
                }
                else if (magicCheck == 0x52554C45 && rule == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("RULEnode", "RULE" + "  ");
                    rule = false;
                }
                else if (magicCheck == 0x464F4E54 && font == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("FONTnode", "FONT" + "  ");
                    font = false;
                }
                else if (misc == true)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes.Add("MISCnode", "MISC");
                    misc = false;
                }
            }
            #endregion
            #region Creating File Nodes
            for (int i = 0; i < rshdCount; i++)
            {
                uint magicCheck = rshdData[i][1];

                if (magicCheck == 0x54585452)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["TXTRnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else if (magicCheck == 0x53545247)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["STRGnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else if (magicCheck == 0x46534D43)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["FSMCnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else if (magicCheck == 0x50415254)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["PARTnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else if (magicCheck == 0x434D444C)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["CMDLnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else if (magicCheck == 0x414E494D)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["ANIMnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else if (magicCheck == 0x43494E46)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["CINFnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else if (magicCheck == 0x43415544)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["CAUDnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else if (magicCheck == 0x43534D50)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["CSMPnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else if (magicCheck == 0x53574843)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["SWHCnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else if (magicCheck == 0x43484152)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["CHARnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else if (magicCheck == 0x43534B52)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["CSKRnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else if (magicCheck == 0x44434C4E)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["DCLNnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else if (magicCheck == 0x46524D45)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["FRMEnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else if (magicCheck == 0x4D4C564C)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["MLVLnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else if (magicCheck == 0x53415657)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["SAVWnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else if (magicCheck == 0x52554C45)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["RULEnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else if (magicCheck == 0x464F4E54)
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["FONTnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
                else
                {
                    treeNode = treeView1.Nodes["Root"].Nodes["MISCnode"].Nodes.Add(i.ToString(), objectNameClass.giveObjectName(rshdData[i][2], rshdData[i][3], ConvertToAscii(rshdData[i][1])));
                }
            }
            #endregion

            treeView1.Nodes["Root"].Expand();

            // For user to know that it is ready
            toolStripStatusLabel1.Text = string.Format("Ready");
            statusStrip1.Refresh();
        }

        // USE THIS TO WRITE TO THE LOG 
        public string writeToLog
        {
            get { return txtLog.Text; }
            set { txtLog.AppendText(value); }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "DKCR Pak files|*.pak";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                fileName = Path.GetFileName(filePath);
            }
            
            try
            {
                readFileStream = new FileStream(filePath, FileMode.Open);
                pakfile = new BinaryReader(readFileStream);

                fileLoad();
            }
            catch
            {
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //MessageBox.Show(e.Node.Name);
            if (e.Node.Level == 2)
            {
                nodeLevel = e.Node.Level;
                reference = Convert.ToInt32(e.Node.Name);

                // Displaying "Edit STRG" button
                if (rshdData[reference][1] == 0x53545247)
                {
                    toolStripSeparator1.Visible = true;
                    editSTRGToolStripMenuItem.Visible = true;
                }

                txtFileReference.Text = (reference + 1).ToString();
                txtFileId.Text = rshdData[reference][2].ToString("X8") + rshdData[reference][3].ToString("X8");
                txtFileExtention.Text = ConvertToAscii(rshdData[reference][1]);
                if (rshdData[reference][0] == 1)
                {
                    txtCompression.Text = "Yes";
                }
                else if (rshdData[reference][0] == 0)
                {
                    txtCompression.Text = "No";
                }
                txtLength.Text = "0x" + rshdData[reference][4].ToString("X2");
                txtAddress.Text = "0x" + (rshdData[reference][5] + 0x80 + strgLength + rshdLength).ToString("X2");
            }
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nodeLevel == 2)
            {
                int length = (int)rshdData[reference][4];
                uint address = rshdData[reference][5] + 0x80 + strgLength + rshdLength;
                string bestandsNaam = rshdData[reference][2].ToString("X8") + rshdData[reference][3].ToString("X8");

                pakfile.BaseStream.Position = address;
                byte[] export = pakfile.ReadBytes(length);

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = ConvertToAscii(rshdData[reference][1]) + "File|*." + ConvertToAscii(rshdData[reference][1]);
                saveFileDialog1.FileName = bestandsNaam;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(saveFileDialog1.FileName, export);
                }
            }
        }

        private void exportDecompressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (readFileStream == null || reference == -1)
            {
                MessageBox.Show("No file Loaded");
            }
            else
            {
                string bestandsNaam = rshdData[reference][2].ToString("X8") + rshdData[reference][3].ToString("X8");

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = ConvertToAscii(rshdData[reference][1]) + "File|*." + ConvertToAscii(rshdData[reference][1]);
                saveFileDialog1.FileName = bestandsNaam;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(saveFileDialog1.FileName, Compress.ExportDecompress(pakfile, reference, rshdData, strgLength, rshdLength));
                }
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string bestandPath = null;
            byte[] fileFinal;
            
            FileStream readen;
            BinaryReader file;

            if (nodeLevel == 2)
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = ConvertToAscii(rshdData[reference][1]) + " File|*." + ConvertToAscii(rshdData[reference][1]);
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    bestandPath = openFileDialog1.FileName;
                }

                try
                {
                    readen = new FileStream(bestandPath, FileMode.Open);
                    file = new BinaryReader(readen);

                    uint magic = Endian.SwapEndianU32(file.ReadUInt32());

                    if (magic == 0x53415657 || magic == 0x4350524D || magic == 0x4D4C564C || magic == 0x4D524541 || magic == 0x5354524D || magic == 0x52554C45 || magic == 0x434D5044)
                    {
                        file.BaseStream.Position = 0x00;
                        fileFinal = file.ReadBytes((int)file.BaseStream.Length);

                        file.Close();
                    }
                    else
                    {
                        fileFinal = Compress.Compress(file);

                        file.Close();
                    }

                    replaceFile(fileFinal);

                    renewFloatingData();

                    loadRSHDData();
                }
                catch { }
            }
            else
            {
                MessageBox.Show("No File Selected");
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileArray == null)
            {
                MessageBox.Show("Nothing Changed Yet");
            }
            else
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = fileName + "|*.pak";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(saveFileDialog1.FileName, fileArray);
                }
            }
            
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileArray == null)
            {
                MessageBox.Show("Nothing Changed Yet");
            }
            else
            {
                File.WriteAllBytes(filePath, fileArray);

                toolStripStatusLabel1.Text = string.Format("File Saved");
                statusStrip1.Refresh();
            }
        }

        private void convertCMDLToOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ConvertVertGraphic graphicConvert = new ConvertVertGraphic();
            CMDL cmdl = new CMDL();
            FileStream CMDLtoOBJ;
            string CMDLtoOBJpath;

            MessageBox.Show("Only Vertex support");
            
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Citrus MoDeL|*.CMDL";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                CMDLtoOBJpath = openFileDialog1.FileName;

                if (CMDLtoOBJpath != null)
                {
                    CMDLtoOBJ = new FileStream(CMDLtoOBJpath, FileMode.Open);
                    BinaryReader CMDLtoOBJBin = new BinaryReader(CMDLtoOBJ);

                    //cmdl.getObjFromCmdl(CMDLtoOBJBin, this);
                    
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "Obj Wavefront|*.obj";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        //File.WriteAllLines(saveFileDialog1.FileName, graphicConvert.ExportObj(CMDLtoOBJBin));
                        File.WriteAllLines(saveFileDialog1.FileName, cmdl.getObjFromCmdl(CMDLtoOBJBin, this));
                    }
                }
            }
        }

        private void editSTRGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            STRGeditor strgEditor = new STRGeditor();
            
            MemoryStream strgStream = new MemoryStream(Compress.ExportDecompress(pakfile, reference, rshdData, strgLength, rshdLength));
            strgEditor.LoadData(strgStream);
            strgEditor.ShowDialog();
        }

        private void tXTRToPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "TXTR|*.TXTR";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                toolStripStatusLabel1.Text = string.Format("Writing PNG...");
                statusStrip1.Refresh();

                string filePathTXTR = openFileDialog1.FileName;

                FileStream txtrStream = new FileStream(filePathTXTR, FileMode.Open);
                BinaryReader txtrbinary = new BinaryReader(txtrStream);

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "|*.png";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    TXTR txtr = new TXTR();

                    txtr.TXTRtoPNG(txtrbinary, saveFileDialog1.FileName);

                    toolStripStatusLabel1.Text = string.Format("Finished");
                    statusStrip1.Refresh();
                }
            }
        }

        private void tXTRToDDSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "TXTR|*.TXTR";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                toolStripStatusLabel1.Text = string.Format("Writing DDS...");
                statusStrip1.Refresh();

                string filePathTXTR = openFileDialog1.FileName;

                FileStream txtrStream = new FileStream(filePathTXTR, FileMode.Open);
                BinaryReader txtrbinary = new BinaryReader(txtrStream);

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "|*.dds";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    TXTR txtr = new TXTR();

                    File.WriteAllBytes(saveFileDialog1.FileName, txtr.TXTRtoDDS(txtrbinary));

                    toolStripStatusLabel1.Text = string.Format("Finished");
                    statusStrip1.Refresh();
                }
            }
        }
    }
}

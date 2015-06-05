using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using EndianLib;

namespace crPakTool
{
    class CMDL
    {
        public uint getPadding(long position)
        {
            uint padding = (uint)position % 0x20;
            if (padding == 0)
            {
                return padding;
            }
            else
            {
                uint correctPadding = ((uint)position - padding) + 20;
                return correctPadding;
            }
        }

        public decimal readFixedFloat(int value)
        {
            /*while (value.Length < 0) value = "0" + value;

            char[] kar = new char[value.Length];
            for (int i = 0; i < i++) kar[i] = value[i];*/

            int intval = ((value & 0x3) << 6) + ((value & 0xC) << 2) + ((value & 0x30) >> 2) + ((value & 0xC0) >> 6);

            return intval;
        }

        public static Single swapSingle32(Single signedInteger) // Unsigned 32
        {
            return Convert.ToSingle(((Convert.ToInt32(signedInteger) & 0x000000ff) << 24) +
                   ((Convert.ToInt32(signedInteger) & 0x0000ff00) << 8) +
                   ((Convert.ToInt32(signedInteger) & 0x00ff0000) >> 8) +
                   ((Convert.ToInt32(signedInteger) & 0xff000000) >> 24));
        }

        public string[] getObjFromCmdl(BinaryReader cmdl, Form1 mainForm)
        {
            /*  Main Variables  */
            string[] visibilityGroupStrings;
            uint[] sectionSizes;
            uint[] submeshOffsets;
            uint[] vertexAttributeArray;
            ulong[] textureFileID;

            string[] vertexArray;
            string[] normalArray;
            string[] floatUVArray;
            string[] shortUVArray;
            List<string> faceArray = new List<string>();

            int currentSection = 0; //Which section its currently processing
            uint nextOffset = 0; //What the next offset is
            uint currentOffset = 0;

            /*  Header  */
            uint magic = Endian.SwapEndianU32(cmdl.ReadUInt32());
            uint flags = Endian.SwapEndianU32(cmdl.ReadUInt32());

            mainForm.writeToLog = "flags: 0x" + flags.ToString("X") + "\n";

            cmdl.BaseStream.Position += 0x18; //skipping boundry box values

            uint sectionCount = Endian.SwapEndianU32(cmdl.ReadUInt32());
            uint materialSetCount = Endian.SwapEndianU32(cmdl.ReadUInt32());

            //read visibility groups
            if ((flags & 0x10) == 0x10)
            {
                mainForm.writeToLog = "String Table Present\n";

                cmdl.BaseStream.Position += 4;
                uint stringCount = Endian.SwapEndianU32(cmdl.ReadUInt32());
                visibilityGroupStrings = new string[stringCount];

                for (int i = 0; i < stringCount; i++)
                {
                    int stringLength = Endian.SwapEndian32(cmdl.ReadInt32());
                    byte[] stringBytes = cmdl.ReadBytes(stringLength);
                    visibilityGroupStrings[i] = Encoding.UTF8.GetString(stringBytes); //Give this to console or use this in obj

                    mainForm.writeToLog = "\n" + visibilityGroupStrings[i].ToString();
                }

                cmdl.BaseStream.Position += 0x14;
            }

            mainForm.writeToLog = "\nSection Sizes:\n";

            //read Section sizes
            sectionSizes = new uint[sectionCount];
            for (int i = 0; i < sectionCount; i++)
            {
                sectionSizes[i] = Endian.SwapEndianU32(cmdl.ReadUInt32());

                mainForm.writeToLog = "Section " + i + ": " + sectionSizes[i].ToString() + "\n";
            }

            while ((cmdl.BaseStream.Position % 0x20) != 0) cmdl.BaseStream.Position += cmdl.BaseStream.Position % 0x20;

            /*  Materials   */
            currentOffset = (uint)cmdl.BaseStream.Position;
            nextOffset =  currentOffset + sectionSizes[currentSection];

            mainForm.writeToLog = "\nMaterials at offset: 0x" + currentOffset.ToString("X");

            uint materialCount = Endian.SwapEndianU32(cmdl.ReadUInt32());
            mainForm.writeToLog = "\nMaterial count:" + materialCount.ToString() + "\n\n";
            vertexAttributeArray = new uint[materialCount];
            textureFileID = new ulong[materialCount];

            for (int i = 0; i < materialCount; i++)
            {
                uint materialSize = Endian.SwapEndianU32(cmdl.ReadUInt32());
                currentOffset = (uint)cmdl.BaseStream.Position;
                uint materialEnd = currentOffset + materialSize;
                
                cmdl.BaseStream.Position += 0xC;
                vertexAttributeArray[i] = Endian.SwapEndianU32(cmdl.ReadUInt32());
                mainForm.writeToLog = "Material " + i + " vertex attribute flag: 0x" + vertexAttributeArray[i].ToString("X") + "\n";

                cmdl.BaseStream.Position += 0x1C;
                textureFileID[i] = Endian.SwapEndianU64(cmdl.ReadUInt64());
                mainForm.writeToLog = "Material file ID: " + textureFileID[i].ToString("X16") + "\n\n";

                cmdl.BaseStream.Position = materialEnd;
            }

            /*  Vertices    */
            cmdl.BaseStream.Position = nextOffset;
            currentOffset = (uint)cmdl.BaseStream.Position;
            currentSection += 1;
            nextOffset = currentOffset + sectionSizes[currentSection];

            bool shortPositions = (flags & 0x20) == 0x20;

            uint vertexCount;
            if (shortPositions == true) vertexCount = sectionSizes[currentSection] / 12;
            else vertexCount = sectionSizes[currentSection] / 8;

            vertexArray = new string[vertexCount];

            mainForm.writeToLog = "Reading vertex section...\nshorts:" + shortPositions.ToString() + "\n";

            for (int i = 0; i < vertexCount; i++)
            {
                if (shortPositions == false)
                {
                    byte[] xposar = BitConverter.GetBytes(cmdl.ReadInt32());
                    byte[] yposar = BitConverter.GetBytes(cmdl.ReadInt32());
                    byte[] zposar = BitConverter.GetBytes(cmdl.ReadInt32());
                    Array.Reverse(xposar);
                    Array.Reverse(yposar);
                    Array.Reverse(zposar);
                    Single xpos = BitConverter.ToSingle(xposar, 0);
                    Single ypos = BitConverter.ToSingle(yposar, 0);
                    Single zpos = BitConverter.ToSingle(zposar, 0);

                    vertexArray[i] = "v " + xpos.ToString("0.0000000", CultureInfo.InvariantCulture) + " " + ypos.ToString("0.000000", CultureInfo.InvariantCulture) + " " + zpos.ToString("0.000000", CultureInfo.InvariantCulture);
                }
                else
                {
                    float xpos = ((float)Endian.SwapEndian16(cmdl.ReadInt16())) / 0x2000;
                    float ypos = ((float)Endian.SwapEndian16(cmdl.ReadInt16())) / 0x2000;
                    float zpos = ((float)Endian.SwapEndian16(cmdl.ReadInt16())) / 0x2000;

                    vertexArray[i] = "v " + xpos.ToString("0.0000000", CultureInfo.InvariantCulture) + " " + ypos.ToString("0.0000000", CultureInfo.InvariantCulture) + " " + zpos.ToString("0.0000000", CultureInfo.InvariantCulture);
                }
            }

            mainForm.writeToLog = "Succesfully read the vertex section!\n\n";

            /*  vertex normals  */
            cmdl.BaseStream.Position = nextOffset;
            currentOffset = (uint)cmdl.BaseStream.Position;
            currentSection += 1;
            nextOffset = currentOffset + sectionSizes[currentSection];

            uint normalCount;
            normalCount = sectionSizes[currentSection] / 12;
            normalArray = new string[normalCount];

            for (int i = 0; i < normalCount; i++)
            {
                float xpos = ((float)Endian.SwapEndian16(cmdl.ReadInt16())) / 0x2000;
                float ypos = ((float)Endian.SwapEndian16(cmdl.ReadInt16())) / 0x2000;
                float zpos = ((float)Endian.SwapEndian16(cmdl.ReadInt16())) / 0x2000;

                normalArray[i] = "vn " + xpos.ToString("0.0000000", CultureInfo.InvariantCulture) + " " + ypos.ToString("0.0000000", CultureInfo.InvariantCulture) + " " + zpos.ToString("0.0000000", CultureInfo.InvariantCulture);
            }

            //Skipping this for now
            //mainForm.writeToLog = "Skipping vertex normals\n\n";

            /*  vertex Colors   */
            cmdl.BaseStream.Position = nextOffset;
            currentOffset = (uint)cmdl.BaseStream.Position;
            currentSection += 1;
            nextOffset = currentOffset + sectionSizes[currentSection];

            //Skipping vertex color because its most of the time empty
            mainForm.writeToLog = "Skipping vertex collors\n\n";

            /*  Float UV's  */
            cmdl.BaseStream.Position = nextOffset;
            currentOffset = (uint)cmdl.BaseStream.Position;
            currentSection += 1;
            nextOffset = currentOffset + sectionSizes[currentSection];

            mainForm.writeToLog = "Reading Float UV's...\n\n";

            uint floatUVCount = sectionSizes[currentSection] / 8;
            floatUVArray = new string[floatUVCount];
            for (int i = 0; i < floatUVCount; i++)
            {
                byte[] uar = BitConverter.GetBytes(cmdl.ReadInt32());
                byte[] var = BitConverter.GetBytes(cmdl.ReadInt32());
                Array.Reverse(uar);
                Array.Reverse(var);
                Single u = BitConverter.ToSingle(uar, 0);
                Single v = BitConverter.ToSingle(var, 0);

                floatUVArray[i] = "vt " + u.ToString("0.0000000", CultureInfo.InvariantCulture) + " " + v.ToString("0.0000000", CultureInfo.InvariantCulture);
            }

            mainForm.writeToLog = "Succesfully read Float UV's\n\n";

            /*  Short UV's  */
            cmdl.BaseStream.Position = nextOffset;
            currentOffset = (uint)cmdl.BaseStream.Position;
            currentSection += 1;
            nextOffset = currentOffset + sectionSizes[currentSection];

            mainForm.writeToLog = "Reading Short UV's...\n\n";

            uint shortUVCount = sectionSizes[currentSection] / 4;
            shortUVArray = new string[shortUVCount];

            for (int i = 0; i < shortUVCount; i++)
            {
                float u = ((float)Endian.SwapEndian16(cmdl.ReadInt16()));
                float v = ((float)Endian.SwapEndian16(cmdl.ReadInt16()));

                shortUVArray[i] = "vt " + u.ToString("0.0000000", CultureInfo.InvariantCulture) + " " + v.ToString("0.0000000", CultureInfo.InvariantCulture);
            }

            mainForm.writeToLog = "Succesfully read Short UV's...\n\n";

            /*  submesh count   */
            cmdl.BaseStream.Position = nextOffset;
            currentOffset = (uint)cmdl.BaseStream.Position;
            currentSection += 1;
            nextOffset = currentOffset + sectionSizes[currentSection];

            mainForm.writeToLog = "Reading submesh counts...\n at offset:" + currentOffset.ToString() + "\n";

            uint submeshCount = Endian.SwapEndianU32(cmdl.ReadUInt32());

            submeshOffsets = new uint[submeshCount];

            for (int i = 0; i < submeshCount; i++)
            {
                submeshOffsets[i] = Endian.SwapEndianU32(cmdl.ReadUInt32());
                mainForm.writeToLog = "Submesh " + i.ToString() + " : 0x" + submeshOffsets[i].ToString() + "\n";
            }

            mainForm.writeToLog = "\n";

            /*  submeshes   */
            for (int i = 0; i < submeshCount; i++)
            {
                //Jump to correct offset each time
                cmdl.BaseStream.Position = nextOffset;
                currentOffset = (uint)cmdl.BaseStream.Position;
                currentSection += 1;
                nextOffset = currentOffset + sectionSizes[currentSection];

                mainForm.writeToLog = "Submesh " + i.ToString() + " Offset: 0x" + currentOffset.ToString("X") + "\n";

                uint[] pivotLocation = new uint[3];
                for (int b = 0; b < 3; b++) pivotLocation[b] = Endian.SwapEndianU32(cmdl.ReadUInt32());
                ushort mantissa = Endian.SwapEndianU16(cmdl.ReadUInt16());
                ushort tableSize = Endian.SwapEndianU16(cmdl.ReadUInt16());
                uint unknown1 = Endian.SwapEndianU32(cmdl.ReadUInt32());
                uint unknown2 = Endian.SwapEndianU32(cmdl.ReadUInt32());
                ushort unknown3 = Endian.SwapEndianU16(cmdl.ReadUInt16());
                ushort matID = Endian.SwapEndianU16(cmdl.ReadUInt16());
                byte unknown4 = cmdl.ReadByte();
                byte visibilityIndex = cmdl.ReadByte();
                ushort uvArrayFlag = Endian.SwapEndianU16(cmdl.ReadUInt16());

                //Checkstuff here
                bool hasNummer;
                int numColors = 0;
                int numUV = 0;
                int skip = 0;

                if ((vertexAttributeArray[matID] & 0xC) == 0xC) hasNummer = true;
                else hasNummer = false;

                switch (vertexAttributeArray[matID] & 0xF0)
                {
                    case 0x0: numColors = 0; break;
                    case 0x30: numColors = 1; break;
                    case 0xC0: numColors = 1; break;
                    case 0xF0: numColors = 2; break;
                }

                switch (vertexAttributeArray[matID] & 0x3FFF00)
                {
                    case 0x0: numUV = 0; break;
                    case 0x300: numUV = 1; break;
                    case 0xF00: numUV = 2; break;
                    case 0x3F00: numUV = 3; break;
                    case 0xFF00: numUV = 4; break;
                    case 0x3FF00: numUV = 5; break;
                    case 0xFFF00: numUV = 6; break;
                    case 0x3FFF00: numUV = 7; break;
                    default: numUV = 2; break;
                }

                switch (vertexAttributeArray[matID] & 0x3000000)
                {
                    case 0x1000000: skip = 1; break;
                    case 0x3000000: skip = 2; break;
                    default: skip = 0; break;
                }

                int primitiveFlag = 1;

                while (primitiveFlag != 0 || cmdl.BaseStream.Position < nextOffset)
                {
                    primitiveFlag = cmdl.ReadByte();
                    int primitiveType = primitiveFlag & 0xF8;
                    if (primitiveFlag == 0) break;

                    uint count = Endian.SwapEndianU16(cmdl.ReadUInt16());

                    if (primitiveType == 0x90) //Triangles
                    {
                        for (int c = 0; c < (count / 3); c++)
                        {
                            ushort[] vertex = new ushort[3];
                            ushort[] normal = new ushort[3];
                            ushort[] UV = new ushort[3];

                            for (int v = 0; v < 3; v++)
                            {
                                cmdl.BaseStream.Position += skip;

                                //vertex
                                vertex[v] = Endian.SwapEndianU16(cmdl.ReadUInt16());
                                //normal
                                normal[v] = 0;
                                if (hasNummer) normal[v] = Endian.SwapEndianU16(cmdl.ReadUInt16());
                                //skip vertex colors
                                cmdl.BaseStream.Position += (2 * numColors);
                                //UV
                                UV[v] = 0;
                                if (numUV > 1)
                                {
                                    UV[v] = Endian.SwapEndianU16(cmdl.ReadUInt16());
                                    cmdl.BaseStream.Position += (2 * (numUV - 1));
                                }
                            }

                            faceArray.Add("f " + vertex[0].ToString() + "/" + normal[0].ToString() + "/" + UV[0].ToString() + " " + vertex[1].ToString() + "/" + normal[1].ToString() + "/" + UV[1].ToString() + " " + vertex[2].ToString() + "/" + normal[2].ToString() + "/" + UV[2].ToString());
                        }
                    }
                }
            }

            string[] faceArrayArray = faceArray.ToArray();

            string[] export = new string[vertexArray.Length + faceArrayArray.Length];

            Array.Copy(vertexArray, 0, export, 0, vertexArray.Length);
            Array.Copy(faceArrayArray, 0, export, vertexArray.Length, faceArrayArray.Length);

            return export;
        }
    }
}

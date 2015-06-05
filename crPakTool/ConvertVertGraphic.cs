using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using EndianLib;

namespace crPakTool
{
    class ConvertVertGraphic
    {
        static uint SwapEndianU32(uint unsignedInteger)
        {
            return ((unsignedInteger & 0x000000ff) << 24) +
                   ((unsignedInteger & 0x0000ff00) << 8) +
                   ((unsignedInteger & 0x00ff0000) >> 8) +
                   ((unsignedInteger & 0xff000000) >> 24);
        }

        static int SwapEndian32(int signedInteger)
        {
            return ((int)(signedInteger & 0x000000ff) << 24) +
                   ((int)(signedInteger & 0x0000ff00) << 8) +
                   ((int)(signedInteger & 0x00ff0000) >> 8) +
                   ((int)(signedInteger & 0xff000000) >> 24);
        }

        static ushort SwapEndianU16(ushort unsignedInteger)
        {
            int swapped = ((unsignedInteger & 0x00ff) << 8) + ((unsignedInteger & 0xff00) >> 8);
            return (ushort)swapped;
        }

        static short SwapEndian16(short signedInteger)
        {
            int swapped = ((signedInteger & 0x00ff) << 8) + ((signedInteger & 0xff00) >> 8);
            return (short)swapped;
        }

        public string[] ExportObj(BinaryReader CMDLFile)
        {
            List<string> OBJvertices = new List<string>();
            List<string> subMeshStrings = new List<string>();
            uint skipSize = 0;

            CMDLFile.BaseStream.Position = 0x00;
            uint magic = SwapEndianU32(CMDLFile.ReadUInt32()),
                 flag = SwapEndianU32(CMDLFile.ReadUInt32()),
                 MBB1 = SwapEndianU32(CMDLFile.ReadUInt32()),
                 MBB2 = SwapEndianU32(CMDLFile.ReadUInt32()),
                 MBB3 = SwapEndianU32(CMDLFile.ReadUInt32()),
                 MBB4 = SwapEndianU32(CMDLFile.ReadUInt32()),
                 MBB5 = SwapEndianU32(CMDLFile.ReadUInt32()),
                 MBB6 = SwapEndianU32(CMDLFile.ReadUInt32()),
                 sectionCount = SwapEndianU32(CMDLFile.ReadUInt32()),
                 materialSetCount = SwapEndianU32(CMDLFile.ReadUInt32());

            if ((flag & 0x20) == 0x10)
            {
                skipSize = 0;
            }

            string[] finishedFile = new string[CMDLFile.BaseStream.Length - 100];

            if (magic == 0x9381000A)
            {
                CMDLFile.BaseStream.Position = 0x28 + skipSize;
                uint materialSectionSize = SwapEndianU32(CMDLFile.ReadUInt32()),
                     vertexSectionSize = SwapEndianU32(CMDLFile.ReadUInt32()),
                     normalSectionSize = SwapEndianU32(CMDLFile.ReadUInt32()),
                     ColorsSectionSize = SwapEndianU32(CMDLFile.ReadUInt32()),
                     floatUvSectionSize = SwapEndianU32(CMDLFile.ReadUInt32()),
                     shortUvSectionSize = SwapEndianU32(CMDLFile.ReadUInt32()),
                     subMeshDefSectionSize = SwapEndianU32(CMDLFile.ReadUInt32());

                int subMeshCounter = 0;
                uint[] subMeshSectionSize = new uint[sectionCount - 7];
                for (int i = 0; i < (sectionCount - 7); i++)
                {
                    subMeshSectionSize[subMeshCounter] = SwapEndianU32(CMDLFile.ReadUInt32());
                    subMeshCounter++;
                }

                //Check flag if vertex are stored as shorts or floats
                int vNumber;
                if ((flag & 0x20) == 0x20)
                {
                    vNumber = 2;
                }
                else
                {
                    vNumber = 4;
                }

                //Go to vertex section
                CMDLFile.BaseStream.Position = 0x60 + materialSectionSize;
                
                //Store vertices in string array
                for (int i = 0; i < vertexSectionSize; i += (vNumber * 3))
                {
                    if (vNumber == 2) //shorts
                    {
                        float xpos = (((float)SwapEndian16(CMDLFile.ReadInt16())) / 0x2000);
                        float ypos = (((float)SwapEndian16(CMDLFile.ReadInt16())) / 0x2000);
                        float zpos = (((float)SwapEndian16(CMDLFile.ReadInt16())) / 0x2000);
                        OBJvertices.Add("v " + xpos.ToString("n6", CultureInfo.InvariantCulture) + " " + ypos.ToString("n6", CultureInfo.InvariantCulture) + " " + zpos.ToString("n6", CultureInfo.InvariantCulture));
                    }
                    else //"floats", actually just saved as decimal for purposes
                    {
                        int xpos = Endian.SwapEndian32(CMDLFile.ReadInt32());// 1000000000
                        int ypos = Endian.SwapEndian32(CMDLFile.ReadInt32());
                        int zpos = Endian.SwapEndian32(CMDLFile.ReadInt32());
                        OBJvertices.Add("v " + xpos.ToString(/*"n6", CultureInfo.InvariantCulture*/) + " " + ypos.ToString(/*"n6", CultureInfo.InvariantCulture*/) + " " + zpos.ToString(/*"n6", CultureInfo.InvariantCulture*/));
                    }
                }

                //Go to SubMesh section
                CMDLFile.BaseStream.Position = 0x60 + materialSectionSize + vertexSectionSize + normalSectionSize + ColorsSectionSize + floatUvSectionSize + shortUvSectionSize + subMeshDefSectionSize;

                //Create Faces
                for (int c = 0; c < subMeshSectionSize.Length; c++)
                {
                    uint subMeshSizeCounter = subMeshSectionSize[c];

                    //Get header information of submesh
                    CMDLFile.BaseStream.Position += 0x1E;
                    subMeshSizeCounter -= 0x1E;
                    short shortsFlag = SwapEndian16(CMDLFile.ReadInt16());
                    subMeshSizeCounter -= 0x2;

                    for (int a = 0; a < subMeshSectionSize[c]; a++)
                    {
                        int primitiveFlag = CMDLFile.ReadByte();
                        int verticesCount = SwapEndian16(CMDLFile.ReadInt16());
                        subMeshSizeCounter -= 0x3;

                        if (shortsFlag == 0x0100)
                        {
                            if ((primitiveFlag & 0x98) == 0x98) //Triangle strip
                            {
                                ushort[] tempData = new ushort[verticesCount * 3];

                                for (int b = 0; b < (verticesCount * 3); b++)
                                {
                                    tempData[b] = SwapEndianU16(CMDLFile.ReadUInt16());
                                    subMeshSizeCounter -= 2;
                                }

                                string tempString = "f";

                                for (int b = 0; b < (verticesCount * 3); b += 3)
                                {
                                    //subMeshStrings.Add("f " + tempData[b] + "/" + tempData[b+2] + "/" + tempData[b+1] + " " + tempData[b+3] + "/" + tempData[b+5] + "/" + tempData[b+4] + " " + tempData[b+6] + "/" + tempData[b+8] + "/" + tempData[b+7]);
                                    tempString += " " + tempData[b];
                                }

                                subMeshStrings.Add(tempString);
                                
                            }
                            else if ((primitiveFlag & 0xA0) == 0xA0) //Triangle fan
                            {
                                ushort[] tempData = new ushort[verticesCount * 3];

                                for (int b = 0; b < (verticesCount * 3); b++)
                                {
                                    tempData[b] = SwapEndianU16(CMDLFile.ReadUInt16());
                                    subMeshSizeCounter -= 2;
                                }

                                for (int b = 0; b < ((verticesCount * 3) - ((verticesCount * 3) / 2)); b += 3)
                                {
                                    subMeshStrings.Add("f " + tempData[0] + " " + tempData[b+3] + " " + tempData[b+6]);
                                }
                            }
                            else if ((primitiveFlag & 0xF8) == 0)
                            {
                                subMeshSizeCounter = subMeshSectionSize[c] - subMeshSizeCounter;
                                CMDLFile.BaseStream.Position += subMeshSizeCounter;
                                break;
                            }
                        }
                        else
                        {

                        }
                    }
                }

                string[] OBJverticesFinished = OBJvertices.ToArray();
                string[] subMeshStringsFinished = subMeshStrings.ToArray();

                Array.Copy(OBJverticesFinished, 0, finishedFile, 0, OBJverticesFinished.Length);
                Array.Copy(subMeshStringsFinished, 0, finishedFile, OBJverticesFinished.Length, subMeshStringsFinished.Length);
            }

            CMDLFile.Close();
            
            return finishedFile;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using EndianLib;

namespace crPakTool
{
    class ZlibCompression
    {
        public byte[] GiveEndValues(int len)
        {
            //int size = (len - 1) - (((len - 1) & 0x7FFFFFF0) + 0x20); First
            //int size = (((len - 1) & 0x7FFFFFF0) + 0x20) - (len - 1); Secondary
            int size = ((len & 0x7FFFFFF0) + 0x20) - len;
            byte[] retVal = new byte[size];
            for (int i = 0;i < retVal.Length;i++)
            {
                retVal[i] = 0xFF;
            }
            return retVal;
        }

        public byte[] Padding(int length)
        {
            float numberone = ((float)(length) + 16f) / 64f;
            float compare = 0.99f;

            while (numberone > compare)
            {
                compare = compare + 1.00f;
            }

            int size = (Convert.ToInt32(compare)) * 64;
            int paddingSize = size - (length + 16);
            byte[] foo = new byte[paddingSize];
            for (int i = 0; i < foo.Length; i++)
            {
                foo[i] = 0xFF;
            }

            return foo;

            /*byte[] retVal = new byte[(length + 16) % 64];
            for (int i = 0; i < retVal.Length; i++) { retVal[i] = 0xFF; }
            return retVal; */
        }

        public byte[] ExportDecompress(BinaryReader mainFile, int node, List<List<uint>> FileList, uint strgLength, uint rshdLength)
        {
            int length = (int)FileList[node][4];
            uint address = FileList[node][5] + 0x80 + strgLength + rshdLength;
            
            mainFile.BaseStream.Position = address;
            uint CMPD = Endian.SwapEndianU32(mainFile.ReadUInt32());
            uint blocks = Endian.SwapEndianU32(mainFile.ReadUInt32());

            ushort[][] blockInfoOne = new ushort[blocks][];
            uint[] blockInfoTwo = new uint[blocks];
            uint totalFileLengthCounter = 0;
            int positionFileCounter = 0;

            // Getting block information
            for (int i = 0; i < blocks; i++)
            {
                blockInfoOne[i] = new ushort[2];
                blockInfoOne[i][0] = Endian.SwapEndianU16(mainFile.ReadUInt16());
                blockInfoOne[i][1] = Endian.SwapEndianU16(mainFile.ReadUInt16());
                blockInfoTwo[i] = Endian.SwapEndianU32(mainFile.ReadUInt32());

                totalFileLengthCounter += blockInfoTwo[i];
            }

            byte[] uncompressed = new byte[totalFileLengthCounter];

            // decompressing and writing everything
            for (int i = 0; i < blocks; i++)
            {
                if (blockInfoOne[i][1] == blockInfoTwo[i])
                {
                    byte[] uncompressedPart = mainFile.ReadBytes(blockInfoOne[i][1]);
                    Buffer.BlockCopy(uncompressedPart, 0, uncompressed, positionFileCounter, uncompressedPart.Length);

                    positionFileCounter += uncompressedPart.Length;
                }
                else
                {
                    byte[] compressed = mainFile.ReadBytes(blockInfoOne[i][1]);
                    byte[] uncompressedPart = Ionic.Zlib.ZlibStream.UncompressBuffer(compressed);

                    Buffer.BlockCopy(uncompressedPart, 0, uncompressed, positionFileCounter, uncompressedPart.Length);

                    positionFileCounter += uncompressedPart.Length;
                }
            }
            
            return uncompressed;
        }

        public byte[] Compress(BinaryReader file)
        {
            int length = (int)file.BaseStream.Length;

            file.BaseStream.Position = 0x00;
            byte[] uncompressed = file.ReadBytes(length);

            byte[] compressed = Ionic.Zlib.ZlibStream.CompressBuffer(uncompressed);
            //short writeCompressedLength = (short)compressed.Length;
            short compressedLength = (short)compressed.Length;
            int uncompressedLength = uncompressed.Length;

            byte[] CMPDHeader = new byte[] { 0x43, 0x4D, 0x50, 0x44, 0x00, 0x00, 0x00, 0x01, 0xA0, 0x00 };
            byte[] CLength = BitConverter.GetBytes(compressedLength);
            byte[] UCLength = BitConverter.GetBytes(uncompressedLength);
            byte[] padding = Padding(compressed.Length);
            Array.Reverse(CLength);
            Array.Reverse(UCLength);

            byte[] together = new byte[16 + compressedLength + padding.Length];

            Buffer.BlockCopy(CMPDHeader, 0, together, 0, 10);
            Buffer.BlockCopy(CLength, 0, together, 10, 2);
            Buffer.BlockCopy(UCLength, 0, together, 12, 4);
            Buffer.BlockCopy(compressed, 0, together, 16, compressed.Length);
            Buffer.BlockCopy(padding, 0, together, (16 + compressed.Length), padding.Length);

            return together;
        }
    }
}

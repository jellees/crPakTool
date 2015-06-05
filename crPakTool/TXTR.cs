using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using EndianLib;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace crPakTool
{
    class TXTR
    {
        byte swapBits(byte value)
        {
            int intval = ((value & 0x3) << 6) + ((value & 0xC) << 2) + ((value & 0x30) >> 2) + ((value & 0xC0) >> 6);
            return (byte)intval;
        }

        byte[] WriteCMPR(byte[] TXTR, int height, int width) // Code by parax
        {
            byte[] pixelOrganize = new byte[TXTR.Length];
            byte[] DDS = new byte[TXTR.Length];
            int s = 0;
            int y = 0;

            int heightOffset = height / 4;
            int widthOffset = width / 4;

            while (y < heightOffset)
            {
                int x = 0;
                while (x < widthOffset)
                {
                    int dy = 0;
                    while (dy < 2)
                    {
                        int dx = 0;
                        while (dx < 2)
                        {
                            if (pixelOrganize.Length == TXTR.Length)
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    int arrayPos = 8 * (((y + dy) * widthOffset) + x + dx) + i;
                                    pixelOrganize[arrayPos] = TXTR[s + i];
                                }
                            }
                            s += 8;
                            dx += 1;
                        }
                        dy += 1;
                    }
                    x += 2;
                }
                y += 2;
            }

            for (int i = 0; i < (pixelOrganize.Length / 8); i++)
            {
                int checknum = 8 * i;

                byte b1 = pixelOrganize[checknum];
                byte b2 = pixelOrganize[checknum + 1];
                byte b3 = pixelOrganize[checknum + 2];
                byte b4 = pixelOrganize[checknum + 3];
                byte b5 = pixelOrganize[checknum + 4];
                byte b6 = pixelOrganize[checknum + 5];
                byte b7 = pixelOrganize[checknum + 6];
                byte b8 = pixelOrganize[checknum + 7];

                DDS[checknum]       = b2;
                DDS[checknum + 1]   = b1;
                DDS[checknum + 2]   = b4;
                DDS[checknum + 3]   = b3;
                DDS[checknum + 4]   = swapBits(b5);
                DDS[checknum + 5]   = swapBits(b6);
                DDS[checknum + 6]   = swapBits(b7);
                DDS[checknum + 7]   = swapBits(b8);
            }

            return DDS;
        }

        byte[] WriteRGB5A3(byte[] TXTR, int height, int width)
        {
            int xasBlockCount = width / 4;
            int position = 0;

            byte[] pixelData = new byte[(width * height) * 4];
            int pixelDataCounter = 0;

            for (int y = 0; y < (height / 4); y++)
            {
                for (int ypos = 0; ypos < 3; ypos++)
                {
                    for (int x = 0; x < xasBlockCount; x++)
                    {
                        for (int xpos = 0; xpos < 3; xpos++)
                        {
                            uint pixel = Endian.SwapEndianU16(BitConverter.ToUInt16(TXTR, position));

                            if ((pixel & 0x8000) == 0x8000)
                            {
                                pixelData[pixelDataCounter + 3] = 0xFF;
                                pixelData[pixelDataCounter] = Convert.ToByte(((pixel & 0x7C00) >> 10) * 0x8);
                                pixelData[pixelDataCounter + 1] = Convert.ToByte(((pixel & 0x3E0) >> 5) * 0x8);
                                pixelData[pixelDataCounter + 2] = Convert.ToByte((pixel & 0x1F) * 0x8);
                                pixelDataCounter += 4;
                            }
                            else
                            {
                                pixelData[pixelDataCounter] = Convert.ToByte(0x20 * (pixel & 0x7000));
                                pixelDataCounter++;
                                pixelData[pixelDataCounter] = Convert.ToByte((pixel & 0xF00) * 0x11);
                                pixelDataCounter++;
                                pixelData[pixelDataCounter] = Convert.ToByte((pixel & 0xF0) * 0x11);
                                pixelDataCounter++;
                                pixelData[pixelDataCounter] = Convert.ToByte((pixel & 0xF) * 0x11);
                                pixelDataCounter++;
                            }

                            position += 2;
                        }
                        position += 24;
                    }
                    position -= (24 * xasBlockCount);
                }
            }

            return pixelData;
        }

        public void TXTRtoPNG(BinaryReader TXTR, string pathName)
        {
            int imageFormat = Endian.SwapEndian32(TXTR.ReadInt32());
            int width = Endian.SwapEndian16(TXTR.ReadInt16());
            int height = Endian.SwapEndian16(TXTR.ReadInt16());
            int mipCount = Endian.SwapEndian32(TXTR.ReadInt32());

            double dpiY = (double)(width / 2) + (width / 4);
            double dpiX = (double)(height / 2) + (height / 4);
            int stride = ((width * 32 + 31) & ~31) / 8;

            byte[] PNGpixel;

            if (imageFormat == 0x8)
            {
                int byteCount = (width * height) * 2;

                byte[] TXTRpixel = TXTR.ReadBytes(byteCount);

                PNGpixel = WriteRGB5A3(TXTRpixel, height, width);
            }
            else
            {
                PNGpixel = new byte[0];
            }

            BitmapSource pngImage = BitmapSource.Create(width, height, dpiX, dpiY, PixelFormats.Bgra32, BitmapPalettes.WebPaletteTransparent, PNGpixel, stride);

            FileStream pngStream = new FileStream(pathName, FileMode.Create);
            PngBitmapEncoder encode = new PngBitmapEncoder();
            encode.Interlace = PngInterlaceOption.On;
            encode.Frames.Add(BitmapFrame.Create(pngImage));
            encode.Save(pngStream);
            pngStream.Close();
        }

        public byte[] TXTRtoDDS(BinaryReader TXTR)
        {
            int imageFormat = Endian.SwapEndian32(TXTR.ReadInt32());
            int width = Endian.SwapEndian16(TXTR.ReadInt16());
            int height = Endian.SwapEndian16(TXTR.ReadInt16());
            int mipCount = Endian.SwapEndian32(TXTR.ReadInt32());

            // DDS header
            byte[] header1 = new byte[] { 0x44, 0x44, 0x53, 0x20, 0x7C, 0x00, 0x00, 0x00, 0x07, 0x10, 0x02, 0x00 };
            byte[] header2 = BitConverter.GetBytes(height);
            byte[] header3 = BitConverter.GetBytes(width);
            byte[] header4 = BitConverter.GetBytes(height * width / 2);
            byte[] header5 = new byte[4];
            byte[] header6 = new byte[] { 0x01, 0x00, 0x00, 0x00 }; // Just one mipmap
            byte[] header7 = new byte[44];
            byte[] header8 = new byte[] { 0x20, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x31, 0x00, 0x00, 0x00, 0x00, 
                                          0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                          0x00, 0x10, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                          0x00, 0x00, 0x00, 0x00};
            //DDS header

            byte[] TXTRFile = TXTR.ReadBytes((width * height) / 2);
            byte[] DDSFile = WriteCMPR(TXTRFile, height, width);

            byte[] FinalFile = new byte[0x80 + DDSFile.Length];

            // Make it all one array
            Buffer.BlockCopy(header1, 0, FinalFile, 0, header1.Length);
            Buffer.BlockCopy(header2, 0, FinalFile, 12, header2.Length);
            Buffer.BlockCopy(header3, 0, FinalFile, 16, header3.Length);
            Buffer.BlockCopy(header4, 0, FinalFile, 20, header4.Length);
            Buffer.BlockCopy(header5, 0, FinalFile, 24, header5.Length);
            Buffer.BlockCopy(header6, 0, FinalFile, 28, header6.Length);
            Buffer.BlockCopy(header7, 0, FinalFile, 32, header7.Length);
            Buffer.BlockCopy(header8, 0, FinalFile, 76, header8.Length);
            Buffer.BlockCopy(DDSFile, 0, FinalFile, 128, DDSFile.Length);

            return FinalFile;
        }
    }
}

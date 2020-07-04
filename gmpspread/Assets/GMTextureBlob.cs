using gmpspread.Base_Classes;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace gmpspread.Assets
{
    public class GMTextureBlob
    {
        public Bitmap TexturePage;

        public GMTextureBlob(BinaryReader binaryReader)
        {
            // Read the supposed texture header.
            byte[] magic = binaryReader.ReadBytes(4);
            binaryReader.BaseStream.Position -= 4;
            if (!CheckDDSHeader(magic))
            {
                Output.Print("Texture is not DDS, trying to read as RAW4444");
                byte[] RAWmagic = binaryReader.ReadBytes(4); // should be "RAW "
                if (!CheckRAWHeader(RAWmagic))
                {
                    throw new InvalidDataException("Header is not 'RAW '. Cannot read further.");
                }

                // On GMPSP it should be 512x512.
                int Width = binaryReader.ReadInt32();
                int Height = binaryReader.ReadInt32();

                // ????????????????
                Debug.Assert(Width == Height);

                // On GMPSP this should always be 1. (in case of Karoshi)
                bool Is4444 = binaryReader.ReadInt32() == 1;
                if (!Is4444) Output.Print("ERROR: The texture is not 4444, gotta crash.");

                // Make a Bitmap
                Bitmap bm = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
                var rect = new Rectangle(0, 0, bm.Width, bm.Height);

                BitmapData bmd = bm.LockBits(rect, ImageLockMode.ReadWrite, bm.PixelFormat);

                // Get pointer to 0,0 coord
                IntPtr ptr = bmd.Scan0;

                for (int i = 0, j = 0; i < (Width * Height * 2); i += 2)
                {
                    byte a, b, g, r;
                    byte d = binaryReader.ReadByte();
                    byte dn = binaryReader.ReadByte();
                    a = (byte)((dn >> 4) & 0x0F);
                    b = (byte)(dn & 0x0F);
                    g = (byte)((d >> 4) & 0x0F);
                    r = (byte)(d & 0x0F);

                    // ARGB
                    Marshal.WriteByte(ptr, j, (byte)((float)b * 255f / 15f));
                    Marshal.WriteByte(ptr, j + 1, (byte)((float)g * 255f / 15f));
                    Marshal.WriteByte(ptr, j + 2, (byte)((float)r * 255f / 15f));
                    Marshal.WriteByte(ptr, j + 3, (byte)((float)a * 255f / 15f));

                    j += 4;
                }
                
                bm.UnlockBits(bmd);
                TexturePage = bm;
            }
            else
            {
                // Since Pfim corrupts our memory for some reason we use an external tool.

                long start_addr = binaryReader.BaseStream.Position;
                binaryReader.ReadBytes(4); // read the DDS header.
                while (true)
                {
                    var new_hdr = binaryReader.ReadBytes(4);
                    if (CheckDDSHeader(new_hdr) || CheckRAWHeader(new_hdr) || CheckAUDOHeader(new_hdr)) break;
                    // Read till the next file.
                }

                binaryReader.BaseStream.Position -= 4;
                long end_addr = binaryReader.BaseStream.Position;
                binaryReader.BaseStream.Position = start_addr;
                var file = binaryReader.ReadBytes((int)(end_addr - start_addr));
                File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "d.dds", file);

                var procinfo = new ProcessStartInfo
                {
                    FileName = AppDomain.CurrentDomain.BaseDirectory + "tools" + Path.DirectorySeparatorChar + "ddspng.exe",
                    Arguments = AppDomain.CurrentDomain.BaseDirectory + "d.dds",
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    UseShellExecute = false
                };
                var proc = new Process() { StartInfo = procinfo };
                proc.Start();
                proc.WaitForExit();
                proc.Close();
                proc.Dispose();
                File.Delete("d.dds");

                using (var bmpTemp = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "d.png"))
                {
                    TexturePage = new Bitmap(bmpTemp);
                }
                File.Delete("d.png");

                Output.Print("Decoded DDS.");
            }
        }

        public bool CheckAUDOHeader(byte[] arr)
        {
            return arr[0] == 'A' && arr[1] == 'U' && arr[2] == 'D' && arr[3] == 'O';
        }

        public bool CheckDDSHeader(byte[] arr)
        {
            // "DDS "
            return arr[0] == 0x44 && arr[1] == 0x44 && arr[2] == 0x53 && arr[3] == 0x20;
        }

        public bool CheckRAWHeader(byte[] arr)
        {
            // "RAW "
            return arr[0] == 0x52 && arr[1] == 0x41 && arr[2] == 0x57 && arr[3] == 0x20;
        }
    }
}

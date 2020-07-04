using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread
{
    public class GMString
    {
        public string Content;

        public byte[] RawContent;

        public int Length;

        public override string ToString()
        {
            return Content;
        }

        public GMString(BinaryReader binaryReader)
        {
            // the GMAssetCompiler itself is pointing directly to the string without
            // subtracting 4 when READING, but when WRITING it saves string length before the string itself.
            int addr = binaryReader.ReadInt32();
            addr -= 4;
            long oldaddr = binaryReader.BaseStream.Position; // save the reader head.
            binaryReader.BaseStream.Position = addr; // set our head to the string object.

            Length = binaryReader.ReadInt32();
            RawContent = binaryReader.ReadBytes(Length);
            Content = Encoding.UTF8.GetString(RawContent); // TODO: figure out if it's really UTF8.

            Debug.Assert(binaryReader.ReadByte() == 0x0); // the next byte should be a null terminator.

            binaryReader.BaseStream.Position = oldaddr; // return back to the old position.
        }

        public GMString(string str)
        {
            Content = str;
            Length = str.Length;
            RawContent = Encoding.UTF8.GetBytes(str);
        }
    }
}

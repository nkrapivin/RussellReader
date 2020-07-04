using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread
{
    public class StreamBase
    {
        // YoYo's implementation of ReadBoolean is fucking cursed.
        public bool ReadBool(BinaryReader binaryReader)
        {
            int value = binaryReader.ReadInt32();
            return value != 0;
        }

        // Probably not needed...???
        public void Align(BinaryReader binaryReader, int align)
        {
            align--;
            while ((binaryReader.BaseStream.Position & align) != 0)
            {
                binaryReader.ReadByte();
            }
        }

        // Use this class to add your own weird Stream I/O functions here...
    }
}

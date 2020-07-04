using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Assets
{
    public class GMBackground : StreamBase
    {
        public GMString Name;

        public bool Transparent;
        public bool Smooth;
        public bool Preload;
        public bool IsTileset;

        public GMTPAGEntry Texture;

        public GMBackground(BinaryReader binaryReader, GMWAD w)
        {
            Name = new GMString(binaryReader);
            Transparent = ReadBool(binaryReader);
            Smooth = ReadBool(binaryReader);
            Preload = ReadBool(binaryReader);
            uint tpag_addr = binaryReader.ReadUInt32();
            if (tpag_addr != 0)
            {
                long prev_addr = binaryReader.BaseStream.Position;
                binaryReader.BaseStream.Position = tpag_addr;
                Texture = new GMTPAGEntry(binaryReader, w);
                binaryReader.BaseStream.Position = prev_addr;
            }
            else Texture = null;
        }
    }
}

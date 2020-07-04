using gmpspread.Base_Classes;
using System;
using System.IO;

namespace gmpspread
{
    public class GMKVPChunkBase : GMChunkBase
    {
        public GMWAD WADPtr;

        public GMKVPChunkBase(BinaryReader binaryReader, GMWAD p = null) : base(binaryReader)
        {
            WADPtr = p;
            MakeList();
            int num_items = binaryReader.ReadInt32();
            for (int i = 0; i < num_items; i++)
            {
                uint pos = binaryReader.ReadUInt32();
                long addr = binaryReader.BaseStream.Position;
                if (pos != 0)
                {
                    binaryReader.BaseStream.Position = pos;
                    ReadItem(binaryReader);
                    binaryReader.BaseStream.Position = addr;
                }
                else
                {
                    Output.Print("Pointer is 0???");
                    NullItem();
                    // IT IS INTENTIONALLY ZERO WHEN THE RESOURCE IS FUCKING UNALIGNED!
                }
            }
        }

        public virtual void NullItem()
        {
            throw new NotSupportedException();
        }

        public virtual void MakeList()
        {
            throw new NotSupportedException();
        }

        public virtual void ReadItem(BinaryReader binaryReader)
        {
            throw new NotSupportedException();
        }
    }
}

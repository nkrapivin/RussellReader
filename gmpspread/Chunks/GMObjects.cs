using gmpspread.Assets;
using System.Collections.Generic;
using System.IO;

namespace gmpspread.Chunks
{
    public class GMObjects : GMKVPChunkBase
    {
        public List<GMObject> Items;

        public GMObjects(BinaryReader binaryReader, GMWAD g) : base(binaryReader, g)
        {
            CheckHeader("OBJT");
            FixChunkAddr(binaryReader);
        }

        public override void NullItem()
        {
            Items.Add(null);
        }

        public override void MakeList()
        {
            Items = new List<GMObject>();
        }

        public override void ReadItem(BinaryReader binaryReader)
        {
            Items.Add(new GMObject(binaryReader, WADPtr));
        }
    }
}

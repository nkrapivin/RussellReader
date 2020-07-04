using System.Collections.Generic;
using System.IO;

namespace gmpspread
{
    public class GMSprites : GMKVPChunkBase
    {
        public List<GMSprite> Items;

        public GMSprites(BinaryReader binaryReader, GMWAD g) : base(binaryReader, g)
        {
            CheckHeader("SPRT");
            FixChunkAddr(binaryReader);
        }

        public override void NullItem()
        {
            Items.Add(null);
        }

        public override void MakeList()
        {
            Items = new List<GMSprite>();
        }

        public override void ReadItem(BinaryReader binaryReader)
        {
            Items.Add(new GMSprite(binaryReader, WADPtr));
        }
    }
}

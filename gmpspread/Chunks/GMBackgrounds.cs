using gmpspread.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Chunks
{
    public class GMBackgrounds : GMKVPChunkBase
    {
        public List<GMBackground> Items;

        public GMBackgrounds(BinaryReader binaryReader, GMWAD g) : base(binaryReader, g)
        {
            CheckHeader("BGND");
            FixChunkAddr(binaryReader);
        }

        public override void NullItem()
        {
            Items.Add(null);
        }

        public override void MakeList()
        {
            Items = new List<GMBackground>();
        }

        public override void ReadItem(BinaryReader binaryReader)
        {
            Items.Add(new GMBackground(binaryReader, WADPtr));
        }
    }
}

using gmpspread.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Chunks
{
    public class GMFonts : GMKVPChunkBase
    {
        public List<GMFont> Items;

        public GMFonts(BinaryReader binaryReader, GMWAD g) : base(binaryReader, g)
        {
            CheckHeader("FONT");
            FixChunkAddr(binaryReader);
        }

        public override void NullItem()
        {
            Items.Add(null);
        }

        public override void MakeList()
        {
            Items = new List<GMFont>();
        }

        public override void ReadItem(BinaryReader binaryReader)
        {
            Items.Add(new GMFont(binaryReader, WADPtr));
        }
    }
}

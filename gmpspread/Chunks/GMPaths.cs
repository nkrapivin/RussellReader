using gmpspread.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Chunks
{
    public class GMPaths : GMKVPChunkBase
    {
        public List<GMPath> Items;

        public GMPaths(BinaryReader binaryReader) : base(binaryReader)
        {
            CheckHeader("PATH");
            FixChunkAddr(binaryReader);
        }

        public override void NullItem()
        {
            Items.Add(null);
        }

        public override void MakeList()
        {
            Items = new List<GMPath>();
        }

        public override void ReadItem(BinaryReader binaryReader)
        {
            Items.Add(new GMPath(binaryReader));
        }
    }
}

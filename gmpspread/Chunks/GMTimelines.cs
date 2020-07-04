using gmpspread.Assets;
using System.Collections.Generic;
using System.IO;

namespace gmpspread.Chunks
{
    public class GMTimelines : GMKVPChunkBase
    {
        public List<GMTimeline> Items;

        public GMTimelines(BinaryReader binaryReader) : base(binaryReader)
        {
            CheckHeader("TMLN");
            FixChunkAddr(binaryReader);
        }

        public override void NullItem()
        {
            Items.Add(null);
        }

        public override void MakeList()
        {
            Items = new List<GMTimeline>();
        }

        public override void ReadItem(BinaryReader binaryReader)
        {
            Items.Add(new GMTimeline(binaryReader));
        }
    }
}

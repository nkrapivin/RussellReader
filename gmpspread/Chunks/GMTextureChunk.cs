using gmpspread.Assets;
using gmpspread.Base_Classes;
using System.Collections.Generic;
using System.IO;

namespace gmpspread.Chunks
{
    public class GMTextureChunk : GMKVPChunkBase
    {
        public List<GMTextureBlob> Items;

        public GMTextureChunk(BinaryReader binaryReader) : base(binaryReader)
        {
            CheckHeader("TXTR");
            FixChunkAddr(binaryReader);
        }

        public override void MakeList()
        {
            Items = new List<GMTextureBlob>();
        }

        public override void ReadItem(BinaryReader binaryReader)
        {
            Items.Add(new GMTextureBlob(binaryReader));
        }
    }
}

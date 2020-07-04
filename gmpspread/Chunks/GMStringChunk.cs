using gmpspread.Base_Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Chunks
{
    public class GMStringChunk : GMKVPChunkBase
    {
        public GMStringChunk(BinaryReader binaryReader) : base(binaryReader)
        {
            CheckHeader("STRG");
            FixChunkAddr(binaryReader);
        }

        public override void NullItem()
        {
        }

        public override void MakeList()
        {
        }

        public override void ReadItem(BinaryReader binaryReader)
        {
            var s = new GMString(binaryReader);
            Output.Print("dump string: " + s.Content);
        }
    }
}

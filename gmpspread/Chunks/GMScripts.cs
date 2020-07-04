using gmpspread.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Chunks
{
    public class GMScripts : GMKVPChunkBase
    {
        public List<GMScript> Items;

        public GMScripts(BinaryReader binaryReader) : base(binaryReader)
        {
            CheckHeader("SCPT");
            FixChunkAddr(binaryReader);
        }

        public override void NullItem()
        {
            Items.Add(null);
        }

        public override void MakeList()
        {
            Items = new List<GMScript>();
        }

        public override void ReadItem(BinaryReader binaryReader)
        {
            Items.Add(new GMScript(binaryReader));
        }
    }
}

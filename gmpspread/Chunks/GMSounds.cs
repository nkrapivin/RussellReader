using System.Collections.Generic;
using System.IO;

namespace gmpspread
{
    public class GMSounds : GMKVPChunkBase
	{
        public List<GMSound> Items;

		public GMSounds(BinaryReader binaryReader) : base(binaryReader)
        {
            CheckHeader("SOND");
            FixChunkAddr(binaryReader);
        }

        public override void NullItem()
        {
            Items.Add(null);
        }

        public override void MakeList()
        {
            Items = new List<GMSound>();
        }

        public override void ReadItem(BinaryReader binaryReader)
        {
            Items.Add(new GMSound(binaryReader));
        }
    }
}

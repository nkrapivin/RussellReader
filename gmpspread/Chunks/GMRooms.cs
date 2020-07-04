using gmpspread.Assets;
using System.Collections.Generic;
using System.IO;

namespace gmpspread.Chunks
{
    public class GMRooms : GMKVPChunkBase
    {
        public List<GMRoom> Items;

        public GMRooms(BinaryReader binaryReader, GMWAD g) : base(binaryReader, g)
        {
            CheckHeader("ROOM");
            FixChunkAddr(binaryReader);
        }

        public override void NullItem()
        {
            Items.Add(null);
        }

        public override void MakeList()
        {
            Items = new List<GMRoom>();
        }

        public override void ReadItem(BinaryReader binaryReader)
        {
            Items.Add(new GMRoom(binaryReader, WADPtr));
        }
    }
}

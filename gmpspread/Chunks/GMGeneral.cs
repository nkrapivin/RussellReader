using gmpspread.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread
{
    public class GMGeneral : GMChunkBase
	{
		public int Debug;

		public GMString Name;

		public int RoomMaxID;

		public int RoomMaxTileID;

		public int ID;

		// Technically they're all set to 0.
		public int guid1;

		public int guid2;

		public int guid3;

		public int guid4;

		public int RoomOrderCount; // Count of RoomOrder list.

		public List<int> RoomOrderInt;

		public List<GMRoom> RoomOrder;

		public GMGeneral(BinaryReader binaryReader) : base(binaryReader)
        {
			CheckHeader("GEN8");
			Debug = binaryReader.ReadInt32();
			Name = new GMString(binaryReader);
			RoomMaxID = binaryReader.ReadInt32();
			RoomMaxTileID = binaryReader.ReadInt32();
			ID = binaryReader.ReadInt32();

			guid1 = binaryReader.ReadInt32();
			guid2 = binaryReader.ReadInt32();
			guid3 = binaryReader.ReadInt32();
			guid4 = binaryReader.ReadInt32();

			RoomOrderCount = binaryReader.ReadInt32();

			RoomOrderInt = new List<int>(RoomOrderCount);
			for (int i = 0; i < RoomOrderCount; i++)
			{
				RoomOrderInt.Add(binaryReader.ReadInt32());
            }
			RoomOrder = new List<GMRoom>(RoomOrderCount);
			// filled later in GMWAD...

			FixChunkAddr(binaryReader);
		}
	}
}

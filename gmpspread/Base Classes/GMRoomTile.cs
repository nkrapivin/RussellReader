using gmpspread.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Base_Classes
{
    public class GMRoomTile : StreamBase
    {
		/*
         * ___s.PatchOffset(___index);
					___s.WriteInteger(_tile.X);
					___s.WriteInteger(_tile.Y);
					___s.WriteInteger(_tile.Index);
					___s.WriteInteger(_tile.XO);
					___s.WriteInteger(_tile.YO);
					___s.WriteInteger(_tile.W);
					___s.WriteInteger(_tile.H);
					___s.WriteInteger(_tile.Depth);
					___s.WriteInteger(_tile.Id);
         */

		public int X;
		public int Y;
		public GMBackground Background; // bg index
		public int XOffset;
		public int YOffset;
		public int Width;
		public int Height;
		public int Depth; // "tile layer"
		public int Id; // tile id

		public GMRoomTile(BinaryReader binaryReader, GMWAD w)
        {
			X = binaryReader.ReadInt32();
			Y = binaryReader.ReadInt32();
			var bg_ind = binaryReader.ReadInt32();
			Background = null;
			if (bg_ind > -1)
            {
				Background = w.Backgrounds.Items[bg_ind];
            }
			XOffset = binaryReader.ReadInt32();
			YOffset = binaryReader.ReadInt32();
			Width = binaryReader.ReadInt32();
			Height = binaryReader.ReadInt32();
			Depth = binaryReader.ReadInt32();
			Id = binaryReader.ReadInt32();
        }
	}
}

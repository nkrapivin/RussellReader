using gmpspread.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Base_Classes
{
    public class GMRoomBackground : StreamBase
    {
		/*
         * 				{
					___s.PatchOffset(___index);
					___s.WriteBoolean(_back.Visible);
					___s.WriteBoolean(_back.Foreground);
					___s.WriteInteger(_back.Index);
					___s.WriteInteger(_back.X);
					___s.WriteInteger(_back.Y);
					___s.WriteBoolean(_back.HTiled);
					___s.WriteBoolean(_back.VTiled);
					___s.WriteInteger(_back.HSpeed);
					___s.WriteInteger(_back.VSpeed);
					___s.WriteBoolean(_back.Stretch);
         */

		public bool Visible;
		public bool Foreground;
		public GMBackground Background; // GMBackground
		public int X;
		public int Y;
		public bool HTiled;
		public bool VTiled;
		public int HSpeed;
		public int VSpeed;
		public bool Stretch;

		public GMRoomBackground(BinaryReader binaryReader, GMWAD w)
        {
			Visible = ReadBool(binaryReader);
			Foreground = ReadBool(binaryReader);
			Background = null;
			var bg_ind = binaryReader.ReadInt32();
			if (bg_ind > -1)
            {
				Background = w.Backgrounds.Items[bg_ind];
            }
			X = binaryReader.ReadInt32();
			Y = binaryReader.ReadInt32();
			HTiled = ReadBool(binaryReader);
			VTiled = ReadBool(binaryReader);
			HSpeed = binaryReader.ReadInt32();
			VSpeed = binaryReader.ReadInt32();
			Stretch = ReadBool(binaryReader);
		}
	}
}

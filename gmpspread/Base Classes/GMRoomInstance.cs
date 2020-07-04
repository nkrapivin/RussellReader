using gmpspread.Assets;
using System;
using System.IO;

namespace gmpspread.Base_Classes
{
    public class GMRoomInstance : StreamBase
    {
		/*
         * 			___s.PatchOffset(___index);
					___s.WriteInteger(_inst.X);
					___s.WriteInteger(_inst.Y);
					___s.WriteInteger(_inst.Index);
					___s.WriteInteger(_inst.Id);
					___iff.AddString(___s, _inst.Code);
					___s.WriteSingle((float)_inst.ScaleX);
					___s.WriteSingle((float)_inst.ScaleY);
					___s.WriteInteger((int)_inst.Colour);
					___s.WriteSingle((float)_inst.Rotation);
         */

		public int X;
		public int Y;
		public GMObject Object; // GMObject
		public int Id; // instance id.
		public GMString CreationCode;

		public GMRoomInstance(BinaryReader binaryReader, GMWAD w)
        {
			X = binaryReader.ReadInt32();
			Y = binaryReader.ReadInt32();
			//Object = binaryReader.ReadInt32();
			var obj_id = binaryReader.ReadInt32();
			if (obj_id > -1)
            {
				Object = w.Objects.Items[obj_id];
            }
			Id = binaryReader.ReadInt32();
			CreationCode = new GMString(binaryReader);
        }
	}
}

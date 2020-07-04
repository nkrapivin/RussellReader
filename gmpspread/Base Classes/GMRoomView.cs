using gmpspread.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Base_Classes
{
    public class GMRoomView : StreamBase
    {
		/*
         * 					___s.WriteBoolean(_view.Visible);
					___s.WriteInteger(_view.XView);
					___s.WriteInteger(_view.YView);
					___s.WriteInteger(_view.WView);
					___s.WriteInteger(_view.HView);
					___s.WriteInteger(_view.XPort);
					___s.WriteInteger(_view.YPort);
					___s.WriteInteger(_view.WPort);
					___s.WriteInteger(_view.HPort);
					___s.WriteInteger(_view.HBorder);
					___s.WriteInteger(_view.VBorder);
					___s.WriteInteger(_view.HSpeed);
					___s.WriteInteger(_view.VSpeed);
					___s.WriteInteger(_view.Index);
         */

		public bool Visible;
		public int[] ViewCoords;
		public int[] PortCoords;
		public int HBorder;
		public int VBorder;
		public int HSpeed;
		public int VSpeed;
		public GMObject Object; // GMObject index...???

		public GMRoomView(BinaryReader binaryReader, GMWAD w)
        {
			Visible = ReadBool(binaryReader);
			ViewCoords = new int[4];
			ViewCoords[0] = binaryReader.ReadInt32();
			ViewCoords[1] = binaryReader.ReadInt32();
			ViewCoords[2] = binaryReader.ReadInt32();
			ViewCoords[3] = binaryReader.ReadInt32();
			PortCoords = new int[4];
			PortCoords[0] = binaryReader.ReadInt32();
			PortCoords[1] = binaryReader.ReadInt32();
			PortCoords[2] = binaryReader.ReadInt32();
			PortCoords[3] = binaryReader.ReadInt32();
			HBorder = binaryReader.ReadInt32();
			VBorder = binaryReader.ReadInt32();
			HSpeed = binaryReader.ReadInt32();
			VSpeed = binaryReader.ReadInt32();
			var obj_ind = binaryReader.ReadInt32();
			if (obj_ind > -1)
            {
				Object = w.Objects.Items[obj_ind];
            }
        }
	}
}

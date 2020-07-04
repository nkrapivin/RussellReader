using gmpspread.Base_Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread
{
	public class GMHelp : GMChunkBase
	{
		public GMColor BackgroundColor;
		public bool Mimic;
		public GMString Caption; // usually "Game Information"

		public int Left;
		public int Top;
		public int Width;
		public int Height;

		public bool Border;
		public bool Sizable;
		public bool OnTop;
		public bool Modal;

		public GMString Text; // it's basically an RTF document.


		public GMHelp(BinaryReader binaryReader) : base(binaryReader)
		{
			CheckHeader("HELP");

			BackgroundColor = new GMColor(binaryReader.ReadUInt32());
			Mimic = ReadBool(binaryReader);
			Caption = new GMString(binaryReader);
			Left = binaryReader.ReadInt32();
			Top = binaryReader.ReadInt32();
			Width = binaryReader.ReadInt32();
			Height = binaryReader.ReadInt32();

			Border = ReadBool(binaryReader);
			Sizable = ReadBool(binaryReader);
			OnTop = ReadBool(binaryReader);
			Modal = ReadBool(binaryReader);

			Text = new GMString(binaryReader);

			FixChunkAddr(binaryReader);
		}
	}
}

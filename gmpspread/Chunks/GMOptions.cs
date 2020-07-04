using gmpspread.Base_Classes;
using System.Collections.Generic;
using System.IO;

namespace gmpspread
{
    public class GMOptions : GMChunkBase
	{
		public bool FullScreen;

		public bool InterpolatePixels;

		public bool NoBorder;

		public bool ShowCursor;

		public int Scale;

		public bool Sizeable;

		public bool StayOnTop;

		public GMColor WindowColor;

		public bool ChangeResolution;

		public int ColorDepth;

		public int Resolution;

		public int Frequency;

		public bool NoButtons;

		public bool VSync; // Sync_Vertex

		public bool ScreenKey;

		public bool HelpKey;

		public bool QuitKey;

		public bool SaveKey;

		public bool ScreenshotKey;

		public bool CloseSec;

		public int Priority;

		public bool Freeze;

		public bool ShowProgress;

		// pointers to texture page, in all two PSP games their addresses are set to 0.
		public GMTPAGEntry tpeBackImage;

		public GMTPAGEntry tpeFrontImage;

		public GMTPAGEntry tpeLoadImage;

		public bool LoadTransparent;

		public int LoadAlpha;

		public bool ScaleProgress;

		public bool DisplayErrors;

		public bool WriteErrors;

		public bool AbortErrors;

		public bool VariableErrors; // treat uninitialized vars as 0?

		public int ConstantCount;

		public List<GMGMLConstant> Constants;

		public GMOptions(BinaryReader binaryReader) : base(binaryReader)
        {
			CheckHeader("OPTN");
			FullScreen = ReadBool(binaryReader);
			InterpolatePixels = ReadBool(binaryReader);
			NoBorder = ReadBool(binaryReader);
			ShowCursor = ReadBool(binaryReader);
			Scale = binaryReader.ReadInt32();
			Sizeable = ReadBool(binaryReader);
			StayOnTop = ReadBool(binaryReader);
			WindowColor = new GMColor(binaryReader.ReadUInt32());
			ChangeResolution = ReadBool(binaryReader);
			ColorDepth = binaryReader.ReadInt32();
			Resolution = binaryReader.ReadInt32();
			Frequency = binaryReader.ReadInt32();
			NoBorder = ReadBool(binaryReader);
			VSync = ReadBool(binaryReader);
			ScreenKey = ReadBool(binaryReader);
			HelpKey = ReadBool(binaryReader);
			QuitKey = ReadBool(binaryReader);
			SaveKey = ReadBool(binaryReader);
			ScreenshotKey = ReadBool(binaryReader);
			CloseSec = ReadBool(binaryReader);
			Priority = binaryReader.ReadInt32();
			Freeze = ReadBool(binaryReader);
			ShowProgress = ReadBool(binaryReader);

			var tpeBackImageAddr = binaryReader.ReadUInt32();
			if (tpeBackImageAddr == 0) tpeBackImage = null;
			var tpeFrontImageAddr = binaryReader.ReadUInt32();
			if (tpeFrontImageAddr == 0) tpeFrontImage = null;
			var tpeLoadImageAddr = binaryReader.ReadUInt32();
			if (tpeLoadImageAddr == 0) tpeLoadImage = null;

			LoadTransparent = ReadBool(binaryReader);
			LoadAlpha = binaryReader.ReadInt32();
			ScaleProgress = ReadBool(binaryReader);
			DisplayErrors = ReadBool(binaryReader);
			WriteErrors = ReadBool(binaryReader);
			AbortErrors = ReadBool(binaryReader);
			VariableErrors = ReadBool(binaryReader);

			ConstantCount = binaryReader.ReadInt32();
			Constants = new List<GMGMLConstant>();
			for (int c = 0; c < ConstantCount; c++)
            {
				var constant = new GMGMLConstant(binaryReader);
				Constants.Add(constant);
            }

			FixChunkAddr(binaryReader);
		}
	}
}

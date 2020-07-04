using gmpspread.Base_Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace gmpspread
{
    public class GMSprite : StreamBase
	{
		public GMString Name;

		public int Width;

		public int Height;

		public int BBoxLeft;

		public int BBoxRight;

		public int BBoxBottom;

		public int BBoxTop;

		public bool Transparent;

		public bool Smooth;

		public bool Preload;

		public MaskShape BBoxMode;

		public bool ColCheck;

		public int XOrigin;

		public int YOrigin;

		public int ImageCount;

		public List<GMTPAGEntry> ImageTextures;

		public int MasksCount;

		public List<byte[]> MaskData;

		public enum MaskShape
        {
			PRECISE,
			RECTANGLE,
			DISK,
			DIAMOND
        }

		public GMSprite(BinaryReader binaryReader, GMWAD w)
		{
			Name = new GMString(binaryReader);
			Width = binaryReader.ReadInt32();
			Height = binaryReader.ReadInt32();
			BBoxLeft = binaryReader.ReadInt32();
			BBoxRight = binaryReader.ReadInt32();
			BBoxBottom = binaryReader.ReadInt32();
			BBoxTop = binaryReader.ReadInt32();
			Transparent = ReadBool(binaryReader);
			Smooth = ReadBool(binaryReader);
			Preload = ReadBool(binaryReader);
			int mode = binaryReader.ReadInt32();
			BBoxMode = (MaskShape)mode;
			ColCheck = ReadBool(binaryReader);
			XOrigin = binaryReader.ReadInt32();
			YOrigin = binaryReader.ReadInt32();
			ImageCount = binaryReader.ReadInt32();
			if (ImageCount != 0)
			{
				ImageTextures = new List<GMTPAGEntry>(ImageCount);
				for (int img = 0; img < ImageCount; img++)
				{
					uint addr = binaryReader.ReadUInt32();
					if (addr != 0)
					{
						long prev_addr = binaryReader.BaseStream.Position;
						binaryReader.BaseStream.Position = addr;
						var item = new GMTPAGEntry(binaryReader, w);
						binaryReader.BaseStream.Position = prev_addr;
						ImageTextures.Add(item);
					}
					else ImageTextures.Add(null);
				}
			}
			else ImageTextures = null;
			MasksCount = binaryReader.ReadInt32();
			if (MasksCount != 0)
			{
				MaskData = new List<byte[]>(MasksCount);
				for (int msk = 0; msk < MasksCount; msk++)
				{
					int size = CalculateMaskSize(Width, Height);
					byte[] data = binaryReader.ReadBytes(size);
					MaskData.Add(data);
				}
			}
			else MaskData = null;

			// this is happening in Karoshi...
			if (ImageCount != MasksCount)
            {
				Output.Print($"Sprite {Name}'s ImageCount and MaskCount do not match. {ImageCount} | {MasksCount}");
            }
		}

		public int CalculateMaskSize(int width, int height)
        {
			return (width + 7) / 8 * height;
        }
	}
}

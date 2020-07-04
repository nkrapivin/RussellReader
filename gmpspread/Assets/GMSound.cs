using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread
{
    public class GMSound : StreamBase
    {
		public GMString Name;

		public int Kind;

		public GMString FileExtension;

		public GMString OrigName;

		public uint Effects;

		public enum EffectEnum : int
        {
			CHORUS,
			ECHO,
			FLANGER,
			GARGLE,
			REVERB,
			__LENGTH
        }

		public bool[] EffectArr;

		public float Volume;

		public float Pan;

		public bool Preload;

		public int SoundID;

		public GMSound(BinaryReader binaryReader)
        {
			Name = new GMString(binaryReader);
			Kind = binaryReader.ReadInt32();
			FileExtension = new GMString(binaryReader);
			OrigName = new GMString(binaryReader);
			Effects = binaryReader.ReadUInt32();

			// Parse "Effects" flag.
			int len = (int)EffectEnum.__LENGTH;
			EffectArr = new bool[len];
			uint bk_e = Effects;
			for (int i = 0; i < len; i++)
			{
				EffectArr[i] = (bk_e & 1U) != 0U;
				bk_e >>= 1;
			}

			Volume = binaryReader.ReadSingle();
			Pan = binaryReader.ReadSingle();
			Preload = ReadBool(binaryReader);
			SoundID = binaryReader.ReadInt32();
		}
	}
}

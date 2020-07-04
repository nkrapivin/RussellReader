using gmpspread.Base_Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Assets
{
    public class GMFont : StreamBase
    {
        public enum Codepage
        {
            ANSI_CHARSET = 0,
            DEFAULT_CHARSET = 1,
            EASTEUROPE_CHARSET = 238,
            RUSSIAN_CHARSET = 204,
            SYMBOL_CHARSET = 2,
            SHIFTJIS_CHARSET = 0x80,
            HANGEUL_CHARSET = 129,
            GB2312_CHARSET = 134,
            CHINESEBIG5_CHARSET = 136,
            JOHAB_CHARSET = 130,
            HEBREW_CHARSET = 177,
            ARABIC_CHARSET = 178,
            GREEK_CHARSET = 161,
            TURKISH_CHARSET = 162,
            VIETNAMESE_CHARSET = 163,
            THAI_CHARSET = 222,
            MAC_CHARSET = 77,
            BALTIC_CHARSET = 186,
            OEM_CHARSET = 0xFF
        }

        public GMString Name;

        public GMString FontName;

        public int FontSize;

        public bool Bold;

        public bool Italic;

        public int First;

        public int Last;

        public GMTPAGEntry Texture;

        public float ScaleX;

        public float ScaleY;

        public int GlyphCount;

        public List<GMFontGlyph> Glyphs;

        public GMFont(BinaryReader binaryReader, GMWAD w)
        {
            Name = new GMString(binaryReader);
            FontName = new GMString(binaryReader);
            FontSize = binaryReader.ReadInt32();
            Bold = ReadBool(binaryReader);
            Italic = ReadBool(binaryReader);
            First = binaryReader.ReadInt32();
            Last = binaryReader.ReadInt32();
            uint tpag_addr = binaryReader.ReadUInt32();
            if (tpag_addr != 0)
            {
                long prev_addr = binaryReader.BaseStream.Position;
                binaryReader.BaseStream.Position = tpag_addr;
                Texture = new GMTPAGEntry(binaryReader, w);
                binaryReader.BaseStream.Position = prev_addr;
            }
            else
            {
                Texture = null;
                Output.Print($"ERROR!! :: Font {Name} has no texture!");
            }

            ScaleX = binaryReader.ReadSingle();
            ScaleY = binaryReader.ReadSingle();
            GlyphCount = binaryReader.ReadInt32();
            Glyphs = new List<GMFontGlyph>(GlyphCount);
            for (int g = 0; g < GlyphCount; g++)
            {
                var glyph = new GMFontGlyph(binaryReader);
                Glyphs.Add(glyph);
            }
        }
    }
}

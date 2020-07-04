using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Assets
{
    public class GMFontGlyph
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public int Shift;
        public int Offset;

        public GMFontGlyph()
        {
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
            Shift = 0;
            Offset = 0;
        }

        public GMFontGlyph(int x, int y, int w, int h, int shift, int offset)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
            Shift = shift;
            Offset = offset;
        }

        public GMFontGlyph(BinaryReader binaryReader)
        {
            X = binaryReader.ReadInt32();
            Y = binaryReader.ReadInt32();
            Width = binaryReader.ReadInt32();
            Height = binaryReader.ReadInt32();
            Shift = binaryReader.ReadInt32();
            Offset = binaryReader.ReadInt32();
        }
    }
}

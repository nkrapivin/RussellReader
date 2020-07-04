using gmpspread.Base_Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Assets
{
    public class GMRoom : StreamBase
    {
        public GMString Name;

        public GMString Caption;

        public int Width;

        public int Height;

        public int Speed;

        public bool Persistent;

        public GMColor Color;

        public bool ShowColor;

        public GMString CreationCode;

        public bool EnableViews;

        public List<GMRoomBackground> Backgrounds;

        public List<GMRoomView> Views;

        public List<GMRoomInstance> Instances;

        public List<GMRoomTile> Tiles;

        public GMRoom(BinaryReader binaryReader, GMWAD ww)
        {
            Name = new GMString(binaryReader);
            Caption = new GMString(binaryReader);
            Width = binaryReader.ReadInt32();
            Height = binaryReader.ReadInt32();
            Speed = binaryReader.ReadInt32();
            Persistent = ReadBool(binaryReader);
            Color = new GMColor(binaryReader.ReadUInt32());
            ShowColor = ReadBool(binaryReader);
            CreationCode = new GMString(binaryReader);
            EnableViews = ReadBool(binaryReader);

            var bg_ptr = binaryReader.ReadUInt32();
            var bg_prev_addr = binaryReader.BaseStream.Position;
            binaryReader.BaseStream.Position = bg_ptr;
            //Read backgrounds here
            var bg_count = binaryReader.ReadInt32();
            Backgrounds = new List<GMRoomBackground>(bg_count);
            for (int bg = 0; bg < bg_count; bg++)
            {
                var bg_bg_ptr = binaryReader.ReadUInt32();
                var bg_bg_prev_addr = binaryReader.BaseStream.Position;
                binaryReader.BaseStream.Position = bg_bg_ptr;
                Backgrounds.Add(new GMRoomBackground(binaryReader, ww));
                binaryReader.BaseStream.Position = bg_bg_prev_addr;
            }
            binaryReader.BaseStream.Position = bg_prev_addr;

            var vi_ptr = binaryReader.ReadUInt32();
            var vi_prev_addr = binaryReader.BaseStream.Position;
            binaryReader.BaseStream.Position = vi_ptr;
            //Read Views here
            var vi_count = binaryReader.ReadInt32();
            Views = new List<GMRoomView>(vi_count);
            for (int vi = 0; vi < vi_count; vi++)
            {
                var vi_vi_ptr = binaryReader.ReadUInt32();
                var vi_vi_prev_addr = binaryReader.BaseStream.Position;
                binaryReader.BaseStream.Position = vi_vi_ptr;
                Views.Add(new GMRoomView(binaryReader, ww));
                binaryReader.BaseStream.Position = vi_vi_prev_addr;
            }
            binaryReader.BaseStream.Position = vi_prev_addr;

            var in_ptr = binaryReader.ReadUInt32();
            var in_prev_addr = binaryReader.BaseStream.Position;
            binaryReader.BaseStream.Position = in_ptr;
            //Read Room Instances here
            var in_count = binaryReader.ReadInt32();
            Instances = new List<GMRoomInstance>(in_count);
            for (int ins = 0; ins < in_count; ins++)
            {
                var in_in_ptr = binaryReader.ReadUInt32();
                var in_in_prev_addr = binaryReader.BaseStream.Position;
                binaryReader.BaseStream.Position = in_in_ptr;
                Instances.Add(new GMRoomInstance(binaryReader, ww));
                binaryReader.BaseStream.Position = in_in_prev_addr;
            }
            binaryReader.BaseStream.Position = in_prev_addr;

            var ti_ptr = binaryReader.ReadUInt32();
            var ti_prev_addr = binaryReader.BaseStream.Position;
            binaryReader.BaseStream.Position = ti_ptr;
            //Read Room Tiles here
            var ti_count = binaryReader.ReadInt32();
            Tiles = new List<GMRoomTile>(ti_count);
            for (int ti = 0; ti < ti_count; ti++)
            {
                var ti_ti_ptr = binaryReader.ReadUInt32();
                var ti_ti_prev_addr = binaryReader.BaseStream.Position;
                binaryReader.BaseStream.Position = ti_ti_ptr;
                Tiles.Add(new GMRoomTile(binaryReader, ww));
                binaryReader.BaseStream.Position = ti_ti_prev_addr;
            }
            binaryReader.BaseStream.Position = ti_prev_addr;
        }
    }
}

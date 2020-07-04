using gmpspread.Base_Classes;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace gmpspread.Assets
{
    public class GMObject : StreamBase
    {
        public GMString Name;

        public int SpriteIndex;

        public GMSprite Sprite;

        public bool Visible;

        public bool Solid;

        public int Depth;

        public bool Persistent;

        public int ParentIndex;

        public GMObject Parent;

        public int MaskIndex;

        public GMSprite Mask;

        public List<List<GMGMLEvent>> Events;

        public GMObject(BinaryReader binaryReader, GMWAD w)
        {
            Name = new GMString(binaryReader);
            SpriteIndex = binaryReader.ReadInt32();
            Sprite = null;
            if (SpriteIndex > -1)
            {
                Sprite = w.Sprites.Items[SpriteIndex];
            }
            Visible = ReadBool(binaryReader);
            Solid = ReadBool(binaryReader);
            Depth = binaryReader.ReadInt32();
            Persistent = ReadBool(binaryReader);
            ParentIndex = binaryReader.ReadInt32();
            Parent = null;
            MaskIndex = binaryReader.ReadInt32();
            Mask = null;
            if (MaskIndex > -1)
            {
                Mask = w.Sprites.Items[MaskIndex];
            }

            // It's a list with list with GMGMLEvents...................
            // yoyo nahooya tak hard blyat?

            int count = binaryReader.ReadInt32();
            Debug.Assert(count == 12); // on PSP it should be 12.
            Events = new List<List<GMGMLEvent>>(count);
            long mprev_addr = binaryReader.BaseStream.Position;

            // Thanks to colinator27 for telling me how that works.
            for (int i = 0; i < count; i++)
            {
                uint e_addr = binaryReader.ReadUInt32();
                long prev_addr = binaryReader.BaseStream.Position;
                binaryReader.BaseStream.Position = e_addr;
                int count2 = binaryReader.ReadInt32();
                List<GMGMLEvent> l = new List<GMGMLEvent>(count2);
                for (int j = 0; j < count2; j++)
                {
                    uint e_addr2 = binaryReader.ReadUInt32();
                    long prev_addr2 = binaryReader.BaseStream.Position;
                    binaryReader.BaseStream.Position = e_addr2;
                    int key = binaryReader.ReadInt32(); // subtype data????
                    var ev = new GMGMLEvent(binaryReader);
                    ev.Key = key;
                    l.Add(ev);
                    binaryReader.BaseStream.Position = prev_addr2;
                }
                binaryReader.BaseStream.Position = prev_addr;
                Events.Add(l);
            }

            binaryReader.BaseStream.Position = mprev_addr;
        }
    }
}
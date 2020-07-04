using System.Collections.Generic;
using System.IO;

namespace gmpspread
{
    public class GMGMLEvent : StreamBase
    {
        public int Count;
        public int Key;
        public List<GMGMLAction> Actions;

        public GMGMLEvent(BinaryReader binaryReader)
        {
            Count = binaryReader.ReadInt32();
            Actions = new List<GMGMLAction>(Count);
            for (int a = 0; a < Count; a++)
            {
                uint action_addr = binaryReader.ReadUInt32();
                long prev_addr = binaryReader.BaseStream.Position;
                binaryReader.BaseStream.Position = action_addr;
                var action = new GMGMLAction(binaryReader);
                binaryReader.BaseStream.Position = prev_addr;
                Actions.Add(action);
            }
        }
    }
}

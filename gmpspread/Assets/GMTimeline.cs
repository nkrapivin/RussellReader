using System.Collections.Generic;
using System.IO;

namespace gmpspread.Assets
{
    public class GMTimeline : StreamBase
    {
        public GMString Name;

        public int MomentCount;

        public List<GMTimelineMoment> Moments;

        public GMTimeline(BinaryReader binaryReader)
        {
            Name = new GMString(binaryReader);
            MomentCount = binaryReader.ReadInt32();
            Moments = new List<GMTimelineMoment>(MomentCount);
            for (int m = 0; m < MomentCount; m++)
            {
                var moment = new GMTimelineMoment();
                moment.Point = binaryReader.ReadInt32(); // moment point
                uint e_addr = binaryReader.ReadUInt32(); // pointer to Event?
                long prev_addr = binaryReader.BaseStream.Position; // save current pos
                binaryReader.BaseStream.Position = e_addr; // set pos to Event pointer
                moment.Event = new GMGMLEvent(binaryReader); // read Event
                binaryReader.BaseStream.Position = prev_addr; // set pos back
                Moments.Add(moment);
            }
        }
    }
}

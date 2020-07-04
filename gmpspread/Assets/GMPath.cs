using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Assets
{
    public class GMPath : StreamBase
    {
        public GMString Name;

        public bool SmoothKind;

        public bool Closed;

        public int Precision;

        public int PointsCount;

        // pointer
        public List<GMPathPoint> Points;

        public GMPath(BinaryReader binaryReader)
        {
            Name = new GMString(binaryReader);
            SmoothKind = ReadBool(binaryReader);
            Closed = ReadBool(binaryReader);
            Precision = binaryReader.ReadInt32();
            PointsCount = binaryReader.ReadInt32();
            if (PointsCount != 0)
            {
                Points = new List<GMPathPoint>(PointsCount);
                for (int p = 0; p < PointsCount; p++)
                {
                    var point = new GMPathPoint(binaryReader);
                    Points.Add(point);
                }
            }
            else Points = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Assets
{
    public class GMPathPoint
    {
        public float X;

        public float Y;

        public float Speed;

        public GMPathPoint(BinaryReader binaryReader)
        {
            X = binaryReader.ReadSingle();
            Y = binaryReader.ReadSingle();
            Speed = binaryReader.ReadSingle();
        }

        public GMPathPoint(float x, float y, float speed)
        {
            X = x;
            Y = y;
            Speed = speed;
        }

        public GMPathPoint()
        {
            X = 0f;
            Y = 0f;
            Speed = 0f;
        }
    }
}

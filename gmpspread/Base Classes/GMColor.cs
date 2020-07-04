using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Base_Classes
{
    public class GMColor
    {
        public uint R;
        public uint G;
        public uint B;
        public uint A;

        public GMColor(uint Value)
        {
            if (Value <= 0xFFFFFF)
            {
                B = (Value >> 16) & 0xFF;
                G = (Value >>  8) & 0xFF;
                R = (Value & 255);
                A = 0xFF;
            }
            else
            {
                A = (Value >> 24) & 0xFF;
                B = (Value >> 16) & 0xFF;
                G = (Value >>  8) & 0xFF;
                R = (Value & 255);
            }
        }

        public GMColor()
        {
            R = 255;
            G = 255;
            B = 255;
            A = 255;
        }

        public GMColor(uint r, uint g, uint b)
        {
            R = r;
            G = g;
            B = b;
        }

        public GMColor(uint r, uint g, uint b, uint a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public override string ToString()
        {
            return R.ToString("X2") + G.ToString("X2") + B.ToString("X2");
        }
    }
}

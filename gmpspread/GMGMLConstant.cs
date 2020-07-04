using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread
{
    public class GMGMLConstant
    {
        public GMString Name;

        public GMString Value;

        public GMGMLConstant(BinaryReader binaryReader)
        {
            Name = new GMString(binaryReader);
            Value = new GMString(binaryReader);
        }

        public GMGMLConstant(string name, string value)
        {
            Name = new GMString(name);
            Value = new GMString(value);
        }
    }
}

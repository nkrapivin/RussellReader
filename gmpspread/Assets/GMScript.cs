using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Assets
{
    public class GMScript
    {
        public GMString Name;

        public GMString Code;

        public GMScript(BinaryReader binaryReader)
        {
            Name = new GMString(binaryReader);
            Code = new GMString(binaryReader);
        }

        public GMScript(string name, string code)
        {
            Name = new GMString(name);
            Code = new GMString(code);
        }
    }
}

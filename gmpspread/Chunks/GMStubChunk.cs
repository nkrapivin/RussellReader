using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Chunks
{
    public class GMStubChunk : GMChunkBase
    {
        public GMStubChunk(BinaryReader binaryReader, string header) : base(binaryReader)
        {
            CheckHeader(header);
            FixChunkAddr(binaryReader);
            // This class is used to stub chunks that aren't interesting for me.
        }
    }
}

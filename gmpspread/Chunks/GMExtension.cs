using System.Diagnostics;
using System.IO;

namespace gmpspread
{
    public class GMExtension : GMChunkBase
    {
        public GMExtension(BinaryReader binaryReader) : base(binaryReader)
        {
            CheckHeader("EXTN");
            // Extensions weren't implemented at that time.
            Debug.Assert(ChunkLength == 0x0);
            FixChunkAddr(binaryReader);
        }
    }
}

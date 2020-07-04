using System.IO;

namespace gmpspread.Chunks
{
    public class GMDataFiles : GMChunkBase
    {
        public GMDataFiles(BinaryReader binaryReader) : base(binaryReader)
        {
            CheckHeader("DAFL");
            FixChunkAddr(binaryReader);
            // In all two .psp games (Karoshi and greenTECH+) this chunk is empty.
            // So I don't bother actually reading it lol.
            // But it has some writing code tho so I can't just do Assert(len == 0);
        }
    }
}

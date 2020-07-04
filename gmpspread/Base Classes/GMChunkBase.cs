using System;
using System.IO;

namespace gmpspread
{
    public class GMChunkBase : StreamBase
    {
        public uint ChunkLength;
        public long ChunkStartAddr;
        public char[] ChunkHeader;

        public GMChunkBase(BinaryReader binaryReader)
        {
            ChunkHeader = binaryReader.ReadChars(4);
            ChunkLength = binaryReader.ReadUInt32();
            ChunkStartAddr = binaryReader.BaseStream.Position; // that's where the chunk data actually starts.
        }

        public void CheckHeader(string expected)
        {
            if (!(
            ChunkHeader[0] == expected[0] &&
            ChunkHeader[1] == expected[1] &&
            ChunkHeader[2] == expected[2] &&
            ChunkHeader[3] == expected[3]))
            {
                throw new Exception("Invalid chunk header, expected " + expected + ", got " + new string(ChunkHeader) + ".");
            }
        }

        public void FixChunkAddr(BinaryReader binaryReader)
        {
            // TODO: probably the worst way to handle this.
            binaryReader.BaseStream.Position = ChunkStartAddr + ChunkLength;
        }
    }
}

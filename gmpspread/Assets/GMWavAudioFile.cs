using System.IO;

namespace gmpspread.Assets
{
    public class GMWavAudioFile
    {
        private int Length;
        public byte[] Data;

        public GMWavAudioFile(BinaryReader binaryReader)
        {
            Length = binaryReader.ReadInt32();
            Data = binaryReader.ReadBytes(Length);
        }
    }
}

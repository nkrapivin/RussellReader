using gmpspread.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Chunks
{
    public class GMAudioChunk : GMKVPChunkBase
    {
        public List<GMWavAudioFile> Items;

        public GMAudioChunk(BinaryReader binaryReader) : base(binaryReader)
        {
            CheckHeader("AUDO");
            FixChunkAddr(binaryReader);
        }

        public override void MakeList()
        {
            Items = new List<GMWavAudioFile>();
        }

        public override void ReadItem(BinaryReader binaryReader)
        {
            Items.Add(new GMWavAudioFile(binaryReader));
        }
    }
}

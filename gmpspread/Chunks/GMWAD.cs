using gmpspread.Base_Classes;
using gmpspread.Chunks;
using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace gmpspread
{
    public class GMWAD
    {
        public string WADPath;
        public uint DataSize;
        public GMGeneral Header;
        public GMOptions Options;
        public GMHelp Help;
        public GMExtension Extensions;
        public GMSounds Sounds;
        public GMSprites Sprites;
        public GMBackgrounds Backgrounds;
        public GMPaths Paths;
        public GMScripts Scripts;
        public GMFonts Fonts;
        public GMTimelines Timelines;
        public GMObjects Objects;
        public GMRooms Rooms;
        public GMDataFiles DataFiles;
        public GMTextureChunk TextureBlobs;
        public GMAudioChunk AudioFiles;

        public GMWAD(string FilePath)
        {
            Output.Print("Loading WAD file...");
            var mainWAD = new BinaryReader(File.Open(FilePath, FileMode.Open, FileAccess.Read));
            WADPath = FilePath;
            if (!CheckHeader(mainWAD))
            {
                Output.Print("Root header is invalid, expected 'FORM'. Are you sure that's a GM file?");
                PauseAndQuit();
            }

            Output.Print("Reading chunk FORM...");
            uint allFileLen = mainWAD.ReadUInt32();
            Debug.Assert(allFileLen == mainWAD.BaseStream.Length - 8);
            DataSize = allFileLen;

            Output.Print("Reading chunk GEN8...");
            Header = new GMGeneral(mainWAD);
            Output.Print("Reading chunk OPTN...");
            Options = new GMOptions(mainWAD);
            Output.Print("Reading chunk HELP...");
            Help = new GMHelp(mainWAD);
            Output.Print("Reading chunk EXTN...");
            Extensions = new GMExtension(mainWAD);
            Output.Print("Reading chunk SOND...");
            Sounds = new GMSounds(mainWAD);
            Output.Print("Reading chunk SPRT...");
            Sprites = new GMSprites(mainWAD, this);
            Output.Print("Reading chunk BGND...");
            Backgrounds = new GMBackgrounds(mainWAD, this);
            Output.Print("Reading chunk PATH...");
            Paths = new GMPaths(mainWAD);
            Output.Print("Reading chunk SCPT...");
            Scripts = new GMScripts(mainWAD);
            Output.Print("Reading chunk FONT...");
            Fonts = new GMFonts(mainWAD, this);
            Output.Print("Reading chunk TMLN...");
            Timelines = new GMTimelines(mainWAD);
            Output.Print("Reading chunk OBJT...");
            Objects = new GMObjects(mainWAD, this);
            UpdateParents(this);
            Output.Print("Reading chunk ROOM...");
            Rooms = new GMRooms(mainWAD, this);
            Output.Print("Reading 'backgrounds as tiles'...");
            TagBackgroundTilesets(this);
            Output.Print("Updating room order...");
            UpdateRoomOrder(this);
            Output.Print("Reading chunk DAFL...");
            DataFiles = new GMDataFiles(mainWAD);
            Output.Print("Reading chunk TPAG...");
            new GMStubChunk(mainWAD, "TPAG");
            Output.Print("Reading chunk STRG...");
            //new GMStringChunk(mainWAD);
            new GMStubChunk(mainWAD, "STRG");
            Output.Print("Reading chunk TXTR...");
            TextureBlobs = new GMTextureChunk(mainWAD);
            Output.Print("Updating texture page referrences...");
            SetTextureReference(this);
            Output.Print("Reading chunk AUDO...");
            AudioFiles = new GMAudioChunk(mainWAD);

            Output.Print("WAD File loaded!");

            //Debug.Assert(AudioFiles.Items.Count == Sounds.Items.Count);
            // Some WAV files are just 128 bytes of 0x00, that's normal, that means this sound is external (.at3)
        }

        public static void UpdateRoomOrder(GMWAD w)
        {
            var l_int = w.Header.RoomOrderInt;
            var l_rm = w.Header.RoomOrder;
            var num = w.Header.RoomOrderCount;
            for (int r = 0; r < num; r++)
            {
                l_rm.Add(w.Rooms.Items[l_int[r]]);
            }
        }

        public static void UpdateParents(GMWAD w)
        {
            for (int o = 0; o < w.Objects.Items.Count; o++)
            {
                var oo = w.Objects.Items[o];
                if (oo == null) continue;

                if (oo.ParentIndex > -1)
                {
                    oo.Parent = w.Objects.Items[oo.ParentIndex];
                }
            }
        }

        /// <summary>
        /// Tags all backgrounds in GMWAD as tilesets if they are used in rooms' tiles.
        /// </summary>
        /// <param name="w">GMWAD class</param>
        public static void TagBackgroundTilesets(GMWAD w)
        {
            for (int i = 0; i < w.Rooms.Items.Count; i++)
            {
                var r = w.Rooms.Items[i];
                if (r == null) continue;

                for (int j = 0; j < r.Tiles.Count; j++)
                {
                    var t = r.Tiles[j];
                    t.Background.IsTileset = true;
                }
            }
        }

        public void DumpAllSounds()
        {
            for (int i = 0; i < Sounds.Items.Count; i++)
            {
                var _s = Sounds.Items[i];
                if (_s == null) continue;

                var emptywav = new byte[128];
                for (int bb = 0; bb < emptywav.Length; bb++) emptywav[bb] = 0;

                var _d = AudioFiles.Items[_s.SoundID].Data;
                if (!_d.SequenceEqual(emptywav))
                {
                    File.WriteAllBytes("Sounds" + Path.DirectorySeparatorChar + _s.Name.Content + ".wav", _d);
                }
            }
        }

        public void DumpAllSprites()
        {
            for (int i = 0; i < Sprites.Items.Count; i++)
            {
                var _s = Sprites.Items[i];
                if (_s == null) continue;

                // Loop through all sprite frames.
                for (int j = 0; j < _s.ImageCount; j++)
                {
                    var _t = _s.ImageTextures[j];
                    string name = _s.Name.Content + "_" + j.ToString() + ".png";
                    string full_name = "Sprites" + Path.DirectorySeparatorChar + name;
                    _t.TextureItem.Save(full_name, ImageFormat.Png);
                }
            }
        }

        /// <summary>
        /// Sets GMTextureBlob reference in all classes that have it.
        /// </summary>
        /// <param name="w">GMWAD class</param>
        public static void SetTextureReference(GMWAD w)
        {
            int i; // iterator integer.

            // Sprites.
            for (i = 0; i < w.Sprites.Items.Count; i++)
            {
                var _s = w.Sprites.Items[i];
                if (_s == null) continue;

                // Loop through all sprite frames.
                for (int j = 0; j < _s.ImageCount; j++)
                {
                    var _t = _s.ImageTextures[j];
                    _t.Texture = w.TextureBlobs.Items[_t.TexID];
                    _t.Crop();
                }
            }

            // Backgrounds.
            for (i = 0; i < w.Backgrounds.Items.Count; i++)
            {
                var _b = w.Backgrounds.Items[i];
                if (_b == null) continue;

                _b.Texture.Texture = w.TextureBlobs.Items[_b.Texture.TexID];
                _b.Texture.Crop();
            }

            // Fonts.
            for (i = 0; i < w.Fonts.Items.Count; i++)
            {
                var _f = w.Fonts.Items[i];
                if (_f == null) continue;

                _f.Texture.Texture = w.TextureBlobs.Items[_f.Texture.TexID];
                _f.Texture.Crop();
            }
        }

        public static bool CheckHeader(BinaryReader WAD)
        {
            return WAD.ReadByte() == 'F' && WAD.ReadByte() == 'O' && WAD.ReadByte() == 'R' && WAD.ReadByte() == 'M';
        }

        public static void PauseAndQuit(int errCode = -1)
        {
            Console.ReadKey();
            Environment.Exit(errCode);
        }
    }
}

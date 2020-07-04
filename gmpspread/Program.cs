using gmpspread.Assets;
using gmpspread.Base_Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using static gmpspread.GMGMLAction;

namespace gmpspread
{
    class Program
    {
        private static void Main(string[] args)
        {
            Output.Print("<--- RussellReader by nkrapivindev and Kirill the Pony --->");
            if (args.Length == 0)
            {
                Output.Print("Usage: gmpspread.exe <full path to .psp file and .at3 files> [-exportgmkxml|-dumpsprites|-dumpsounds]");
                Output.Print("This reader will only work for two PSP Minis games (greenTECH+ and Mr. Karoshi)");
                Output.Print("RussellReader expects a 'tools' directory with:");
                Output.Print("- gmksplit.jar (and installed Java of course)");
                Output.Print("- ffmpeg.exe");
                Output.Print("- ddspng.exe (a simple tool to convert DDS textures to PNG)");
                Output.Print("- at3tool.exe (this one is not bundled with the tool, go find it yourself.)");
                Output.Print(".at3 files must be in the same directory game.psp is (not 'games'). Otherwise it won't work.");
                Output.Print(string.Empty);
                Output.Print("Happy Russelling!");
            }
            else
            {
                string WADPath = args[0];
                if (!File.Exists(WADPath))
                {
                    Output.Print("Invalid argument. Argument must be a file, not a directory.");
                }
                else
                {
                    Output.Print("Opening " + WADPath + "...");
                    if (Path.GetExtension(WADPath) != ".psp")
                    {
                        Output.Print("The file is not a .psp file. RussellReader doesn't support this file.");
                        Output.ReadKey();
                        return;
                    }
                    GMWAD w = LoadMain(WADPath);

                    if (args.Length > 1)
                    {
                        if (args[1] == "-exportgmkxml")
                        {
                            Output.Print("Exporting WAD to GmkSplitterXml...");
                            ExportWAD(w);
                            Output.Print("Export finished! Press any key to quit...");
                            Output.ReadKey();
                        }
                        else if (args[1] == "-dumpsprites")
                        {
                            Output.Print("Dumping sprites to 'Sprites' directory...");
                            try
                            {
                                Directory.Delete("Sprites");
                                Directory.CreateDirectory("Sprites");
                            }
                            catch { }
                            w.DumpAllSprites();
                            Output.Print("Sprites dumped! Press any key to quit...");
                            Output.ReadKey();
                        }
                        else if (args[2] == "-dumpsounds")
                        {
                            Output.Print("Dumping .wav sounds to 'Sounds' directory...");
                            try
                            {
                                Directory.Delete("Sounds");
                                Directory.CreateDirectory("Sounds");
                            }
                            catch { }
                            w.DumpAllSounds();
                            Output.Print("Sounds dumped! Press any key to quit...");
                            Output.ReadKey();
                        }
                    }
                }
            }
        }

        private static GMWAD LoadMain(string path)
        {
            GMWAD Assets = new GMWAD(path);
            return Assets;
        }

        private static void ExportWAD(GMWAD gMWAD)
        {
            string fPath = gMWAD.Header.Name.Content;
            if (Directory.Exists(fPath)) Directory.Delete(fPath, true);
            Directory.CreateDirectory(fPath);
            fPath += Path.DirectorySeparatorChar;
            MakeProjDirs(fPath);
            ExportGGS(fPath + "Global Game Settings.xml", gMWAD);
            ExportHelp(fPath + "Game Information", gMWAD); // adds .txt and .rtf
            ExportExtensions(fPath + "Extension Packages.xml"); // lol.
            ExportConstants(fPath + "Constants.xml", gMWAD);
            ExportScripts(fPath + "Scripts" + Path.DirectorySeparatorChar, gMWAD);
            ExportPaths(fPath + "Paths" + Path.DirectorySeparatorChar, gMWAD);
            ExportFonts(fPath + "Fonts" + Path.DirectorySeparatorChar, gMWAD);
            ExportRooms(fPath + "Rooms" + Path.DirectorySeparatorChar, gMWAD);
            ExportSounds(fPath + "Sounds" + Path.DirectorySeparatorChar, gMWAD);
            ExportTimelines(fPath + "Time Lines" + Path.DirectorySeparatorChar, gMWAD);
            ExportObjects(fPath + "Objects" + Path.DirectorySeparatorChar, gMWAD);
            ExportBackgrounds(fPath + "Backgrounds" + Path.DirectorySeparatorChar, gMWAD);
            ExportSprites(fPath + "Sprites" + Path.DirectorySeparatorChar, gMWAD);
            MakeDummyIcon(fPath + "game icon.ico");
            InvokeGMKSplitter(fPath, gMWAD.Header.Name.Content + ".gmk");
        }

        private static void InvokeGMKSplitter(string p, string o)
        {
            var gmkinfo = new ProcessStartInfo()
            {
                FileName = "java",
                Arguments = "-jar " + AppDomain.CurrentDomain.BaseDirectory + "tools" + Path.DirectorySeparatorChar + "gmksplit.jar " + p + " " + o,
                UseShellExecute = false,
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            var gmkproc = new Process() { StartInfo = gmkinfo };
            gmkproc.Start();
            gmkproc.WaitForExit();
            gmkproc.Close();
            gmkproc.Dispose();
        }

        private static void ConvertAT3ToMP3(string p)
        {
            string appdir = AppDomain.CurrentDomain.BaseDirectory;
            string wavpath = Path.ChangeExtension(p, ".wav");
            string mp3path = Path.ChangeExtension(p, ".mp3");

            try
            {
                File.Delete(wavpath);
                File.Delete(mp3path);
            }
            catch { }

            var at3info = new ProcessStartInfo()
            {
                FileName = appdir + Path.DirectorySeparatorChar + "tools" + Path.DirectorySeparatorChar + "at3tool.exe",
                Arguments = "-d " + p + " " + wavpath,
                UseShellExecute = false
            };
            var at3proc = new Process() { StartInfo = at3info };
            at3proc.Start();
            at3proc.WaitForExit();
            at3proc.Close();
            at3proc.Dispose();

            var ffmpeginfo = new ProcessStartInfo()
            {
                FileName = appdir + Path.DirectorySeparatorChar + "tools" + Path.DirectorySeparatorChar + "ffmpeg.exe",
                Arguments = "-i " + wavpath + " " + mp3path,
                UseShellExecute = false
            };
            var ffmpegproc = new Process() { StartInfo = ffmpeginfo };
            ffmpegproc.Start();
            ffmpegproc.WaitForExit();
            ffmpegproc.Close();
            ffmpegproc.Dispose();

            if (!File.Exists(mp3path))
            {
                Output.Print("Audio conversion failed for sound " + p);
            }

            try
            {
                File.Delete(wavpath);
            }
            catch { }
        }

        private static void MakeDummyIcon(string p)
        {
            // Yes, just to make a 32x32 icon. That many code.

            Bitmap b = new Bitmap(32, 32, PixelFormat.Format24bppRgb);
            Icon i = Icon.FromHandle(b.GetHicon());
            var f = new FileStream(p, FileMode.CreateNew, FileAccess.Write);
            i.Save(f);
            f.Flush(true); // flush everything to disk right here right now.
            f.Dispose();
            i.Dispose();
            b.Dispose();
        }

        private static void ExportSprites(string p, GMWAD w)
        {
            var e = new XElement("resources");
            var scnt = w.Sprites.Items.Count;
            for (int s = 0; s < scnt; s++)
            {
                GMSprite ss = w.Sprites.Items[s];
                if (ss == null) continue;
                string name = ss.Name.Content;
                e.Add(new XElement("resource", new XAttribute("name", name), new XAttribute("type", "RESOURCE")));

                Directory.CreateDirectory(p + name + ".images");

                var sx = new XElement("sprite",
                        new XElement("origin", new XAttribute("x", ss.XOrigin), new XAttribute("y", ss.YOrigin)),
                        new XElement("mask",
                            new XElement("separate", ss.MasksCount == ss.ImageCount),
                            new XElement("shape", ss.BBoxMode.ToString()),
                            new XElement("bounds", new XAttribute("alphaTolerance", 0), new XAttribute("mode", "AUTO"))
                        ),
                        new XElement("preload", ss.Preload),
                        new XElement("smoothEdges", ss.Smooth),
                        new XElement("transparent", "false") // causes sprites to lose black color o_O
                    );
                var sd = new XDocument(sx);
                sd.Declaration = new XDeclaration("1.0", "UTF-8", "no");
                sd.Save(p + name + ".xml");

                string imgFPath = p + name + ".images" + Path.DirectorySeparatorChar;
                Directory.CreateDirectory(imgFPath);
                for (int f = 0; f < ss.ImageCount; f++)
                {
                    var tex = ss.ImageTextures[f];
                    tex.TextureItem.Save(imgFPath + "image " + f.ToString() + ".png", ImageFormat.Png);
                }
            }

            var d = new XDocument(e);
            d.Declaration = new XDeclaration("1.0", "UTF-8", "no");
            d.Save(p + "_resources.list.xml");
        }

        private static void ExportBackgrounds(string p, GMWAD w)
        {
            var e = new XElement("resources");
            var bcnt = w.Backgrounds.Items.Count;
            for (int b = 0; b < bcnt; b++)
            {
                var bb = w.Backgrounds.Items[b];
                if (bb == null) continue;
                string name = bb.Name.Content;

                e.Add(new XElement("resource", new XAttribute("name", name), new XAttribute("type", "RESOURCE")));

                XElement xtt;
                if (bb.IsTileset)
                {
                    xtt = new XElement("tiles",
                                // This info is not stored in .psp :(
                                new XElement("size", new XAttribute("height", 16), new XAttribute("width", 16)),
                                new XElement("offset", new XAttribute("x", 0), new XAttribute("y", 0)),
                                new XElement("separation", new XAttribute("x", 0), new XAttribute("y", 0)));
                }
                else xtt = null;

                var xbb = new XElement("background",
                        new XElement("useAsTileset", bb.IsTileset),
                        xtt,
                        new XElement("preload", bb.Preload),
                        new XElement("smoothEdges", bb.Smooth),
                        new XElement("transparent", "false") // same as sprites...
                    );
                var xdd = new XDocument(xbb);
                xdd.Declaration = new XDeclaration("1.0", "UTF-8", "no");
                xdd.Save(p + name + ".xml");

                bb.Texture.TextureItem.Save(p + name + ".png", ImageFormat.Png);
            }

            var d = new XDocument(e);
            d.Declaration = new XDeclaration("1.0", "UTF-8", "no");
            d.Save(p + "_resources.list.xml");
        }

        private static string GetFriendlyEventName(int i, GMGMLEvent e, GMWAD wad, GMObject self)
        {
            #region // Generate dictionaries with event names, taken from GMKSplitter src.

            string[] step_names = new string[3] { "Step", "Begin Step", "End Step" };

            // yes I know that this is cursed, I copied that from gmksplit source code.
            Dictionary<int, string> other_names = new Dictionary<int, string>();
            other_names.Add(0, "Outside Room");
            other_names.Add(1, "Intersect Boundary");
            other_names.Add(2, "Game Start");
            other_names.Add(3, "Game End");
            other_names.Add(4, "Room Start");
            other_names.Add(5, "Room End");
            other_names.Add(6, "No more lives");
            other_names.Add(7, "Animation end");
            other_names.Add(8, "End of Path");
            other_names.Add(9, "No more health");
            other_names.Add(30, "Close Button");

            Dictionary<int, string> mouse_names = new Dictionary<int, string>();
            mouse_names.Add(0, "Mouse left button");
            mouse_names.Add(1, "Mouse right button");
            mouse_names.Add(2, "Mouse middle button");
            mouse_names.Add(3, "Mouse no button");
            mouse_names.Add(4, "Mouse left button pressed");
            mouse_names.Add(5, "Mouse right button pressed");
            mouse_names.Add(6, "Mouse middle button pressed");
            mouse_names.Add(7, "Mouse left button released");
            mouse_names.Add(8, "Mouse right button released");
            mouse_names.Add(9, "Mouse middle button released");
            mouse_names.Add(10, "Mouse enter");
            mouse_names.Add(11, "Mouse leave");
            mouse_names.Add(16, "Joystick 1 left");
            mouse_names.Add(17, "Joystick 1 right");
            mouse_names.Add(18, "Joystick 1 up");
            mouse_names.Add(19, "Joystick 1 down");
            mouse_names.Add(21, "Joystick 1 button 1");
            mouse_names.Add(22, "Joystick 1 button 2");
            mouse_names.Add(23, "Joystick 1 button 3");
            mouse_names.Add(24, "Joystick 1 button 4");
            mouse_names.Add(25, "Joystick 1 button 5");
            mouse_names.Add(26, "Joystick 1 button 6");
            mouse_names.Add(27, "Joystick 1 button 7");
            mouse_names.Add(28, "Joystick 1 button 8");
            mouse_names.Add(31, "Joystick 2 left");
            mouse_names.Add(32, "Joystick 2 right");
            mouse_names.Add(33, "Joystick 2 up");
            mouse_names.Add(34, "Joystick 2 down");
            mouse_names.Add(36, "Joystick 2 button 1");
            mouse_names.Add(37, "Joystick 2 button 2");
            mouse_names.Add(38, "Joystick 2 button 3");
            mouse_names.Add(39, "Joystick 2 button 4");
            mouse_names.Add(40, "Joystick 2 button 5");
            mouse_names.Add(41, "Joystick 2 button 6");
            mouse_names.Add(42, "Joystick 2 button 7");
            mouse_names.Add(43, "Joystick 2 button 8");
            mouse_names.Add(50, "Mouse global left button");
            mouse_names.Add(51, "Mouse global right button");
            mouse_names.Add(52, "Mouse global middle button");
            mouse_names.Add(53, "Mouse global left pressed");
            mouse_names.Add(54, "Mouse global right pressed");
            mouse_names.Add(55, "Mouse global middle pressed");
            mouse_names.Add(56, "Mouse global left released");
            mouse_names.Add(57, "Mouse global right released");
            mouse_names.Add(58, "Mouse global middle released");
            mouse_names.Add(60, "Mouse wheel up");
            mouse_names.Add(61, "Mouse wheel down");

            Dictionary<int, string> keyboard_names = new Dictionary<int, string>();

            keyboard_names.Add(37, "left");
            keyboard_names.Add(39, "right");
            keyboard_names.Add(38, "up");
            keyboard_names.Add(40, "down");

            keyboard_names.Add(17, "control");
            keyboard_names.Add(18, "alt");
            keyboard_names.Add(16, "shift");
            keyboard_names.Add(32, "space");
            keyboard_names.Add(13, "enter");

            keyboard_names.Add(96, "numpad 0");
            keyboard_names.Add(97, "numpad 1");
            keyboard_names.Add(98, "numpad 2");
            keyboard_names.Add(99, "numpad 3");
            keyboard_names.Add(100, "numpad 4");
            keyboard_names.Add(101, "numpad 5");
            keyboard_names.Add(102, "numpad 6");
            keyboard_names.Add(103, "numpad 7");
            keyboard_names.Add(104, "numpad 8");
            keyboard_names.Add(105, "numpad 9");

            keyboard_names.Add(111, "numpad divide");
            keyboard_names.Add(106, "numpad multiply");
            keyboard_names.Add(109, "numpad subtract");
            keyboard_names.Add(107, "numpad add");
            keyboard_names.Add(110, "numpad decimal");

            keyboard_names.Add(48, "0");
            keyboard_names.Add(49, "1");
            keyboard_names.Add(50, "2");
            keyboard_names.Add(51, "3");
            keyboard_names.Add(52, "4");
            keyboard_names.Add(53, "5");
            keyboard_names.Add(54, "6");
            keyboard_names.Add(55, "7");
            keyboard_names.Add(56, "8");
            keyboard_names.Add(57, "9");

            keyboard_names.Add(65, "A");
            keyboard_names.Add(66, "B");
            keyboard_names.Add(67, "C");
            keyboard_names.Add(68, "D");
            keyboard_names.Add(69, "E");
            keyboard_names.Add(70, "F");
            keyboard_names.Add(71, "G");
            keyboard_names.Add(72, "H");
            keyboard_names.Add(73, "I");
            keyboard_names.Add(74, "J");
            keyboard_names.Add(75, "K");
            keyboard_names.Add(76, "L");
            keyboard_names.Add(77, "M");
            keyboard_names.Add(78, "N");
            keyboard_names.Add(79, "O");
            keyboard_names.Add(80, "P");
            keyboard_names.Add(81, "Q");
            keyboard_names.Add(82, "R");
            keyboard_names.Add(83, "S");
            keyboard_names.Add(84, "T");
            keyboard_names.Add(85, "U");
            keyboard_names.Add(86, "V");
            keyboard_names.Add(87, "W");
            keyboard_names.Add(88, "X");
            keyboard_names.Add(89, "Y");
            keyboard_names.Add(90, "Z");

            keyboard_names.Add(112, "f1");
            keyboard_names.Add(113, "f2");
            keyboard_names.Add(114, "f3");
            keyboard_names.Add(115, "f4");
            keyboard_names.Add(116, "f5");
            keyboard_names.Add(117, "f6");
            keyboard_names.Add(118, "f7");
            keyboard_names.Add(119, "f8");
            keyboard_names.Add(120, "f9");
            keyboard_names.Add(121, "f10");
            keyboard_names.Add(122, "f11");
            keyboard_names.Add(123, "f12");

            keyboard_names.Add(8, "backspace");
            keyboard_names.Add(27, "escape");
            keyboard_names.Add(36, "home");
            keyboard_names.Add(35, "end");
            keyboard_names.Add(33, "pageup");
            keyboard_names.Add(34, "pagedown");
            keyboard_names.Add(46, "delete");
            keyboard_names.Add(45, "insert");

            keyboard_names.Add(0, "no key");
            keyboard_names.Add(1, "any key");

            #endregion

            EventName i_e = (EventName)i;
            switch (i_e)
            {
                case EventName.EV_CREATE:
                    return "Create";
                case EventName.EV_DRAW:
                    return "Draw";
                case EventName.EV_DESTROY:
                    return "Destroy";
                case EventName.EV_STEP:
                    return step_names[e.Key];
                case EventName.EV_ALARM:
                    return $"Alarm {e.Key}";
                case EventName.EV_COLLISION:
                    {
                        var supposed_gmobj = e.Key;
                        GMObject gmobj_ref;
                        if (supposed_gmobj > -1)
                        {
                            gmobj_ref = wad.Objects.Items[supposed_gmobj];
                            return "Collision with " + gmobj_ref.Name.Content;
                        }
                        else if (supposed_gmobj == -1)
                            return "Collision with " + self.Name.Content;
                        else throw new Exception("what.");
                        
                    }
                case EventName.EV_OTHER:
                    {
                        if (other_names.ContainsKey(e.Key))
                            return other_names[e.Key];
                        else if (e.Key >= 10 && e.Key <= 25)
                            return "User Event " + (e.Key - 10).ToString();
                        else if (e.Key >= 40 && e.Key <= 47)
                            return "Outside View " + (e.Key - 40).ToString();
                        else if (e.Key >= 50 && e.Key <= 57)
                            return "Boundary View " + (e.Key - 50).ToString();
                        else
                            return "Other Event " + e.Key.ToString();
                    }
                case EventName.EV_MOUSE:
                    {
                        if (mouse_names.ContainsKey(e.Key))
                            return mouse_names[e.Key];
                        else
                            return "Mouse unknown " + e.Key.ToString();
                    }
                case EventName.EV_KEYBOARD:
                case EventName.EV_KEYPRESS:
                case EventName.EV_KEYRELEASE:
                    {
                        if (keyboard_names.ContainsKey(e.Key))
                            return "Key " + keyboard_names[e.Key];
                        else
                            return "Key code " + e.Key.ToString();
                    }
                case EventName.EV_TRIGGER: // interesting...
                    {
                        return "Trigger id " + e.Key.ToString();
                    }
                default:
                    return $"Error oh and btw lojical sux like {i} times";
                    // in C# switch-cases must have a default: statement so I made one!
            }
        }

        private static string GetActionArgumentValue(ArgTypes type, GMString str, GMWAD wad)
        {
            switch (type)
            {
                case ArgTypes.GMOBJECT:
                    {
                        int ind = int.Parse(str.Content);
                        return wad.Objects.Items[ind].Name.Content;
                    }
                case ArgTypes.BACKGROUND:
                    {
                        int ind = int.Parse(str.Content);
                        return wad.Backgrounds.Items[ind].Name.Content;
                    }
                case ArgTypes.ROOM:
                    {
                        int ind = int.Parse(str.Content);
                        return wad.Rooms.Items[ind].Name.Content;
                    }
                case ArgTypes.SPRITE:
                    {
                        int ind = int.Parse(str.Content);
                        return wad.Sprites.Items[ind].Name.Content;
                    }
                case ArgTypes.SOUND:
                    {
                        int ind = int.Parse(str.Content);
                        return wad.Sounds.Items[ind].Name.Content;
                    }
                case ArgTypes.TIMELINE:
                    {
                        int ind = int.Parse(str.Content);
                        return wad.Timelines.Items[ind].Name.Content;
                    }
                case ArgTypes.PATH:
                    {
                        int ind = int.Parse(str.Content);
                        return wad.Paths.Items[ind].Name.Content;
                    }
                default:
                    {
                        return str.Content;
                    }
            }
        }

        private static void ExportObjects(string p, GMWAD w)
        {
            var e = new XElement("resources");
            var ocnt = w.Objects.Items.Count;
            for (int o = 0; o < ocnt; o++)
            {
                var oo = w.Objects.Items[o];
                if (oo == null) continue;
                string name = oo.Name.Content;

                e.Add(new XElement("resource", new XAttribute("name", name), new XAttribute("type", "RESOURCE")));

                XElement xpa, xmm, xspr;
                var op = oo.Parent;
                if (op == null) xpa = new XElement("parent");
                else xpa = new XElement("parent", op.Name.Content);
                var om = oo.Mask;
                if (om == null) xmm = new XElement("mask");
                else xmm = new XElement("mask", om.Name.Content);

                var os = oo.Sprite;
                if (os == null) xspr = new XElement("sprite");
                else xspr = new XElement("sprite", os.Name.Content);


                var xobj = new XElement("object",
                        new XAttribute("id", o),
                        xspr,
                        new XElement("solid", oo.Solid),
                        new XElement("visible", oo.Visible),
                        new XElement("depth", oo.Depth),
                        new XElement("persistent", oo.Persistent),
                        xpa,
                        xmm
                    );

                var xddd = new XDocument(xobj);
                xddd.Declaration = new XDeclaration("1.0", "UTF-8", "no");
                xddd.Save(p + name + ".xml");

                // Export object events
                Directory.CreateDirectory(p + name + ".events");
                for (int ee = 0; ee < oo.Events.Count; ee++)
                {
                    var eee = oo.Events[ee];
                    if (eee.Count == 0) continue; // the event doesn't exist (it's empty)
                    string ee_n = GetFriendlyEventName(ee, eee[0], w, oo);
                    var actx = new XElement("actions");
                    for (int ae = 0; ae < eee[0].Count; ae++)
                    {
                        var acto = eee[0].Actions[ae];

                        string whos;
                        if (acto.Who == -1) whos = ".self";
                        else if (acto.Who == -2) whos = ".other";
                        else whos = w.Objects.Items[acto.Who].Name.Content;

                        XElement actargs = new XElement("arguments");
                        for (int ar = 0; ar < acto.ArgumentCount; ar++)
                        {
                            var arg_list = acto.ArgTypesList;
                            var arg_l2 = acto.Arguments;
                            actargs.Add(
                                    new XElement("argument",
                                        new XAttribute("kind", arg_list[ar].ToString()),
                                        GetActionArgumentValue(arg_list[ar], arg_l2[ar], w)
                                    )
                                );
                        }

                        actx.Add(
                            new XElement("action",
                                new XAttribute("id", acto.ID),
                                new XAttribute("library", acto.LibID),
                                new XComment("action name: " + acto.Name.Content),
                                new XElement("kind", acto.Kind.ToString()),
                                new XElement("allowRelative", acto.UseRelative),
                                new XElement("question", acto.IsQuestion),
                                new XElement("canApplyTo", acto.UseApplyTo),
                                new XElement("actionType", acto.ExeType.ToString()),
                                new XElement("functionName", ""), // TODO
                                new XElement("relative", acto.Relative),
                                new XElement("not", acto.IsNot),
                                new XElement("appliesTo", whos),
                                actargs
                                )
                            );
                    }
                    var eex = new XElement("event",
                            new XAttribute("category", ((EventName)ee).ToString().Replace("EV_", string.Empty)),
                            (ee != 4) ? new XAttribute("id", eee[0].Key) : new XAttribute("with", w.Objects.Items[eee[0].Key].Name.Content),
                            actx
                        );
                    var eedoc = new XDocument(eex);
                    eedoc.Declaration = new XDeclaration("1.0", "UTF-8", "no");
                    eedoc.Save(p + name + ".events" + Path.DirectorySeparatorChar + ee_n + ".xml");
                }
            }

            var d = new XDocument(e);
            d.Declaration = new XDeclaration("1.0", "UTF-8", "no");
            d.Save(p + "_resources.list.xml");
        }

        private static void ExportTimelines(string p, GMWAD w)
        {
            var e = new XElement("resources");
            var tcnt = w.Timelines.Items.Count;
            for (int t = 0; t < tcnt; t++)
            {
                var tt = w.Timelines.Items[t];
                if (tt == null) continue;
                string name = w.Timelines.Items[t].Name.Content;
                e.Add(new XElement("resource", new XAttribute("name", name), new XAttribute("type", "RESOURCE")));

                var tx = new XElement("timeline");
                for (int tm = 0; tm < tt.MomentCount; tm++)
                {
                    var mmm = tt.Moments[tm];
                    var mm = new XElement("moment", new XAttribute("stepNo", mmm.Point.ToString()));
                    for (int ta = 0; ta < mmm.Event.Count; ta++)
                    {
                        var taa = mmm.Event.Actions[ta];
                        XElement actf, args;
                        if (taa.Name.Content == "") actf = new XElement("functionName");
                        else actf = new XElement("functionName", taa.Name.Content);

                        if (taa.ArgumentCount == 0) args = new XElement("arguments");
                        else
                        {
                            args = new XElement("arguments");
                            for (int a = 0; a < taa.ArgumentCount; a++)
                            {
                                var arg_list = taa.ArgTypesList;
                                var arg_l2 = taa.Arguments;
                                args.Add(
                                        new XElement("argument", new XAttribute("kind", arg_list[a].ToString()), GetActionArgumentValue(arg_list[a], arg_l2[a], w)));
                            }
                        }

                        string whos;
                        if (taa.Who == -1) whos = ".self";
                        else if (taa.Who == -2) whos = ".other";
                        else whos = w.Objects.Items[taa.Who].Name.Content;


                        var actx = new XElement("action",
                                new XAttribute("id", taa.ID),
                                new XAttribute("library", taa.LibID),
                                new XComment("action name: " + taa.Name),
                                new XElement("kind", taa.Kind.ToString()),
                                new XElement("allowRelative", taa.UseRelative),
                                new XElement("question", taa.IsQuestion),
                                new XElement("canApplyTo", taa.UseApplyTo),
                                new XElement("actionType", taa.ExeType.ToString()),
                                actf,
                                new XElement("relative", taa.Relative),
                                new XElement("not", taa.IsNot),
                                new XElement("appliesTo", whos),
                                args
                            );

                        mm.Add(actx);
                    }

                    tx.Add(mm);
                }
                var td = new XDocument(tx);
                td.Declaration = new XDeclaration("1.0", "UTF-8", "no");
                td.Save(p + name + ".xml");
            }

            var d = new XDocument(e);
            d.Declaration = new XDeclaration("1.0", "UTF-8", "no");
            d.Save(p + "_resources.list.xml");
        }

        public enum SoundKind
        {
            NORMAL, BACKGROUND, SPATIAL, MULTIMEDIA
        }

        private static void ExportSounds(string p, GMWAD w)
        {
            var e = new XElement("resources");
            var scnt = w.Sounds.Items.Count;
            for (int s = 0; s < scnt; s++)
            {
                var ss = w.Sounds.Items[s];
                if (ss == null) continue;
                string name = w.Sounds.Items[s].Name.Content;
                e.Add(new XElement("resource", new XAttribute("name", name), new XAttribute("type", "RESOURCE")));

                var sx = new XElement("sound",
                    new XElement("filename", ss.OrigName.Content),
                    new XElement("filetype", ss.FileExtension.Content),
                    new XElement("kind", ((SoundKind)ss.Kind).ToString()),
                    new XElement("pan", ss.Pan),
                    new XElement("volume", ss.Volume),
                    new XElement("preload", ss.Preload),
                    new XElement("effects", // none of the two games seem to use these...
                        new XElement("chorus", ss.EffectArr[(int)GMSound.EffectEnum.CHORUS]),
                        new XElement("echo", ss.EffectArr[(int)GMSound.EffectEnum.ECHO]),
                        new XElement("flanger", ss.EffectArr[(int)GMSound.EffectEnum.FLANGER]),
                        new XElement("gargle", ss.EffectArr[(int)GMSound.EffectEnum.GARGLE]),
                        new XElement("reverb", ss.EffectArr[(int)GMSound.EffectEnum.REVERB]))
                    );
                var sdd = new XDocument(sx);
                sdd.Declaration = new XDeclaration("1.0", "UTF-8", "no");
                sdd.Save(p + name + ".xml");
                byte[] snddat = w.AudioFiles.Items[ss.SoundID].Data;
                byte[] empty128 = new byte[128];
                for (int zzz = 0; zzz < empty128.Length; zzz++) empty128[zzz] = 0x00;
                if (snddat.SequenceEqual(empty128))
                {
                    Output.Print("Sound data is an empty wav. Trying to convert...");
                    ConvertAT3ToMP3(Path.GetDirectoryName(w.WADPath) + Path.DirectorySeparatorChar + Path.ChangeExtension(ss.OrigName.Content, ".at3"));
                    File.Copy(Path.GetDirectoryName(w.WADPath) + Path.DirectorySeparatorChar + ss.OrigName.Content, p + name + ss.FileExtension.Content);
                }
                else File.WriteAllBytes(p + name + ".wav", snddat);
                // TODO
            }

            var d = new XDocument(e);
            d.Declaration = new XDeclaration("1.0", "UTF-8", "no");
            d.Save(p + "_resources.list.xml");
        }

        private static void ExportRooms(string p, GMWAD w)
        {
            var e = new XElement("resources");
            var rcnt = w.Rooms.Items.Count;
            for (int r = 0; r < rcnt; r++)
            {
                var ro = w.Rooms.Items[r];
                if (ro == null) continue;
                string name = w.Rooms.Items[r].Name.Content;

                var rb = new XElement("backgrounds");
                for (int b = 0; b < ro.Backgrounds.Count; b++)
                {
                    var bb = ro.Backgrounds[b];

                    XElement bi;
                    if (bb.Background != null)
                    {
                        bi = new XElement("backgroundImage", bb.Background.Name.Content);
                    }
                    else
                    {
                        bi = new XElement("backgroundImage");
                    }

                    rb.Add(
                            new XElement("backgroundDef",
                                new XElement("visibleOnRoomStart", bb.Visible),
                                new XElement("isForeground", bb.Foreground),
                                bi,
                                new XElement("offset", new XAttribute("x", bb.X), new XAttribute("y", bb.Y)),
                                new XElement("speed", new XAttribute("x", bb.HSpeed), new XAttribute("y", bb.VSpeed)),
                                new XElement("tileHorizontally", bb.HTiled),
                                new XElement("tileVertically", bb.VTiled),
                                new XElement("stretch", bb.Stretch)
                            )
                        );
                }

                var rv = new XElement("views");
                if (ro.EnableViews)
                {
                    for (int v = 0; v < 8; v++)
                    {
                        var vv = ro.Views[v];
                        var robj = vv.Object;

                        XElement rox;
                        if (robj != null)
                        {
                            rox = new XElement(
                                        new XElement("objectFollowing",
                                        new XAttribute("hBorder", vv.HBorder),
                                        new XAttribute("hSpeed", vv.HSpeed),
                                        new XAttribute("vBorder", vv.VBorder),
                                        new XAttribute("vSpeed", vv.VSpeed),
                                        robj.Name.Content)
                                );
                        }
                        else
                        {
                            rox = new XElement(
                                    new XElement("objectFollowing",
                                    new XAttribute("hBorder", vv.HBorder),
                                    new XAttribute("hSpeed", vv.HSpeed),
                                    new XAttribute("vBorder", vv.VBorder),
                                    new XAttribute("vSpeed", vv.VSpeed)
                                    )
                            );
                        }

                        rv.Add(
                            new XElement("view",
                                new XElement("visibleOnRoomStart", vv.Visible),
                                new XElement("viewInRoom",
                                    new XAttribute("height", vv.ViewCoords[3]),
                                    new XAttribute("width", vv.ViewCoords[2]),
                                    new XAttribute("x", vv.ViewCoords[0]),
                                    new XAttribute("y", vv.ViewCoords[1])),
                                new XElement("portOnScreen",
                                    new XAttribute("height", vv.PortCoords[3]),
                                    new XAttribute("width", vv.PortCoords[2]),
                                    new XAttribute("x", vv.ViewCoords[0]),
                                    new XAttribute("y", vv.ViewCoords[1])),
                                rox)
                            );
                    }
                }
                var ri = new XElement("instances");
                for (int ins = 0; ins < ro.Instances.Count; ins++)
                {
                    var rins = ro.Instances[ins];
                    var ix = new XElement("instance",
                            new XElement("object", rins.Object.Name.Content), // TODO
                            new XElement("position", new XAttribute("x", rins.X), new XAttribute("y", rins.Y)),
                            new XElement("creationCode", rins.CreationCode.Content),
                            new XElement("locked", "false")
                        );
                    ri.Add(ix);
                }
                var rt = new XElement("tiles");
                for (int tt = 0; tt < ro.Tiles.Count; tt++)
                {
                    var rtt = ro.Tiles[tt];
                    var xtt = new XElement("tile",
                            new XElement("background", rtt.Background.Name.Content), // TODO!!!!
                            new XElement("backgroundPosition", new XAttribute("x", rtt.XOffset), new XAttribute("y", rtt.YOffset)),
                            new XElement("roomPosition", new XAttribute("x", rtt.X), new XAttribute("y", rtt.Y)),
                            new XElement("size", new XAttribute("height", rtt.Height), new XAttribute("width", rtt.Width)),
                            new XElement("depth", rtt.Depth),
                            new XElement("locked", "false")
                        );
                    rt.Add(xtt);
                }
                var rx = new XDocument(
                        new XElement("room",
                        new XElement("caption", ro.Caption.Content),
                        new XElement("size", new XAttribute("height", ro.Height), new XAttribute("width", ro.Width)),
                        new XElement("grid",
                            new XElement("isometric", "false"),
                            new XElement("snap", new XAttribute("x", 16), new XAttribute("y", 16))
                            ),
                        new XElement("speed", ro.Speed),
                        new XElement("persistent", ro.Persistent),
                        new XElement("creationCode", ro.CreationCode),
                        new XElement("backgroundColor", ro.Color.ToString()),
                        new XElement("drawBackgroundColor", ro.ShowColor),
                        rb,
                        new XElement("enableViews", ro.EnableViews),
                        ro.EnableViews ? rv : null,
                        ri,
                        rt,
                        new XElement("editorSettings", new XAttribute("remember", "false"))
                        )
                    );
                rx.Declaration = new XDeclaration("1.0", "UTF-8", "no");
                rx.Save(p + name + ".xml");
            }

            // Rooms have a special "Room Order"
            for (int rord = 0; rord < w.Header.RoomOrderCount; rord++)
            {
                string name = w.Header.RoomOrder[rord].Name.Content;
                e.Add(
                    new XElement("resource", new XAttribute("name", name), new XAttribute("type", "RESOURCE"))
                    );
            }

            var d = new XDocument(e);
            d.Declaration = new XDeclaration("1.0", "UTF-8", "no");
            d.Save(p + "_resources.list.xml");
        }

        private static void ExportFonts(string p, GMWAD w)
        {
            var e = new XElement("resources");
            var fcnt = w.Fonts.Items.Count;
            for (int f = 0; f < fcnt; f++)
            {
                
                var fo = w.Fonts.Items[f];
                if (fo == null) continue;
                string name = w.Fonts.Items[f].Name.Content;
                e.Add(
                    new XElement("resource", new XAttribute("name", name), new XAttribute("type", "RESOURCE"))
                    );

                var fx = new XDocument(
                        new XElement("font",

                        new XElement("fontName", fo.FontName),
                        new XElement("bold", fo.Bold),
                        new XElement("italic", fo.Italic),
                        new XElement("rangeMin", fo.First),
                        new XElement("rangeMax", fo.Last),
                        new XElement("size", fo.FontSize)
                        )
                    );
                fx.Declaration = new XDeclaration("1.0", "UTF-8", "no");
                fx.Save(p + name + ".xml");
            }

            var d = new XDocument(e);
            d.Declaration = new XDeclaration("1.0", "UTF-8", "no");
            d.Save(p + "_resources.list.xml");
        }

        private static void ExportPaths(string p, GMWAD w)
        {
            var e = new XElement("resources");
            var pcnt = w.Paths.Items.Count;
            for (int pa = 0; pa < pcnt; pa++)
            {
                if (w.Paths.Items[pa] == null) continue;
                string name = w.Paths.Items[pa].Name.Content;
                e.Add(
                        new XElement("resource", new XAttribute("name", name), new XAttribute("type", "RESOURCE"))
                    );
                var po = new XElement("points");
                var pocnt = w.Paths.Items[pa].PointsCount;
                for (int poi = 0; poi < pocnt; poi++)
                {
                    var pox = new XElement("point",
                            new XAttribute("speed", w.Paths.Items[pa].Points[poi].Speed),
                            new XAttribute("x", w.Paths.Items[pa].Points[poi].X),
                            new XAttribute("y", w.Paths.Items[pa].Points[poi].Y)
                        );
                    po.Add(pox);
                }
                var pd = new XDocument(
                        new XElement("path",
                        po,
                        new XElement("backgroundRoom"),
                        new XElement("closed", w.Paths.Items[pa].Closed),
                        new XElement("precision", w.Paths.Items[pa].Precision),
                        new XElement("smooth", w.Paths.Items[pa].SmoothKind),
                        new XElement("snap", new XAttribute("x", 16), new XAttribute("y", 16)) // default value is 16.
                        )
                    );
                pd.Declaration = new XDeclaration("1.0", "UTF-8", "no");
                pd.Save(p + name + ".xml");
            }

            var d = new XDocument(
                    e
                );
            d.Declaration = new XDeclaration("1.0", "UTF-8", "no");
            d.Save(p + "_resources.list.xml");
        }

        private static void ExportScripts(string p, GMWAD w)
        {
            var e = new XElement("resources");
            var scnt = w.Scripts.Items.Count;
            for (int s = 0; s < scnt; s++)
            {
                if (w.Scripts.Items[s] == null) continue;
                string name = w.Scripts.Items[s].Name.Content;
                string code = w.Scripts.Items[s].Code.Content;
                e.Add(
                    new XElement("resource", new XAttribute("name", name), new XAttribute("type", "RESOURCE"))
                    );
                File.WriteAllText(p + name + ".gml", code);
            }

            var d = new XDocument(
                    e
                );
            d.Declaration = new XDeclaration("1.0", "UTF-8", "no");
            d.Save(p + "_resources.list.xml");
        }

        private static void ExportConstants(string p, GMWAD w)
        {
            var e = new XElement("constants");
            var cnt = w.Options.ConstantCount;
            for (int c = 0; c < cnt; c++)
            {
                if (w.Options.Constants[c] == null) continue;
                string name = w.Options.Constants[c].Name.Content;
                string value = w.Options.Constants[c].Name.Content;
                e.Add(
                    new XElement("constant", new XAttribute("name", name), new XAttribute("value", value))
                    );
            }
            var d = new XDocument(
                    e
                );
            d.Declaration = new XDeclaration("1.0", "UTF-8", "no");
            d.Save(p);
        }

        private static void ExportExtensions(string p)
        {
            // As I said, extensions weren't implemented at that time.
            File.WriteAllText(p, "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>\r\n<extensionPackages/>\r\n", Encoding.UTF8);
        }

        private static void ExportHelp(string p, GMWAD w)
        {
            File.WriteAllText(p + ".txt", w.Help.Text.Content);
            XDocument d = new XDocument(
                    new XElement("gameInformation",
                        new XElement("windowPosition",
                            new XElement("left", w.Help.Left),
                            new XElement("top", w.Help.Top),
                            new XElement("width", w.Help.Width),
                            new XElement("height", w.Help.Height)),
                        new XElement("allowResize", "true"),
                        new XElement("backgroundColor", w.Help.BackgroundColor.ToString()),
                        new XElement("formCaption", w.Help.Caption.Content),
                        new XElement("mimicGameWindow", w.Help.Mimic),
                        new XElement("pauseGame", w.Help.Modal),
                        new XElement("showBorder", w.Help.Border),
                        new XElement("stayOnTop", w.Help.OnTop))
                );
            d.Declaration = new XDeclaration("1.0", "UTF-8", "no");
            d.Save(p + ".xml");
        }

        private static void ExportGGS(string p, GMWAD w)
        {
            XDocument d = new XDocument(
                    new XElement("settings",
                        new XElement("graphics",
                            new XElement("scalingPercent", w.Options.Scale),
                            new XElement("displayCursor", w.Options.ShowCursor),
                            new XElement("useVsync", w.Options.VSync),
                            new XElement("interpolateColors", w.Options.InterpolatePixels),
                            new XElement("colorOutsideRoom", w.Options.WindowColor.ToString())
                        ),
                        new XElement("windowing",
                            new XElement("startFullscreen", w.Options.FullScreen),
                            new XElement("dontDrawBorder", w.Options.NoBorder),
                            new XElement("allowWindowResize", w.Options.Sizeable),
                            new XElement("alwaysOnTop", w.Options.StayOnTop),
                            new XElement("dontShowButtons", w.Options.NoButtons),
                            new XElement("switchVideoMode", w.Options.ChangeResolution)),
                        new XElement("splashImage",
                            new XElement("showCustom", "false"),
                            new XElement("partiallyTransparent", w.Options.LoadTransparent),
                            new XElement("alphaTransparency", w.Options.LoadAlpha)),
                        new XElement("progressBar",
                            new XElement("mode", "LOADBAR_DEFAULT"),
                            new XElement("scaleImage", w.Options.ScaleProgress)),
                        new XElement("keys",
                            new XElement("letF1ShowGameInfo", w.Options.HelpKey),
                            new XElement("letF4SwitchFullscreen", w.Options.ScreenKey),
                            new XElement("letF5SaveF6Load", w.Options.SaveKey),
                            new XElement("letF9Screenshot", w.Options.ScreenshotKey),
                            new XElement("letEscEndGame", w.Options.QuitKey),
                            new XElement("treatCloseAsEscape", w.Options.CloseSec)),
                        new XElement("errors",
                            new XElement("displayErrors", w.Options.DisplayErrors),
                            new XElement("writeToLog", w.Options.WriteErrors),
                            new XElement("abortOnError", w.Options.AbortErrors),
                            new XElement("treatUninitializedAsZero", w.Options.VariableErrors)),
                        new XElement("gameInfo",
                            new XElement("gameId", w.Header.ID),
                            new XElement("author", "RussellReader"),
                            new XElement("version", "228"), // kek.
                            new XElement("information", "woah hello there. lojical sucks btw."),
                            new XElement("versionMajor", 1), // I couldn't find any version related info in .psp
                            new XElement("versionMinor", 0),
                            new XElement("versionRelease", 0),
                            new XElement("versionBuild", 0),
                            new XElement("company", "nkrapivindev"),
                            new XElement("product", "a game"),
                            new XElement("copyright", "russell senpai notice me ltd."),
                            new XElement("description", "a description"),
                            new XElement("directPlayGuid", "00000000000000000000000000000000")), // in .psp the guid is always empty.
                        new XElement("system",
                            new XElement("processPriority", ((Priority)w.Options.Priority).ToString()),
                            new XElement("disableScreensavers", "false"),
                            new XElement("freezeOnLoseFocus", w.Options.Freeze)),
                        new XElement("includes", // what.
                            new XElement("overwriteExisting", "false"),
                            new XElement("removeAtGameEnd", "false"),
                            new XElement("useTempFolder", "false")))
                );
            d.Declaration = new XDeclaration("1.0", "UTF-8", "no");
            d.Save(p);
        }

        private enum Priority
        {
            PRIORITY_NORMAL,
            PRIORITY_HIGH,
            PRIORITY_HIGHEST
        }

        private static void MakeProjDirs(string path)
        {
            Directory.CreateDirectory(path + "Backgrounds");
            Directory.CreateDirectory(path + "Fonts");
            Directory.CreateDirectory(path + "Objects");
            Directory.CreateDirectory(path + "Paths");
            Directory.CreateDirectory(path + "Rooms");
            Directory.CreateDirectory(path + "Scripts");
            Directory.CreateDirectory(path + "Sounds");
            Directory.CreateDirectory(path + "Sprites");
            Directory.CreateDirectory(path + "Time Lines");
        }
    }
}

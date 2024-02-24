using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace FontConverterTFT
{
    internal class Program
    {
        /// <summary>
        /// Creates a png file for a single character for testing the quality.
        /// </summary>
        /// <remarks>
        /// The file name is created using the character and its code, with a "Bitmap " prefix.
        /// </remarks>
        /// <param name="font">The <see cref="Font"/> including the size.</param>
        /// <param name="c">The character to render.</param>
        private static void TestCharacter(Font font, char c, string path)
        {
            using (Bitmap result = CharacterRenderer.Render(font, c, out var crops))
            {
                Console.Write(c);
                Console.Write(' ');
                Console.Write(result.Width);
                Console.Write(", ");
                Console.Write(result.Height);
                Console.Write(", Left ");
                Console.Write(crops.Left);
                Console.Write(", Right ");
                Console.Write(crops.Right);
                Console.Write(", Top ");
                Console.Write(crops.Top);
                Console.Write(", Bottom ");
                Console.Write(crops.Bottom);

                result.Save(Path.Combine(path, "Bitmap " + c + $"{(int)c}.png"), ImageFormat.Png);
            }
        }

        /// <summary>
        /// Displays a short documentation.
        /// </summary>
        /// <param name="additional">Additional text, usually for exception information.</param>
        private static void Using(string additional = null)
        {
            Console.Error.WriteLine("Using:\nFontConverterTFT /p:<path> {/f:<family>|/n:<ttf>} [/s:<size>] [/a:<style>] [/r:<first-last>]");
            Console.Error.WriteLine("Creates a GFXfont header file that can be used for Arduino IDE sketches.");
            Console.Error.WriteLine("The name of the GFXfont header file is the name of the font + .h.");
            Console.Error.WriteLine("/p:<path>        : Folder where the resulting code file (*.h) will be stored.");
            Console.Error.WriteLine("/f:<family>      : Optional name of the installed font family to convert,");
            Console.Error.WriteLine("                   \"Monospace\", \"Serif\" or \"SansSerif\".");
            Console.Error.WriteLine("/n:<ttf>         : Optional full path of the TTF font file to convert.");
            Console.Error.WriteLine("                   One of the three options /f or /n or /b must be specified.");
            Console.Error.WriteLine("/b:<fon>         : Optional full path of the FON bitmap font file to convert.");
            Console.Error.WriteLine("                   One of the two options /f or /n or /b must be specified.");
            Console.Error.WriteLine("/s:<size>        : Size in pt (Point). May be a floating point value.");
            Console.Error.WriteLine("                   Default is 7.0. Ignored when converting bitmap font (FON).");
            Console.Error.WriteLine("/a:<style>       : Optional style (Bold, Italic, Regular, Strikeout, Underline).");
            Console.Error.WriteLine("                   Combine with '+' (i.e Bold+Italic). Default is Regular.");
            Console.Error.WriteLine("/r:<7|8>         : Optional ASCII (7 bits) or ANSI (8 bits) range of characters.");
            if (!string.IsNullOrEmpty(additional))
            {
                Console.Error.WriteLine("");
                Console.Error.WriteLine(additional);
            }
        }

        /// <summary>
        /// Gets the <see cref="FontStyle"/> enum values from the string parameters.
        /// </summary>
        /// <param name="styleNames">The style names to convert to a <see cref="FontStyle"/>.</param>
        /// <returns>A <see cref="FontStyle"/> created from the <paramref name="styleNames"/>.</returns>
        private static FontStyle GetStyles(string styleNames)
        {
            string[] names = styleNames?.Split('+');
            FontStyle style = FontStyle.Regular;
            if (names != null)
            {
                foreach (var s in names)
                {
                    style |= (FontStyle)Enum.Parse(typeof(FontStyle), s);
                }
            }
            return style;
        }

        // /p:"C:\\Users\\karst\\OneDrive\\Arduino\\IBM PC mit ESP32\\ibmesp32" /n:"C:\\Users\\karst\\OneDrive\\Arduino\\IBM PC mit ESP32\\ibmesp32\\oldschool_pc_font_pack_v2.2_win\\ttf - Mx (mixed outline+bitmap)\\Mx437_IBM_BIOS.ttf" /s:9 /t:

        /// <summary>
        /// The main method of the font converter.
        /// </summary>
        /// <remarks>
        /// See <see cref="Using"/> for informations about the commad line options.
        /// </remarks>
        /// <param name="args">See <see cref="Using"/> for informations about the commad line options.</param>
        static void Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 4)
            {
                Using();
                return;
            }

            try
            {
                string path = string.Empty;
                string family = string.Empty;
                string file = null;
                string bmFont = string.Empty;
                float size = 7.0f;
                char last = '\xff';
                FontStyle style = FontStyle.Regular;
                bool test = false;
                foreach (string arg in args)
                {
                    switch (arg.Substring(0,3))
                    {
                        case "/p:":
                            path = arg.Substring(3);
                            break;
                        case "/f:":
                            family = arg.Substring(3);
                            break;
                        case "/n:":
                            file = arg.Substring(3);
                            break;
                        case "/b:":
                            bmFont = arg.Substring(3);
                            break;
                        case "/s:":
                            size = float.Parse(arg.Substring(3));
                            break;
                        case "/a:":
                            style = GetStyles(arg.Substring(3));
                            break;
                        case "/r:":
                            last = arg == "7" ? '\x7f' : '\xff';
                            break;
                        case "/t:":
                            test = true;
                            break;
                        default:
                            Using($"Unknown option \"{arg}\".");
                            return;
                    }
                }

                Font font;
                if (string.IsNullOrEmpty(family))
                {
                    if (!string.IsNullOrEmpty (bmFont))
                    {
                        GfxFont gfxFont = BitmapFontConverter.Convert(bmFont, last);
                        WriteGfxFont(gfxFont, path);
                        return;
                    }
                    else if (!string.IsNullOrEmpty (file))
                    {
                        PrivateFontCollection collection = new PrivateFontCollection();
                        collection.AddFontFile(file);
                        font = new Font(collection.Families[0], size);
                    }
                    else
                    {
                        Using("Neither /f or /n or /b has been specified!");
                        return;
                    }
                }
                else
                {
                    font = new Font(family, size, style);
                }

                using (font)
                {
                    var gfxFont = GfxFontCreator.Create(font, ' ', last);

                    if (!test)
                    {
                        WriteGfxFont(gfxFont, path);
                    }
                    else
                    {
                        TestCharacter(font, ' ', path);
                        TestCharacter(font, 'A', path);
                        TestCharacter(font, 'W', path);
                        TestCharacter(font, 'y', path);
                        TestCharacter(font, 'Ä', path);
                        TestCharacter(font, 'Ö', path);
                        TestCharacter(font, 'Ü', path);
                        TestCharacter(font, 'ä', path);
                        TestCharacter(font, 'ö', path);
                        TestCharacter(font, 'ü', path);
                        TestCharacter(font, 'ß', path);
                        TestCharacter(font, '!', path);
                        TestCharacter(font, '.', path);
                    }
                }
            }
            catch(Exception ex)
            {
                Using(ex.Message);
            }
        }

        /// <summary>
        /// Writes a GFX font header to the specified file path.
        /// </summary>
        /// <remarks>
        /// The file name is created by using the font name of the <see cref="GfxFont"/>.
        /// </remarks>
        /// <param name="gfxFont">The <see cref="GfxFont"/> object to write.</param>
        /// <param name="path">The destination folder.</param>
        private static void WriteGfxFont(GfxFont gfxFont, string path)
        {
            var code = GfxFontCodeGenerator.Create(gfxFont);

            string headerName = Path.Combine(path, gfxFont.Name + ".h");

            using (TextWriter writer = new StreamWriter(headerName))
            {
                writer.Write(code);
            }
            Console.WriteLine("Font file successfully created:");
            Console.WriteLine(headerName);
        }
    }
}

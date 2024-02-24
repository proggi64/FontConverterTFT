using System;
using System.IO;

using FontConverterTFT.BitmapFont;

namespace FontConverterTFT
{
    /// <summary>
    /// Converter of FON bitmap font files to GFX font header.
    /// </summary>
    /// <remarks>
    /// Uses the original bitmaps instead of rendering the font into a device bitmap
    /// as it is done for TrueType fonts.
    /// </remarks>
    internal class BitmapFontConverter
    {
        /// <summary>
        /// Path of the bitmap font file.
        /// </summary>
        private string fontFile;
        /// <summary>
        /// First character of the GFX font (Blank, 32).
        /// </summary>
        private const byte First = 0x20;
        /// <summary>
        /// Last character of the bitmap font file (max 255).
        /// </summary>
        private char last;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapFontConverter"/>.
        /// </summary>
        /// <param name="fontFile">Path to the FON file.</param>
        /// <param name="path">Destination path of the converted font.</param>
        /// <param name="last">Last character of the converted character set (max. 255).</param>
        private BitmapFontConverter(string fontFile, char last) 
        { 
            this.fontFile = fontFile;
            this.last = last;
        }

        /// <summary>
        /// Creates a <see cref="GfxFont"/> from the specified FON file.
        /// </summary>
        /// <param name="fontFile">The full path to the FON file with a Windows bitmap font.</param>
        /// <param name="last">The highest code of the last character. Maximum is 255.</param>
        /// <returns>A new <see cref="GfxFont"/> object containing the bitmap font.</returns>
        public static GfxFont Convert(string fontFile, char last)
        {
            var instance = new BitmapFontConverter(fontFile, last);
            return instance.Convert();
        }

        /// <summary>
        /// Creates a <see cref="GfxFont"/> from the FON file.
        /// </summary>
        /// <returns>A new <see cref="GfxFont"/> object containing the bitmap font.</returns>
        private GfxFont Convert()
        {
            WinFont font = Read();

            GfxFont gfxFont = new GfxFont();
            gfxFont.First = First;
            gfxFont.Last = (byte)last;
            gfxFont.YAdvance = (byte)(font.height + Math.Max(font.height * 10 / 9, 1));
            gfxFont.Name = font.facename.Replace(' ', '_');
            
            for (byte c = gfxFont.First; c < last; c++)
            {
                gfxFont.Bitmaps.Add(GetBitmap(font, c));
                gfxFont.Glyphs.Add(GetGlyph(font, c));
            }

            return gfxFont;
        }

        /// <summary>
        /// Gets the bitmap of the specified character from the <see cref="WinFont"/>.
        /// </summary>
        /// <param name="font">The <see cref="WinFont"/> containing the data of the bitmap font.</param>
        /// <param name="c">The code of the character to get the bitmap for.</param>
        /// <returns>The bitmap as a GFX conform byte array.</returns>
        private byte[] GetBitmap(WinFont font, byte c)
        {
            int bytesPerCharacter = font.wbytes * font.height;
            int offset = (c - font._fn_info.dfFirstChar) * bytesPerCharacter;
            byte[] bytes = new byte[bytesPerCharacter];
            //Console.WriteLine("'" + (char)c + "'");
            for (int i = 0; i < bytesPerCharacter; i++)
            {
                bytes[i] = font.bitmap[offset + i];
                //Console.WriteLine(ToBits(bytes[i]));
            }
            //Console.WriteLine();
            return bytes;
        }

        /*
        private string ToBits(byte b)
        {
            return System.Convert.ToString(b, 2).PadLeft(8, '0');
        }*/

        /// <summary>
        /// Gets the glyph of the specified character from the <see cref="WinFont"/>.
        /// </summary>
        /// <param name="font">The <see cref="WinFont"/> containing the data of the bitmap font.</param>
        /// <param name="c">The code of the character to get the bitmap for.</param>
        /// <returns>The GFX glyph.</returns>
        private GfxGlyph GetGlyph(WinFont font, byte c)
        {
            var glyph = new GfxGlyph
            {
                BitmapOffset = (ushort)((c - First) * font.wbytes * font.height),
                Character = (char)c,
                Width = (byte)font.width,
                Height = (byte)font.height,
                XOffset = 0,
                YOffset = (sbyte)-font.height,
                XAdvance = (byte)font.width
            };
            return glyph;
        }

        /// <summary>
        /// Reads a FON file and returns the <see cref="WinFont"/> font resource content.
        /// </summary>
        /// <returns>A <see cref="WinFont"/> object containing the font resource of the FON file.</returns>
        /// <exception cref="FileLoadException">The file is not a valid FON file.</exception>
        private WinFont Read()
        {
            using (var reader = new BinaryReader(new FileStream(fontFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                MzHeader mzHeader = new MzHeader();
                mzHeader.Deserialize(reader);

                // Move to NE Header
                reader.BaseStream.Position = mzHeader.e_lfanew;

                NeHeader neHeader = new NeHeader();
                neHeader.Deserialize(reader);

                // Move to the resource table
                reader.BaseStream.Position = mzHeader.e_lfanew + neHeader.ne_rsrctab;

                var winFont = new WinFont();
                int count = winFont.Deserialize(reader);
                if (count == 0)
                    throw new FileLoadException("No FNT resources found.");

                return winFont;
            }
        }
    }
}

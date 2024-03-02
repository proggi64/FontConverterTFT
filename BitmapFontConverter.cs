using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            gfxFont.YAdvance = (byte)font.height;
            gfxFont.Name = $"{font.facename.Replace(' ', '_').Replace('-', '_')}_{font.width}x{font.height}";

            bitmapOffset = 0;

            for (ushort c = gfxFont.First; c <= last; c++)
            {
                Crops crops;
                byte[] raw = GetBitmap(font, (byte)c).AutoCrop(out crops, font.width, font.height);
                byte[] bytes = CreateBytes(raw, font, crops);
                gfxFont.Bitmaps.Add(bytes);
                gfxFont.Glyphs.Add(GetGlyph(font, (byte)c, crops, (ushort)bytes.Length));
            }

            return gfxFont;
        }

        /// <summary>
        /// Creates the byte array for the specified <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bitmap">The bitmap that represents a rendered font character.</param>
        /// <returns>An array of bytes. Each bit which is set is a pixel of the character,</returns>
        private byte[] CreateBytes(byte[] bitmap, WinFont font, Crops crops)
        {
            if (crops.Right == -1)
                return new byte[0];

            IList<byte> bytes = new List<byte>();

            int width = font.width - crops.Left - crops.Right;
            int height = font.height - crops.Top - crops.Bottom;
            int end = width * height;
            int sourceLineBitCounter = 0;
            int sourceIndex = 0;

            for (int i = 0; i < end; i++)
            {
                int destinationIndex = i / 8;

                if (i % 8 == 0)
                    bytes.Add(0);

                bytes[destinationIndex] |= GetBit(bitmap[sourceIndex], i, sourceLineBitCounter);

                sourceLineBitCounter++;

                if (sourceLineBitCounter == width)
                {
                    sourceIndex++;
                    sourceLineBitCounter = 0;
                }
                else if (sourceLineBitCounter > 0 &&
                         sourceLineBitCounter % 8 == 0)
                {
                    sourceIndex++;
                }
            }

            return bytes.ToArray();
        }

        // Bei 1 Pixel i, bei 10 Pixel Breite
        /// <summary>
        /// Get the current bit mask for the destination byte.
        /// </summary>
        /// <param name="bitmap">The source byte.</param>
        /// <param name="i">The bit index of the character bitmap.</param>
        /// <param name="sourceBitIndex">The bit index of the current byte.</param>
        /// <returns>A <see cref="byte"/> containing the bit at the right destination position (0 or 1).</returns>
        private byte GetBit(byte sourceByte, int i, int sourceBitIndex)
        {
            byte shifted = (byte)(0x80 >> i % 8);
            byte mask = (byte)(0x80 >> sourceBitIndex);
            bool isBitSet = (sourceByte & mask) != 0;
            byte result = (byte)(isBitSet ? shifted : 0);
            return result;
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

            for (int i = 0; i < font.height * font.wbytes; i += font.wbytes)
            {
                for (int j = 0; j < font.wbytes; j++)
                {
                    bytes[i + j] = font.bitmap[offset + i + j];
                }
            }
#if DEBUG
            PrintBits(bytes, font);
#endif

            return bytes;
        }


#if DEBUG
        private void PrintBits(byte[] bits, WinFont font)
        {
            for (int line = 0; line < font.height; line ++)
            {
                for (int b = 0; b < font.wbytes; b++)
                {
                    Console.Write(System.Convert.ToString(bits[line * font.wbytes + b], 2).PadLeft(8, '0').Replace('0', '.'));
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
#endif

        ushort bitmapOffset = 0;

        /// <summary>
        /// Gets the glyph of the specified character from the <see cref="WinFont"/>.
        /// </summary>
        /// <param name="font">The <see cref="WinFont"/> containing the data of the bitmap font.</param>
        /// <param name="c">The code of the character to get the bitmap for.</param>
        /// <param name="crops">A <see cref="Crops"/> instance containing the lines and columns that have
        /// been cut off on the top, bottom, left, and right.</param>
        /// <param name="length">Length of the bitmap array.</param>
        /// <returns>The GFX glyph.</returns>
        private GfxGlyph GetGlyph(WinFont font, byte c, Crops crops, ushort length)
        {
            byte width = (byte)(crops.Right == -1 ? 0 : (font.width - (crops.Left + crops.Right)));
            byte height = (byte)(crops.Bottom == -1 ? 0 : (font.height - (crops.Top + crops.Bottom)));
            sbyte yOffset = (sbyte)-(crops.Bottom == -1 ? 0 : font.height - crops.Top);

            var glyph = new GfxGlyph
            {
                BitmapOffset = bitmapOffset,
                Character = (char)c,
                Width = width,
                Height = height,
                XOffset = 0,
                YOffset = yOffset,
                XAdvance = (byte)font.width
            };

            bitmapOffset += length;

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

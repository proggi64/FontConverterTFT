using System;
using System.IO;
using System.Text;

namespace FontConverterTFT.BitmapFont
{
    /// <summary>
    /// Represents a Windows bitmap font (FON) for GFX conversion.
    /// </summary>
    public struct WinFont
    {
        private const ushort RT_FONTDIR = (ushort)0x8007u;
        private const ushort RT_FONT = (ushort)0x8008u;

        private const ushort DF_VER2 = (ushort)0x200u;
        private const ushort DF_VER3 = (ushort)0x300u;

        /// <summary>
        /// Character set of the bitmap font.
        /// </summary>
        public enum WinFont_CharSet
        {
            /// <summary>
            /// ANSI character set (ANSI codepage, 255 characters).
            /// </summary>
            WinFont_CharSetANSI = 0,
            /// <summary>
            ///  Default character set.
            /// </summary>
            WinFont_CharSetDefault = 1,
            /// <summary>
            /// Symbol character set (e.g. Wingdings).
            /// </summary>
            WinFont_CharSetSymbol = 2,
            /// <summary>
            /// Enhanced ASCII character set.
            /// </summary>
            WinFont_CharSetOEM = 255,
            /// <summary>
            /// Codepage 437 (IBM PC).
            /// </summary>
            WinFont_CharSetCP437 = WinFont_CharSetOEM,
            /// <summary>
            /// Codepage 437 (IBM PC).
            /// </summary>
            WinFont_CharSetIBM437 = WinFont_CharSetOEM,
        };

        /// <summary>
        /// Font name.
        /// </summary>
        public string facename;
        /// <summary>
        /// Number of glyphs in font.
        /// </summary>
        public int nglyphs;
        /// <summary>
        /// Glyph width in pixels.
        /// </summary>
        public int width;
        /// <summary>
        /// Glyph height in pixels.
        /// </summary>
        public int height;
        /// <summary>
        /// Glyph byte width.
        /// </summary>
        public int wbytes;
        /// <summary>
        /// Character set.
        /// </summary>
        public WinFont_CharSet charset;
        /// <summary>
        /// A <see cref="FontDirEntry"/> for this font.
        /// </summary>
        public FontDirEntry _fn_info;
        /// <summary>
        /// Bitmaps of the complete font.
        /// </summary>
        public byte[] bitmap;

        /// <summary>
        /// Reads the data of this <see cref="WinFont"/> from the <see cref="BinaryReader"/>. 
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read from.</param>
        /// <returns>Count of fonts in the FON file.</returns>
        public int Deserialize(BinaryReader reader)
        {
            ushort shift = reader.ReadUInt16();
            int rcount = 0;
            ushort fntcount = 0;

            while (true)
            {
                var re = new ResEntry();
                re.Deserialize(reader);

                if (re.reType == 0)
                    break;

                long foff = reader.BaseStream.Position;
                if (re.reType == RT_FONT)
                {
                    fntcount = re.reCount;
                    long fntoff = (re.reOffset << shift);
                    reader.BaseStream.Position = fntoff;

                    ReadFontResource(reader);
                    if (fntcount == ++rcount)
                        break;
                }
                reader.BaseStream.Position = foff;
            }
            return fntcount;
        }

        /// <summary>
        /// Reads a string from the file without changing the file position.
        /// </summary>
        /// <param name="offset">The file offset to read from.</param>
        /// <param name="reader">The <see cref="BinaryReader"/> to read from.</param>
        /// <returns>The <see langword="string"/> read from the specified position.</returns>
        private string ReadString(long offset, BinaryReader reader)
        {
            long savedOffset = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            Encoding encoding = Encoding.Default;

            byte[] bytes = new byte[256];
            byte b;
            int i = 0;
            while ((b = reader.ReadByte()) != 0 && i < 256)
            {
                bytes[i++] = b;
            }
            string result = encoding.GetString(bytes, 0, i);
            reader.BaseStream.Position = savedOffset;
            return result;
        }

        /// <summary>
        /// Reads the complete bitmaps of the font.
        /// </summary>
        /// <param name="w">Width of a single character in pixels.</param>
        /// <param name="h">Height of a single character in pixel lines.</param>
        /// <param name="wbytes">Count of bytes of a single pixel line.</param>
        /// <param name="nglyphs">Count of glyphs in the font file.</param>
        /// <param name="reader">The <see cref="BinaryReader"/> to read from.</param>
        /// <returns>The bitmaps as byte array.</returns>
        private byte[] ReadBitmap(int w, int h, int wbytes, int nglyphs, BinaryReader reader)
        {
            byte[] result = new byte[wbytes * h * nglyphs];
            int dest;
            int colb;
            int gb = 0;

            for (int c = 0; c < nglyphs; c++)
            {
                dest = colb = gb;
                for (int i = 0; i < wbytes * h; i++)
                {
                    if (i != 0 && i % h == 0)
                        dest = ++colb;
                    result[dest] = reader.ReadByte();
                    dest += wbytes;
                }
                gb += wbytes * h;
            }

            return result;
        }

        /// <summary>
        /// Reads the complete font from the resouces of the FON file.
        /// </summary>
        /// <param name="reader"></param>
        /// <exception cref="FileLoadException"></exception>
        private void ReadFontResource(BinaryReader reader)
        {
            long fntBase = reader.BaseStream.Position;

            _fn_info = new FontDirEntry();
            _fn_info.Deserialize(reader);

            if ((_fn_info.dfType & 1) != 0)
                throw new FileLoadException("Not a bitmap font.");

            facename = ReadString(fntBase + _fn_info.dfFace, reader);

            nglyphs = _fn_info.dfLastChar - _fn_info.dfFirstChar + 2;

            reader.BaseStream.Position = fntBase + _fn_info.dfBitsOffset;

            width = _fn_info.dfPixWidth;
            height = _fn_info.dfPixHeight;

            wbytes = (int)Math.Ceiling(width / 8.0f);
            bitmap = ReadBitmap(width, height, wbytes, nglyphs, reader);

            charset = WinFont_CharSet.WinFont_CharSetCP437;
        }
    }
}

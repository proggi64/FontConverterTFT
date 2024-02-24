using System;
using System.Text;

namespace FontConverterTFT
{
    /// <summary>
    /// Writes a converted GFX font as C code.
    /// </summary>
    internal class GfxFontCodeGenerator
    {
        private StringBuilder buffer;
        private GfxFont gfxFont;

        /// <summary>
        /// Initializes a new instance of the <see cref="GfxFontCodeGenerator"/> class.
        /// </summary>
        /// <param name="gfxFont">The <see cref="GfxFont"/> to write the code for.</param>
        private GfxFontCodeGenerator(GfxFont gfxFont) 
        {
            this.gfxFont = gfxFont;
            buffer = new StringBuilder();
        }

        /// <summary>
        /// Creates the C code for the speicfied <see cref="GfxFont"/>.
        /// </summary>
        /// <param name="gfxFont">The <see cref="GfxFont"/> to write the C code for.</param>
        /// <returns>A <see cref="string"/> containing the complete C code for the GFX font.</returns>
        public static string Create(GfxFont gfxFont)
        {
            var instance = new GfxFontCodeGenerator(gfxFont);
            instance.WriteBitmaps();
            instance.WriteGlyphs();
            instance.WriteInstance();
            return instance.buffer.ToString();
        }

        /// <summary>
        /// Writes the instance variable of the GfxFont.
        /// </summary>
        private void WriteInstance()
        {
            buffer.AppendLine("");
            buffer.AppendLine($"const GFXfont {gfxFont.Name} PROGMEM = {{");
            buffer.AppendLine($"  (uint8_t  *){gfxFont.Name}Bitmaps,");
            buffer.AppendLine($"  (GFXglyph *){gfxFont.Name}Glyphs,");
            buffer.AppendLine($"  0x{gfxFont.First:x2}, 0x{gfxFont.Last:x2}, {gfxFont.YAdvance} }};");
        }

        /// <summary>
        /// Writes the glyphs byte array.
        /// </summary>
        private void WriteGlyphs()
        {
            buffer.AppendLine("");
            buffer.AppendLine($"const GFXglyph {gfxFont.Name}Glyphs[] PROGMEM = {{");

            for (int i = 0; i < gfxFont.Glyphs.Count; i++)
            {
                var glyph = gfxFont.Glyphs[i];
                WriteGlyph(glyph);
                buffer.Append(IsLastGlyph(i) ? " }; " : ",   ");
                buffer.AppendLine($"   // 0x{(byte)glyph.Character:x2} '{glyph.Character}'");
            }
        }

        private void WriteGlyph(GfxGlyph glyph)
        {
            buffer.Append($"  {{ {glyph.BitmapOffset,6},{glyph.Width,4},{glyph.Height,4},{glyph.XAdvance,4},{glyph.XOffset,5},{glyph.YOffset,5} }}");
        }

        private bool IsLastGlyph(int i)
        {
            return gfxFont.Glyphs.Count - 1 == i;
        }

        /// <summary>
        /// Writes the bitmaps of the font characters as array of uint8_t elements (bytes).
        /// </summary>
        /// <remarks>
        /// Each character is preceded by a comment line with the visible character and its hexadecimal code.
        /// </remarks>
        private void WriteBitmaps()
        {
            buffer.AppendLine($"const uint8_t {gfxFont.Name}Bitmaps[] PROGMEM = {{");
            char c = (char)gfxFont.First;
            bool pendingComma = false;
            foreach ( var bitmap in gfxFont.Bitmaps ) 
            {
                if (pendingComma)
                    buffer.AppendLine(",");
                WriteBitmap(bitmap, c);
                pendingComma = bitmap.Length != 0;
                c++;
            }
            buffer.AppendLine(" };");
        }

        /// <summary>
        /// Writes the bitmap of the character as a sequence of hexadecimal uint8_t elements (bytes).
        /// </summary>
        /// <remarks>
        /// The bytes are preceded by a comment line with the visible character and its hexadecimal code.
        /// </remarks>
        /// <param name="bitmap">The sequence of bytes to write that represent the bits of the rendered character.</param>
        /// <param name="c">The character the bytes belong to.</param>
        private void WriteBitmap(byte[] bitmap, char c)
        {
            if (bitmap.Length == 0)
            {
                buffer.AppendLine($"  // ' ' Code {(byte)c:x2}");
                return;
            }

            buffer.AppendLine($"  // '{c}' Code {(byte)c:x2}");
            int i = 0;
            while ( i < bitmap.Length )
            {
                WriteTenBytes(bitmap, ref i);
            } 
        }

        /// <summary>
        /// Writes a sequence of a maximum of ten bytes out of the <paramref name="bitmap"/>
        /// array, starting from the index <paramref name="i"/>.
        /// </summary>
        /// <remarks>
        /// If the array is smaller than 10 bytes then the writing is stopped and no comma is appended.
        /// The comma should be appended by the caller if another character should be written.
        /// </remarks>
        /// <param name="bitmap">The sequence of bytes to write that represent the bits of the rendered character.</param>
        /// <param name="i">The current index in the byte array.</param>
        private void WriteTenBytes(byte[] bitmap, ref int i)
        {
            buffer.Append(' ');
            int count = 10;
            while ( i < bitmap.Length && count > 0 )
            {
                buffer.AppendFormat(" 0x{0:x2},", bitmap[i]);
                i++;
                count--;
            }
            if (count == 0 && i < bitmap.Length)
            {
                buffer.AppendLine("");
            }
            if (i == bitmap.Length)
            {
                buffer.Remove(buffer.Length - 1, 1);
            }
        }
    }
}

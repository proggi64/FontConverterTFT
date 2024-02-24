using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FontConverterTFT
{
    /// <summary>
    /// Converter for installed fonts to <see cref="GfxFont"/> objects.
    /// </summary>
    internal class GfxFontCreator
    {
        private Font font;
        private GfxFont gfxFont;
        private ushort currentBitmapOffset = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="GfxFontCreator"/> class
        /// </summary>
        /// <param name="font"></param>
        /// <param name="gfxFont"></param>
        private GfxFontCreator(Font font, GfxFont gfxFont) 
        { 
            this.font = font; 
            this.gfxFont = gfxFont; 
        }

        /// <summary>
        /// Vreate a <see cref="GfxFont"/> object from the specified <see cref="Font"/>.
        /// </summary>
        /// <remarks>
        /// Each character in the range will be rendered into a bitmap. The bitmaps will be reduced to the
        /// rows and lines where at least one pixel ist set. These bitmaps are the source of the bit arrays
        /// describing the font characters. Additionally, the glyphs are created by unsing the sizes of
        /// the bitmaps and the sizes of the removed lines at the bottom and the top of the bitmaps.
        /// </remarks>
        /// <param name="font">The source <see cref="Font"/>.</param>
        /// <param name="first">The first character to be included into the <see cref="GfxFont"/>.</param>
        /// <param name="first">The lasst character to be included into the <see cref="GfxFont"/>.</param>
        /// <returns>The resulting <see cref="GfxFont"/> object.</returns>
        public static GfxFont Create(Font font, char first, char last)
        {
            GfxFontCreator creator = new GfxFontCreator(font,
                new GfxFont
                {
                    Name = $"{font.Name.Replace(' ', '_')}{(int)font.SizeInPoints}pt" + (((last - first) > 127) ? "8b" : "7b"),
                    First = (byte)first,
                    Last = (byte)last,
                    YAdvance = CharacterRenderer.GetAdvance(font)
                });

            for (char c = first; c <= last; c++)
            {
                creator.AddChar(c);
            }

            return creator.gfxFont;
        }

        /// <summary>
        /// Adds the data of the specified character to the <see cref="GfxFont"/> object.
        /// </summary>
        /// <param name="c">The character to add.</param>
        private void AddChar(char c)
        {
            Bitmap bitmap = CharacterRenderer.Render(font, c, out Crops crops);

            if (IsSpace(crops))
            {
                AddSpaceGlyph(c, (byte)crops.Left);
                AddSpaceBitmap();
                return;
            }

            gfxFont.Glyphs.Add(new GfxGlyph
            {
                Character = c,
                BitmapOffset = currentBitmapOffset,
                Height = (byte)bitmap.Height,
                Width = (byte)bitmap.Width,
                XAdvance = (byte)(bitmap.Width + crops.Left + crops.Right),
                XOffset = (sbyte)crops.Left,
                YOffset = (sbyte)-(crops.Bottom + bitmap.Height)
            });

            byte[] bytes = CreateBytes(bitmap);
            gfxFont.Bitmaps.Add(bytes);

            currentBitmapOffset += (ushort)bytes.Length;
        }

        /// <summary>
        /// Adds a new <see cref="GfxGlyph"/> object for the specified character.
        /// </summary>
        /// <param name="c">The character to add the glyph for.</param>
        /// <param name="xAdvance">The width to advance for this character in pixels.</param>
        private void AddSpaceGlyph(char c, byte xAdvance)
        {
            gfxFont.Glyphs.Add(new GfxGlyph
            {
                Character = c,
                BitmapOffset = currentBitmapOffset,
                Height = 0,
                Width = 0,
                XAdvance = xAdvance,
                XOffset = 0,
                YOffset = 1
            });
        }

        /// <summary>
        /// Adds the empty bitmap for a blank.
        /// </summary>
        private void AddSpaceBitmap() 
        {
            byte[] bytes = {};
            gfxFont.Bitmaps.Add(bytes);
        }

        /// <summary>
        /// Tests whether the <see cref="Crops"/> represent a blank.
        /// </summary>
        /// <remarks>
        /// The <paramref name="crops"/> data represents a blank if the <see cref="Crops.Right"/>
        /// value is -1.
        /// </remarks>
        /// <param name="crops">The <see cref="Crops"/> object to test.</param>
        /// <returns><see langword="true"/> if the data represents a blank; otherwise, <see langword="false"/>.</returns>
        private bool IsSpace(Crops crops)
        {
            return crops.Right == -1;
        }

        /// <summary>
        /// Creates the byte array for the specified <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bitmap">The bitmap that reprsent a rendered font character.</param>
        /// <returns>An array of bytes. Each bit which is set is a pixel of the character,</returns>
        private byte[] CreateBytes(Bitmap bitmap)
        {
            IList<byte> bytes = new List<byte>();

            int end = bitmap.Width * bitmap.Height;

            for (int i = 0; i < end; i++)
            {
                if (i % 8 == 0)
                    bytes.Add(0);
                bytes[i / 8] |= GetBit(bitmap, i);
            }

            return bytes.ToArray();
        }

        /// <summary>
        /// Gets a single bit of the specified <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bitmap">The bitmap to get the bit for.</param>
        /// <param name="i">The index of the bit in the <paramref name="bitmap"/>.</param>
        /// <returns>A byte with the bit set at the correct position. Should be ORed with the other bits.</returns>
        private byte GetBit(Bitmap bitmap, int i)
        {
            int x = i % bitmap.Width;
            int y = i / bitmap.Width;
            Color pixelColor = bitmap.GetPixel(x, y);
            byte pixel = (byte)(pixelColor.ToArgb().Equals(Configuration.Foreground.ToArgb()) ? 0x80 : 0);
            return (byte)(pixel >> (i & 7));
        }
    }
}

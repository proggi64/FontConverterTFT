using System.Collections.Generic;

namespace FontConverterTFT
{
    /// <summary>
    /// Data stored for 8 bits font as a whole.
    /// </summary>
    /// <remarks>
    /// Represents nearly the format that will be exported as GFX font as it is
    /// defined by https://glenviewsoftware.com/projects/products/adafonteditor/adafruit-gfx-font-format/.
    /// </remarks>
    internal class GfxFont
    {
        /// <summary>
        /// Gets or sets the name of the GFX font.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets the concatenated glyph bitmaps.
        /// </summary>
        public ICollection<byte[]> Bitmaps { get; } = new List<byte[]>();
        /// <summary>
        /// Gets or sets the array of <see cref="GfxGlyph"/> values, one per character.
        /// </summary>
        public IList<GfxGlyph> Glyphs { get; } = new List<GfxGlyph>();
        /// <summary>
        /// Gets or sets the ASCII/ANSI code where the characters start.
        /// </summary>
        public byte First { get; set; }
        /// <summary>
        /// Gets or sets the ASCII/ANSI code where the characters end.
        /// </summary>
        public byte Last { get; set; }
        /// <summary>
        /// Gets or sets the new line distance (Y axis) for this font.
        /// </summary>
        public byte YAdvance { get; set; }
    }
}

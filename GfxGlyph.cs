namespace FontConverterTFT
{
    /// <summary>
    /// Data stored per glyph.
    /// </summary>
    /// <remarks>
    /// Represents nearly the format that will be exported as GFX glyph as it is
    /// defined by https://glenviewsoftware.com/projects/products/adafonteditor/adafruit-gfx-font-format/.
    /// </remarks>
    /// <seealso cref="GfxFont"/>
    internal class GfxGlyph
    {
        /// <summary>
        /// Gets or sets the character the glyüh belongs to.
        /// </summary>
        public char Character { get; set; }
        /// <summary>
        /// Gets or sets the byte offset of the first bitmap byte of this character in the <see cref="GfxFont.Bitmaps"/> array.
        /// </summary>
        public ushort BitmapOffset { get; set; }
        /// <summary>
        /// Gets or sets the width of the character in pixels without padding.
        /// </summary>
        /// <remarks>May be zero for a space character.</remarks>
        public byte Width { get; set; }
        /// <summary>
        /// Gets or sets the height of the character in pixels without padding.
        /// </summary>
        /// <remarks>May be zero for a space character.</remarks>
        public byte Height { get; set; }
        /// <summary>
        /// Gets or sets the X advance of the cursor for this character.
        /// </summary>
        public byte XAdvance { get; set; }
        /// <summary>
        /// Gets or sets the X distance from the cursor to the upper left corner of the character.
        /// </summary>
        public sbyte XOffset { get; set; }
        /// <summary>
        /// Gets or sets the Y distance from the cursor to the upper left corner of the character.
        /// </summary>
        public sbyte YOffset { get; set; }
    }
}

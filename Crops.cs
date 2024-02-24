namespace FontConverterTFT
{
    /// <summary>
    /// Represents the crop lines and rows of a specific character in pixels.
    /// </summary>
    /// <seealso cref="BitmapExtender.AutoCrop(System.Drawing.Bitmap, out Crops)"/>
    internal class Crops
    {
        /// <summary>
        /// Gets or sets the count of empty pixel lines at the top that are cropped from the rendered character bitmap.
        /// </summary>
        public int Top {get; set;}
        /// <summary>
        /// Gets or sets the count of empty pixel lines at the bottom that are cropped from the rendered character bitmap.
        /// </summary>
        public int Bottom { get; set; }
        /// <summary>
        /// Gets or sets the count of empty pixel rows at the left that are cropped from the rendered character bitmap.
        /// </summary>
        public int Left { get; set; }
        /// <summary>
        /// Gets or sets the count of empty pixel rows at the right that are cropped from the rendered character bitmap.
        /// </summary>
        public int Right { get; set; }
    }
}

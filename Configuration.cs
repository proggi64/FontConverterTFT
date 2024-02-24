using System.Drawing;

namespace FontConverterTFT
{
    /// <summary>
    /// Configuration data for the font converter classes.
    /// </summary>
    internal static class Configuration
    {
        /// <summary>
        /// Foreground color for rendered character bitmaps.
        /// </summary>
        public static readonly Color Foreground = Color.White;
        /// <summary>
        /// Background color for rendered character bitmaps.
        /// </summary>
        public static readonly Color Background = Color.Black;
        /// <summary>
        /// Resolution for rendering and measuring.
        /// </summary>
        public const int Dpi = 96;
        /// <summary>
        /// Factor for calculating the line advance in pixels.
        /// </summary>
        public const double LineAdvanceFactor = 1.0;
    }
}

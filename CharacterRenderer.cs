using System;
using System.Drawing;
using System.Windows.Forms;

namespace FontConverterTFT
{
    /// <summary>
    /// Renders characters into a <see cref="Bitmap"/> for converting fonts and provides necessary properties.
    /// </summary>
    internal static class CharacterRenderer
    {
        /// <summary>
        /// Measures the size of a rendered character including the padding on all sides.
        /// </summary>
        /// <param name="font">The font to be used for measuring.</param>
        /// <param name="c">The character to render.</param>
        /// <returns>The <see cref="Size"/> of the character in pixels.</returns>
        public static Size Measure(Font font, char c)
        {
            using (Bitmap bitmap = new Bitmap(1024, 1024))
            {
                bitmap.SetResolution(Configuration.Dpi, Configuration.Dpi);
                using (Graphics measureCanvas = Graphics.FromImage(bitmap))
                {
                    measureCanvas.PageUnit = GraphicsUnit.Pixel;
                    Size proposedSize = new Size(int.MaxValue, int.MaxValue);
                    return TextRenderer.MeasureText(measureCanvas, c.ToString(), font, proposedSize, TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
                }
            }
        }

        /// <summary>
        /// Calculates the line advance for the given font.
        /// </summary>
        /// <param name="font">The <see cref="Font"/> as base for the calculation.</param>
        /// <returns>The line advance as <see cref="byte"/> value. The minimum is the height of the character W plus one.
        /// The value is calculated by multiplying the height with the double value <see cref="Configuration.LineAdvanceFactor"/>.</returns>
        public static byte GetAdvance(Font font)
        {
            int height = Measure(font, 'W').Height;
            return (byte)Math.Min(height * Configuration.LineAdvanceFactor, height + 1);
        }

        /// <summary>
        /// Creates a bitmap from the given character using the given font.
        /// </summary>
        /// <remarks>
        /// Uses black as background and white as foreground.
        /// </remarks>
        /// <param name="font">The font to be used for creating the character bitmap.</param>
        /// <param name="c">The character to render.</param>
        /// <param name="crops">OUT: The crop values of the four edges the determine the real position
        /// of the character in a character cell in pixels. These values are needed for additional data
        /// when generating pixel fonts for Arduino/ESP32 displays.</param>
        /// <returns>The <see cref="Bitmap"/> containing the character without empty edges (padding).</returns>
        public static Bitmap Render(Font font, char c, out Crops crops)
        {
            Size result = Measure(font, c);

            if (result.Width == 0 || result.Height == 0) 
            {
                // each character that cannot be rendered should be a space
                c = ' ';
                result = Measure(font, c);
            }
            
            Bitmap bitmap = new Bitmap(result.Width, result.Height);
            using (Graphics drawCanvas = Graphics.FromImage(bitmap))
            {
                bitmap.SetResolution(Configuration.Dpi, Configuration.Dpi);
                drawCanvas.Clear(Configuration.Background);

                TextRenderer.DrawText(drawCanvas, c.ToString(), font, new Point(0, 0), Configuration.Foreground, TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
                return bitmap.AutoCrop(out crops);
            }
        }
    }
}

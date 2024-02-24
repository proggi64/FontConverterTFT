using System.Drawing;

namespace FontConverterTFT
{
    /// <summary>
    /// Extender for the <see cref="Bitmap"/> class that crop all empty lines and rows from the bitmap's edges.
    /// </summary>
    internal static class BitmapExtender
    {
        /// <summary>
        /// Returns the count of pixel lines to crop from the given edge.
        /// </summary>
        /// <param name="bitmap">The <see cref="Bitmap"/> used to calculate the result.</param>
        /// <param name="startOuter">The index to start the outer loop (may be an X or Y value, depending on <paramref name="isVertical"/>).</param>
        /// <param name="endOuter">The index to end the outer loop (may be an X or Y value, depending on <paramref name="isVertical"/>).</param>
        /// <param name="startInner">The index to start the outer loop (may be an Y or X value, depending on <paramref name="isVertical"/>).<</param>
        /// <param name="endInner">The index to end the outer loop (may be an Y or X value, depending on <paramref name="isVertical"/>).</param>
        /// <param name="isVertical">Determines whether the bitmap is cropped left/right (<see langword="true"/>) or top/bottom (<see langword="false"/>).</param>
        /// <returns>The amount of pixel lines resp. rows to crop.</returns>
        private static int CalculateCrop(Bitmap bitmap, int startOuter, int endOuter, int startInner, int endInner, bool isVertical = true)
        {
            int crop = endOuter;
            bool isEmpty = true;
            int stepOuter = startOuter > endOuter ? -1 : 1;
            int stepInner = startInner > endInner ? -1 : 1;
            for (int x = startOuter; stepOuter > 0 ? x < endOuter : x >= endOuter; x += stepOuter)
            {
                for (int y = startInner; stepInner > 0 ? y < endInner : y >= endInner; y += stepInner)
                {
                    Color c = bitmap.GetPixel(isVertical ? x : y, isVertical ? y : x);
                    if (!c.ToArgb().Equals(Configuration.Background.ToArgb()))
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (!isEmpty)
                {
                    crop = stepOuter > 0 ? x : (isVertical ? bitmap.Width : bitmap.Height) - 1 - x;
                    break;
                }
            }
            return crop;
        }

        /// <summary>
        /// Cuts the edges on all four sides that have no pixel set.
        /// </summary>
        /// <remarks>
        /// The <paramref name="bitmap"/> must be a black and white two color bitmap.
        /// Lines or rows with only black pixels are cropped.
        /// </remarks>
        /// <param name="bitmap">The <see cref="Bitmap"/> to crop.</param>
        /// <param name="crops">The lines resp. rows that have been cropped of all four edges.</param>
        /// <returns>The new <see cref="Bitmap"/> with no edges without a pixel.</returns>
        public static Bitmap AutoCrop(this Bitmap bitmap, out Crops crops)
        {
            int leftCrop = CalculateCrop(bitmap, 0, bitmap.Width, 0, bitmap.Height);
            if (leftCrop == bitmap.Width)
            {
                crops = new Crops { Top = bitmap.Height, Bottom = -1, Left = bitmap.Width, Right = -1 };
                return new Bitmap(1, 1);
            }
            int topCrop = CalculateCrop(bitmap, 0, bitmap.Height, 0, bitmap.Width, false);
            int rightCrop = CalculateCrop(bitmap, bitmap.Width - 1, 0, 0, bitmap.Height); ;
            int bottomCrop = CalculateCrop(bitmap, bitmap.Height - 1, 0, 0, bitmap.Width, false);

            Rectangle cropRect = new Rectangle
                (
                leftCrop,
                topCrop,
                bitmap.Width - (leftCrop + rightCrop),
                bitmap.Height - (topCrop + bottomCrop)
                );

            crops = new Crops { Top = topCrop, Bottom = bottomCrop, Left = leftCrop, Right = rightCrop };

            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(bitmap, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
            }
            return target;
        }
    }
}

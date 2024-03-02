
using System.Collections;

namespace FontConverterTFT
{
    internal static class ByteArrayExtender
    {
        private static bool IsBitSet(byte[] bitmap, int width, int x, int y)
        {
            int index = x / 8 + y * ((7 + width) / 8);
            int mask = 0x80 >> (x % 8);
            return (bitmap[index] & mask) != 0;
        }

        private static int CalculateCrop(byte[] bitmap, int width, int height, int startOuter, int endOuter, int startInner, int endInner, bool isVertical = true)
        {
            int crop = endOuter;
            bool isEmpty = true;
            int stepOuter = startOuter > endOuter ? -1 : 1;
            int stepInner = startInner > endInner ? -1 : 1;
            for (int x = startOuter; stepOuter > 0 ? x < endOuter : x >= endOuter; x += stepOuter)
            {
                for (int y = startInner; stepInner > 0 ? y < endInner : y >= endInner; y += stepInner)
                {
                    bool c = IsBitSet(bitmap, width, isVertical ? x : y, isVertical ? y : x);
                    if (c)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (!isEmpty)
                {
                    crop = stepOuter > 0 ? x : (isVertical ? width : height) - 1 - x;
                    break;
                }
            }
            return crop;
        }

        private static void CopyBits(byte[] source, byte[] target, int ySourceByteCount, Rect cropRect)
        {
            int yTargetByteCount = (7 + cropRect.width) / 8;
            for (int y = cropRect.top; y < cropRect.top + cropRect.height; y++)
            {
                byte[] byteLine = ShiftLeft(GetBitLine(source, ySourceByteCount, y), cropRect.left);
                int targetOffset = (y - cropRect.top) * yTargetByteCount;
                for (int i = 0; i < yTargetByteCount; i++)
                    target[targetOffset + i] = byteLine[i];
            }
        }

        private static byte[] GetBitLine(byte[] source, int ySourceByteCount, int y)
        {
            int offset = y * ySourceByteCount;
            byte[] result = new byte[ySourceByteCount];
            for (int i = 0; i < ySourceByteCount; i++)
                result[i] = source[i + offset];
            return result;
        }

        private static byte[] ShiftLeft(byte[] bitLine, int count)
        {
            byte[] result = new byte[bitLine.Length];
            bitLine.CopyTo(result, 0);
            for (int i = 0; i < count; i++)
                ShiftLeft(result);
            return result;
        }

        private static void ShiftLeft(byte[] bitLine)
        {
            int start = bitLine.Length - 1;
            byte flag = 0;
            for (int i = start; i >= 0; i--)
            {
                byte bitSet = (byte)((bitLine[start] & 0x80) != 0 ? 1 : 0);
                bitLine[i] <<= 1;
                bitLine[i] += flag;
                flag = bitSet;
            }
        }

        private class Rect
        {
            public int left;
            public int top;
            public int width;
            public int height;

            public Rect(int left, int top, int width, int height)
            {
                this.left = left;
                this.top = top;
                this.width = width;
                this.height = height;
            }
        }

        public static byte[] AutoCrop(this byte[] bitmap, out Crops crops, int width, int height)
        {
            int leftCrop = CalculateCrop(bitmap, width, height, 0, width, 0, height);
            if (leftCrop == width)
            {
                crops = new Crops { Top = height, Bottom = -1, Left = width, Right = -1 };
                return new byte[0];
            }
            int topCrop = CalculateCrop(bitmap, width, height, 0, height, 0, width, false);
            int rightCrop = CalculateCrop(bitmap, width, height, width - 1, 0, 0, height); ;
            int bottomCrop = CalculateCrop(bitmap, width, height, height - 1, 0, 0, width, false);

            Rect cropRect = new Rect(
                leftCrop,
                topCrop,
                width - (leftCrop + rightCrop),
                height - (topCrop + bottomCrop)
            );

            crops = new Crops { Top = topCrop, Bottom = bottomCrop, Left = leftCrop, Right = rightCrop };

            byte[] target = new byte[(7 + cropRect.width) / 8 * cropRect.height];
            CopyBits(bitmap, target, (7 + width) / 8, cropRect);
            return target;
        }
    }
}

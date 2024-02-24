using System.IO;
using System.Runtime.InteropServices;

namespace FontConverterTFT.BitmapFont
{
    /// <summary>
    /// Called IMAGE_OS2_HEADER in winnt.h.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    unsafe public struct NeHeader
    {
        /// <summary>
        /// Magic signature.
        /// </summary>
        private const ushort Magic = (ushort)0x454Eu;

        /// <summary>
        /// NE signature 'NE'.
        /// </summary>
        ushort ne_magic;
        fixed byte _skip[34];
        /// <summary>
        /// Offset to resource table.
        /// </summary>
        public ushort ne_rsrctab;
        /// <summary>
        /// Offset to resident-name table.
        /// </summary>
        public ushort ne_restab;

        /// <summary>
        /// Reads the <see cref="MzHeader"/> from a binary reader.
        /// </summary>
        /// <param name="reader">The reader to get the data from.</param>
        /// <exception cref="FileLoadException">The magic number is wrong.</exception>
        public void Deserialize(BinaryReader reader)
        {
            ne_magic = reader.ReadUInt16();
            reader.BaseStream.Seek(34 * sizeof(byte), SeekOrigin.Current);
            ne_rsrctab = reader.ReadUInt16();
            ne_restab = reader.ReadUInt16();
            Test();
        }

        /// <summary>
        /// Tests the magic number.
        /// </summary>
        /// <exception cref="FileLoadException">The magic number is wrong.</exception>
        private void Test()
        {
            if (ne_magic != Magic)
            {
                throw new FileLoadException("Invalid FON file format. NE magic mismatch.");
            }
        }
    }
}

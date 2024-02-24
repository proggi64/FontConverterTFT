using System.IO;
using System.Runtime.InteropServices;

namespace FontConverterTFT.BitmapFont
{
    /// <summary>
    /// Called IMAGE_DOS_HEADER in winnt.h.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    unsafe public struct MzHeader
    {
        /// <summary>
        /// Magic signature.
        /// </summary>
        private const ushort Magic = (ushort)0x5A4Du;

        /// <summary>
        /// Magic number.
        /// </summary>
        ushort e_magic;
        fixed ushort _skip[29];
        /// <summary>
        /// File address of new exe (NE) header.
        /// </summary>
        public uint e_lfanew;

        /// <summary>
        /// Reads the <see cref="MzHeader"/> from a binary reader.
        /// </summary>
        /// <param name="reader">The reader to get the data from.</param>
        /// <exception cref="FileLoadException">The magic number is wrong.</exception>
        public void Deserialize(BinaryReader reader)
        {
            e_magic = reader.ReadUInt16();
            reader.BaseStream.Seek(29*sizeof(ushort), SeekOrigin.Current);
            e_lfanew = reader.ReadUInt32();
            Test();
        }

        /// <summary>
        /// Tests the magic number.
        /// </summary>
        /// <exception cref="FileLoadException">The magic number is wrong.</exception>
        private void Test()
        {
            if (e_magic != Magic) 
            {
                throw new FileLoadException("Invalid FON file format. MZ magic mismatch.");
            }
        }
    }
}

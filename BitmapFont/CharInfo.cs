using System.IO;

namespace FontConverterTFT.BitmapFont
{
    /// <summary>
    /// Represents informations about a bitmap font.
    /// </summary>
    public struct CharInfo_v2
    {
        /// <summary>
        /// Width of a single character.
        /// </summary>
        public ushort width;
        /// <summary>
        /// Offset.
        /// </summary>
        public ushort offset;

        /// <summary>
        /// Reads the data from the <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> where the data is read from.</param>
        public void Deserialize(BinaryReader reader)
        {
            width = reader.ReadUInt16();
            offset = reader.ReadUInt16();
        }
    }

    /// <summary>
    /// Represents informations about a bitmap font.
    /// </summary>
    public struct CharInfo_v3
    {
        /// <summary>
        /// Width of a single character.
        /// </summary>
        public ushort width;
        /// <summary>
        /// Offset.
        /// </summary>
        public uint offset;

        /// <summary>
        /// Reads the data from the <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> where the data is read from.</param>
        public void Deserialize(BinaryReader reader)
        {
            width = reader.ReadUInt16();
            offset = reader.ReadUInt32();
        }
    }
}

using System.IO;

namespace FontConverterTFT.BitmapFont
{
    /// <summary>
    /// Represents an entry in the resource dictionary of a resource file.
    /// </summary>
    public struct ResEntry
    {
        /// <summary>
        /// Type of the resource. 0x8008 means a font.
        /// </summary>
        public ushort reType;
        /// <summary>
        /// Count of items.
        /// </summary>
        public ushort reCount;
        /// <summary>
        /// Filler.
        /// </summary>
        public uint _pad;
        /// <summary>
        /// File offset of the font data.
        /// </summary>
        public ushort reOffset;
        /// <summary>
        /// Filler.
        /// </summary>
        public byte[] _pad2; // [10];

        /// <summary>
        /// Reads the data from the specified <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> where the data is read.</param>
        public void Deserialize(BinaryReader reader)
        {
            reType = reader.ReadUInt16();
            reCount = reader.ReadUInt16();
            _pad = reader.ReadUInt32();
            reOffset = reader.ReadUInt16();
            _pad2 = reader.ReadBytes(10);
        }
    }
}

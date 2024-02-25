using System.IO;
using System.Text;

namespace FontConverterTFT.BitmapFont
{
    public struct FontDirEntry
    {
        public ushort dfVersion;
        public uint dfSize;
        public string dfCopyright;  // [60]
        public ushort dfType;
        public ushort dfPoints;
        public ushort dfVertRes;
        public ushort dfHorizRes;
        public ushort dfAscent;
        public ushort dfInternalLeading;
        public ushort dfExternalLeading;
        public byte dfItalic;
        public byte dfUnderline;
        public byte dfStrikeOut;
        public ushort dfWeight;
        public byte dfCharSet;
        public ushort dfPixWidth;
        public ushort dfPixHeight;
        public byte dfPitchAndFamily;
        public ushort dfAvgWidth;
        public ushort dfMaxWidth;
        public byte dfFirstChar;
        public byte dfLastChar;
        public byte dfDefaultChar;
        public byte dfBreakChar;
        public ushort dfWidthBytes;
        public uint dfDevice;
        public uint dfFace;
        public uint dfBitsPointer;
        public uint dfBitsOffset;
        public byte dfReserved;

        public void Deserialize(BinaryReader reader)
        {
            dfVersion = reader.ReadUInt16();
            dfSize = reader.ReadUInt32();
            
            byte[] copyright = reader.ReadBytes(60);
            Encoding encoding = Encoding.Default;
            int i = 0;
            while (copyright[i] != 0)
                i++;
            dfCopyright = encoding.GetString(copyright, 0, i);

            dfType = reader.ReadUInt16();
            dfPoints = reader.ReadUInt16();
            dfVertRes = reader.ReadUInt16();
            dfHorizRes = reader.ReadUInt16();
            dfAscent = reader.ReadUInt16();
            dfInternalLeading = reader.ReadUInt16();
            dfExternalLeading = reader.ReadUInt16();
            dfItalic = reader.ReadByte();
            dfUnderline = reader.ReadByte();
            dfStrikeOut = reader.ReadByte();
            dfWeight = reader.ReadUInt16();
            dfCharSet = reader.ReadByte();
            dfPixWidth = reader.ReadUInt16();
            dfPixHeight = reader.ReadUInt16();
            dfPitchAndFamily = reader.ReadByte();
            dfAvgWidth = reader.ReadUInt16();
            dfMaxWidth = reader.ReadUInt16();
            dfFirstChar = reader.ReadByte();
            dfLastChar = reader.ReadByte();
            dfDefaultChar = reader.ReadByte();
            dfBreakChar = reader.ReadByte();
            dfWidthBytes = reader.ReadUInt16();
            dfDevice = reader.ReadUInt32();
            dfFace = reader.ReadUInt32();
            dfBitsPointer = reader.ReadUInt32();
            dfBitsOffset = reader.ReadUInt32();
            dfReserved = reader.ReadByte();
        }
    }
}

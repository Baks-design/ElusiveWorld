using System.IO;
using System.Runtime.CompilerServices;
using ElusiveWorld.Core.Assets.Scripts.Utils.Helpers;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class BinaryReaderExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        /// <summary>
        /// Reads a SerializableGuid written as 4 uints (16 bytes, little-endian).
        /// </summary>
        [MethodImpl(INLINE)]
        public static SerializableGuid Read(this BinaryReader reader) =>
            new(reader.ReadUInt32(),
                reader.ReadUInt32(),
                reader.ReadUInt32(),
                reader.ReadUInt32());
    }
}
using System;
using System.IO;
using System.Runtime.CompilerServices;
using ElusiveWorld.Core.Assets.Scripts.Utils.Helpers;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class BinaryWriterExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        /// <summary>
        /// Writes a SerializableGuid as 4 uints (16 bytes, little-endian).
        /// </summary>
        [MethodImpl(INLINE)]
        public static void Write(this BinaryWriter writer, SerializableGuid guid)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            writer.Write(guid.Part1);
            writer.Write(guid.Part2);
            writer.Write(guid.Part3);
            writer.Write(guid.Part4);
        }
    }
}
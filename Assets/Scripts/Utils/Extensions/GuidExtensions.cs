using System;
using System.Runtime.CompilerServices;
using ElusiveWorld.Core.Assets.Scripts.Utils.Helpers;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class GuidExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        [MethodImpl(INLINE)]
        public static SerializableGuid ToSerializableGuid(this Guid guid)
        {
            Span<byte> bytes = stackalloc byte[16];
            guid.TryWriteBytes(bytes);
            return new SerializableGuid(
                ReadUInt32(bytes, 0),
                ReadUInt32(bytes, 4),
                ReadUInt32(bytes, 8),
                ReadUInt32(bytes, 12)
            );
        }

        [MethodImpl(INLINE)]
        public static Guid ToSystemGuid(this SerializableGuid serializable)
        {
            Span<byte> bytes = stackalloc byte[16];
            WriteUInt32(bytes, 0, serializable.Part1);
            WriteUInt32(bytes, 4, serializable.Part2);
            WriteUInt32(bytes, 8, serializable.Part3);
            WriteUInt32(bytes, 12, serializable.Part4);
            return new Guid(bytes);
        }

        [MethodImpl(INLINE)]
        static uint ReadUInt32(Span<byte> bytes, int offset) =>
            (uint)(
                bytes[offset] |
                (bytes[offset + 1] << 8) |
                (bytes[offset + 2] << 16) |
                (bytes[offset + 3] << 24)
            );

        [MethodImpl(INLINE)]
        static void WriteUInt32(Span<byte> bytes, int offset, uint value)
        {
            bytes[offset] = (byte)(value);
            bytes[offset + 1] = (byte)(value >> 8);
            bytes[offset + 2] = (byte)(value >> 16);
            bytes[offset + 3] = (byte)(value >> 24);
        }
    }
}
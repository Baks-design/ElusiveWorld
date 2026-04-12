using System;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Helpers
{
    [Serializable]
    public readonly struct SerializableGuid : IEquatable<SerializableGuid> //FIXME
    {
        [SerializeField, HideInInspector] public readonly uint Part1;
        [SerializeField, HideInInspector] public readonly uint Part2;
        [SerializeField, HideInInspector] public readonly uint Part3;
        [SerializeField, HideInInspector] public readonly uint Part4;

        public static SerializableGuid Empty => new(0, 0, 0, 0);
        public bool IsEmpty => Part1 == 0 && Part2 == 0 && Part3 == 0 && Part4 == 0;

        public SerializableGuid(uint p1, uint p2, uint p3, uint p4)
        {
            Part1 = p1;
            Part2 = p2;
            Part3 = p3;
            Part4 = p4;
        }

        public SerializableGuid(Guid guid)
        {
            var bytes = guid.ToByteArray();
            Part1 = BitConverter.ToUInt32(bytes, 0);
            Part2 = BitConverter.ToUInt32(bytes, 4);
            Part3 = BitConverter.ToUInt32(bytes, 8);
            Part4 = BitConverter.ToUInt32(bytes, 12);
        }

        public static SerializableGuid NewGuid() => Guid.NewGuid();

        public static SerializableGuid FromHexString(string hex)
        {
            if (string.IsNullOrEmpty(hex) || hex.Length != 32)
                throw new ArgumentException("Invalid GUID hex string", nameof(hex));

            return new SerializableGuid(
                Convert.ToUInt32(hex.Substring(0, 8), 16),
                Convert.ToUInt32(hex.Substring(8, 8), 16),
                Convert.ToUInt32(hex.Substring(16, 8), 16),
                Convert.ToUInt32(hex.Substring(24, 8), 16)
            );
        }

        public override string ToString() => $"{Part1:X8}{Part2:X8}{Part3:X8}{Part4:X8}";

        public Guid ToGuid()
        {
            Span<byte> bytes = stackalloc byte[16];
            Write(bytes, 0, Part1);
            Write(bytes, 4, Part2);
            Write(bytes, 8, Part3);
            Write(bytes, 12, Part4);
            return new Guid(bytes);
        }

        static void Write(Span<byte> bytes, int offset, uint value)
        {
            bytes[offset] = (byte)value;
            bytes[offset + 1] = (byte)(value >> 8);
            bytes[offset + 2] = (byte)(value >> 16);
            bytes[offset + 3] = (byte)(value >> 24);
        }

        public static implicit operator Guid(SerializableGuid g) => g.ToGuid();
        public static implicit operator SerializableGuid(Guid g) => new(g);

        public bool Equals(SerializableGuid other)
            => Part1 == other.Part1 && Part2 == other.Part2 &&
               Part3 == other.Part3 && Part4 == other.Part4;

        public override bool Equals(object obj)
            => obj is SerializableGuid other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(Part1, Part2, Part3, Part4);

        public static bool operator ==(SerializableGuid a, SerializableGuid b) => a.Equals(b);
        public static bool operator !=(SerializableGuid a, SerializableGuid b) => !a.Equals(b);
    }
}
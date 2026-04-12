using UnityEngine;
using System;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Helpers
{
    [Serializable]
    public readonly struct SerializableQuaternion : IEquatable<SerializableQuaternion>
    {
        public readonly float x;
        public readonly float y;
        public readonly float z;
        public readonly float w;

        public static SerializableQuaternion Identity => new(0f, 0f, 0f, 1f);

        public SerializableQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public override string ToString() => $"[{x}, {y}, {z}, {w}]";

        public bool Equals(SerializableQuaternion other) => x == other.x && y == other.y && z == other.z && w == other.w;

        public override bool Equals(object obj) => obj is SerializableQuaternion other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(x, y, z, w);

        public static bool operator ==(SerializableQuaternion a, SerializableQuaternion b) => a.Equals(b);

        public static bool operator !=(SerializableQuaternion a, SerializableQuaternion b) => !a.Equals(b);

        public bool Approximately(SerializableQuaternion other, float epsilon = 0.0001f) =>
            Mathf.Abs(x - other.x) < epsilon &&
                Mathf.Abs(y - other.y) < epsilon &&
                Mathf.Abs(z - other.z) < epsilon &&
                Mathf.Abs(w - other.w) < epsilon;

        public bool SameRotation(SerializableQuaternion other, float epsilon = 0.001f)
        {
            var a = (Quaternion)this;
            var b = (Quaternion)other;
            return Quaternion.Angle(a, b) < epsilon;
        }

        public Quaternion ToQuaternion() => new(x, y, z, w);

        public Quaternion ToNormalizedQuaternion() => ToQuaternion().normalized;

        public static implicit operator Quaternion(SerializableQuaternion q) => new(q.x, q.y, q.z, q.w);

        public static implicit operator SerializableQuaternion(Quaternion q) => new(q.x, q.y, q.z, q.w);
    }
}
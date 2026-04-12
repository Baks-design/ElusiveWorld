using System;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Helpers
{
    [Serializable]
    public readonly struct SerializableVector3 : IEquatable<SerializableVector3>
    {
        public readonly float x;
        public readonly float y;
        public readonly float z;

        public static SerializableVector3 Zero => new(0f, 0f, 0f);

        public SerializableVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString() => $"[{x}, {y}, {z}]";

        public bool Equals(SerializableVector3 other) => x == other.x && y == other.y && z == other.z;

        public override bool Equals(object obj) => obj is SerializableVector3 other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(x, y, z);

        public static bool operator ==(SerializableVector3 a, SerializableVector3 b) => a.Equals(b);

        public static bool operator !=(SerializableVector3 a, SerializableVector3 b) => !a.Equals(b);

        public bool Approximately(SerializableVector3 other, float epsilon = 0.0001f) =>
            Mathf.Abs(x - other.x) < epsilon &&
                Mathf.Abs(y - other.y) < epsilon &&
                Mathf.Abs(z - other.z) < epsilon;

        public static implicit operator Vector3(SerializableVector3 v) => new(v.x, v.y, v.z);

        public static implicit operator SerializableVector3(Vector3 v) => new(v.x, v.y, v.z);
    }
}
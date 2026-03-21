using System.Text;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Helpers
{
    /// <summary>
    /// Represents a serializable version of the Unity Vector3 struct.
    /// </summary>
    public struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override readonly string ToString()
        {
            var sb = new StringBuilder(24); 
            sb.Append('[');
            sb.Append(x);
            sb.Append(", ");
            sb.Append(y);
            sb.Append(", ");
            sb.Append(z);
            sb.Append(']');
            return sb.ToString();
        }

        public static implicit operator Vector3(SerializableVector3 vector) 
            => new(vector.x, vector.y, vector.z);

        public static implicit operator SerializableVector3(Vector3 vector) 
            => new(vector.x, vector.y, vector.z);
    }
}
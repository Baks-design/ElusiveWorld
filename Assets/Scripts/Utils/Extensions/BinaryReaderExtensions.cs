using System.IO;
using ElusiveWorld.Core.Assets.Scripts.Utils.Helpers;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static SerializableGuid Read(this BinaryReader reader) 
            => new(reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32());
    }
}
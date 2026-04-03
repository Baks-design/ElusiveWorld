using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class ListExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        [MethodImpl(INLINE)]
        public static void RefreshWith<T>(this List<T> list, IEnumerable<T> items)
        {
            list.Clear();
            list.AddRange(items);
        }
    }
}
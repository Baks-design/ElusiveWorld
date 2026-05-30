using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class ListExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        /// <summary>
        /// Replaces the contents of the list with the provided items.
        /// Avoids reallocations when possible.
        /// </summary>
        [MethodImpl(INLINE)]
        public static void ReplaceWith<T>(this List<T> list, IEnumerable<T> items)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));

            if (items == null)
            {
                list.Clear();
                return;
            }

            if (ReferenceEquals(list, items)) return;

            if (items is ICollection<T> collection)
            {
                list.Clear();
                list.Capacity = collection.Count;
                list.AddRange(collection);
            }
            else
            {
                list.Clear();
                list.AddRange(items);
            }
        }
    }
}
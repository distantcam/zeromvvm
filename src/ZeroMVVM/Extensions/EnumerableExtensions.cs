using System;
using System.Collections.Generic;

namespace ZeroMVVM
{
    internal static class EnumerableExtensions
    {
        public static void Apply<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }
    }
}
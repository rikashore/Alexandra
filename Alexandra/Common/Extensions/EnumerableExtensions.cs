using System;
using System.Collections.Generic;

namespace Alexandra.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static string NewlineQuote<T>(this T[] list)
            => NewlineQuote((IEnumerable<T>)list);

        public static string NewlineQuote<T>(this IEnumerable<T> list)
            => $"> {string.Join("\n> ", list)}";
        
        public static T Random<T>(this T[] source, Random random = null)
        {
            random ??= new Random();
            return source[random.Next(0, source.Length)];
        }

        public static T Random<T>(this IList<T> source, Random random = null)
        {
            random ??= new Random();
            return source[random.Next(0, source.Count)];
        }
    }
}
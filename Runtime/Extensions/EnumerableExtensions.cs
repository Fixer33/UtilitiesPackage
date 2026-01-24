using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Utilities.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action?.Invoke(item);
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> collection, T item)
        {
            if (item == null)
                return -1;
            
            int counter = 0;
            foreach (var i in collection)
            {
                if (item.Equals(i))
                {
                    return counter;
                }

                counter++;
            }

            return -1;
        }

        public static T GetRandom<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
                throw new NullReferenceException();

            int counter = 0;
            foreach (var item in collection)
            {
                counter++;
            }

            int index = Random.Range(0, counter);
            foreach (var item in collection)
            {
                if (--counter == index)
                    return item;
            }

            return default;
        }
    }
}
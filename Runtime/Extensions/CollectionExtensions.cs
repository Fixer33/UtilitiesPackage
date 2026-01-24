using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Utilities.Extensions
{
    public static class CollectionExtensions
    {
        public static T GetRandom<T>(this ICollection<T> collection)
        {
            if (collection == null)
                throw new NullReferenceException();
            
            var itemCount = collection.Count;
            if (itemCount <= 0)
                return default;

            int counter = 0;
            int index = Random.Range(0, itemCount);
            foreach (var item in collection)
            {
                if (counter == index)
                    return item;
                counter++;
            }

            return default;
        }

        public static void ClearWithGameObjectDestroy<T>(this ICollection<T> collection)
            where T : Component
        {
            collection.ForEach(i => Object.Destroy(i.gameObject));
            collection.Clear();
        }
        
        public static void ClearWithGameObjectDestroy(this ICollection<GameObject> collection)
        {
            collection.ForEach(Object.Destroy);
            collection.Clear();
        }
    }
}
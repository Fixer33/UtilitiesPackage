using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Utilities
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

    public static class ButtonExtensions
    {
        public static void AddClickListener(this Button button, UnityAction listener)
        {
            button.onClick.AddListener(listener);
        }
        
        public static void RemoveClickListener(this Button button, UnityAction listener)
        {
            button.onClick.RemoveListener(listener);
        }
        
        public static void RemoveAllClickListeners(this Button button)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
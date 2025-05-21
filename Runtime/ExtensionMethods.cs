using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

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
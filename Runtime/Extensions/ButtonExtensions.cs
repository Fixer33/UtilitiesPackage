using UnityEngine.Events;
using UnityEngine.UI;

namespace Utilities.Extensions
{
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
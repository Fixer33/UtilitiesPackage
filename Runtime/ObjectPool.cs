using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Utilities
{
    /// <summary>
    /// Wrapper for Unity default object pool
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> : IObjectPool<T>
        where T : class
    {
        private readonly UnityEngine.Pool.ObjectPool<T> _pool;
        private readonly IList<T> _activeItems;
        
        public ObjectPool(Func<T> createFunc, 
            Action<T> actionOnGet = null, Action<T> actionOnRelease = null,
            Action<T> actionOnDestroy = null, 
            bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
        {
            Action<T> onItemGet = actionOnGet;
            onItemGet += item => _activeItems.Add(item);
            onItemGet += OnItemGet;
            Action<T> onItemRelease = actionOnRelease;
            onItemRelease += item => _activeItems.Remove(item);
            onItemRelease += OnItemRelease;
            Action<T> onItemDestroy = actionOnDestroy;
            onItemDestroy += OnItemDestroy;
            
            _pool = new UnityEngine.Pool.ObjectPool<T>(() =>
                {
                    var item = createFunc.Invoke();
                    OnItemCreate(item);
                    return item;
                }, onItemGet, onItemRelease, onItemDestroy,
                collectionCheck, defaultCapacity, maxSize);
            _activeItems = new List<T>();
        }

        protected virtual void OnItemCreate(T item)
        {
            
        }

        protected virtual void OnItemDestroy(T item)
        {
            
        }

        protected virtual void OnItemRelease(T item)
        {
            
        }

        protected virtual void OnItemGet(T item)
        {
            
        }

        public T Get()
        {
            return _pool.Get();
        }

        public PooledObject<T> Get(out T v)
        {
            return _pool.Get(out v);
        }

        public void Release(T element)
        {
            _pool.Release(element);
        }

        public void Clear()
        {
            List<T> buf = new();
            buf.AddRange(_activeItems);
            buf.ForEach(i => _pool.Release(i));
            _activeItems.Clear();
            _pool.Clear();
        }

        public int CountInactive => _pool.CountInactive;
    }

    public class TemplateObjectPool<T> : ObjectPool<T> where T : MonoBehaviour
    {
        private readonly bool _setAsLastSiblingOnGet;
        
        public TemplateObjectPool(T template, bool setAsLastSiblingOnGet = true,
            Action<T> actionOnGet = null, Action<T> actionOnRelease = null, 
            Action<T> actionOnDestroy = null, 
            bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000) : 
            base(() => CreateFromTemplate(template), actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, defaultCapacity, maxSize)
        {
            _setAsLastSiblingOnGet = setAsLastSiblingOnGet;
            template.gameObject.SetActive(false);
        }

        protected override void OnItemGet(T item)
        {
            item.gameObject.SetActive(true);
            if (_setAsLastSiblingOnGet)
                item.transform.SetAsLastSibling();
        }

        protected override void OnItemRelease(T item)
        {
            item.gameObject.SetActive(false);
        }

        private static T CreateFromTemplate(T template)
        {
            return Object.Instantiate(template, template.transform.parent);
        }
    }
}
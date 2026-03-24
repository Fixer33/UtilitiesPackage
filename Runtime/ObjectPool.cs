using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Utilities
{
    /// <summary>
    /// A generic wrapper for <see cref="UnityEngine.Pool.ObjectPool{T}"/> that tracks active items.
    /// </summary>
    /// <typeparam name="T">The type of items to pool. Must be a class.</typeparam>
    public class ObjectPool<T> : IObjectPool<T>, IEnumerable<T>
        where T : class
    {
        /// <summary>
        /// List of currently active (borrowed) items.
        /// </summary>
        public IList<T> ActiveItems => _activeItems;

        private readonly UnityEngine.Pool.ObjectPool<T> _pool;
        private readonly IList<T> _activeItems;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectPool{T}"/> class.
        /// </summary>
        /// <param name="createFunc">Function to create a new item.</param>
        /// <param name="actionOnGet">Action called when an item is retrieved from the pool.</param>
        /// <param name="actionOnRelease">Action called when an item is returned to the pool.</param>
        /// <param name="actionOnDestroy">Action called when an item is destroyed.</param>
        /// <param name="collectionCheck">Whether to check if an item is already in the pool when releasing.</param>
        /// <param name="defaultCapacity">Initial capacity of the pool.</param>
        /// <param name="maxSize">Maximum number of items the pool can hold.</param>
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

        /// <summary>
        /// Called when a new item is created.
        /// </summary>
        protected virtual void OnItemCreate(T item)
        {
            
        }

        /// <summary>
        /// Called when an item is destroyed.
        /// </summary>
        protected virtual void OnItemDestroy(T item)
        {
            
        }

        /// <summary>
        /// Called when an item is released back to the pool.
        /// </summary>
        protected virtual void OnItemRelease(T item)
        {
            
        }

        /// <summary>
        /// Called when an item is retrieved from the pool.
        /// </summary>
        protected virtual void OnItemGet(T item)
        {
            
        }

        /// <summary>
        /// Retrieves an item from the pool.
        /// </summary>
        public T Get()
        {
            return _pool.Get();
        }

        /// <summary>
        /// Retrieves an item from the pool with a disposable wrapper.
        /// </summary>
        public PooledObject<T> Get(out T v)
        {
            return _pool.Get(out v);
        }

        /// <summary>
        /// Returns an item to the pool.
        /// </summary>
        public void Release(T element)
        {
            _pool.Release(element);
        }

        /// <summary>
        /// Clears the pool and destroys all items.
        /// </summary>
        public void Clear()
        {
            _pool.Clear();
        }

        /// <summary>
        /// Releases all active items back to the pool.
        /// </summary>
        public void ReleaseAll()
        {
            List<T> buf = new();
            buf.AddRange(_activeItems);
            buf.ForEach(i => _pool.Release(i));
            _activeItems.Clear();
        }

        /// <summary>
        /// Number of inactive items currently in the pool.
        /// </summary>
        public int CountInactive => _pool.CountInactive;

        [MustDisposeResource] public IEnumerator<T> GetEnumerator() => _activeItems.GetEnumerator();
        [MustDisposeResource] IEnumerator IEnumerable.GetEnumerator() => _activeItems.GetEnumerator();
    }

    /// <summary>
    /// Specialized object pool for Unity components that uses a template object for instantiation.
    /// </summary>
    /// <typeparam name="T">Component type.</typeparam>
    public class TemplateObjectPool<T> : ObjectPool<T> where T : Component
    {
        private readonly bool _setAsLastSiblingOnGet;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateObjectPool{T}"/> class.
        /// </summary>
        /// <param name="template">The template component to instantiate.</param>
        /// <param name="setAsLastSiblingOnGet">If true, moves the item to the end of its parent's children when retrieved.</param>
        /// <param name="actionOnGet">Action called on retrieval.</param>
        /// <param name="actionOnRelease">Action called on release.</param>
        /// <param name="actionOnDestroy">Action called on destruction.</param>
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
    
    /// <summary>
    /// Specialized object pool for GameObjects that uses a template GameObject for instantiation.
    /// </summary>
    public class TemplateGameObjectObjectPool : ObjectPool<GameObject>
    {
        private readonly bool _setAsLastSiblingOnGet;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateGameObjectObjectPool"/> class.
        /// </summary>
        /// <param name="template">The template GameObject to instantiate.</param>
        /// <param name="setAsLastSiblingOnGet">If true, moves the item to the end of its parent's children when retrieved.</param>
        /// <param name="actionOnGet">Action called on retrieval.</param>
        /// <param name="actionOnRelease">Action called on release.</param>
        /// <param name="actionOnDestroy">Action called on destruction.</param>
        public TemplateGameObjectObjectPool(GameObject template, bool setAsLastSiblingOnGet = true,
            Action<GameObject> actionOnGet = null, Action<GameObject> actionOnRelease = null, 
            Action<GameObject> actionOnDestroy = null, 
            bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000) : 
            base(() => CreateFromTemplate(template), actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, defaultCapacity, maxSize)
        {
            _setAsLastSiblingOnGet = setAsLastSiblingOnGet;
            template.gameObject.SetActive(false);
        }

        protected override void OnItemGet(GameObject item)
        {
            item.gameObject.SetActive(true);
            if (_setAsLastSiblingOnGet)
                item.transform.SetAsLastSibling();
        }

        protected override void OnItemRelease(GameObject item)
        {
            item.gameObject.SetActive(false);
        }

        private static GameObject CreateFromTemplate(GameObject template)
        {
            return Object.Instantiate(template, template.transform.parent);
        }
    }
}

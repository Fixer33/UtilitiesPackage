using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Non-generic base to allow a single custom editor to target all RandomPool assets.
    /// </summary>
    public abstract class RandomPoolBase : ScriptableObject
    {
        // Intentionally empty. Serves as an editor target.
    }

    /// <summary>
    /// A serializable weighted random pool ScriptableObject that can hold any Unity-inspector-serializable type.
    /// Weights are treated as percentage points and the editor/runtime can normalize them to sum to 100.
    /// </summary>
    /// <typeparam name="T">Any type serializable by Unity (primitive, struct with [Serializable], UnityEngine.Object, etc.).</typeparam>
    public abstract class RandomPool<T> : RandomPoolBase
    {
        [Serializable]
        public struct Entry
        {
            public T item;
            [Min(0f)] public float weight; // percentage points; the set is normalized to sum 100
        }

        [SerializeField] private bool autoNormalize = true;
        [SerializeField] private List<Entry> entries = new List<Entry>();

        /// <summary>
        /// Read-only view of entries.
        /// </summary>
        public IReadOnlyList<Entry> Entries => entries;

        /// <summary>
        /// Returns the total weight (should be 100 after normalization).
        /// </summary>
        public float TotalWeight
        {
            get
            {
                float sum = 0f;
                for (int i = 0; i < entries.Count; i++) sum += Mathf.Max(0f, entries[i].weight);
                return sum;
            }
        }

        /// <summary>
        /// Normalize weights so they sum exactly to 100 (or evenly distribute if total is 0 but there are items).
        /// </summary>
        public void Normalize()
        {
            ClampNonNegative();
            float sum = TotalWeight;
            if (entries.Count == 0) return;

            if (sum <= 0.0001f)
            {
                float even = 100f / entries.Count;
                for (int i = 0; i < entries.Count; i++)
                {
                    var e = entries[i];
                    e.weight = even;
                    entries[i] = e;
                }
                return;
            }

            float scale = 100f / sum;
            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                e.weight = Mathf.Max(0f, e.weight) * scale;
                entries[i] = e;
            }
        }

        /// <summary>
        /// Tries to pick a random item according to the weights. Returns false if the pool is empty or invalid.
        /// </summary>
        public bool TryGetRandom(out T value)
        {
            int idx = GetRandomIndex();
            if (idx < 0)
            {
                value = default;
                return false;
            }

            value = entries[idx].item;
            return true;
        }

        /// <summary>
        /// Picks a random item. Throws InvalidOperationException if the pool is empty or weights are all zero.
        /// </summary>
        public T GetRandom()
        {
            if (!TryGetRandom(out var v))
                throw new InvalidOperationException($"{name}: RandomPool is empty or has no positive weights.");
            return v;
        }

        /// <summary>
        /// Get a random index according to weights. Returns -1 if invalid/empty.
        /// </summary>
        public int GetRandomIndex()
        {
            if (entries == null || entries.Count == 0)
                return -1;

            float sum = TotalWeight;
            if (sum <= 0.0001f)
                return -1;

            float r = UnityEngine.Random.Range(0f, sum);
            float acc = 0f;
            for (int i = 0; i < entries.Count; i++)
            {
                float w = Mathf.Max(0f, entries[i].weight);
                acc += w;
                if (r <= acc)
                    return i;
            }

            // Fallback to last index due to floating point precision
            return entries.Count - 1;
        }

        /// <summary>
        /// Deterministic pick using a provided System.Random.
        /// </summary>
        public int GetRandomIndex(System.Random rng)
        {
            if (entries == null || entries.Count == 0)
                return -1;

            float sum = TotalWeight;
            if (sum <= 0.0001f)
                return -1;

            double r = rng.NextDouble() * sum;
            double acc = 0.0;
            for (int i = 0; i < entries.Count; i++)
            {
                double w = Mathf.Max(0f, entries[i].weight);
                acc += w;
                if (r <= acc)
                    return i;
            }
            return entries.Count - 1;
        }

        protected virtual void OnValidate()
        {
            ClampNonNegative();
            if (autoNormalize)
            {
                Normalize();
            }
        }

        private void ClampNonNegative()
        {
            if (entries == null) return;
            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                e.weight = Mathf.Max(0f, e.weight);
                entries[i] = e;
            }
        }
    }
}
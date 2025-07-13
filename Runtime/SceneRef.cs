using System;
using UnityEngine;

namespace Utilities
{
    [Serializable]
    public struct SceneRef
    {
        public string FullPath => _fullPath;

        [SerializeField] private string _fullPath;
    }
}
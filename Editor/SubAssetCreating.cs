using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.Editor
{
    internal static class SubAssetCreating
    {
        private const int MaxSecondsToPickSubAssets = 60;

        private static DateTime _targetSelectedTime = DateTime.MinValue;
        private static Object _targetObject;

        private static void MakeAssetASubAsset(Object assetToMove, Object parent)
        {
            string path = AssetDatabase.GetAssetPath(assetToMove);
            AssetDatabase.RemoveObjectFromAsset(assetToMove);
            AssetDatabase.AddObjectToAsset(assetToMove, parent);
            AssetDatabase.DeleteAsset(path);
        }

        [MenuItem("Assets/Fixer33/Sub-assets/Set selected object as target asset")]
        private static void SetSelectedObjectAsTargetAsset()
        {
            _targetObject = Selection.activeObject;
            _targetSelectedTime = DateTime.Now;
        }

        [MenuItem("Assets/Fixer33/Sub-assets/Set selected object as target asset", isValidateFunction: true)]
        private static bool SetSelectedObjectAsTargetAssetIsValid() =>
            Selection.activeObject && Selection.objects is not { Length: > 1 } &&
            Selection.activeObject is not DefaultAsset;

        [MenuItem("Assets/Fixer33/Sub-assets/Add selected assets to target asset")]
        private static void AddSelectedAssetsToTarget()
        {
            foreach (var asset in Selection.objects)
            {
                MakeAssetASubAsset(asset, _targetObject);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Fixer33/Sub-assets/Add selected assets to target asset", isValidateFunction: true)]
        private static bool AddSelectedAssetsToTargetIsValid() =>
            _targetObject != null &&
            (DateTime.Now - _targetSelectedTime).TotalSeconds < MaxSecondsToPickSubAssets &&
            Selection.objects is { Length: > 0 } &&
            Selection.objects[0] is not DefaultAsset;

        [MenuItem("Assets/Fixer33/Sub-assets/Add selected assets to asset by file dialog")]
        private static void AddSelectedAssetsToTargetByDialog()
        {
            string mainAssetPath = EditorUtility.OpenFilePanel("Select Main Asset", "Assets", "");
            if (string.IsNullOrEmpty(mainAssetPath))
            {
                Debug.LogError("No main asset selected.");
                return;
            }

            mainAssetPath = FileUtil.GetProjectRelativePath(mainAssetPath);
            var mainAsset = AssetDatabase.LoadMainAssetAtPath(mainAssetPath);
            if (mainAsset == null)
            {
                Debug.LogError("Failed to load the main asset.");
                return;
            }

            foreach (var asset in Selection.objects)
            {
                MakeAssetASubAsset(asset, mainAsset);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Fixer33/Sub-assets/Add selected assets to asset by file dialog", isValidateFunction: true)]
        private static bool AddSelectedAssetsToTargetByDialogIsValid() =>
            Selection.objects is { Length: > 0 } && Selection.objects[0] is not DefaultAsset;

        [MenuItem("Assets/Fixer33/Sub-assets/Extract Assets")]
        private static void RemoveSelectedSubAsset()
        {
            var selectedObjects = Selection.objects;

            // Get the path of the parent asset
            string parentAssetPath = AssetDatabase.GetAssetPath(selectedObjects.First());
            string parentDirectory = Path.GetDirectoryName(parentAssetPath);

            foreach (var subAsset in selectedObjects)
            {
                // Create a new asset from the sub-asset in the same directory as the parent asset
                string newAssetPath = Path.Combine(parentDirectory, $"{subAsset.name}.asset");
                newAssetPath = AssetDatabase.GenerateUniqueAssetPath(newAssetPath); // Ensure unique path
                AssetDatabase.CreateAsset(Object.Instantiate(subAsset), newAssetPath);
                AssetDatabase.RemoveObjectFromAsset(subAsset);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Sub-assets extracted successfully.");
        }

        [MenuItem("Assets/Fixer33/Sub-assets/Extract Assets", isValidateFunction: true)]
        private static bool RemoveSelectedSubAssetIsValid()
        {
            var selectedObjects = Selection.objects;

            if (selectedObjects.Length == 0) return false;

            // Check if all selected objects are sub-assets of the same parent asset
            string parentAssetPath = AssetDatabase.GetAssetPath(selectedObjects.First());

            return selectedObjects.All(obj =>
                AssetDatabase.GetAssetPath(obj) == parentAssetPath &&
                obj != AssetDatabase.LoadMainAssetAtPath(parentAssetPath)
            );
        }
    }
}
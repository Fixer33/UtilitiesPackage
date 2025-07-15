using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Utilities.Editor
{
    internal static class FBXExport
    {
        [MenuItem("Assets/Fixer33/Extract Animations/Export")]
        public static void AnimationClipsExport()
        {
            Object[] selectionAsset = Selection.GetFiltered(typeof(Object), SelectionMode.Unfiltered);
            Object[] assetsInObject;
            AnimationClip bufferClip;
            string assetFilePath, fbxName, animPath;

            const string pathKey = "last_selected_fbx_export_path";
            string lastPath = EditorPrefs.GetString(pathKey, "Assets");
            string path =  EditorUtility.OpenFolderPanel("Select Main Asset", lastPath, "");
            path = Regex.Replace(path, ".+/Assets/", "Assets/");
            foreach (Object assetFile in selectionAsset)
            {
                assetFilePath = AssetDatabase.GetAssetPath(assetFile);
                assetsInObject = AssetDatabase.LoadAllAssetsAtPath(assetFilePath);
                for (int i = 0; i < assetsInObject.Length; i++)
                {
                    if (assetsInObject[i] is not AnimationClip clip || clip.name.Contains("preview")) 
                        continue;
                        
                    animPath = path + "/" + clip.name + ".anim";
                    AnimationClip existingClip = AssetDatabase.LoadAssetAtPath(animPath, typeof(AnimationClip)) as AnimationClip;
                        
                    if (existingClip != null)
                    {
                        EditorUtility.CopySerialized(clip, existingClip);
                    }
                    else
                    {
                        bufferClip = new AnimationClip();
                        EditorUtility.CopySerialized(clip, bufferClip);
                        AssetDatabase.CreateAsset(bufferClip, animPath);
                    }
                }
            }

            EditorPrefs.SetString(pathKey, path);
            AssetDatabase.Refresh();
        }
            
            
        [MenuItem("Assets/Fixer33/Extract Animations/Export", isValidateFunction: true)]
        public static bool AnimationClipsExportIsValid()
        {
            return Selection.objects is { Length: > 0 } && Selection.objects[0] is not DefaultAsset;
        }
    }
}
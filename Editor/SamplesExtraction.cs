using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace Utilities.Editor
{
    internal static class SamplesExtraction
    {
        [MenuItem("Assets/Fixer33/Samples/Extract folder contents")]
        private static void ExtractFolderContents()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject as DefaultAsset);
            path = Application.dataPath.Replace("Assets", "") + path;
            MoveDirectoryContents(path, path);
            AssetDatabase.Refresh();
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(Selection.activeObject as DefaultAsset));
        }

        private static void MoveDirectoryContents(string directory, string samplePath)
        {
            string targetDirPath = directory.Replace(samplePath, Application.dataPath);
            if (Directory.Exists(targetDirPath) == false)
                Directory.CreateDirectory(targetDirPath);
            
            foreach (var file in Directory.GetFiles(directory))
            {
                MoveFile(file, samplePath);
            }

            foreach (var subDirectory in Directory.GetDirectories(directory))
            {
                MoveDirectoryContents(subDirectory, samplePath);
            }
        }

        private static void MoveFile(string filePath, string samplePath)
        {
            string targetPath = filePath.Replace(samplePath, Application.dataPath);
            if (File.Exists(targetPath))
            {
                int decision = EditorUtility.DisplayDialogComplex("File already exists!",
                    $"Target file for {filePath} already exists: \n{targetPath} \n What to do?",
                    "Replace existing file", "Abort", "Skip the file");

                if (decision == 1)
                    throw new OperationCanceledException("Moving files aborted by user");
                
                if (decision == 2)
                    return;

                File.Delete(targetPath);
            }

            Debug.Log(filePath + "\n" + targetPath);
            File.Move(filePath, targetPath);
        }
        
        [MenuItem("Assets/Fixer33/Samples/Extract folder contents", true)]
        private static bool ExtractFolderContents_Validate()
        {
            if (Selection.objects is not { Length: 1 })
                return false;

            if (Selection.activeObject is not DefaultAsset folderAsset)
                return false;

            string path = AssetDatabase.GetAssetPath(folderAsset);
            
            return Regex.Match(path, @"Assets/Samples/.+/[\d\.]+/[\w\d ]+$").Success;
        }
    }
}

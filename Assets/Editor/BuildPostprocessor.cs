#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Editor
{
    public static class BuildPostprocessor
    {
        public static string SourcePath, DestinationPath;
        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            SourcePath = Application.dataPath + "/Python";
            var lastSlash = pathToBuiltProject.LastIndexOf("/", StringComparison.Ordinal);
            DestinationPath = pathToBuiltProject.Remove(lastSlash)+"/Python";
            MoveDirectories(SourcePath,DestinationPath);
        }

        public static void MoveDirectories(string sourcePath,string destinationPath)
        {
            Debug.Log("Transferring all files from: *"+sourcePath+"* to *"+destinationPath+"*");
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", 
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, ".", 
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
            Debug.Log("All python files are transferred to the build location!");
        }
    }
    
}
#endif
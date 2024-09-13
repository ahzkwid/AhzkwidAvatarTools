#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.IO;

namespace Ahzkwid.AvatarTool
{
    public class AssetManager
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                //DeleteTempFolder();
            }
        }
        public enum FileOptions
        {
            Normal, NoSave, TempSave
        }
        public static string GetFolderPath(FileOptions fileOptions)
        {
            var folderPath = $"Assets/EasyWearDatas/";

            if (fileOptions == FileOptions.TempSave)
            {
                folderPath += "Temp/";
            }
            return folderPath;
        }
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void DeleteTempFolder()
        {
            var folderPath = GetFolderPath(FileOptions.TempSave);
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
            AssetDatabase.Refresh();
        }
        public static void SaveAsset(Object asset, FileOptions fileOptions)
        {

            switch (fileOptions)
            {
                case FileOptions.Normal:
                    break;
                case FileOptions.NoSave:
                    return;
                case FileOptions.TempSave:
                    break;
                default:
                    break;
            }

            var folderPath = GetFolderPath(fileOptions);

            /*
            var folderPath = $"Assets/EasyWearDatas/";

            if (fileOptions== FileOptions.TempSave)
            {
                folderPath += "Temp/";
            }
            */

            /*
            if (EditorApplication.isPlaying)
            {
                folderPath += "Temp/";
            }
            */
            if (System.IO.Directory.Exists(folderPath) == false)
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            var ext = ".asset";
            if (asset is RuntimeAnimatorController)
            {
                ext = ".controller";
            }
            if ((asset is AnimationClip) || (asset is BlendTree))
            {
                ext = ".anim";
            }
            //var fileName = $"{asset.name}{(System.DateTime.Now.Ticks - new System.DateTime(2024, 1, 1).Ticks) / 1000}";
            var fileName = asset.name;


            {
                var plusIndices = new List<int>();
                for (int i = 0; i < fileName.Length; i++)
                {
                    if (fileName[i] == '+')
                    {
                        plusIndices.Add(i);
                    }
                }

                if (plusIndices.Count >= 2)
                {
                    int secondLastPlusIndex = plusIndices[plusIndices.Count - 2];
                    fileName = fileName.Substring(secondLastPlusIndex + 1);
                }
            }



            switch (fileOptions)
            {
                case FileOptions.TempSave:
                    fileName = $"{(System.DateTime.Now.Ticks - new System.DateTime(2024, 1, 1).Ticks)}";
                    break;
                case FileOptions.Normal:
                case FileOptions.NoSave:
                default:
                    if (fileName.Length > 40)
                    {
                        fileName = $"{(System.DateTime.Now.Ticks - new System.DateTime(2024, 1, 1).Ticks)}";
                    }
                    break;
            }

            var path = $"{folderPath}/{fileName}{ext}";
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            asset.name = System.IO.Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.Refresh();

        }

    }
}

#endif
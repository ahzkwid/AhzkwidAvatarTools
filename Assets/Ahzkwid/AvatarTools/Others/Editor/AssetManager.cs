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
            var folderPath = $"Assets/EasyWearDatas";

            if (fileOptions == FileOptions.TempSave)
            {
                folderPath += "/Temp";
            }
            return folderPath;
        }
        public static string GetFolderPath(DefaultAsset forder)
        {
            var folderPath = AssetDatabase.GetAssetPath(forder);
            //folderPath =$"{folderPath}/";
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
        public static string GetUniqueFileName()
        {
            return $"{(System.DateTime.Now.Ticks - new System.DateTime(2024, 1, 1).Ticks)}";
        }
        public static void SaveAsset(Object asset, string folderPath)
        {

            if (asset == null)
            {
                return;
            }

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
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = GetUniqueFileName();
            }

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


            /*
            switch (fileOption)
            {
                case FileOptions.TempSave:
                    fileName = GetUniqueFileName();
                    break;
                case FileOptions.Normal:
                case FileOptions.NoSave:
                default:
                    if (fileName.Length > 40)
                    {
                        fileName = GetUniqueFileName();
                    }
                    break;
            }
            */

            if (fileName.Length == 0)
            {
                fileName = GetUniqueFileName();
            }
            if (fileName.Length > 40)
            {
                fileName = GetUniqueFileName();
            }

            var path = $"{folderPath}/{fileName}{ext}";
            Debug.Log($"path: {path}");
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            asset.name = System.IO.Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.Refresh();

        }
        public static void SaveAsset(Object asset, DefaultAsset forder)
        {
            var folderPath = AssetDatabase.GetAssetPath(forder);
            SaveAsset(asset, folderPath);
        }
        public static void SaveAsset(Object asset, FileOptions fileOption)
        {

            switch (fileOption)
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

            var folderPath = GetFolderPath(fileOption);
            SaveAsset(asset, folderPath);

        }

    }
}

#endif
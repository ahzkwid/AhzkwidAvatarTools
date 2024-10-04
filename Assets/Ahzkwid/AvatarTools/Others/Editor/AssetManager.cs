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
            if (fileOptions == FileOptions.NoSave)
            {
                return null;
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

        public static void SaveAsset(Object asset, string folderPath, bool refresh = true)
        {

            void AddObjectToAsset(Object asset, string path)
            {
                if (AssetDatabase.GetAssetPath(asset) == string.Empty)
                {
                    if (asset is AnimatorState)
                    {
                        asset.hideFlags = HideFlags.HideInHierarchy;
                    }
                    if (asset is AnimatorStateMachine)
                    {
                        asset.hideFlags = HideFlags.HideInHierarchy;
                    }
                    AssetDatabase.AddObjectToAsset(asset, path);
                }
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssetIfDirty(asset);
            }
            if (asset == null)
            {
                return;
            }
            if (folderPath==null)
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
            if (AssetDatabase.GetAssetPath(asset) == string.Empty)
            {
                AssetDatabase.CreateAsset(asset, path);
            }
            if (asset is RuntimeAnimatorController runtimeAnimatorController)
            {
                var controller = runtimeAnimatorController as AnimatorController;
                foreach (var layer in controller.layers)
                {
                    if (layer.stateMachine == null)
                    {
                        continue;
                    }

                    if (AssetDatabase.GetAssetPath(layer.stateMachine) == string.Empty)
                    {
                        //SaveAsset(layer.stateMachine, $"{folderPath}/StateMachine",false);
                    }
                    AddObjectToAsset(layer.stateMachine, path);

                    foreach (var state in layer.stateMachine.states)
                    {
                        if (state.state==null)
                        {
                            continue;
                        }

                        /*
                        if (AssetDatabase.GetAssetPath(state.state) == string.Empty)
                        {
                            //SaveAsset(state.state, $"{folderPath}/State");
                            AssetDatabase.AddObjectToAsset(state.state, path);
                        }
                        EditorUtility.SetDirty(state.state);
                        AssetDatabase.SaveAssetIfDirty(state.state);
                        */
                        AddObjectToAsset(state.state, path);

                        var motion = state.state.motion;
                        if (motion != null)
                        {
                            if (AssetDatabase.GetAssetPath(motion) == string.Empty)
                            {
                                SaveAsset(motion, $"{folderPath}/Motion", false);
                            }
                        }


                        foreach (var behaviour in state.state.behaviours)
                        {
                            if (behaviour == null)
                            {
                                continue;
                            }
                            //EditorUtility.SetDirty(behaviour);
                            //AssetDatabase.SaveAssetIfDirty(behaviour);

                            AddObjectToAsset(behaviour, path);
                            /*
                            if (AssetDatabase.GetAssetPath(behaviour) == string.Empty)
                            {
                                SaveAsset(behaviour, $"{folderPath}/Behaviour", false);
                            }
                            */
                        }
                    }
                }

                AssetDatabase.SaveAssets();
            }
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);

            /*

#if !YOUR_VRCSDK3_AVATARS && !YOUR_VRCSDK3_WORLDS && VRC_SDK_VRCSDK3
#if UDON
#else


            if (asset is VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu menus)
            {
                var controller = runtimeAnimatorController as AnimatorController;
                foreach (var layer in controller.layers)
                {
                    foreach (var state in layer.stateMachine.states)
                    {
                        if (state.state == null)
                        {
                            continue;
                        }

                        var motion = state.state.motion;
                        if (motion != null)
                        {
                            if (AssetDatabase.GetAssetPath(motion) == string.Empty)
                            {
                                SaveAsset(motion, $"{folderPath}/Motion");
                            }
                        }


                        foreach (var behaviour in state.state.behaviours)
                        {
                            if (behaviour == null)
                            {
                                continue;
                            }
                            //EditorUtility.SetDirty(behaviour);
                            //AssetDatabase.SaveAssetIfDirty(behaviour);
                            if (AssetDatabase.GetAssetPath(behaviour) == string.Empty)
                            {
                                SaveAsset(behaviour, $"{folderPath}/Behaviour");
                            }
                        }
                    }
                }
            }
#endif
#endif
            */
            if (refresh)
            {
                AssetDatabase.Refresh();
            }

        }
        public static void SaveAsset(Object asset, DefaultAsset forder)
        {
            var folderPath = AssetDatabase.GetAssetPath(forder);
            SaveAsset(asset, folderPath);
        }
        public static void SaveAsset(Object asset, FileOptions fileOption)
        {
            /*
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
            */
            var folderPath = GetFolderPath(fileOption);
            SaveAsset(asset, folderPath);

        }

    }
}

#endif
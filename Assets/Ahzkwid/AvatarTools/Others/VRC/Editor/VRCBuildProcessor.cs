
#if UNITY_EDITOR
using Ahzkwid.AvatarTool;
using System.Reflection;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;
using VRC.SDKBase.Editor.BuildPipeline;
using static AnimationRepairTool;
//public class VRCBuildProcessor : IVRCSDKBuildRequestedCallback, IVRCSDKPreprocessAvatarCallback

namespace Ahzkwid
{
    //public class EarlyInitialization
    //{
    //    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //    static void OnBeforeSceneLoad()
    //    {
    //        VRCBuildProcessor.MergeAll();

    //        /*
    //    void Awake()
    //    {
    //        if (EditorApplication.isPlaying)
    //        {
    //            Update();
    //        }
    //    }
    //        */
    //        Debug.Log("Merge");
    //    }
    //}
    [InitializeOnLoad]
    public class VRCBuildProcessorPlaying : IVRCSDKPreprocessAvatarCallback//IVRCSDKBuildRequestedCallback
    {
        public int callbackOrder => int.MinValue+100;
        public bool OnPreprocessAvatar(GameObject avatarGameObject)
        {
            if (EditorApplication.isPlaying)
            {
                VRCBuildProcessor.MergeAll();
            }
            else
            {
                VRCBuildProcessor.MergeAllFirst();
            }
            return true;
        }
        /*
        public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
        {
            if (EditorApplication.isPlaying)
            {
                VRCBuildProcessor.MergeAll();
            }
            return true;
        }
        */
    }


    [InitializeOnLoad]
    public class VRCBuildProcessorSave : IVRCSDKPreprocessAvatarCallback//IVRCSDKBuildRequestedCallback
    {
        public int callbackOrder => int.MinValue + 200;
        public bool OnPreprocessAvatar(GameObject avatarGameObject)
        {
            if (EditorApplication.isPlaying==false)
            {
                VRCBuildProcessor.SaveAll(avatarGameObject);
            }
            return true;
        }
    }



    [InitializeOnLoad]
    public class VRCBuildProcessor : IVRCSDKPreprocessAvatarCallback
    {
        /*
        class AllPath
        {
            public GameObject gameObject;
            public Transform[] transforms;
            public string rootPath;
            public string[] transformPaths;
            public AllPath(GameObject gameObject)
            {
                 this.gameObject = gameObject;
                 transforms = gameObject.GetComponentsInChildren<Transform>();
                 rootPath = SearchUtils.GetHierarchyPath(gameObject, false);
                 transformPaths = System.Array.ConvertAll(transforms, transform => SearchUtils.GetHierarchyPath(transform.gameObject, false));
                 transformPaths = System.Array.ConvertAll(transformPaths, path => System.IO.Path.GetRelativePath(rootPath, path));
            }
            public Transform GetTransform(string path)
            {
                var index = -1;


                index = System.Array.FindIndex(transformPaths, x => x == path);

                if (index >= 0)
                {
                    return transforms[index];
                }
                return null;
            }
            public string GetPath(Transform transform)
            {
                var index = -1;


                index = System.Array.FindIndex(transforms, x => x == transform);

                if (index >= 0)
                {
                    return transformPaths[index];
                }
                return null;
            }
        }
        */
        //public int callbackOrder => 0;
        //public int callbackOrder => -12345;
        public int callbackOrder => int.MinValue + 101;
        /*
        static VRCBuildProcessor()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }
        static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Debug.Log("Entered Play Mode");
            }
        }
        */
        /*
        public static VRC_AvatarDescriptor GetSelectedAvatar()
        {
            var type = System.Type.GetType("VRC.SDK3A.Editor.VRCSdkControlPanelAvatarBuilder, Assembly-CSharp-Editor");

            if (type == null)
                throw new System.InvalidOperationException("Ŭ������ ã�� �� �����ϴ�.");

            var fieldInfo = type.GetField("_selectedAvatar", BindingFlags.Static | BindingFlags.NonPublic);

            if (fieldInfo == null)
                throw new System.InvalidOperationException("�ʵ带 ã�� �� �����ϴ�.");

            return (VRC_AvatarDescriptor)fieldInfo.GetValue(null);
        }
        */
        static readonly System.Type[] autoSetterTypes = new System.Type[] { typeof(AutoMerge), typeof(AutoDescriptor), typeof(AutoObjectSetting), typeof(AutoPosition), typeof(AnimationCreator) };
        public static void MergeAll()
        {
            var roots = ObjectPath.GetRoots();
            foreach (var root in roots)
            {
                VRCBuildProcessor.Merge(root.gameObject);
            }

        }

        public static void MergeAllFirst()
        {
            var roots = ObjectPath.GetRoots();
            foreach (var root in roots)
            {
                var avatarGameObject = root.gameObject;
                var objects = avatarGameObject.GetComponentsInChildren<AutoDescriptor>(true);
                foreach (var item in objects)
                {
                    if (item.isAwake == false)
                    {
                        continue;
                    }
                    item.Run();
                }
            }

        }




        public static void SaveAll(GameObject avatarGameObject)
        {
            void Save(Object asset)
            {
                var fileOption = AssetManager.FileOptions.TempSave;
                if (AssetDatabase.GetAssetPath(asset) == string.Empty)
                {
                    AssetManager.SaveAsset(asset, fileOption);
                }
            }
            var avatarDescriptor = avatarGameObject.GetComponentInChildren<VRCAvatarDescriptor>(true);

            if (avatarDescriptor.customizeAnimationLayers)
            {
                for (int i = 0; i < avatarDescriptor.baseAnimationLayers.Length; i++)
                {
                    var baseAnimationLayer = avatarDescriptor.baseAnimationLayers[i];
                    if (baseAnimationLayer.isDefault)
                    {
                        continue;
                    }
                    switch (baseAnimationLayer.type)
                    {
                        case VRCAvatarDescriptor.AnimLayerType.Base:
                        case VRCAvatarDescriptor.AnimLayerType.Additive:
                        case VRCAvatarDescriptor.AnimLayerType.Gesture:
                        case VRCAvatarDescriptor.AnimLayerType.Action:
                        case VRCAvatarDescriptor.AnimLayerType.FX:
                            var fileOption = AssetManager.FileOptions.TempSave;
                            var asset = baseAnimationLayer.animatorController;
                            if (AssetDatabase.GetAssetPath(asset) == string.Empty)
                            {
                                asset.name = baseAnimationLayer.type.ToString();
                                AssetManager.SaveAsset(asset, fileOption);
                            }
                            //Save(baseAnimationLayer.animatorController);
                            break;
                        case VRCAvatarDescriptor.AnimLayerType.Deprecated0:
                        case VRCAvatarDescriptor.AnimLayerType.Sitting:
                        case VRCAvatarDescriptor.AnimLayerType.TPose:
                        case VRCAvatarDescriptor.AnimLayerType.IKPose:
                            break;
                        default:
                            break;
                    }
                }
            }
            if (avatarDescriptor.customExpressions)
            {
                //Save(avatarDescriptor.expressionParameters);
                //Save(avatarDescriptor.expressionsMenu);
            }
            /*
            if ((action.value is Animator) || (action.value is RuntimeAnimatorController))
            {
                switch (action.target)
                {
                    case AutoDescriptor.Target.PlayableLayersBase:
                    case AutoDescriptor.Target.PlayableLayersAddtive:
                    case AutoDescriptor.Target.PlayableLayersGesture:
                    case AutoDescriptor.Target.PlayableLayersAction:
                    case AutoDescriptor.Target.PlayableLayersFX:
                        break;
                    default:
                        var name = action.value.name.ToLower();
                        action.target = AutoDescriptor.Target.PlayableLayersFX;
                        if (name.Contains("base"))
                        {
                            action.target = AutoDescriptor.Target.PlayableLayersBase;
                        }
                        if (name.Contains("addtive"))
                        {
                            action.target = AutoDescriptor.Target.PlayableLayersAddtive;
                        }
                        if (name.Contains("gesture"))
                        {
                            action.target = AutoDescriptor.Target.PlayableLayersGesture;
                        }
                        if (name.Contains("action"))
                        {
                            action.target = AutoDescriptor.Target.PlayableLayersAction;
                        }
                        break;
                }
            }
            if (action.value is VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters)
            {
                action.target = AutoDescriptor.Target.ExpressionsParameters;
            }
            if (action.value is VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu)
            {
                action.target = AutoDescriptor.Target.ExpressionsMenu;
            }
            */
        }

        public static void Merge(GameObject avatarGameObject)
        {

            {
                //���������� ���� ��ġ�� �ȵ�
                var objects = avatarGameObject.GetComponentsInChildren<AutoMerge>(true);
                foreach (var item in objects)
                {
                    item.Run();
                }
            }

            {
                var objects = avatarGameObject.GetComponentsInChildren<AutoPosition>(true);
                foreach (var item in objects)
                {
                    item.Run();
                }
            }
            {
                //var scriptCode = Random.Range(0,int.MaxValue); 
                //Debug.Log($"AutoDescriptor Merge Start Time: {System.DateTime.Now} {scriptCode}");


                var objects = avatarGameObject.GetComponentsInChildren<AutoDescriptor>(true);
                foreach (var item in objects)
                {
                    item.Run();
                }


                //Debug.Log($"AutoDescriptor Merge End Time: {System.DateTime.Now} {scriptCode}");
            }
            {
                var objects = avatarGameObject.GetComponentsInChildren<AutoObjectSetting>(true);
                foreach (var item in objects)
                {
                    item.Run();
                }
            }
            {
                //RemoveAll

                foreach (var type in autoSetterTypes)
                {
                    var objects = avatarGameObject.GetComponentsInChildren(type, true);
                    foreach (var item in objects)
                    {
                        Object.DestroyImmediate(item);
                    }
                }
            }
        }
        static void Run(GameObject avatarGameObject)
        {
            VRC_AvatarDescriptor selectedAvatar = null;





            var fieldInfo = typeof(VRC.SDK3A.Editor.VRCSdkControlPanelAvatarBuilder).GetField("_selectedAvatar", BindingFlags.Static | BindingFlags.NonPublic);
            selectedAvatar = (VRC_AvatarDescriptor)fieldInfo.GetValue(null);
            //selectedAvatar = GetSelectedAvatar();

            /*
            {
                var gameObject = selectedAvatar.gameObject;
                var transforms = gameObject.GetComponentsInChildren<Transform>();
                var rootPath = SearchUtils.GetHierarchyPath(gameObject, false);
                var transformPaths = System.Array.ConvertAll(transforms, transform => SearchUtils.GetHierarchyPath(transform.gameObject, false));
                transformPaths = System.Array.ConvertAll(transformPaths, path => System.IO.Path.GetRelativePath(rootPath, path));
            } 
            */

            {
                {
                    //ObjectPath.ComponentsCopy(selectedAvatar.transform, avatarGameObject.transform, autoSetterTypes);
                }
                Merge(avatarGameObject);

            }




            //AnimatorCombiner.SaveAsset(avatarGameObject, true);

            if (debugMode)
            {
                Object.Instantiate(avatarGameObject);
            }
            
            
        }
        static bool debugMode = false;


        private void UnsubscribeEvents()
        {
            if (builder == null)
            {
                var fieldInfo = typeof(VRC.SDK3A.Editor.VRCSdkControlPanelAvatarBuilder).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
                builder = (VRC.SDK3A.Editor.VRCSdkControlPanelAvatarBuilder)fieldInfo.GetValue(null);
            }
            if (builder != null)
            {
                builder.OnSdkBuildError -= UploadError;
                builder.OnSdkUploadFinish -= UploadFinish;
            }
        }
        private void RemoveAll()
        {
            AvatarTool.AssetManager.DeleteTempFolder();
            UnsubscribeEvents();
        }
        private void UploadError(object sender, string error)
        {
            RemoveAll();
        }
        private void UploadFinish(object sender, string message)
        {
            RemoveAll();
        }
        VRC.SDK3A.Editor.VRCSdkControlPanelAvatarBuilder builder = null;
        
        public bool OnPreprocessAvatar(GameObject avatarGameObject)
        {
            Debug.Log($"PreprocessAvatar.{System.DateTime.Now}");


            //AvatarTool.AssetManager.DeleteTempFolder();

            if (EditorApplication.isPlaying==false)
            {
                {
                    var fieldInfo = typeof(VRC.SDK3A.Editor.VRCSdkControlPanelAvatarBuilder).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
                    builder = (VRC.SDK3A.Editor.VRCSdkControlPanelAvatarBuilder)fieldInfo.GetValue(null);
                }
                if (builder != null)
                {
                    UnsubscribeEvents();
                    builder.OnSdkUploadFinish += UploadFinish;
                    builder.OnSdkBuildError += UploadError;
                }


                //avatar = avatarGameObject;

                /*
                Debug.Log($"avatarGameObject.name: {avatarGameObject.name}");
                {
                    var objects = avatar.GetComponentsInChildren<TestScr>(true);


                    Debug.Log($"objects.Length : {objects.Length}");
                    foreach (var item in objects)
                    {
                        Debug.Log($"item.Run.{System.DateTime.Now}");
                        item.Run();
                        UnityEditor.EditorUtility.SetDirty(item.handyfan);
                    }
                }
                */

                //{

                //    var editorWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
                //    foreach (var window in editorWindows)
                //    {
                //        /*
                //        if (window.GetType().Name == "VRCSdkControlPanelAvatarBuilder")
                //        {
                //            var fieldInfo = window.GetType().GetField("_selectedAvatar", BindingFlags.Instance | BindingFlags.NonPublic);
                //            if (fieldInfo != null)
                //            {
                //                selectedAvatar = (VRC_AvatarDescriptor)fieldInfo.GetValue(window);
                //                break;
                //            }
                //        }
                //        */
                //    }

                //}
                Run(avatarGameObject);

                /*
                {
                    var objects = avatar.GetComponentsInChildren<Transform>(true);


                    Debug.Log($"objects.Length : {objects.Length}");
                    foreach (var item in objects)
                    {
                        if (item.name.ToLower().Contains("cube"))
                        {
                            item.gameObject.SetActive(false);
                        }
                    }
                }
                */
            }
            if (debugMode)
            {
                return false;
            }
            return true;
        }
        /*
        static VRCBuildProcessor()
        {
            //EditorApplication.quitting += RestoreStates;
        }
        static string json=null;
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            // ���� ������ �����մϴ�.
            //RestoreScene();

            Debug.Log("Scene restored to original state.");
        }
        private static void RestoreStates()
        {
            //Undo.PerformUndo();
            //RestoreScene();
        }

        private static string SaveSceneBackup()
        {
            string backupPath = "Assets/TempBackup.unity";
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), backupPath);
            return backupPath;
        }
        private static void RestoreScene()
        {
            Debug.Log("����");
            if (!string.IsNullOrEmpty(backupScenePath))
            {
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(originalScenePath) != null)
                {
                    EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), originalScenePath);
                }

                AssetDatabase.DeleteAsset(backupScenePath);
                Debug.Log("Scene restored to original state.");
            }
        }
        private static string backupScenePath;
        private static string originalScenePath=null;
        [InitializeOnLoadMethod]
        private static void OnEditorLoaded()
        {
            // Ensure that we restore the original scene when the editor is reloaded after the build
            EditorApplication.update += () =>
            {
                //Debug.Log($"OnEditorLoaded.{DateTime.Now}");
                if (!string.IsNullOrEmpty(originalScenePath))
                {
                    //EditorSceneManager.OpenScene(originalScenePath);
                    Debug.Log($"Original scene restored.");
                    originalScenePath = null;
                }
            };
        }
        */
        //public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
        //{

        //    /*
        //     //undo��
        //    {
        //        var objects = Object.FindObjectsOfType<VRC_AvatarDescriptor>();

        //        foreach (var item in objects)
        //        {
        //            UnityEditor.Undo.RecordObject(item.gameObject, "temp");
        //        }
        //    }
        //    {
        //        var objects = Object.FindObjectsOfType<TestScr>();

        //        json = JsonUtility.ToJson(objects[0].gameObject);
        //        foreach (var item in objects)
        //        {
        //            item.Run();
        //            //Object.Destroy(item.gameObject);
        //        }
        //    }
        //    */



        //    /*
        //     //�� �������
        //    backupScenePath = SaveSceneBackup();
        //    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();


        //    VRC_AvatarDescriptor selectedAvatar = null;
        //    var fieldInfo = typeof(VRCSdkControlPanelAvatarBuilder).GetField("_selectedAvatar", BindingFlags.Static | BindingFlags.NonPublic);
        //    selectedAvatar = (VRC_AvatarDescriptor)fieldInfo.GetValue(null);
        //    {
        //        var objects = selectedAvatar.GetComponentsInChildren<TestScr>(true);

        //        foreach (var item in objects)
        //        {
        //            item.Run();
        //        }
        //    }

        //    */
        //    /*


        //    UnityEditor.Undo.RecordObject(selectedAvatar.gameObject, "temp");
        //    {
        //        var objects = selectedAvatar.GetComponentsInChildren<TestScr>(true);

        //        foreach (var item in objects)
        //        {
        //            item.Run();
        //        }
        //    }
        //    */
        //    /*
        //    //�� ������� v2
        //    //backupScenePath = SaveSceneBackup();
        //    //originalScenePath = EditorSceneManager.GetActiveScene().path;
        //    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();


        //    VRC_AvatarDescriptor selectedAvatar = null;
        //    var fieldInfo = typeof(VRCSdkControlPanelAvatarBuilder).GetField("_selectedAvatar", BindingFlags.Static | BindingFlags.NonPublic);
        //    selectedAvatar = (VRC_AvatarDescriptor)fieldInfo.GetValue(null);
        //    {
        //        var objects = selectedAvatar.GetComponentsInChildren<TestScr>(true);

        //        foreach (var item in objects)
        //        {
        //            item.Run();
        //        }
        //    }
        //    */

        //    /*
        //    //�ƹ�Ÿ �纻��
        //    VRC_AvatarDescriptor selectedAvatar = null;
        //    var fieldInfo = typeof(VRCSdkControlPanelAvatarBuilder).GetField("_selectedAvatar", BindingFlags.Static | BindingFlags.NonPublic);
        //    selectedAvatar = (VRC_AvatarDescriptor)fieldInfo.GetValue(null);
        //    var gameObject = Object.Instantiate(selectedAvatar.gameObject);
        //    selectedAvatar.gameObject.SetActive(false);
        //    //var avatarDescriptor = Object.FindObjectOfType<VRC_AvatarDescriptor>();
        //    var avatarDescriptor = gameObject.GetComponentInChildren<VRC_AvatarDescriptor>(true);
        //    if (VRCSdkControlPanel.window != null)
        //    {
        //        VRCSdkControlPanelAvatarBuilder.SelectAvatar(avatarDescriptor);
        //    }
        //    {
        //        var objects = avatarDescriptor.GetComponentsInChildren<TestScr>(true);

        //        foreach (var item in objects)
        //        {
        //            item.Run();
        //        }
        //    }
        //    //�ƹ�Ÿ �纻��2
        //    VRC_AvatarDescriptor selectedAvatar = null;
        //    {
        //        var fieldInfo = typeof(VRCSdkControlPanelAvatarBuilder).GetField("_selectedAvatar", BindingFlags.Static | BindingFlags.NonPublic);
        //        selectedAvatar = (VRC_AvatarDescriptor)fieldInfo.GetValue(null);
        //    }

        //    //var gameObject = Object.Instantiate(selectedAvatar.gameObject);
        //    //selectedAvatar.gameObject.SetActive(false);
        //    //var avatarDescriptor = Object.FindObjectOfType<VRC_AvatarDescriptor>();
        //    //var avatarDescriptor = gameObject.GetComponentInChildren<VRC_AvatarDescriptor>(true);



        //    VRCSdkControlPanelAvatarBuilder builder = null;
        //    {
        //        var fieldInfo = typeof(VRCSdkControlPanelAvatarBuilder).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
        //        builder = (VRCSdkControlPanelAvatarBuilder)fieldInfo.GetValue(null);
        //    }
        //    */
        //    /*
        //    VRCSdkControlPanelAvatarBuilder.SelectAvatar(avatarDescriptor);
        //    */
        //    /*
        //    {
        //        var fieldInfo = typeof(VRCSdkControlPanelAvatarBuilder).GetField("_selectedAvatar", BindingFlags.Static | BindingFlags.NonPublic);
        //        fieldInfo.SetValue(,avatarDescriptor);
        //    }
        //    */
        //    /*
        //    if (VRCSdkControlPanel.window != null)
        //    {
        //        //VRCSdkControlPanelAvatarBuilder.SelectAvatar(avatarDescriptor);
        //        VRCSdkControlPanelAvatarBuilder.SelectAvatar(avatarDescriptor);
        //    }
        //    */

        //    /*
        //    builder.OnSdkBuildStart -= BuildStart;
        //    builder.OnSdkBuildProgress -= BuildProgress;
        //    builder.OnSdkBuildFinish -= BuildFinish;
        //    builder.OnSdkBuildSuccess -= BuildSuccess;

        //    builder.OnSdkBuildStart += BuildStart;
        //    builder.OnSdkBuildProgress += BuildProgress;
        //    builder.OnSdkBuildFinish += BuildFinish;
        //    builder.OnSdkBuildSuccess += BuildSuccess;


        //     void BuildStart(object sender, object target)
        //    {
        //        Debug.Log($"BuildStart.{System.DateTime.Now}");
        //    }

        //     void BuildProgress(object sender, string status)
        //    {
        //        Debug.Log($"BuildProgress.{System.DateTime.Now}");
        //        //{
        //        //    var objects = selectedAvatar.GetComponentsInChildren<TestScr>(true);

        //        //    foreach (var item in objects)
        //        //    {
        //        //        //item.Run();
        //        //    }
        //        //}
        //        builder.OnSdkBuildProgress -= BuildProgress;
        //    }
        //     void BuildFinish(object sender, string status)
        //    {
        //        Debug.Log($"BuildFinish.{System.DateTime.Now}");
        //        //{
        //        //    var objects = selectedAvatar.GetComponentsInChildren<TestScr>(true);

        //        //    foreach (var item in objects)
        //        //    {
        //        //        //item.Back();
        //        //    }
        //        //}
        //        builder.OnSdkBuildFinish -= BuildFinish;
        //    }

        //    void BuildSuccess(object sender, string path)
        //    {
        //        Debug.Log($"BuildSuccess.{System.DateTime.Now}");
        //        builder.OnSdkBuildSuccess -= BuildSuccess;
        //    }
        //    {
        //        Debug.Log($"BuildRequested.{System.DateTime.Now}");
        //        //{
        //        //    var objects = selectedAvatar.GetComponentsInChildren<TestScr>(true);

        //        //    foreach (var item in objects)
        //        //    {
        //        //        //item.Run();
        //        //    }
        //        //}
        //}
        //        */
        //    /*
        //    {
        //        var objects = avatarDescriptor.GetComponentsInChildren<TestScr>(true);

        //        foreach (var item in objects)
        //        {
        //            item.Run();
        //        }
        //    }
        //    */
        //    // ���带 ��� �����Ϸ��� true, �ߴ��Ϸ��� false�� ��ȯ�մϴ�.
        //    return true;
        //}
    }
}

#endif
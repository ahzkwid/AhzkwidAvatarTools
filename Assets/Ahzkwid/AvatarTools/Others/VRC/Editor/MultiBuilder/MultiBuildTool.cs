

namespace Ahzkwid
{
#if UNITY_EDITOR

    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public class MultiBuildTool : EditorWindow
    {
        public static void Init()
        {
            var window = GetWindow<MultiBuildTool>(utility: false, title: nameof(MultiBuildTool));
            window.minSize = new Vector2(300, 200);
            //window.maxSize = window.minSize;
            window.Show();
        }
        SerializedObject serializedObject;
        string[] characterPaths = new string[]
        {
        };
        public void RemoveAllCharacter()
        {
            {
                var path = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), "VRChat", "VRChat", "Avatars");
                var d = new System.IO.DirectoryInfo(path);

                foreach (var f in d.GetFiles())
                {
                    f.Delete();
                }

                foreach (var dir in d.GetDirectories())
                {
                    dir.Delete(true);
                }

            }



            var editorWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            foreach (var window in editorWindows)
            {

                //VRCSdkControlPanelAvatarBuilder
                if (window.GetType().Name == "VRCSdkControlPanel")
                {
                    {
                        var t = window.GetType();
                        var m = t.GetMethod("FetchUploadedData", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        m?.Invoke(null, null);
                    }
                }
            }
        }
        //public void BuildAll()
        public async System.Threading.Tasks.Task BuildAll()
        {
            //{
            //    VRC.SDK3A.Editor.VRCSdkControlPanelAvatarBuilder builder = null;
            //    {
            //        var fieldInfo = typeof(VRC.SDK3A.Editor.VRCSdkControlPanelAvatarBuilder).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
            //        builder = (VRC.SDK3A.Editor.VRCSdkControlPanelAvatarBuilder)fieldInfo.GetValue(null);
            //    }
            //    if (builder != null)
            //    {
            //        {
            //            var method = builder.GetType().GetMethod("OnBuildAndPublishAction", BindingFlags.Instance | BindingFlags.NonPublic);
            //            if (method != null) method.Invoke(builder, null);

            //        }
            //        //builder.OnSdkUploadFinish += UploadFinish;
            //        //builder.OnSdkBuildError += UploadError;
            //        //{
            //        //    var m = builder.GetType().GetMethod("RunBuildAndPublish", BindingFlags.Instance | BindingFlags.NonPublic);
            //        //    var task = (System.Threading.Tasks.Task)m.Invoke(builder, new object[] { null, null });
            //        //    await task;

            //        //}
            //    }
            //    /*
            //    var windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            //    foreach (var window in windows)
            //    {
            //        if (window.GetType().Name == "VRCSdkControlPanelAvatarBuilder")
            //        {
            //            var method = window.GetType().GetMethod("OnBuildAndPublishAction", BindingFlags.Instance | BindingFlags.NonPublic);
            //            Debug.Log("BuildAll");
            //            if (method != null) method.Invoke(window, null);
            //            break;
            //        }
            //    }
            //    */

            //    //var editorWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            //    //foreach (var window in editorWindows)
            //    //{
            //    //    if (window.GetType().Name == "VRCSdkControlPanelAvatarBuilder")
            //    //    {
            //    //        var fieldInfo = window.GetType().GetField("_selectedAvatar", BindingFlags.Instance | BindingFlags.NonPublic);
            //    //        if (fieldInfo != null)
            //    //        {
            //    //            //selectedAvatar = (VRC_AvatarDescriptor)fieldInfo.GetValue(window);
            //    //            break;
            //    //        }
            //    //    }
            //    //}

            //}
        }
        public GameObject[] characters = null;
        void OnGUI()
        {
            if (serializedObject == null)
            {
                serializedObject = new SerializedObject(this);
            }
            serializedObject.Update();
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(characters)));


                EditorGUILayout.Space();
                //EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("BuildAll"))
                {
                    _ = BuildAll();
                }
                if (GUILayout.Button("Remove All TestAvatar"))
                {
                    RemoveAllCharacter();
                }
                //EditorGUILayout.EndHorizontal();
            }
            serializedObject.ApplyModifiedProperties();
        }
        GameObject guidTarget;
    }

#endif
}
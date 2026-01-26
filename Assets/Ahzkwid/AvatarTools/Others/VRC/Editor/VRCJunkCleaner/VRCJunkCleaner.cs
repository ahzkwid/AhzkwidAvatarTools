
namespace Ahzkwid
{
#if UNITY_EDITOR

    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class VRCJunkCleaner : EditorWindow
    {
        public static void Init()
        {
            var window = GetWindow<VRCJunkCleaner>(utility: false, title: nameof(VRCJunkCleaner));
            window.minSize = new Vector2(300, 200);
            //window.maxSize = window.minSize;
            window.Show();
        }


        public void Clean()
        {
            if (rootFolder == null)
            {
                Debug.LogError("Root Folder is not set.");
                return;
            }
            var rootPath = AssetDatabase.GetAssetPath(rootFolder);
            var guids = AssetDatabase.FindAssets("t:VRCExpressionsMenu", new[] { rootPath });
            var menus = System.Array.ConvertAll(guids, g => AssetDatabase.LoadAssetAtPath<VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu>(AssetDatabase.GUIDToAssetPath(g)));
            var importTargets =new List<string>();

            foreach (var menu in menus)
            {
                foreach (var control in menu.controls)
                {
                    switch (control.type)
                    {
                        case VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.Button:
                        case VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.Toggle:
                            Undo.RecordObject(menu, "VRC Junk Clean");
                            control.subParameters = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.Parameter[] { };
                            EditorUtility.SetDirty(menu);
                            importTargets.Add(AssetDatabase.GetAssetPath(menu));
                            break;
                    }
                }
            }
            AssetDatabase.SaveAssets();
            foreach (var path in importTargets)
            {
                AssetDatabase.ImportAsset(path);
            }
        }



        SerializedObject serializedObject;

        public DefaultAsset rootFolder;

        void OnGUI()
        {
            if (serializedObject == null)
            {
                serializedObject = new SerializedObject(this);
            }



            serializedObject.Update();
            {

                serializedObject.Update();
                {
                    EditorGUI.BeginChangeCheck();
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(rootFolder)));
                    }
                }
                serializedObject.ApplyModifiedProperties();


                if (GUILayout.Button("Clean"))
                {
                    Clean();
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }

#endif
}
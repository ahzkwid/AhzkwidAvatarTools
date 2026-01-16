
namespace Ahzkwid
{
    using System;
    using System.IO;
#if UNITY_EDITOR

    using UnityEditor;
    using UnityEngine;

    //class CharacterRefreshHook : AssetPostprocessor
    //{
    //    static void OnPostprocessAllAssets(string[] i, string[] d, string[] m, string[] mf)
    //    {
    //        var w = EditorWindow.GetWindow<CharacterSelectWindow>();
    //        if (w == null)
    //        {
    //            return;
    //        }
    //        w.UpdateCharacters();
    //    }
    //}
    public class CharacterSelectWindow : EditorWindow
    {
        public static void Init()
        {
            var window = GetWindow<CharacterSelectWindow>(utility: false, title: nameof(CharacterSelectWindow));
            window.minSize = new Vector2(300, 200);
            //window.maxSize = window.minSize;
            window.Show();
        }
        SerializedObject serializedObject;
        string[] characterPaths = new string[]
        {
            "Assets/Amatousagi/Plum/Plum.prefab",
            "Assets/sep-neko-ya/Eku/Prefab/Eku.prefab",
            "Assets/Shinano/Prefab/Shinano.prefab",
            "a82540363502b8740ac9f1a9087f8bcb",//nanodevi
            "Assets/Sisters!/Nochica/Prefabs/Nochica.prefab",
            "Assets/StudioYONO/Hanka-ÚèùÇ-/Prefab/Hanka.prefab",
            "Assets/sep-neko-ya/Cian_PC/Prefab/Cian.prefab",
            "Assets/RIONESTA/Rinasciita/Rinasciita.prefab",
            "Assets/PLUSONE/Mafuyu/Prefab/Mafuyu.prefab",
            "Assets/PLUSONE/Milfy/Prefab/Milfy.prefab",
            "Assets/MOCHIYAMA/Mamehinata/Prefab/Mamehinata_PC.prefab",
            "Assets/MOCHIYAMA/Kipfel/Prefab/Kipfel.prefab",
            "Assets/MANUKA/Prefab/MANUKA_lilToon.prefab",
            "Assets/LUMINA/LUMINA.prefab",
            "Assets/Lapwing/Prefabs/Lapwing.prefab",
            "Assets/KIRA_RABI/Maho/Prefab/Maho.prefab",
            "51e9e3d3e72368349861290f60704ff2",//Lasyusha
            "Assets/IKUSIA/mizuki/Prefab/liltoon/mizuki.prefab",
            "Assets/IKUSIA/mao/Prefab/mao.prefab",
            "Assets/IKUSIA/ririka/Prefab/ririka.prefab",
            "Assets/IKUSIA/rurune/Prefab/rurune.prefab",
            "Assets/HARUNOPUPU/Shiratsume/Prefab/Shiratsume_All.prefab",
            "52b73763fdd01e2408f2807ed629aa86",//ichigo
            "Assets/EMOLab Avatars/Ramune/Ramune.prefab",
            "Assets/DOLOSart/1.Milltina/prefab/Milltina.prefab",
            "Assets/Chocolate rice/0. Sio_2/Sio_2.prefab",
            "Assets/Beryl/Beryl_Full Variant.prefab",
            "Assets/BeroarN/bn0010_Marycia/Prefab/bn0010_Marycia.prefab",
            "Assets/Amatousagi/Plum/Plum.prefab",
            "Assets/Amatousagi/Chocolat/Chocolat.prefab",
            "Assets/Amatousagi/Chiffon/Chiffon.prefab",
            "Assets/onair/Alue/Prefab/Alue.prefab",
            "",
            "",
            "",
        };
        public void UpdateCharacters()
        {
            characters = System.Array.ConvertAll(characterPaths, x => {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(x);
                if (go == null)
                {
                    go= AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(x));
                }
                return go ;
            });
            characters = System.Array.FindAll(characters, x => x != null);
            System.Array.Sort(characters, (a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));

        }
        public GameObject[] characters = null;
        void OnGUI()
        {
            if (serializedObject == null)
            {
                serializedObject = new SerializedObject(this);
            }

            if (characters==null)
            {
                UpdateCharacters();
            }
            serializedObject.Update();
            {
                EditorGUILayout.Space();
                foreach (var item in characters)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUI.enabled = false;
                        {
                            EditorGUILayout.ObjectField(item, typeof(UnityEngine.GameObject), false);
                        }
                        GUI.enabled = true;
                        if (GUILayout.Button("Spawn"))
                        {
                            var poxX = 0f;
                            var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                            foreach (var root in roots)
                            {
                                if (root.activeInHierarchy == false)
                                {
                                    continue;
                                }
                                poxX = Mathf.Min(poxX, root.transform.position.x);
                            }
                            var go = (GameObject)PrefabUtility.InstantiatePrefab(item);
                            go.transform.position = new Vector3(poxX - 0.75f, 0f, 0f);

                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(characters)));


                if (GUILayout.Button("Refresh"))
                {
                    UpdateCharacters();
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("GUID Viewer");
                //EditorGUILayout.BeginHorizontal();
                {
                    guidTarget = (GameObject)EditorGUILayout.ObjectField(guidTarget, typeof(GameObject), false);
                    if (guidTarget !=null)
                    {
                        var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(guidTarget));
                        EditorGUILayout.TextField(guid);
                    }
                }
                //EditorGUILayout.EndHorizontal();
            }
            serializedObject.ApplyModifiedProperties();
        }
        GameObject guidTarget;
    }

#endif
}
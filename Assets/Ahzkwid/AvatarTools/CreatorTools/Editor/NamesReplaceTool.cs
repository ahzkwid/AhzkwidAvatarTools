


#if UNITY_EDITOR

using UnityEngine;


//텍스처 폴더 경로를 지정한 폴더 하위경로로 수정함
using UnityEditor;
[InitializeOnLoad]
class NamesReplaceTool : EditorWindow
{
    public GameObject[] gameObjects;
    [SerializeField]
    //public Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() { { " Variant", "" } };]

    [System.Serializable]
    public class NamesReplaceData
    {
        public string key;
        public string value;
    }
    public NamesReplaceData[] keyValuePairs = new NamesReplaceData[]{ new NamesReplaceData(){ key=" Variant" ,value = "" } };


    //[UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(NamesReplaceTool))]
    public static void Init()
    {
        var window = GetWindow<NamesReplaceTool>(utility: false, title: nameof(NamesReplaceTool));
        window.minSize = new Vector2(300, 200);
        //window.maxSize = window.minSize;
        window.Show();
    }
    public void NamesReplace(GameObject[] gameObjects, NamesReplaceData[] keyValuePairs)
    {
        foreach (var gameObject in gameObjects)
        {
            var name = gameObject.name;
            foreach (var keyValuePair in keyValuePairs)
            {
                name=name.Replace(keyValuePair.key, keyValuePair.value);
            }
            //

            var assetPath = AssetDatabase.GetAssetPath(gameObject);
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                gameObject.name = name;
            }
            else
            {
                AssetDatabase.RenameAsset(assetPath, name);
            }


        }
    }
    SerializedObject serializedObject;
    void OnGUI()
    {
        if (serializedObject == null)
        {
            serializedObject = new SerializedObject(this);
        }



        var allReady = true;




        serializedObject.Update();
        {

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(gameObjects)));
            if (keyValuePairs == null || keyValuePairs.Length == 0)
            {
                allReady = false;
            }
            GUI.enabled = allReady;
            if (GUILayout.Button("Reset GameObjects"))
            {
                gameObjects = null;
            }
            GUI.enabled = true;
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(keyValuePairs)));
            EditorGUILayout.Space();
            //if (textureFolder == null)
            {
                //allReady = false;
            }
            if (allReady)
            {
                //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(createBackup)));
            }
        }
        serializedObject.ApplyModifiedProperties();


        GUI.enabled = allReady;
        if (GUILayout.Button("Run"))
        {
            NamesReplace(gameObjects, keyValuePairs);
        }
        GUI.enabled = true;
    }

}
#endif

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
class PrefabPropertyCopyTool : EditorWindow
{
    public GameObject character;
    public GameObject property;
    public bool createBackup = true;


    //[UnityEditor.MenuItem("Ahzkwid/AvatarTools/" + nameof(PrefabPropertyCopyTool))]
    public static void Init()
    {
        var window = GetWindow<PrefabPropertyCopyTool>(utility: false, title: nameof(PrefabPropertyCopyTool));
        window.minSize = new Vector2(300, 150);
        //window.maxSize = window.minSize;
        window.Show();
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(character)));
            EditorGUILayout.Space();
            if (character == null)
            {
                allReady = false;
            }
            else
            {
                if (PrefabUtility.GetPrefabAssetType(character) == PrefabAssetType.NotAPrefab)
                {
                    EditorGUILayout.HelpBox("This is not a prefab", MessageType.Error);
                    allReady = false;
                }
            }


            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(property)));
            EditorGUILayout.Space();
            if (property == null)
            {
                allReady = false;
            }
            else
            {
                if (PrefabUtility.GetPrefabAssetType(property) == PrefabAssetType.NotAPrefab)
                {
                    EditorGUILayout.HelpBox("This is not a prefab", MessageType.Error);
                    allReady = false;
                }
            }
            if (allReady)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(createBackup)));
            }
        }
        serializedObject.ApplyModifiedProperties();


        GUI.enabled = allReady;
        if (GUILayout.Button("PropertyCopy"))
        {
            if (createBackup)
            {
                var characterCopy = Instantiate(character);
                characterCopy.name = character.name+" (Backup)";
                characterCopy.SetActive(false);
            }
            PropertyCopy(character, property);
        }
        GUI.enabled = true;
    }

    public void PropertyCopy(GameObject character, GameObject property)
    {
        var modifications = PrefabUtility.GetPropertyModifications(property);
        PrefabUtility.SetPropertyModifications(character, modifications);
    }
}
#endif
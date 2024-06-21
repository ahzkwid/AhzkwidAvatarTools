


#if UNITY_EDITOR
using UnityEngine;
using System.IO;
using UnityEditor.Search;
using UnityEditor;

[InitializeOnLoad]
class AvatarPoseCopyTool : EditorWindow
{
    public GameObject[] characters;
    public GameObject pose;
    public bool backupPose = true;


    //[UnityEditor.MenuItem("Ahzkwid/AvatarTools/" + nameof(AvatarPoseCopyTool))]
    public static void Init()
    {
        var window = GetWindow<AvatarPoseCopyTool>(utility: false, title: nameof(AvatarPoseCopyTool));
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(characters)));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(pose)));
            EditorGUILayout.Space();
            if (characters == null || characters.Length == 0)
            {
                allReady = false;
            }
            else
            {
                if (characters[0]!=null)
                {
                    var animator = characters[0].GetComponent<Animator>();
                    if (animator == null)
                    {
                        EditorGUILayout.HelpBox("This character does not include an animator", MessageType.Info);
                        allReady = false;
                    }
                    else if (animator.isHuman == false)
                    {
                        EditorGUILayout.HelpBox("This character is not a humanoid", MessageType.Info);
                        allReady = false;
                    }
                }
            }
            if (pose == null)
            {
                allReady = false;
            }
            else
            {
                var animator = pose.GetComponent<Animator>();
                if (animator == null)
                {
                    EditorGUILayout.HelpBox("This pose character does not include an animator", MessageType.Info);
                    allReady = false;
                }
                else if (animator.isHuman == false)
                {
                    EditorGUILayout.HelpBox("This pose character is not a humanoid", MessageType.Info);
                    allReady = false;
                }
            }
            if (allReady)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(backupPose)));
            }
        }
        serializedObject.ApplyModifiedProperties();


        //GUI.enabled = allReady;
        if (GUILayout.Button("PoseCopy"))
        {
            foreach (var character in characters)
            {
                if (backupPose)
                {
                    var characterCopy = Instantiate(character);
                    characterCopy.name = character.name + " (BackupPose)";
                    characterCopy.SetActive(false);
                }
                if (allReady)
                {
                    PoseCopy(character, pose);
                }
                else
                {
                    PoseCopyPath(character, pose);
                }
            }
        }
        GUI.enabled = true;
    }

    public void PoseCopyPath(GameObject character, GameObject pose)
    {

        var pathCharacter = SearchUtils.GetHierarchyPath(character.gameObject, false);
        var pathPose = SearchUtils.GetHierarchyPath(pose.gameObject, false);

        var characterChilds = character.GetComponentsInChildren<Transform>(true);
        var poseChilds = pose.GetComponentsInChildren<Transform>(true);

        var pathPoseChilds = System.Array.ConvertAll(poseChilds, x => SearchUtils.GetHierarchyPath(x.gameObject, false));
        var relativePathPose = System.Array.ConvertAll(pathPoseChilds, x => Path.GetRelativePath(pathCharacter, x));

        foreach (var characterChild in characterChilds)
        {
            var pathCharacterChild = SearchUtils.GetHierarchyPath(characterChild.gameObject, false);
            var relativePathCharacter = Path.GetRelativePath(pathCharacter, pathCharacterChild);


            var index = System.Array.FindIndex(relativePathPose, path => path == pathCharacterChild);
            if (index < 0)
            {
                return;
            }

            var poseChild = poseChilds[index];

            characterChild.localRotation = poseChild.localRotation;
            UnityEditor.EditorUtility.SetDirty(characterChild);
        }
    }
    public void PoseCopy(GameObject character, GameObject pose)
    {
        var characterAnimator = character.GetComponent<Animator>();
        characterAnimator.runtimeAnimatorController = null;
        var poseAnimator = pose.GetComponent<Animator>();
        foreach (var humanBodyBone in (HumanBodyBones[])System.Enum.GetValues(typeof(HumanBodyBones)))
        {

            if (humanBodyBone < 0)
            {
                continue;
            }
            if (humanBodyBone >= HumanBodyBones.LastBone)
            {
                continue;
            }
            Transform characterTransform;
            try
            {
                characterTransform = characterAnimator.GetBoneTransform(humanBodyBone);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                Debug.LogError(humanBodyBone);
                throw;
            }
            var poseTransform = poseAnimator.GetBoneTransform(humanBodyBone);
            //characterTransform.localPosition = poseTransform.localPosition;
            if (characterTransform == null)
            {
                continue;
            }
            if (poseTransform == null)
            {
                continue;
            }
            characterTransform.localRotation = poseTransform.localRotation;
        }
    }
}
#endif
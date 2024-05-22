
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
#if UNITY_EDITOR
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
                var animator = characters[0].GetComponent<Animator>();
                if (animator == null)
                {
                    EditorGUILayout.HelpBox("This character does not include an animator", MessageType.Error);
                    allReady = false;
                }
                else if (animator.isHuman == false)
                {
                    EditorGUILayout.HelpBox("This character is not a humanoid", MessageType.Error);
                    allReady = false;
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
                    EditorGUILayout.HelpBox("This pose character does not include an animator", MessageType.Error);
                    allReady = false;
                }
                else if (animator.isHuman == false)
                {
                    EditorGUILayout.HelpBox("This pose character is not a humanoid", MessageType.Error);
                    allReady = false;
                }
            }
            if (allReady)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(backupPose)));
            }
        }
        serializedObject.ApplyModifiedProperties();


        GUI.enabled = allReady;
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
                PoseCopy(character, pose);
            }
        }
        GUI.enabled = true;
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

namespace Ahzkwid
{
#if UNITY_EDITOR

    using UnityEditor;

    [InitializeOnLoad]
    class CharacterSelectWindowMenuItem
    {
        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/UserTools/" + nameof(CharacterSelectWindow))]
        public static void CharacterSelectWindowInit()
        {
            CharacterSelectWindow.Init();
        }


    }
#endif
}

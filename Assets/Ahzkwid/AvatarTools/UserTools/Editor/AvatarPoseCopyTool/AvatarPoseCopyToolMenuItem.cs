
namespace Ahzkwid
{
#if UNITY_EDITOR

    using UnityEditor;

    [InitializeOnLoad]
    class AvatarPoseCopyToolMenuItem
    {
        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/UserTools/" + nameof(AvatarPoseCopyTool))]
        public static void AvatarPoseCopyToolInit()
        {
            AvatarPoseCopyTool.Init();
        }

        
    }
#endif
}

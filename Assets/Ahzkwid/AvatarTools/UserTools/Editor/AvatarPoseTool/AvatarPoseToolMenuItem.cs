
namespace Ahzkwid
{
#if UNITY_EDITOR

    using UnityEditor;

    [InitializeOnLoad]
    class AvatarPoseCopyToolMenuItem
    {
        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/UserTools/" + nameof(AvatarPoseTool))]
        public static void AvatarPoseToolInit()
        {
            AvatarPoseTool.Init();
        }

        
    }
#endif
}

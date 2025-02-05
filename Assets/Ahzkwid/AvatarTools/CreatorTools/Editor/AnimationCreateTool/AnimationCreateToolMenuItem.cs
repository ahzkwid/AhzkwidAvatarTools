
namespace Ahzkwid
{
#if UNITY_EDITOR

    using UnityEditor;

    [InitializeOnLoad]
    class AnimationCreateToolMenuItem
    {
        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/AnimTool/" + nameof(AnimationCreateTool))]
        public static void AnimationCreateToolInit()
        {
            AnimationCreateTool.Init();
        }

        
    }
#endif
}

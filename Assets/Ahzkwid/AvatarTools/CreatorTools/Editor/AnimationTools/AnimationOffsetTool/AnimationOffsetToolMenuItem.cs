

namespace Ahzkwid
{
#if UNITY_EDITOR

    using UnityEditor;

    [InitializeOnLoad]
    class AnimationOffsetToolMenuItem
    {
        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/AnimTool/" + nameof(AnimationOffsetTool))]
        public static void Init()
        {
            AnimationOffsetTool.Init();
        }


    }
#endif
}

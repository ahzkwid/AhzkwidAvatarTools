
namespace Ahzkwid
{
#if UNITY_EDITOR

    using UnityEditor;

    [InitializeOnLoad]
    class AnimatorCombineMenuItem
    {
        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(AnimatorCombineTool))]
        public static void AnimatorCombineToolInit()
        {
            AnimatorCombineTool.Init();
        }

        
    }
#endif
}

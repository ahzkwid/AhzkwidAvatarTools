namespace Ahzkwid
{
#if UNITY_EDITOR

    using UnityEditor;

    [InitializeOnLoad]
    class MultiBuildToolMenuItem
    {
        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(MultiBuildTool))]
        public static void MultiBuildToolInit()
        {
            MultiBuildTool.Init();
        }


    }
#endif
}

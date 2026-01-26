
namespace Ahzkwid
{
#if UNITY_EDITOR

    using UnityEditor;

    [InitializeOnLoad]
    class VRCJunkCleanerMenuItem
    {
        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/AssetsTool/" + nameof(VRCJunkCleaner))]
        public static void Init()
        {
            VRCJunkCleaner.Init();
        }


    }
#endif
}

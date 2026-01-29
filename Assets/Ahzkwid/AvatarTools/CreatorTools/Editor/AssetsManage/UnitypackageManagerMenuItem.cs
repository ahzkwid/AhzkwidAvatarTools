using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ahzkwid
{
#if UNITY_EDITOR

    using UnityEditor;

    [InitializeOnLoad]
    class UnitypackageManagerMenuItem
    {
        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/AssetsTool/" + nameof(UnitypackageManager))]
        public static void Init()
        {
            UnitypackageManager.Init();
        }


    }
#endif
}


namespace Ahzkwid
{
#if UNITY_EDITOR

    using UnityEditor;

    [InitializeOnLoad]
    class MenuItems
    {
        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/MaterialTool/" + nameof(MaterialPropertyTool))]
        public static void OutlineRepairToolInit()
        {
            MaterialPropertyTool.Init();
        }



        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/MaterialTool/" + nameof(MaterialPathRepairTool))]
        public static void MaterialPathRepairToolInit()
        {
            MaterialPathRepairTool.Init();
        }


        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(BoundsRepairTool))]
        public static void BoundsRepairToolInit()
        {
            BoundsRepairTool.Init();
        }


        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(AnchorOverrideTool))]
        public static void AnchorOverrideToolInit()
        {
            AnchorOverrideTool.Init();
        }


        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/AssetsTool/" + nameof(TexturesCompressionTool))]
        public static void TexturesCompressionToolInit()
        {
            TexturesCompressionTool.Init();
        }


        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/AssetsTool/" + nameof(AssetsCloningTool))]
        public static void AssetsCloningToolInit()
        {
            AssetsCloningTool.Init();
        }

        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/MaterialTool/" + nameof(InsertMaterialsTool))]
        public static void InsertMaterialsToolInit()
        {
            InsertMaterialsTool.Init();
        }

        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/AssetsTool/" + nameof(NamesReplaceTool))]
        public static void NamesReplaceToolInit()
        {
            NamesReplaceTool.Init();
        }

        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/AnimTool/" + nameof(AnimationRepairTool))]
        public static void AnimationRepairToolInit()
        {
            AnimationRepairTool.Init();
        }

        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(GetBoneNamesTool))]
        public static void GetBoneNamesToolInit()
        {
            GetBoneNamesTool.Init();
        }


        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(IntervalTool))]
        public static void IntervalToolInit()
        {
            IntervalTool.Init();
        }

        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(AssetsExportTool))]
        public static void AssetsExportToolInit()
        {
            AssetsExportTool.Init();
        }




        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/UserTools/" + nameof(AvatarMergeTool))]
        public static void AvatarMergeToolInit()
        {
            AvatarMergeTool.Init();
        }



        /*
        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/UserTools/" + nameof(AvatarPoseCopyTool))]
        public static void AvatarPoseCopyToolInit()
        {
            AvatarPoseCopyTool.Init();
        }
        */



        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/UserTools/" + nameof(ShapekeyCopyTool))]
        public static void ShapekeyCopyToolInit()
        {
            ShapekeyCopyTool.Init();
        }


        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/UserTools/" + nameof(PrefabPropertyCopyTool))]
        public static void PrefabPropertyCopyToolInit()
        {
            PrefabPropertyCopyTool.Init();
        }

        
    }
#endif
}


namespace Ahzkwid
{
#if UNITY_EDITOR

    using UnityEditor;

    [InitializeOnLoad]
    class MenuItems
    {
        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(MaterialPropertyTool))]
        public static void OutlineRepairToolInit()
        {
            MaterialPropertyTool.Init();
        }



        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(MaterialPathRepairTool))]
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


        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(TexturesCompressionTool))]
        public static void TexturesCompressionToolInit()
        {
            TexturesCompressionTool.Init();
        }


        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(AssetsCloningTool))]
        public static void AssetsCloningToolInit()
        {
            AssetsCloningTool.Init();
        }

        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(InsertMaterialsTool))]
        public static void InsertMaterialsToolInit()
        {
            InsertMaterialsTool.Init();
        }

        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(NamesReplaceTool))]
        public static void NamesReplaceToolInit()
        {
            NamesReplaceTool.Init();
        }

        [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(AnimationRepairTool))]
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

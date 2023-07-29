using System;
using FortRise;
using Monocle;
using TowerFall;

namespace PassThroughTeamVariantMod
{
    [Fort("com.reddude.PassThroughTeamVariantMod", "Pass Through Team Variant Mod")]
    public class Mod : FortModule
    {
        public static Atlas MyAtlas;

        public static Mod Instance;

        public Mod()
        {
            Instance = this;
        }

        public override void LoadContent()
        {
            MyAtlas = Content.LoadAtlas("atlas.xml", "atlas.png");
        }

        public override void OnVariantsRegister(MatchVariants variants, bool noPerPlayer = false)
        {
            var info = new VariantInfo(Mod.MyAtlas)
            {
                Description = "DO NOT BUMP INTO TEAMMATES ANYMORE"
                , Header = "RULES"
            };
            var passThroughTeam = variants.AddVariant(
            "PassThroughTeam", info, VariantFlags.PerPlayer, noPerPlayer);

            // variants.CreateCustomLinks(passThroughTeam); //, variants.TeamRevive);
        }

        public override Type SettingsType => typeof(PassThroughTeamVariantModSettings);
        public static PassThroughTeamVariantModSettings Settings => (PassThroughTeamVariantModSettings) Instance.InternalSettings;

        public override void Load()
        {
            PassThroughTeamVariant.Load();
        }

        public override void Unload()
        {
            PassThroughTeamVariant.Unload();
        }
    }
}

// Harmony can be supported

// [HarmonyPatch(typeof(MainMenu), "BoolToString")]
// public class MyPatcher
// {
//     static void Postfix(ref string __result)
//     {
//         if (__result == "ON")
//         {
//             __result = "ENABLED";
//             return;
//         }

//         __result = "DISABLED";
//     }
// }

/* 
Example of interppting with libraries
Learn more: https://github.com/MonoMod/MonoMod/blob/master/README-ModInterop.md
*/

//     [ModExportName("PassThroughTeamVariantModExport")]
//     public static class ModExports
//     {
//         public static int Add(int x, int y) => x + y;
//     }
// }

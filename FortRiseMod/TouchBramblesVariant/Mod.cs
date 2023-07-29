using System;
using FortRise;
using Monocle;
using TowerFall;

namespace TouchBramblesVariantMod
{
    [Fort("com.reddude.TouchBramblesVariantMod", "TouchBramblesVariantMod")]
    public class Mod : FortModule
    {
        public static Atlas MyAtlas;
        public static SpriteData Data;

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
                Description = "CREATE BRAMBLES HOWEVER THE ARCHER TOUCH",
                Header = "MODS"
            };
            var TouchBrambles = variants.AddVariant(
            "TouchBrambles", info, VariantFlags.PerPlayer, noPerPlayer);
        }

        public override Type SettingsType => typeof(TouchBramblesVariantModSettings);
        public static TouchBramblesVariantModSettings Settings => (TouchBramblesVariantModSettings) Instance.InternalSettings;

        public override void Load()
        {
            TouchBramblesVariant.Load();
            // Settings.FlightTest = () => { Music.Play("Flight"); };
            // var harmony = new Harmony("com.terriatf.TouchBramblesVariantMod");
            // Uncomment this line to patch all of Harmony's patches
            // harmony.PatchAll();

            // typeof(ModExports).ModInterop();
        }

        public override void Unload()
        {
            TouchBramblesVariant.Unload();
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

//     [ModExportName("TouchBramblesVariantModExport")]
//     public static class ModExports
//     {
//         public static int Add(int x, int y) => x + y;
//     }
// }

using FortRise;
using Monocle;
using MonoMod.ModInterop;
using TowerFall;

namespace TeamOutlineVariant
{
    [Fort("com.reddude.TeamOutlineVariant", "Team outline colors")]
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
            MyAtlas = Content.LoadAtlas("Atlas/atlas.xml", "Atlas/atlas.png");
            TeamOutlineVariant.outlines = Content.LoadAtlas("Atlas/BaseOutlines/atlas.xml", "Atlas/BaseOutlines/atlas.png");
        }

        public override void OnVariantsRegister(MatchVariants variants, bool noPerPlayer = false)
        {
            var info = new VariantInfo(Mod.MyAtlas)
            {
                Description = "KEEP THE STYLE WITH THOSE COLORED NICE OUTLINES (NO CUSTOM ARCHERS, THO)"
                , Header = "RULES"
            };
            var teamOutlineVariant = variants.AddVariant(
            "TeamOutline", info, VariantFlags.PerPlayer, noPerPlayer);

            // variants.CreateCustomLinks(teamOutlineVariant);
        }

        // public override Type SettingsType => typeof(TeamOutlineVariantSettings);
        // public static TeamOutlineVariantSettings Settings => (TeamOutlineVariantSettings) Instance.InternalSettings;

        public override void Load()
        {
            TeamOutlineVariant.Load();
            typeof(ModExports).ModInterop();
        }

        public override void Unload()
        {
            TeamOutlineVariant.Unload();
            // PinkSlime.UnloadPatch();
            // TriggerBrambleArrow.Unload();
            // PatchEnemyBramble.Unload();
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

    [ModExportName("TeamOutlineVariantExport")]
    public static class ModExports
    {
        public static bool IsTeamOutlineEnabled(Player player)
        {
            var matchVariants = player.Level.Session.MatchSettings.Variants;
            return player.Allegiance == Allegiance.Neutral ||
                   !matchVariants.GetCustomVariant("TeamOutline")[player.PlayerIndex];
        }
    }
}

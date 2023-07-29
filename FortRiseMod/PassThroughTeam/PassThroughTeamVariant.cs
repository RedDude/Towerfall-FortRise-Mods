using System.Reflection;
using MonoMod.RuntimeDetour;
using TowerFall;

namespace PassThroughTeamVariantMod
{
    public class PassThroughTeamVariant
    {
        private static IDetour hook_PlayerOnPlayer;
        
        public static void Load()
        {
            hook_PlayerOnPlayer = new Hook(
                typeof(Player).GetMethod("PlayerOnPlayer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static),
                typeof(PassThroughTeamVariant).GetMethod("PassThroughTeam_patch", BindingFlags.Public | BindingFlags.Static)
            );
        }

        public static void Unload()
        {
            hook_PlayerOnPlayer.Dispose();
        }

        
        public delegate void orig_Player_PlayerOnPlayer(Player a, Player b);

        public static void PassThroughTeam_patch(orig_Player_PlayerOnPlayer orig, Player a, Player b)
        {
            if (Mod.Settings.enabledGlobally)
            {
                return;
            }
            var matchVariants = a.Level.Session.MatchSettings.Variants;
            if (matchVariants.GetCustomVariant("PassThroughTeam")[a.PlayerIndex] && a.Allegiance == b.Allegiance && a.Allegiance != Allegiance.Neutral)
            {
                return;
            }
        
            orig(a, b);
        }
        
    }
}

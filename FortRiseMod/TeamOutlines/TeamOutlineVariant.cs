using System.Reflection;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;
using TowerFall;

namespace TeamOutlineVariant
{
    public class TeamOutlineVariant
    {
        private static IDetour hook_LoseHat;

        public static Atlas outlines;
        public static void Load()
        {
            // hook_LoseHat = new Hook(
            //     typeof(Player).GetMethod("LoseHat", BindingFlags.NonPublic | BindingFlags.Instance),
            //     typeof(PassThroughTeamVariant).GetMethod("OnHatOnCreateHat", BindingFlags.Public | BindingFlags.Static)
            // );

            On.TowerFall.Player.Added += OnPlayerOnAdded;
        }

        // public static Hat OnHatOnCreateHat(On.TowerFall.Hat.orig_CreateHat orig, ArcherData data, Allegiance color, Player.HatStates state, Vector2 position, Arrow arrow, bool flipped, int index)
        // {
        //     var hat = orig(data, color, state, position, arrow, flipped, index);
        //     var matchVariants = hat.Level.Session.MatchSettings.Variants;
        //     if (matchVariants.GetCustomVariant("TeamOutlineVariant")[hat.PlayerIndex] || hat.Allegiance == Allegiance.Neutral)
        //     {
        //         hat.
        //         return;
        //     }
        //     
        //   
        //     return hat;
        // }

        private static void OnPlayerOnAdded(On.TowerFall.Player.orig_Added orig, Player self)
        {
            orig(self);
            var matchVariants = self.Level.Session.MatchSettings.Variants;
            if (self.Allegiance == Allegiance.Neutral || !matchVariants.GetCustomVariant("TeamOutline")[self.PlayerIndex])
            {
                return;
            }
            var teamOutlineComponent = new TeamOutlineComponent(self.Allegiance == Allegiance.Red ? 
                ArcherData.GetColorA(self.PlayerIndex, Allegiance.Red) :
                ArcherData.GetColorA(self.PlayerIndex, Allegiance.Blue), 1,true, true);

            var wingsIndex = 0; 
            for (var i = 0; i < self.Components.Count; i++)
            {
                if (self.Components[i] is PlayerWings)
                {
                    wingsIndex = i;
                }
            }
            self.Add(teamOutlineComponent);
            self.Components.Remove(teamOutlineComponent);
            self.Components.Insert(wingsIndex, teamOutlineComponent);
           
        }

        public static void Unload()
        {
            On.TowerFall.Player.Added -= OnPlayerOnAdded;
        }
        
        // public static void PassThroughTeam_patch(orig_Player_PlayerOnPlayer orig, Player a, Player b)
        // {
        //     var matchVariants = a.Level.Session.MatchSettings.Variants;
        //     if (matchVariants.GetCustomVariant("PassThroughTeam")[a.PlayerIndex] && a.Allegiance == b.Allegiance && a.Allegiance != Allegiance.Neutral)
        //     {
        //         return;
        //     }
        //
        //     orig(a, b);
        // }
        
    }
}

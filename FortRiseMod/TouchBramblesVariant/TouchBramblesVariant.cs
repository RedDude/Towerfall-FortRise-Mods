using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using TowerFall;

namespace TouchBramblesVariantMod
{
    public class TouchBramblesVariant
    {
        public static Dictionary<Player, Counter> timers = new Dictionary<Player, Counter>();
        public static float startTimer = 0;
        private static int _startDelay = 150;

        public static void Load()
        {
            On.TowerFall.Player.Update += TouchBrambles_patch;
            On.TowerFall.DarkWorldControl.Added += TouchBramblesStart_patch;
        }

        public static void Unload()
        {
            On.TowerFall.Player.Update -= TouchBrambles_patch;
            On.TowerFall.DarkWorldControl.Added -= TouchBramblesStart_patch;
        }

        private static void TouchBramblesStart_patch(On.TowerFall.DarkWorldControl.orig_Added orig, DarkWorldControl self)
        {
            orig(self);
            startTimer = 0;
        }

        private static void TouchBrambles_patch(On.TowerFall.Player.orig_Update orig, Player self)
        {
            orig(self);
            var matchVariants = self.Level.Session.MatchSettings.Variants;
            if (!matchVariants.GetCustomVariant("TouchBrambles")[self.PlayerIndex])
                return;

            startTimer += Engine.TimeMult;
            
            if(startTimer < _startDelay)
                return;

            if(self.State == Player.PlayerStates.Frozen)
                return;
            
            if (timers.ContainsKey(self))
            {
                timers[self].Update();
                if (timers[self].Value <= 0)
                {
                    timers.Remove(self);
                }
                return;
            }

            if (!self.OnGround && (!CanWallJump(self, Facing.Left) && !CanWallJump(self, Facing.Right))) return;
            if (self.CollideCheck(GameTags.Brambles)) return;
            timers[self] = new Counter(30);
            var c = new Coroutine(Brambles.CreateBrambles(self.Level, self.Position, self.PlayerIndex, null, Mod.Settings.Spread, Mod.Settings.FasterWearoff));
            self.Add(c);
            //  timers[self] = new Coroutine(Brambles.CreateBrambles(self.Level, self.Position, self.PlayerIndex, null));
            // if(timers[self].)
            // self.Add(timers[self]); 

            //() => canDie = true)));

        }
        
        public static bool CanWallJump(Player player, Facing dir) => !player.CollideCheck(GameTags.Solid, player.Position + Vector2.UnitY * 5f) && player.CollideCheck(GameTags.Solid, WrapMath.Vec(player.X + (float) (2 * (int) dir), player.Y));


        // public delegate bool orig_Player_CanGrabLedge(Player self, int targetY, int direction);

        // public static bool Player_CanGrabLedge_patch(orig_Player_CanGrabLedge orig, Player self, int targetY, int direction) 
        // {
        //     var matchVariants = self.Level.Session.MatchSettings.Variants;
        //     if (matchVariants.GetCustomVariant("NoLedgeGrab")[self.PlayerIndex]) 
        //         return false;

        //     return orig(self, targetY, direction);
        // }

        // public delegate int orig_Player_GetDodgeExitState(Player self);

        // public static int Player_GetDodgeExitState(orig_Player_GetDodgeExitState orig, Player self) 
        // {
        //     var matchVariants = self.Level.Session.MatchSettings.Variants;
        //     if (matchVariants.GetCustomVariant("NoDodgeCooldowns")[self.PlayerIndex]) 
        //     {
        //         var dynData = new DynData<Player>(self);
        //         dynData.Set("dodgeCooldown", false);
        //         dynData.Dispose();
        //     }
        //     return orig(self);
        // }

        // public delegate void orig_Player_ShootArrow(Player self);

        // public static void Player_ShootArrow(orig_Player_ShootArrow orig, Player self) 
        // {
        //     var matchVariants = self.Level.Session.MatchSettings.Variants;
        //     if (matchVariants.GetCustomVariant("InfiniteArrows")[self.PlayerIndex]) 
        //     {
        //         var arrow = self.Arrows.Arrows[0];
        //         orig(self);
        //         self.Arrows.AddArrows(arrow);
        //         return;
        //     }
        //     orig(self);
        // }
    }
}

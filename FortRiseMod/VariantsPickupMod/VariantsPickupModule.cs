using System;
using System.Linq;
using FortRise;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.ModInterop;
using TowerFall;
using LevelSystem = On.TowerFall.LevelSystem;
using PauseMenu = On.TowerFall.PauseMenu;
using Player = On.TowerFall.Player;

namespace VariantsPickupMod
{


    [Fort("com.reddude.variantsPickup", "Variants Pickup")]
    public class VariantsPickupModule : FortModule
    {
        public static Atlas ExampleAtlas;
        public static SpriteData Data;

        public static VariantsPickupModule Instance;
        // private Harmony harmony;

        public VariantsPickupModule()
        {
            Instance = this;
        }

        public override Type SettingsType => typeof(VariantsPickupSettings);
        public static VariantsPickupSettings Settings => (VariantsPickupSettings) Instance.InternalSettings;

        public override void LoadContent()
        {
         
        }

        public override void Load()
        {
            On.TowerFall.Player.Die_Arrow += OnPlayerOnDie_Arrow;
            On.TowerFall.Player.Die_DeathCause_int_bool_bool += OnPlayerOnDie_DeathCause_Int_Bool_Bool;
            On.TowerFall.Session.EndRound += OnSessionOnEndRound;
            On.TowerFall.LevelSystem.Dispose += OnLevelSystemOnDispose;
            // On.TowerFall.PauseMenu.Quit += OnQuit;
            // On.TowerFall.PauseMenu.QuitAndSave += OnQuitAndSave;
            // On.TowerFall.PauseMenu.DarkWorldRestart += OnPauseMenuOnDarkWorldRestart;
            // On.TowerFall.PauseMenu.QuestRestart += OnPauseMenuOnQuestRestart;
            // On.TowerFall.PauseMenu.TrialsRestart += OnPauseMenuOnTrialsRestart;
            // RiseCore.Events.OnLevelExited += VariantPickupManager.CleanAll;

            OnLevelEnteredEvent();
            // harmony = new Harmony("com.terriatf.VariantsPickup");
            // Uncomment this line to patch all of Harmony's patches
            // harmony.PatchAll();

            // PinkSlime.LoadPatch();
            // TriggerBrambleArrow.Load();
            // PatchEnemyBramble.Load();
            // BrambleFunPatcher.Load();

            // typeof(ModExports).ModInterop();
        }

    
        public override void Unload()
        {
            On.TowerFall.Player.Die_Arrow -= OnPlayerOnDie_Arrow;
            On.TowerFall.Player.Die_DeathCause_int_bool_bool -= OnPlayerOnDie_DeathCause_Int_Bool_Bool;
            On.TowerFall.Session.EndRound -= OnSessionOnEndRound;
            On.TowerFall.LevelSystem.Dispose -= OnLevelSystemOnDispose;
            // On.TowerFall.PauseMenu.Quit -= OnQuit;
            // On.TowerFall.PauseMenu.QuitAndSave -= OnQuitAndSave;
            // On.TowerFall.PauseMenu.DarkWorldRestart -= OnPauseMenuOnDarkWorldRestart;
            // On.TowerFall.PauseMenu.QuestRestart -= OnPauseMenuOnQuestRestart;
            // On.TowerFall.PauseMenu.TrialsRestart -= OnPauseMenuOnTrialsRestart;
        }
        
        private void OnLevelEnteredEvent()
        {
            RiseCore.Events.OnLevelEntered += () =>
            {
                if (!(Engine.Instance.Scene is Level level)) return;
                VariantPickupManager.level = level;
                
                var matchVariants = level.Session.MatchSettings.Variants;
                if (level.Session.RoundIndex == 0)
                {
                    VariantPickupManager.BeginSessionVariantPickup(level);
                    AddVariant(matchVariants.BigHeads, level, 0, -50, true);
                    AddVariant(matchVariants.ClumsyArchers, level, -20, -50, false, true);
                    AddVariant(matchVariants.DoubleJumping, level, 20, -50, true);
                    AddVariant(matchVariants.MaxArrows, level, 0, 0, true);
                    AddVariant(matchVariants.DarkPortals, level, -20, 0);
                    AddVariant(matchVariants.SuperDodging, level, 20, 0);
                    AddVariant(matchVariants.SlipperyFloors, level, 0, 30);
                }

                if (level.Session.RoundIndex == 1)
                {
                    if (matchVariants.CustomVariants.Keys.Contains("TouchBrambles")) 
                        AddVariant(matchVariants.GetCustomVariant("TouchBrambles"), level, 0, 50, false, true);
                }

                // var chalicePad = new ChalicePad(new Vector2(level.Tiles.Width / 2, level.Tiles.Height / 2), 2);
                // chalicePad.LayerIndex = 0;
                // level.Tiles.Grid.CheckRect(level.Tiles.Grid.CellsY)
            };
        }
        private void OnLevelSystemOnDispose(LevelSystem.orig_Dispose orig, TowerFall.LevelSystem self)
        {
            VariantPickupManager.CleanAll();
            orig(self);
        }

        private void OnPauseMenuOnTrialsRestart(PauseMenu.orig_TrialsRestart orig, TowerFall.PauseMenu self)
        {
            if (!(Engine.Instance.Scene is Level level)) return;
            VariantPickupManager.CleanAll(level);
            orig(self);
        }

        private void OnPauseMenuOnQuestRestart(PauseMenu.orig_QuestRestart orig, TowerFall.PauseMenu self)
        {
            if (!(Engine.Instance.Scene is Level level)) return;
            VariantPickupManager.CleanAll(level);
            orig(self);
        }

        private void OnPauseMenuOnDarkWorldRestart(PauseMenu.orig_DarkWorldRestart orig, TowerFall.PauseMenu self)
        {
            if (!(Engine.Instance.Scene is Level level)) return;
            VariantPickupManager.CleanAll(level);
            orig(self);
        }

        private void OnQuit(PauseMenu.orig_Quit orig, TowerFall.PauseMenu self)
        {
            if (!(Engine.Instance.Scene is Level level)) return;
            VariantPickupManager.CleanAll(level);
            orig(self);
        }
        
        private void OnQuitAndSave(PauseMenu.orig_QuitAndSave orig, TowerFall.PauseMenu self)
        {
            if (!(Engine.Instance.Scene is Level level)) return;
            VariantPickupManager.CleanAll(level);
            orig(self);
        }

        private void OnSessionOnEndRound(On.TowerFall.Session.orig_EndRound orig, Session self)
        {
            VariantPickupManager.EndRoundVariantPickupRevert();
            orig(self);
        }

        private PlayerCorpse OnPlayerOnDie_DeathCause_Int_Bool_Bool(Player.orig_Die_DeathCause_int_bool_bool orig, TowerFall.Player self, DeathCause cause, int index, bool brambled, bool laser)
        {
            VariantPickupManager.OnDeathVariantPickupRevert(self);
            return orig(self, cause, index, brambled, laser);
        }

        private void OnPlayerOnDie_Arrow(Player.orig_Die_Arrow orig, TowerFall.Player self, Arrow arrow)
        {
            VariantPickupManager.OnDeathVariantPickupRevert(self);
            orig(self, arrow);
        }

        public void AddVariant(Variant variant, Level level, float x, float y, bool onDeath = false, bool onRound = false)
        {
            var pickup = new VariantPickup(variant,
                new Vector2(level.Tiles.Width / 2 + x, (level.Tiles.Height / 2) + y),
                new Vector2(level.Tiles.Width / 2 + x, (level.Tiles.Height / 2) + y))
            {
                IsLoseOnDeath = onDeath,
                IsLoseOnEndRound = onRound
            };

            level.Add(pickup);
            
        }

        public override void Initialize()
        {
            // ModExports.QuestLevelXMLModifier?.Invoke("Content/Levels/Quest/00.oel", x =>
            // {
            //     var playerSpawns = x["Entities"].GetElementsByTagName("PlayerSpawn");
            //     playerSpawns[0].Attributes["x"].Value = "50";
            //     playerSpawns[1].Attributes["x"].Value = "250";
            // });
            // var vector2 = new Vector2(40, 40);
            // Logger.Log(vector2.X);
            // Logger.Log(vector2.Y);
        }

       
    }

// Harmony can be supported

    // [HarmonyPatch]
    // public class MyPatcher
    // {
    //     [HarmonyPostfix]
    //     [HarmonyPatch(typeof(MainMenu), "BoolToString")]
    //     static void BoolToStringPostfix(ref string __result)
    //     {
    //         if (__result == "ON")
    //         {
    //             __result = "ENABLED";
    //             return;
    //         }
    //
    //         __result = "DISABLED";
    //     }
    // }


/* 
Example of interppting with libraries
Learn more: https://github.com/MonoMod/MonoMod/blob/master/README-ModInterop.md
*/

    // [ModImportName("MapPatcherHelper")]
    // public static class ModExports
    // {
    //     public static Action<string, Action<XmlElement>> QuestLevelXMLModifier;
    // }
}

using System;
using System.Collections.Generic;
using FortRise;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace ReaperChaliceVariantMod
{
    [Fort("com.reddude.ReaperChaliceVariantMod", "ReaperChaliceVariantMod")]
    public class Mod : FortModule
    {
        public static Atlas MyAtlas;
        public static SpriteData Data;

        public static Mod Instance;
        public static List<Rectangle> rects;

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
                Description = "FILL THE CHALICE TO SUMMON THE REAPER",
                Header = "MODS"
            };
            var ReaperChalice = variants.AddVariant(
            "ReaperChalice", info, VariantFlags.None, noPerPlayer);
        }

        public override Type SettingsType => typeof(ReaperChaliceVariantModSettings);
        public static ReaperChaliceVariantModSettings Settings => (ReaperChaliceVariantModSettings) Instance.InternalSettings;

        public override void Load()
        {
            ReaperChaliceVariant.Load();


            // RiseCore.Events.OnLevelEntered += ReaperChaliceVariant.StartChalices;

            // Settings.FlightTest = () => { Music.Play("Flight"); };
            // var harmony = new Harmony("com.terriatf.ReaperChaliceVariantMod");
            // Uncomment this line to patch all of Harmony's patches
            // harmony.PatchAll();

            // typeof(ModExports).ModInterop();
        }
        
      
        
        public List<Rectangle> FindRectangles(LevelTiles grid)
        {
            var rows = grid.Grid.CellsX;
            var columns = grid.Grid.CellsY;
            
            var rectangles = new List<Rectangle>();

            for (var top = 0; top < rows; top++)
            {
                for (var left = 0; left < columns; left++)
                {
                    if (grid.Grid[top, left]) continue;
                    var bottom = top;
                    // for (var bottom = top; bottom < rows; bottom++)
                    // {
                        for (var right = left; right < columns; right++)
                        {
                            if (!IsRectangle(grid.Grid, top, left, bottom, right)) continue;
                            var width = right - left + 1;
                            var height = bottom - top + 1;

                            // var isFullGround = true;
                            // for (var i = 0; i < width; i++)
                            // {
                            //     if (grid.Grid[left + i, top + height])
                            //     {
                            //         isFullGround = false;
                            //         break;
                            //     }
                            // }
                            //
                            // if(!isFullGround)
                                rectangles.Add(new Rectangle(top, left, width, height));
                            
                        }
                    // }
                }
            }

            return rectangles;
        }

        private bool IsRectangle(Grid grid, int top, int left, int bottom, int right)
        {
            for (var row = top; row <= bottom; row++)
            {
                for (var col = left; col <= right; col++)
                {
                    if (grid[row, col])
                        return false;
                }
            }

            return true;
        }

        public override void Unload()
        {
            ReaperChaliceVariant.Unload();
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

//     [ModExportName("ReaperChaliceVariantModExport")]
//     public static class ModExports
//     {
//         public static int Add(int x, int y) => x + y;
//     }
// }

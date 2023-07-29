using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using TowerFall;

namespace ReaperChaliceVariantMod
{
    public class ReaperChaliceVariant
    {
        public static Dictionary<Player, Counter> timers = new Dictionary<Player, Counter>();
        public static float startTimer = 0;
        private static int _startDelay = 150;

        private static bool done = false;
        
        public static void Load()
        {
            // On.TowerFall.Player.Update += ReaperChalice_patch;
            // On.TowerFall.RoundLogic.OnLevelLoadFinish += (orig, self) =>
            // {
            //     orig(self);
            //     StartChalices(self.Session.CurrentLevel);
            // };
            On.TowerFall.RoundLogic.OnLevelLoadFinish += (orig, self) =>
            {
                orig(self);
                StartChalices(self.Session.CurrentLevel);
            };
            On.TowerFall.LevelEntity.Render += (orig, self) =>
            {
                // 
                orig(self);
                var tilesGrid = self.Level.Tiles.Grid;
                
                // for (var i = 0; i < tilesGrid.CellsX; i++)
                // {
                //     for (var j = 0; j < tilesGrid.CellsY; j++)
                //     {
                //         if (!tilesGrid[i, j] && tilesGrid[i, j + 1])
                //         {
                //             Draw.HollowRect(tilesGrid.AbsoluteLeft + i * tilesGrid.CellWidth,
                //                 tilesGrid.AbsoluteTop + j * tilesGrid.CellHeight, tilesGrid.CellWidth,
                //                 tilesGrid.CellHeight, tilesGrid[i, j] ? Color.Red : Color.Cyan);
                //         }
                //     }
                // }

                if(Mod.rects != null)
                for (var i = 0; i < Mod.rects.Count; i++)
                {
                    var rectangle = Mod.rects[i];
                    // var b = rectangle.Height < 4 || rectangle.Width < 2;
                    // if(b)
                    //     continue;
                    
                    Draw.HollowRect(
                        (tilesGrid.AbsoluteLeft + rectangle.X) * tilesGrid.CellWidth, 
                        (tilesGrid.AbsoluteTop + (rectangle.Y - rectangle.Height) + 1) * tilesGrid.CellHeight, 
                    rectangle.Width * tilesGrid.CellWidth, 
                        (rectangle.Height)  * tilesGrid.CellHeight,
                        tilesGrid.CenterX / 10 == rectangle.X || tilesGrid.CenterY / 10 == rectangle.Y ? Color.Red : Color.Cyan);
                
                    // Draw.HollowRect((Mod.rects[i].X )* self.Level.Tiles.Grid.CellWidth, 
                    //     (Mod.rects[i].Y  - 1)* self.Level.Tiles.Grid.CellHeight,
                    //     Mod.rects[i].Width * self.Level.Tiles.Grid.CellWidth,
                    //     Mod.rects[i].Height * self.Level.Tiles.Grid.CellHeight, Color.Cyan);
                    // Draw.HollowRect(Mod.rects[i].X * self.Level.Tiles.Grid.CellWidth, 
                    // Mod.rects[i].Y * self.Level.Tiles.Grid.CellHeight,
                    // Mod.rects[i].Width,
                    // Mod.rects[i].Height, Color.Cyan);
                    // Draw.HollowRect(Mod.rects[i].X * self.Level.Tiles.Grid.CellWidth, 
                    //     Mod.rects[i].Y * self.Level.Tiles.Grid.CellHeight,
                    //     Mod.rects[i].Width * self.Level.Tiles.Grid.CellWidth,
                    //     Mod.rects[i].Height * self.Level.Tiles.Grid.CellHeight, Color.Cyan);
                }
                
             
            };
            
            // On.TowerFall.VersusModeButton.Update += (orig, self) =>
            // {
            //     self.base_Update();
            //     if (!self.Selected)
            //     {
            //         return;
            //     }
            //
            //     if (MenuInput.Right)
            //     {
            //         int mode = (int) MainMenu.VersusMatchSettings.Mode;
            //         if (mode != 6)
            //         {
            //             mode++;
            //             MainMenu.VersusMatchSettings.Mode = (Modes) mode;
            //             Sounds.ui_move2.Play();
            //             // iconWiggler.Start();
            //             DynamicData.For(self).Invoke("OnConfirm");
            //             DynamicData.For(self).Invoke("UpdateSides");
            //         }
            //     }
            //     else if (MenuInput.Left)
            //     {
            //         int mode2 = (int) MainMenu.VersusMatchSettings.Mode;
            //         if (mode2 != 3)
            //         {
            //             mode2--;
            //             MainMenu.VersusMatchSettings.Mode = (Modes) mode2;
            //             Sounds.ui_move2.Play();
            //             // iconWiggler.Start();
            //             DynamicData.For(self).Invoke("OnConfirm");
            //             DynamicData.For(self).Invoke("UpdateSides");
            //         }
            //     }
            // };
            // On.TowerFall.DarkWorldControl.Added += ReaperChaliceStart_patch;
        }

        public static void StartChalices(Level level)
        {
            var matchVariants = level.Session.MatchSettings.Variants;
            if (!matchVariants.GetCustomVariant("ReaperChalice"))
                return;

            var chalicePad =
                new ChalicePad(new Vector2((level.Tiles.Width / 2) - 10, (level.Tiles.Height / 2) - 32), 20);

            // level.Add(chalicePad);
            // level.Add(new Chalice(chalicePad));
            Mod.rects = FindGroundRectangles(level.Tiles);
        }

        public static void Unload()
        {
            // On.TowerFall.Player.Update -= ReaperChalice_patch;
            // On.TowerFall.DarkWorldControl.Added -= ReaperChaliceStart_patch;
        }

        public static List<Rectangle> FindGroundRectangles(LevelTiles grid)
        {
            var rows = grid.Grid.CellsX;
            var columns = grid.Grid.CellsY;

            var spawns = grid.Level.Session.CurrentLevel.GetXMLPositions("PlayerSpawn");
            if (spawns.Count == 0)
            {
                spawns.AddRange(grid.Level.Session.CurrentLevel.GetXMLPositions("TeamSpawn"));
            }
            
            var treasures = grid.Level.Session.CurrentLevel.GetXMLPositions("TreasureChest");
            treasures.AddRange(grid.Level.Session.CurrentLevel.GetXMLPositions("BigTreasureChest"));
            
            var otherThings =
            grid.Level.Session.CurrentLevel.GetXMLPositions("JumpPad");
                // grid.Level.Session.CurrentLevel.GetXMLPositions("Spawner");
            // otherThings.AddRange(grid.Level.Session.CurrentLevel.GetXMLPositions("JumpPad"));
            
            var spawnsPositions = new List<Vector2>();
            var tilesGrid = grid.Level.Tiles.Grid;

            foreach (var spawn in spawns)
            {
                spawnsPositions.Add(new Vector2(spawn.X / tilesGrid.CellWidth, spawn.Y / tilesGrid.CellHeight));   
            }

            var grounds = new List<Rectangle>();
            var verticalRectangles = new List<Rectangle>();
            
            var dynamicSpawns = new List<DynamicSpawnPoint>();
            
            for (var y = 0; y < columns; y++)
            {
                for (var x = 0; x < rows; x++)
                {
                    if (!grid.Grid[x, y] && grid.Grid[x, y + 1])
                    {
                        var rectPos = new Vector2(x, y);
                        var canAdd = true;
                        var collideCheck = grid.Level.Tiles.CollideCheck(new []
                        {
                            GameTags.Actor, GameTags.Solid, GameTags.TreasureChest
                        }, x * 10, y * 10);
                        
                        if(collideCheck)
                            canAdd = false;
                        //
                        foreach (var vector2 in spawnsPositions)
                        {
                          
                            // var r = new Rectangle(x, y, 1, 1);
                            // if (r.Contains(new Point((int) vector2.X, (int) vector2.Y))
                            // || r.Contains(new Point((int) vector2.X + 1, (int) vector2.Y))
                            // || r.Contains(new Point((int) vector2.X -1, (int) vector2.Y))
                            //     || r.Contains(new Point((int) vector2.X + 2, (int) vector2.Y))
                            //     || r.Contains(new Point((int) vector2.X -2, (int) vector2.Y))
                            //     || r.Contains(new Point((int) vector2.X + 3, (int) vector2.Y))
                            //     || r.Contains(new Point((int) vector2.X - 3, (int) vector2.Y)))
                            if(Vector2.Distance(vector2, rectPos) < 2)
                            {
                                canAdd = false;
                                break;
                            }
                        }

                        // foreach (var vector2 in treasures)
                        // {
                        //     if (Math.Abs(vector2.X - rectPos.X * 10) < 20 && Math.Abs(vector2.Y - rectPos.Y * 10 ) < 30)
                        //     {
                        //         canAdd = false;
                        //     }
                        // }
                        //
                        foreach (var vector2 in otherThings)
                        {
                            if (Math.Abs(vector2.X - rectPos.X * 10) < 10 && Math.Abs(vector2.Y - rectPos.Y * 10 ) < 10)
                            {
                                canAdd = false;
                            }
                        }

                        
                        if(canAdd)
                            grounds.Add(new Rectangle(x, y, 1, 1));
                    }
                      
                }
            }
            for (var index = 0; index < grounds.Count; index++)
            {
                var rectangle = grounds[index];
                // for (var y = columns; y > 0; y--)
                for (var y = rectangle.Y-1; y >= -1; y--)
                {
                    if (!grid.Grid[rectangle.X, y] && y != rectangle.Y-4) continue;
                    if(rectangle.Y - y >= 4)
                        verticalRectangles.Add(new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Y - y));
                    break;
                }
            }

           
            return verticalRectangles;
            // var rectangles3 = new List<Rectangle>();
            //
            // var currentX = verticalRetangles[0].X;
            // var currentHeight = verticalRetangles[0].Height;
            // var currentWidth = verticalRetangles[0].Width;
            // for (var index = 0; index < verticalRetangles.Count-1; index++)
            // {
            //     var rectangle = verticalRetangles[index];
            //     var rectangleNext = verticalRetangles[index+1];
            //
            //     if (rectangleNext.X > rectangle.X + 1 || rectangle.Y != rectangleNext.Y || rectangleNext.Height < currentHeight)
            //     {
            //         var newWidth = currentWidth + (rectangleNext.X - currentX)-1;
            //         if(newWidth >= 2)
            //             rectangles3.Add(new Rectangle(currentX, rectangle.Y, newWidth, currentHeight));
            //         currentX = rectangle.X;
            //         currentHeight = rectangle.Height;
            //         currentWidth = rectangle.Width;
            //     }
            // }
            //
            // return rectangles3;
        }

        private static void ReaperChaliceStart_patch(On.TowerFall.DarkWorldControl.orig_Added orig, DarkWorldControl self)
        {
            orig(self);
            startTimer = 0;
        }

        private static void ReaperChalice_patch(On.TowerFall.Player.orig_Update orig, Player self)
        {
            orig(self);
            var matchVariants = self.Level.Session.MatchSettings.Variants;
            if (!matchVariants.GetCustomVariant("ReaperChalice")[self.PlayerIndex])
                return;

            startTimer += Engine.TimeMult;
            
            if(startTimer < _startDelay)
                return;

            if(self.State == Player.PlayerStates.Frozen)
                return;

            if (!done)
            {
           
                done = true;
            }
                
            if (timers.ContainsKey(self))
            {
                timers[self].Update();
                if (timers[self].Value <= 0)
                {
                    timers.Remove(self);
                 }
                return;
            }

            Mod.rects = FindGroundRectangles(self.Level.Tiles);
            //  timers[self] = new Coroutine(Brambles.CreateBrambles(self.Level, self.Position, self.PlayerIndex, null));
            // if(timers[self].)
            // self.Add(timers[self]); 

            //() => canDie = true)));

        }
        
        public static bool CanWallJump(Player player, Facing dir) => !player.CollideCheck(GameTags.Solid, player.Position + Vector2.UnitY * 5f) && player.CollideCheck(GameTags.Solid, WrapMath.Vec(player.X + (float) (2 * (int) dir), player.Y));

    }

    public class DynamicSpawnPoint
    {
        public Rectangle rectangle;
        public List<Entity> colisions;
    }
}

using System;
using System.Collections.Generic;
using Monocle;
using FortRise;
using TowerFall;

namespace TeamOutlineVariant
{
    
// public class TeamOutlineVariantSettings : ModuleSettings 
// {
//     // [SettingsName("Cheat Mode")]
//     // public bool CheatMode;
//     
//     [SettingsNumber(1, 20, 1)]
//     public int Spread = 2;
//     
//     [SettingsName("Faster Wearoff")]
//     public bool FasterWearoff;
//
//     // public Action FlightTest;
// }

// public static class CommandList 
// {
//     [Command("hello")]
//     public static void SayHello(string[] args) 
//     {
//         Engine.Instance.Commands.Log("Hello");
//     }
//
//     [Command("arrows")]
//     public static void AddArrow(string[] args) 
//     {
//         if (Engine.Instance.Scene is Level)
//         {
//             int num = Commands.ParseInt(args, 0, 0);
//             if (num < 0 || num >= Arrow.ARROW_TYPES + RiseCore.ArrowsID.Count)
//             {
//                 Engine.Instance.Commands.Log("Invalid arrow type!");
//                 return;
//             }
//             ArrowTypes arrowTypes = (ArrowTypes)num;
//             using (List<Entity>.Enumerator enumerator = (Engine.Instance.Scene as Level).Players.GetEnumerator())
//             {
//                 while (enumerator.MoveNext())
//                 {
//                     Entity entity = enumerator.Current;
//                     ((Player)entity).Arrows.AddArrows(new ArrowTypes[]
//                     {
//                         arrowTypes,
//                         arrowTypes
//                     });
//                 }
//                 return;
//             }
//         }
//         Engine.Instance.Commands.Log("Command can only be used during gameplay!");
//     }
// }
}

using System;
using FortRise;
using HarmonyLib;
using Monocle;
using MonoMod.Utils;
using TowerFall;

namespace SpeedrunTimer;

public static class QuestCompletePatch
{

    static SpeedrunTimerModule speedrunTimer;

    public static void Load(IHarmony harmony, SpeedrunTimerModule speedrunTimerModule)
    {
        speedrunTimer = speedrunTimerModule;

        var originalConstructor = typeof(QuestComplete).GetConstructor(new[] { typeof(QuestRoundLogic) });
        var postfix = new HarmonyMethod(typeof(QuestCompletePatch), nameof(QuestControl_LevelSequence_Postfix));
        harmony.Patch(originalConstructor, postfix: postfix);

        var questGameOverOriginalConstructor = typeof(QuestGameOver).GetConstructor(new[] { typeof(QuestRoundLogic) });
        var postfixGameOver = new HarmonyMethod(typeof(QuestCompletePatch), nameof(QuestControl_LevelSequence_Postfix));
        harmony.Patch(questGameOverOriginalConstructor, postfix: postfixGameOver);


        harmony.Patch(
            AccessTools.DeclaredMethod(typeof(DarkWorldComplete), "Sequence"),
            postfix: new HarmonyMethod(DarkWorldComplete_Restart_Prefix)
        );
    }

    private static void UsePatch(DarkWorldComplete self)
    {
        speedrunTimer.SpeedrunTimerControl.canEnd = false;
    }

    private static void UsePatch(QuestComplete self, QuestRoundLogic quest)
    {
        // Console.WriteLine(time);
        speedrunTimer.SpeedrunTimerControl.time = quest.Time;
        speedrunTimer.SpeedrunTimerControl.canEnd = false;
        Console.WriteLine(speedrunTimer.SpeedrunTimerControl.time);
    }

    private static void DarkWorldComplete_Restart_Prefix(DarkWorldComplete __instance)
    {
        if (SpeedrunTimerModule.Settings.Active) 
        {
            UsePatch(__instance);
        }
    }

    private static void QuestControl_LevelSequence_Postfix(QuestComplete __instance, QuestRoundLogic quest)
    {
        if (SpeedrunTimerModule.Settings.Active) 
        {
            UsePatch(__instance, quest);
        }
    }
}
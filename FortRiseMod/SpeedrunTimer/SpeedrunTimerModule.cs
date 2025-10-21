using System;
using FortRise;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace SpeedrunTimer;

public sealed class SpeedrunTimerModule : Mod
{
    public static SpeedrunTimerModule Instance = null!;

    public static SpeedrunTimerSettings Settings => Instance.GetSettings<SpeedrunTimerSettings>()!;

    public SpeedrunTimerControl SpeedrunTimerControl;

    public SpeedrunTimerModule(IModContent content, IModuleContext context, ILogger logger) : base(content, context, logger)
    {
        Instance = this;
        QuestCompletePatch.Load(context.Harmony, this);
        SpeedrunTimerControl = new SpeedrunTimerControl();

    var position = SpeedrunTimerModule.Settings.PinPosition;
        Console.WriteLine(position);
        
        context.Events.OnLevelLoaded += OnLevelLoaded;
        context.Events.OnLevelExited += OnLevelExited;
    }

    private void OnLevelExited(object? sender, Level e)
    {       
        SpeedrunTimerControl.RemoveSelf();
    }


    private void OnLevelLoaded(object? sender, RoundLogic e)
    {
        bool isDarkWorld = e.Session.RoundLogic is DarkWorldRoundLogic;
        bool isQuest = e.Session.RoundLogic is QuestRoundLogic;

        if (isDarkWorld || isQuest)
        {
            SpeedrunTimerControl = new SpeedrunTimerControl(e);
            e.Session.CurrentLevel.Add(SpeedrunTimerControl);
        }
    }


    public override ModuleSettings? CreateSettings()
    {
        return new SpeedrunTimerSettings();
    }
}
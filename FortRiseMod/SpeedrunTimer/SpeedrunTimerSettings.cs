using FortRise;

namespace SpeedrunTimer;

public sealed class SpeedrunTimerSettings : ModuleSettings 
{
    public bool Active { get; set; } = true;
    public string PinPosition { get; set; }

    public override void Create(ISettingsCreate settings)
    {
        settings.CreateOnOff("Active", Active, (x) => Active = x);
        settings.CreateOptions("Position", PinPosition, [
          "TOP RIGHT", "TOP CENTER", "BOTTOM RIGHT", "BOTTOM CENTER", "BOTTOM LEFT", "TOP LEFT"
      ], (x) => PinPosition = x.Item1);
    }
}
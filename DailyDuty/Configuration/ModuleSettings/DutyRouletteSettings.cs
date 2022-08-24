using System.Numerics;
using DailyDuty.Configuration.Components;
using DailyDuty.Utilities;

namespace DailyDuty.Configuration.ModuleSettings;

public class DutyRouletteSettings : GenericSettings
{
    public Setting<bool> EnableClickableLink = new(false);
    public Setting<bool> HideExpertWhenCapped = new(false);

    public Setting<bool> OverlayEnabled = new(false);
    public Setting<Vector4> CompleteColor = new(Colors.Green);
    public Setting<Vector4> IncompleteColor = new(Colors.Red);


    public TrackedRoulette[] TrackedRoulettes =
    {
        new(RouletteType.Expert, new Setting<bool>(false), false),
        new(RouletteType.Level90, new Setting<bool>(false), false),
        new(RouletteType.Level50607080, new Setting<bool>(false), false),
        new(RouletteType.Leveling, new Setting<bool>(false), false), 
        new(RouletteType.Trials, new Setting<bool>(false), false),
        new(RouletteType.MSQ, new Setting<bool>(false), false),
        new(RouletteType.Guildhest, new Setting<bool>(false), false),
        new(RouletteType.Alliance, new Setting<bool>(false), false),
        new(RouletteType.Normal, new Setting<bool>(false), false),
        new(RouletteType.Frontline, new Setting<bool>(false), false),
        new(RouletteType.Mentor, new Setting<bool>(false), false),
    };
}
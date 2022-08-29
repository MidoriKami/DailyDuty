using System.Numerics;
using DailyDuty.Configuration.Components;
using DailyDuty.Utilities;

namespace DailyDuty.Configuration.ModuleSettings;

public class DutyRouletteSettings : GenericSettings
{
    public Setting<bool> EnableClickableLink = new(false);
    public Setting<bool> HideExpertWhenCapped = new(false);

    public Setting<bool> OverlayEnabled = new(true);
    public Setting<Vector4> CompleteColor = new(Colors.Green);
    public Setting<Vector4> IncompleteColor = new(Colors.Red);
    public Setting<Vector4> OverrideColor = new(Colors.Orange);

    public TrackedRoulette[] TrackedRoulettes =
    {
        new(RouletteType.Expert, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.Level90, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.Level50607080, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.Leveling, new Setting<bool>(false), RouletteState.Incomplete), 
        new(RouletteType.Trials, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.MSQ, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.Guildhest, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.Alliance, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.Normal, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.Frontline, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.Mentor, new Setting<bool>(false), RouletteState.Incomplete),
    };
}
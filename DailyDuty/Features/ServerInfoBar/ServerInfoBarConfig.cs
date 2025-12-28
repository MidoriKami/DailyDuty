using DailyDuty.Enums;
using DailyDuty.Interfaces;

namespace DailyDuty.Features.ServerInfoBar;

public class ServerInfoBarConfig : Savable {
    public bool SoloDaily;
    public bool SoloWeekly;
    public bool Combo = true;
    public bool HideSeconds;

    public DtrMode CurrentMode = DtrMode.Daily;

    protected override string FileExtension => ".config.json";
}

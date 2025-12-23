using DailyDuty.Classes;
using DailyDuty.Enums;

namespace DailyDuty.Features.ServerInfoBar;

public class ServerInfoBarConfig : ConfigBase {
    public bool SoloDaily;
    public bool SoloWeekly;
    public bool Combo = true;
    public bool HideSeconds;

    public DtrMode CurrentMode = DtrMode.Daily;
}

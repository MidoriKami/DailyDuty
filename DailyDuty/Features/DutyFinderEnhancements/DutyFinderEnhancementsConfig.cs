using System.Numerics;
using DailyDuty.Classes;
using KamiToolKit.Classes;

namespace DailyDuty.Features.DutyFinderEnhancements;

public class DutyFinderEnhancementsConfig : ConfigBase {
    public Vector4 Color = ColorHelper.GetColor(8);
    public bool HideSeconds = false;
    public bool OpenDailyDutyButton = true;
}

using System.Numerics;
using DailyDuty.Classes;
using KamiToolKit.Classes;

namespace DailyDuty.Features.DutyFinderTimer;

public class Config : ConfigBase {
    public Vector4 Color = ColorHelper.GetColor(8);
    public bool HideSeconds = false;
}

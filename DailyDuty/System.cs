using DailyDuty.Addons;
using DailyDuty.Controllers;

namespace DailyDuty;

public static class System {
    public static SystemController Controller { get; set; } = null!;
    public static ConfigWindow ConfigWindow { get; set; } = null!;
}

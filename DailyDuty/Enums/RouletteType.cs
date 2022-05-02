using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Enums
{
    public enum RouletteType
    {
        Expert = 5,
        Level90 = 8,
        Level50607080 = 2,
        Leveling = 1,
        Trials = 6,
        MSQ = 3,
        Guildhest = 4,
        Alliance = 15,
        Normal = 17,
        Mentor = 9,
        Frontline = 7
    }

    public static class RouletteTypeExtensions
    {
        public static string LookupName(this RouletteType type)
        {
            return Service.DataManager.GetExcelSheet<ContentRoulette>()!.GetRow((uint) type)!.Category.RawString;
        }
    }
}

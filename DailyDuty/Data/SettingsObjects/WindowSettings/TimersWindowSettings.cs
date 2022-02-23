using DailyDuty.Data.SettingsObjects.Timers;

namespace DailyDuty.Data.SettingsObjects.WindowSettings
{
    public class TimersWindowSettings : GenericWindowSettings
    {
        public TimerSettings Daily = new();
        public TimerSettings Weekly = new();
        public TimerSettings FashionReport = new();
        public TimerSettings TreasureMap = new();
        public TimerSettings JumboCactpot = new();
    }
}

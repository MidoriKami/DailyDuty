using DailyDuty.Data.SettingsObjects.Timers;

namespace DailyDuty.Interfaces
{
    public interface ITimer
    {
        public string Name { get; }
        public TimerSettings Settings { get; set; }

        public void Draw()
        {
            if (Settings.Enabled)
            {
                DrawContents();
            }
        }

        protected void DrawContents();

    }
}
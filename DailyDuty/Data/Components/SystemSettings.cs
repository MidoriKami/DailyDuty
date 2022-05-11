using System;

namespace DailyDuty.Data.Components
{
    [Serializable]
    public class SystemSettings
    {
        public int MinutesBetweenThrottledMessages = 5;
        public bool MessageDelay = false;
        public DayOfWeek DelayDay = DayOfWeek.Tuesday;
        public string SelectedLanguage = string.Empty;
    }
}
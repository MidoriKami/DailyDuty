using System;

namespace DailyDuty.Data.SettingsObjects
{
    [Serializable]
    public class SystemSettings
    {
        public int MinutesBetweenThrottledMessages = 5;
        public bool ShowSaveDebugInfo = false;
        public bool ClickableLinks = false;
        public bool EnableDebugOutput = false;
        public bool ShowVersionNumber = true;
    }
}
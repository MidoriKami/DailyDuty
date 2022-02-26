using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Interfaces;
using ImGuiNET;

namespace DailyDuty.Modules.Weekly
{
    internal class ChallengeLog : 
        ICollapsibleHeader, 
        IUpdateable,
        ICompletable
    {
        public void Dispose()
        {
        }

        private ChallengeLogSettings Settings => Service.Configuration.Current().ChallengeLog;

        public CompletionType Type => CompletionType.Weekly;
        public string HeaderText => "Challenge Log";
        public GenericSettings GenericSettings => Settings;
        public bool IsCompleted()
        {
            return false;
        }

        void ICollapsibleHeader.DrawContents()
        {
            ImGui.Text("Not Implemented Yet");
        }

        public void Update()
        {
            
        }
    }
}
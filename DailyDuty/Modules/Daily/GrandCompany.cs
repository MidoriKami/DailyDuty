using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Daily;
using DailyDuty.Interfaces;
using FFXIVClientStructs.FFXIV.Client.UI;
using ImGuiNET;

namespace DailyDuty.Modules.Daily
{
    internal unsafe class GrandCompany : 
        ICollapsibleHeader, 
        IUpdateable,
        ICompletable
    {
        private GrandCompanySettings Settings => Service.Configuration.Current().GrandCompany;

        public CompletionType Type => CompletionType.Daily;
        public string HeaderText => "Grand Company";
        public GenericSettings GenericSettings => Settings;

        public GrandCompany()
        {
        }

        public bool IsCompleted()
        {
            return false;
        }

        void ICollapsibleHeader.DrawContents()
        {
            ImGui.Text("Not Implemented Yet");
        }

        public void Dispose()
        {
        }

        public void Update()
        {
            
        }
    }
}
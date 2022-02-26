using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Interfaces;
using ImGuiNET;

namespace DailyDuty.Modules.Weekly
{
    internal class DomanEnclave : 
        ICollapsibleHeader, 
        IUpdateable,
        ICompletable
    {
        public void Dispose()
        {

        }

        private DomanEnclaveSettings Settings => Service.Configuration.Current().DomanEnclave;
        public CompletionType Type => CompletionType.Weekly;
        public string HeaderText => "Doman Enclave";
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
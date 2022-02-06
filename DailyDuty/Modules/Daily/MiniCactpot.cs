using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.DailySettings;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using ImGuiNET;

namespace DailyDuty.Modules.Daily
{
    internal class MiniCactpot : 
        IConfigurable,
        IUpdateable,
        IDailyResettable,
        IZoneChangeThrottledNotification,
        ILoginNotification
    {
        private MiniCactpotSettings Settings => Service.Configuration.Current().MiniCactpot;
        public GenericSettings GenericSettings => Settings;

        public string HeaderText => "Mini Cactpot";

        public DateTime NextReset
        {
            get => Settings.NextReset;
            set => Settings.NextReset = value;
        }
        
        public void NotificationOptions()
        {
        }

        public void EditModeOptions()
        {
        }

        public void DisplayData()
        {
            ImGui.Text("Test?!");
        }

        public void Dispose()
        {

        }

        public void Update()
        {
            
        }

        void IResettable.ResetThis(CharacterSettings settings)
        {
            settings.MiniCactpot.TicketsRemaining = 3;
        }

        public void SendNotification()
        {
            if (Settings.TicketsRemaining > 0)
            {
                Chat.Print(HeaderText, $"{Settings.TicketsRemaining} Tickets Remaining");
            }
        }
    }
}

using System;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.Levequests;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Daily;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Utility.Signatures;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Condition = DailyDuty.Utilities.Condition;

namespace DailyDuty.Modules.Daily
{
    internal unsafe class Levequests : 
        IConfigurable, 
        ICompletable,
        IZoneChangeThrottledNotification,
        ILoginNotification
    {
        private LeviquestSettings Settings => Service.Configuration.Current().Leviquest;
        public CompletionType Type => CompletionType.Daily;
        public string HeaderText => "Levequests";
        public GenericSettings GenericSettings => Settings;

        // Stolen from https://github.com/Ottermandias/Accountant/blob/858f97c4a3d0e7b37d132048f3e93e5dc0405f26/Accountant.GameData/SeFunctions/StaticLeveAllowances.cs#L6
        [Signature("88 05 ?? ?? ?? ?? 0F B7 41 06", ScanType = ScanType.StaticAddress)]
        private readonly LevequestStruct* levequestStruct = null;

        public Levequests()
        {
            SignatureHelper.Initialise(this);
        }

        public void NotificationOptions()
        {
            Draw.OnLoginReminderCheckbox(Settings, HeaderText);

            Draw.OnTerritoryChangeCheckbox(Settings, HeaderText);
            
            Draw.Checkbox("Threshold Warning", HeaderText, ref Settings.AboveThresholdWarning, "Display notifications if above the threshold");

            if (Settings.AboveThresholdWarning)
            {
                ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                Draw.EditNumberField("Threshold", HeaderText, ref Settings.WarningThreshold);
                ImGuiComponents.HelpMarker("Display a warning if the current number of leve allowances is above this value");

                ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
            }
        }

        public void SendNotification()
        {
            if (Settings.AboveThresholdWarning && Condition.IsBoundByDuty() == false)
            {
                if (levequestStruct->AllowancesRemaining > Settings.WarningThreshold)
                {
                    Chat.Print(HeaderText, "Above allowance threshold");
                }
            }
        }

        public void EditModeOptions()
        {

        }

        public void DisplayData()
        {
            Draw.NumericDisplay("Allowances Remaining", levequestStruct->AllowancesRemaining);

            Draw.NumericDisplay("Leves Accepted", levequestStruct->LevesAccepted);

            Draw.TimeSpanDisplay("Time Until +3 Allowances",  Time.NextLeveAllowanceReset() - DateTime.UtcNow);

        }

        public bool IsCompleted()
        {
            if (Settings.AboveThresholdWarning)
            {
                if (levequestStruct->AllowancesRemaining > Settings.WarningThreshold)
                {
                    return false;
                }
            }

            return true;
        }

        public void Dispose()
        {

        }
    }
}
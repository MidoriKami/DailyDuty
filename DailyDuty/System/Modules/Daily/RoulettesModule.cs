using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using DailyDuty.ConfigurationSystem;
using DailyDuty.Data;
using DailyDuty.System.Utilities;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System.Modules.Daily
{
    internal unsafe class RoulettesModule : Module
    {
        private ConfigurationSystem.Daily.Roulettes Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].RouletteSettings;
        public override string ModuleName => "Roulettes";
        public override GenericSettings GenericSettings => Settings;

        public override void Update()
        {
            if (Settings.Enabled && GetContentsFinderConfirmPointer() != null)
            {
                var result = GetByStringFromUI();

                if (result is {Completed: false, Tracked: true})
                {
                    result.Completed = true;
                    Service.Configuration.Save();
                }
            }
        }

        protected override void OnLoginDelayed()
        {
            if (Settings.Enabled && Settings.LoginReminder)
            {
                PrintNotification();
            }
        }

        protected override void ThrottledOnTerritoryChanged(object? sender, ushort e)
        {
            if (ConditionManager.IsBoundByDuty() == true) return;

            if (Settings.Enabled && Settings.TerritoryChangeReminder)
            {
                PrintNotification();
            }
        }

        public override bool IsCompleted()
        {
            var allCompleted = Settings.TrackedRoulettes
                .Where(roulette => roulette.Tracked == true)
                .All(tracked => tracked.Completed);

            return allCompleted;
        }

        public override void DoDailyReset(Configuration.CharacterSettings settings)
        {
            foreach (var tracked in settings.RouletteSettings.TrackedRoulettes)
            {
                tracked.Completed = false;
            }
        }

        public override void DoWeeklyReset(Configuration.CharacterSettings settings)
        {
        }

        public TrackedRoulette GetByType(DutyRoulette type)
        {
            return Settings.TrackedRoulettes.Where(roulette => roulette.Roulette == type).First();
        }

        public string? GetStringFromUI()
        {
            var basePointer = GetContentsFinderConfirmPointer();
            if (basePointer == null) return null;

            var textNode = basePointer->GetNodeById(49);
            if (textNode == null) return null;

            return textNode->GetAsAtkTextNode()->NodeText.ToString();
        }

        public TrackedRoulette? GetByStringFromUI()
        {
            var uiString = GetStringFromUI();
            if(uiString == null) return null;

            var dutyID = Service.DataManager.GetExcelSheet<ContentRoulette>()!
                .Where(r =>(r.Name.ToString() == uiString))
                .FirstOrDefault();

            if (dutyID == null) return null;

            var element = GetByType((DutyRoulette) dutyID.RowId);
            return element;
        }

        private AtkUnitBase* GetContentsFinderConfirmPointer()
        {
            return (AtkUnitBase*) Service.GameGui.GetAddonByName("ContentsFinderConfirm", 1);
        }

        private int RemainingRoulettesCount()
        {
            return Settings.TrackedRoulettes
                .Where(r => r.Tracked == true)
                .Count(r => !r.Completed);
        }

        private void PrintNotification()
        {
            if (RemainingRoulettesCount() > 0)
            {
                Util.PrintRoulettes($"You have {RemainingRoulettesCount()} Roulettes remaining today.");
            }
        }
    }
}

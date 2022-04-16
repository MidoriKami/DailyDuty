using System;
using System.Linq;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.HuntMarks;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using DailyDuty.Utilities.Helpers.Delegates;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Utility.Signatures;
using ImGuiNET;

namespace DailyDuty.Modules.Weekly
{
    internal unsafe class WeeklyHuntMark : 
        IConfigurable, 
        IWeeklyResettable,
        ILoginNotification,
        IZoneChangeThrottledNotification,
        ICompletable,
        IUpdateable
    {

        private WeeklyHuntMarksSettings Settings => Service.Configuration.Current().WeeklyHuntMarks;
        public CompletionType Type => CompletionType.Weekly;
        public string HeaderText => "Hunt Marks (Weekly)";
        public GenericSettings GenericSettings => Settings;
        public DateTime NextReset
        {
            get => Settings.NextReset;
            set => Settings.NextReset = value;
        }

        // https://github.com/SheepGoMeh/HuntBuddy/blob/master/Structs/MobHuntStruct.cs
        [Signature("D1 48 8D 0D ?? ?? ?? ?? 48 83 C4 20 5F E9 ?? ?? ?? ??", ScanType = ScanType.StaticAddress)]
        private readonly MobHuntStruct* huntData = null;

        public WeeklyHuntMark()
        {
            SignatureHelper.Initialise(this);
        }

        public void Dispose()
        {

        }

        public bool IsCompleted() => GetIncompleteCount() == 0;

        public void SendNotification()
        {
            if (Condition.IsBoundByDuty() == true) return;

            if (GetIncompleteCount() != 0)
            {
                Chat.Print(HeaderText, $"{GetIncompleteCount()} Hunts Remaining");
            }
        }

        public void NotificationOptions()
        {
            Draw.OnLoginReminderCheckbox(Settings);

            Draw.OnTerritoryChangeCheckbox(Settings);
        }

        public void EditModeOptions()
        {
            ImGui.Text("Force Update");
            ImGui.Spacing();

            if (ImGui.BeginTable("##EditTable", 2))
            {
                foreach (var hunt in Settings.TrackedHunts)
                {
                    ImGui.PushID((int)hunt.Type);

                    var label = GetExpansionForHuntType(hunt.Type).Description();

                    ImGui.TableNextColumn();
                    ImGui.Text(label);

                    ImGui.SameLine();
                    ImGui.TableNextColumn();

                    if (ImGui.Button("Next State"))
                    {
                        hunt.State = hunt.State switch
                        {
                            TrackedHuntState.Unobtained => TrackedHuntState.Obtained,
                            TrackedHuntState.Obtained => TrackedHuntState.Killed,
                            TrackedHuntState.Killed => TrackedHuntState.Unobtained,
                            _ => hunt.State
                        };

                        Service.Configuration.Save();
                    }

                    ImGui.PopID();
                }

                ImGui.EndTable();
            }
        }

        public void DisplayData()
        {
            ImGui.Text("Selected lines will be evaluated for notifications");
            ImGui.Spacing();

            if (ImGui.BeginTable("##DataTable", 2))
            {
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 150f * ImGuiHelpers.GlobalScale);
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 200f * ImGuiHelpers.GlobalScale);

                foreach (var hunt in Settings.TrackedHunts)
                {
                    var label = GetExpansionForHuntType(hunt.Type).Description();

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    if (ImGui.Checkbox($"##{hunt.Type}", ref hunt.Tracked))
                    {
                        Service.Configuration.Save();
                    }
                    ImGui.SameLine();
                    ImGui.Text(label);

                    ImGui.TableNextColumn();
                    switch (hunt.State)
                    {
                        case TrackedHuntState.Unobtained:
                            ImGui.TextColored(Colors.Red, "Mark Available");
                            break;
                        case TrackedHuntState.Obtained:
                            ImGui.TextColored(Colors.Orange, "Mark Obtained");
                            break;
                        case TrackedHuntState.Killed:
                            ImGui.TextColored(Colors.Green, "Mark Killed");
                            break;
                    }
                }

                ImGui.EndTable();
            }
        }

        void IResettable.ResetThis()
        {
            foreach (var hunt in Settings.TrackedHunts)
            {
                hunt.State = TrackedHuntState.Unobtained;
            }
        }

        public void Update()
        {
            foreach (var hunt in Settings.TrackedHunts)
            {
                UpdateState(hunt);
            }
        }

        //
        // Implementation
        //

        private void UpdateState(TrackedHunt hunt)
        {
            var data = huntData->Get(hunt.Type);

            switch (hunt.State)
            {
                case TrackedHuntState.Unobtained when data.Obtained == true:
                    hunt.State = TrackedHuntState.Obtained;
                    Service.Configuration.Save();
                    break;

                //case TrackedHuntState.Obtained when data.Obtained == false && data.KillCounts.First != 1:
                //    hunt.State = TrackedHuntState.Unobtained;
                //    Service.Configuration.Save();
                //    break;

                case TrackedHuntState.Obtained when data.KillCounts.First == 1:
                    hunt.State = TrackedHuntState.Killed;
                    Service.Configuration.Save();
                    break;
            }
        }

        private ExpansionType GetExpansionForHuntType(HuntMarkType type)
        {
            return type switch
            {
                HuntMarkType.RealmReborn_Elite => ExpansionType.RealmReborn,
                HuntMarkType.Heavensward_Elite => ExpansionType.Heavensward,
                HuntMarkType.Stormblood_Elite => ExpansionType.Stormblood,
                HuntMarkType.Shadowbringers_Elite => ExpansionType.Shadowbringers,
                HuntMarkType.Endwalker_Elite => ExpansionType.Endwalker,
                _ => new()
            };
        }

        private int GetIncompleteCount()
        {
            return Settings.TrackedHunts.Count(hunt => hunt.Tracked && hunt.State != TrackedHuntState.Killed);
        }
    }
}
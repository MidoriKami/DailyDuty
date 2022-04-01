using System;
using System.Linq;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.HuntMarks;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;
using Dalamud.Utility.Signatures;
using ImGuiNET;

namespace DailyDuty.Modules.Weekly
{
    internal unsafe class HuntMarks : 
        IConfigurable, 
        IUpdateable,
        IWeeklyResettable,
        ILoginNotification,
        IZoneChangeThrottledNotification,
        ICompletable
    {

        private HuntMarksSettings Settings => Service.Configuration.Current().HuntMarks;
        public CompletionType Type => CompletionType.Weekly;
        public string HeaderText => "Hunt Marks";
        public GenericSettings GenericSettings => Settings;
        public DateTime NextReset
        {
            get => Settings.NextReset;
            set => Settings.NextReset = value;
        }

        // https://github.com/SheepGoMeh/HuntBuddy/blob/master/Structs/MobHuntStruct.cs
        [Signature("D1 48 8D 0D ?? ?? ?? ?? 48 83 C4 20 5F E9 ?? ?? ?? ??", ScanType = ScanType.StaticAddress)]
        private readonly MobHuntStruct* huntData = null;

        public HuntMarks()
        {
            SignatureHelper.Initialise(this);
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
                    var label = hunt.Expansion.Description();

                    ImGui.TableNextColumn();
                    ImGui.Text(label);

                    ImGui.SameLine();
                    ImGui.TableNextColumn();

                    if (ImGui.Button("Force Update"))
                    {
                        hunt.State = hunt.State switch
                        {
                            TrackedHuntState.Unobtained => TrackedHuntState.Obtained,
                            _ => hunt.State
                        };
                    }
                }

                ImGui.EndTable();
            }
        }
    
        public void DisplayData()
        {
            ImGui.Text("Only the checked lines will be evaluated for notifications");
            ImGui.Spacing();

            if (ImGui.BeginTable("##DataTable", 2))
            {
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 150f * ImGuiHelpers.GlobalScale);
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 200f * ImGuiHelpers.GlobalScale);

                foreach (var hunt in Settings.TrackedHunts)
                {
                    var label = hunt.Expansion.Description();

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    if (ImGui.Checkbox($"##{hunt.Expansion}", ref hunt.Tracked))
                    {
                        Service.Configuration.Save();
                    }
                    ImGui.SameLine();
                    ImGui.Text(label);

                    ImGui.TableNextColumn();
                    switch (hunt.State)
                    {
                        case TrackedHuntState.Unobtained:
                            ImGui.TextColored(Colors.Red, "Hunt Mark Available");
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
    
        public void Dispose()
        {

        }

        public void Update()
        {
            foreach (var hunt in Settings.TrackedHunts)
            {
                UpdateState(hunt);
            }
        }

        void IResettable.ResetThis()
        {
            foreach (var hunt in Settings.TrackedHunts)
            {
                hunt.State = TrackedHuntState.Unobtained;
            }
        }

        //
        // Implementation
        //

        private void UpdateState(TrackedHunt hunt)
        {
            var obtained = GetObtained(hunt.Expansion);
            var eliteKillCount = GetEliteKillCount(hunt.Expansion);

            switch (hunt.State)
            {
                case TrackedHuntState.Unobtained when obtained == true:
                    hunt.State = TrackedHuntState.Obtained;
                    Service.Configuration.Save();
                    break;

                case TrackedHuntState.Obtained when obtained == false && eliteKillCount != 1:
                    hunt.State = TrackedHuntState.Unobtained;
                    Service.Configuration.Save();
                    break;

                case TrackedHuntState.Obtained when eliteKillCount == 1:
                    hunt.State = TrackedHuntState.Killed;
                    Service.Configuration.Save();
                    break;
            }
        }

        private bool GetObtained(ExpansionType expansion)
        {
            return expansion switch
            {
                ExpansionType.RealmReborn => huntData->RealmReborn.ObtainedEliteHuntBill,
                ExpansionType.Heavensward => huntData->HeavensWard.ObtainedEliteHuntBill,
                ExpansionType.Stormblood => huntData->StormBlood.ObtainedEliteHuntBill,
                ExpansionType.Shadowbringers => huntData->ShadowBringers.ObtainedEliteHuntBill,
                ExpansionType.Endwalker => huntData->Endwalker.ObtainedEliteHuntBill,
                _ => false
            };
        }

        private int GetEliteKillCount(ExpansionType expansion)
        {
            return expansion switch
            {
                ExpansionType.RealmReborn => huntData->RealmReborn.EliteMark,
                ExpansionType.Heavensward => huntData->HeavensWard.EliteMark,
                ExpansionType.Stormblood => huntData->StormBlood.EliteMark,
                ExpansionType.Shadowbringers => huntData->ShadowBringers.EliteMark,
                ExpansionType.Endwalker => huntData->Endwalker.EliteMark,
                _ => 0
            };
        }

        private int GetIncompleteCount()
        {
            return Settings.TrackedHunts.Count(hunt => hunt.Tracked && hunt.State != TrackedHuntState.Killed);
        }
    }
}
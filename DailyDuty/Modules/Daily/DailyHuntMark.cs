using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.HuntMarks;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Daily;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using DailyDuty.Utilities.Helpers.Delegates;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Utility.Signatures;
using ImGuiNET;

namespace DailyDuty.Modules.Daily
{
    internal unsafe class DailyHuntMark :
        IConfigurable, 
        IDailyResettable,
        ILoginNotification,
        IZoneChangeThrottledNotification,
        ICompletable
    {
        private DailyHuntMarksSettings Settings => Service.Configuration.Current().DailyHuntMarks;
        public CompletionType Type => CompletionType.Daily;
        public string HeaderText => "Hunt Marks (Daily)";
        public GenericSettings GenericSettings => Settings;
        public DateTime NextReset
        {
            get => Settings.NextReset;
            set => Settings.NextReset = value;
        }

        [Signature("80 FA 12 0F 83 ?? ?? ?? ?? 55 56", DetourName = nameof(MobHunt_MarkObtained))]
        private readonly Hook<Functions.Other.MobHunt.MarkObtained>? markObtainedHook = null;

        [Signature("80 FA 12 0F 83 ?? ?? ?? ?? 48 89 6C 24", DetourName = nameof(MobHunt_OnHuntKill))]
        private readonly Hook<Functions.Other.MobHunt.MobKill>? huntKillHook = null;

        [Signature("48 83 EC 28 80 FA 12 73 7E", DetourName = nameof(MobHunt_MarkComplete))]
        private readonly Hook<Functions.Other.MobHunt.MarkComplete>? markCompleteHook = null;

        // https://github.com/SheepGoMeh/HuntBuddy/blob/master/Structs/MobHuntStruct.cs
        [Signature("D1 48 8D 0D ?? ?? ?? ?? 48 83 C4 20 5F E9 ?? ?? ?? ??", ScanType = ScanType.StaticAddress)]
        private readonly MobHuntStruct* huntData = null;

        public DailyHuntMark()
        {
            SignatureHelper.Initialise(this);

            huntKillHook?.Enable();
            markObtainedHook?.Enable();
            markCompleteHook?.Enable();
        }

        public void Dispose()
        {
            huntKillHook?.Dispose();
            markObtainedHook?.Dispose();
            markCompleteHook?.Dispose();
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
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 175f * ImGuiHelpers.GlobalScale);
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 85f * ImGuiHelpers.GlobalScale);

                foreach (var hunt in Settings.TrackedHunts)
                {
                    ImGui.PushID((int)hunt.Type);

                    var label = hunt.Type.Description();

                    ImGui.TableNextRow();
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

            foreach (var expansion in Enum.GetValues<ExpansionType>())
            {
                ImGui.PushID((int)expansion);

                if (ImGui.CollapsingHeader(expansion.Description()))
                {
                    ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                    if (ImGui.BeginTable("##DataTable", 2))
                    {
                        ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 100f * ImGuiHelpers.GlobalScale);
                        ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 100f * ImGuiHelpers.GlobalScale);

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        if (ImGui.Button("Track All", ImGuiHelpers.ScaledVector2(100f, 23f)))
                        {
                            SetAllByExpansion(expansion, true);
                            Service.Configuration.Save();
                        }

                        ImGui.TableNextColumn();
                        if (ImGui.Button("Untrack All", ImGuiHelpers.ScaledVector2(100f, 23f)))
                        {
                            SetAllByExpansion(expansion, false);
                            Service.Configuration.Save();
                        }

                        foreach (var hunt in Settings.TrackedHunts)
                        {
                            if (GetExpansionForHuntType(hunt.Type) == expansion)
                            {
                                var label = GetLabelForHuntType(hunt.Type);

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
                        }
                        ImGui.EndTable();
                    }
                    ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
                }
                ImGui.PopID();
            }
        }

        private void MobHunt_MarkObtained(void* a1, byte a2, int a3)
        {
            Chat.Debug("DailyHuntMark::MarkObtained::Updating");

            Update();

            markObtainedHook!.Original(a1, a2, a3);
        }

        private void MobHunt_OnHuntKill(void* a1, byte a2, uint a3, uint a4)
        {
            Chat.Debug("DailyHuntMark::HuntMobKilled::Updating");

            Update();

            huntKillHook!.Original(a1, a2, a3, a4);
        }

        private void MobHunt_MarkComplete(void* a1, byte a2)
        {
            Chat.Debug("DailyHuntMark::MarkComplete::Updating");

            Update();

            markCompleteHook!.Original(a1, a2);
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

        private void Update()
        {
            foreach (var hunt in Settings.TrackedHunts)
            {
                UpdateState(hunt);
            }
        }

        private void UpdateState(TrackedHunt hunt)
        {
            var data = huntData->Get(hunt.Type);

            switch (hunt.State)
            {
                case TrackedHuntState.Unobtained when data.Obtained == true:
                    hunt.State = TrackedHuntState.Obtained;
                    Service.Configuration.Save();
                    break;

                case TrackedHuntState.Obtained when data.Obtained == false && data.KillCounts.First != 1:
                    hunt.State = TrackedHuntState.Unobtained;
                    Service.Configuration.Save();
                    break;

                case TrackedHuntState.Obtained when data.KillCounts.First == 1:
                    hunt.State = TrackedHuntState.Killed;
                    Service.Configuration.Save();
                    break;
            }
        }

        private ExpansionType GetExpansionForHuntType(HuntMarkType type)
        {
            switch (type)
            {
                case HuntMarkType.RealmReborn_LevelOne:
                    return ExpansionType.RealmReborn;

                case HuntMarkType.Heavensward_LevelOne:
                case HuntMarkType.Heavensward_LevelTwo:
                case HuntMarkType.Heavensward_LevelThree:
                    return ExpansionType.Heavensward;

                case HuntMarkType.Stormblood_LevelOne:
                case HuntMarkType.Stormblood_LevelTwo:
                case HuntMarkType.Stormblood_LevelThree:
                    return ExpansionType.Stormblood;

                case HuntMarkType.Shadowbringers_LevelOne:
                case HuntMarkType.Shadowbringers_LevelTwo:
                case HuntMarkType.Shadowbringers_LevelThree:
                    return ExpansionType.Shadowbringers;

                case HuntMarkType.Endwalker_LevelOne:
                case HuntMarkType.Endwalker_LevelTwo:
                case HuntMarkType.Endwalker_LevelThree:
                    return ExpansionType.Endwalker;

                case HuntMarkType.RealmReborn_Elite:
                case HuntMarkType.Heavensward_Elite:
                case HuntMarkType.Stormblood_Elite:
                case HuntMarkType.Shadowbringers_Elite:
                case HuntMarkType.Endwalker_Elite:
                default:
                    return new();
            }
        }

        private string GetLabelForHuntType(HuntMarkType type)
        {
            return type switch
            {
                HuntMarkType.RealmReborn_LevelOne => "Level 1",
                HuntMarkType.Heavensward_LevelOne => "Level 1",
                HuntMarkType.Heavensward_LevelTwo => "Level 2",
                HuntMarkType.Heavensward_LevelThree => "Level 3",
                HuntMarkType.Stormblood_LevelOne => "Level 1",
                HuntMarkType.Stormblood_LevelTwo => "Level 2",
                HuntMarkType.Stormblood_LevelThree => "Level 3",
                HuntMarkType.Shadowbringers_LevelOne => "Level 1",
                HuntMarkType.Shadowbringers_LevelTwo => "Level 2",
                HuntMarkType.Shadowbringers_LevelThree => "Level 3",
                HuntMarkType.Endwalker_LevelOne => "Level 1",
                HuntMarkType.Endwalker_LevelTwo => "Level 2",
                HuntMarkType.Endwalker_LevelThree => "Level 3",
                _ => "Unknown HuntType"
            };
        }

        private void SetAllByExpansion(ExpansionType expansion, bool tracked)
        {
            foreach (var hunt in Settings.TrackedHunts)
            {
                if (GetExpansionForHuntType(hunt.Type) == expansion)
                {
                    hunt.Tracked = tracked;
                }
            }
        }

        private int GetIncompleteCount()
        {
            return Settings.TrackedHunts.Count(hunt => hunt.Tracked && hunt.State != TrackedHuntState.Killed);
        }
    }
}

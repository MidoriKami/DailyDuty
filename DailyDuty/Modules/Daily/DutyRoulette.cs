using System;
using System.Linq;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.DutyRoulette;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Daily;
using DailyDuty.Data.SettingsObjects.Windows.SubComponents;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using Condition = DailyDuty.Utilities.Condition;

namespace DailyDuty.Modules.Daily
{
    internal unsafe class DutyRoulette : 
        IConfigurable,
        IZoneChangeLogic,
        ILoginNotification,
        IZoneChangeThrottledNotification,
        ICompletable,
        IDailyResettable
    {
        private DutyRouletteSettings Settings => Service.Configuration.Current().DutyRoulette;
        public CompletionType Type => CompletionType.Daily;
        public string HeaderText => "Duty Roulette";
        public GenericSettings GenericSettings => Settings;

        [Signature("E9 ?? ?? ?? ?? 8B 93 ?? ?? ?? ?? 48 83 C4 20")]
        private readonly delegate* unmanaged<AgentInterface*, byte, byte, IntPtr> openRouletteDuty = null!;

        private readonly DalamudLinkPayload openDutyFinder;

        public DateTime NextReset
        {
            get => Settings.NextReset;
            set => Settings.NextReset = value;
        }

        private readonly byte* selectedRoulette;

        public DutyRoulette()
        {
            SignatureHelper.Initialise(this);

            // UI State Pointer
            var clientStruct = FFXIVClientStructs.FFXIV.Client.Game.UI.UIState.Instance();

            // Offset to Client::Game::UI::InstanceContent (maybe?)
            //var offset = (byte*)clientStruct + 0x118A8;

            // +330 Bytes from UIState
            selectedRoulette = (byte*)clientStruct + 0x119F2;

            openDutyFinder = Service.PluginInterface.AddChatLinkHandler((uint) FunctionalPayloads.OpenRouletteDutyFinder, OpenRouletteDutyFinder);
        }

        public void Dispose()
        {
            Service.PluginInterface.RemoveChatLinkHandler((uint) FunctionalPayloads.OpenRouletteDutyFinder);
        }

        private void OpenRouletteDutyFinder(uint arg1, SeString arg2)
        {
            // Will cause crash, no touchy
            //ClearSelectedDuties();
        
            var agent = GetAgentContentsFinder();
            openRouletteDuty(agent, GetFirstMissingRoulette(), 0);
        }

        private void ClearSelectedDuties()
        {
            var addonPointer = Service.GameGui.GetAddonByName("ContentsFinder", 1);
            if (addonPointer != IntPtr.Zero)
            {
                try
                {
                    Chat.Print("Debug", $"AddonPointer:{addonPointer:x8}");

                    var vf5 = ((void**) addonPointer)[5];

                    Chat.Print("Debug", $"vf5:{(IntPtr)vf5:x8}");

                    var clearSelection = (delegate* unmanaged<IntPtr, byte, uint, long>) vf5;
                    Chat.Print("Debug", $"AddonPointer:{(IntPtr)clearSelection:x8}");

                    Chat.Print("Debug", "Calling fp!");
                    clearSelection(addonPointer, 0, 15);
                }
                catch (Exception e)
                {
                    PluginLog.Information(e.Message);
                }
            }
        }

        private AgentInterface* GetAgentContentsFinder()
        {
            var framework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
            var uiModule = framework->GetUiModule();
            var agentModule = uiModule->GetAgentModule();
            return agentModule->GetAgentByInternalId(AgentId.ContentsFinder);
        }

        public bool IsCompleted()
        {
            return RemainingRoulettesCount() == 0;
        }

        public void DrawTask(TaskColors colors, bool showCompletedTasks)
        {
            if (GenericSettings.Enabled == false)
            {
                return;
            }

            if(Settings.SingleTask)
            {
                if (IsCompleted() == false)
                {
                    ImGui.TextColored(colors.IncompleteColor, HeaderText);
                }
                else if (IsCompleted() == true && showCompletedTasks)
                {
                    ImGui.TextColored(colors.CompleteColor, HeaderText);
                }
            }
            else
            {
                foreach (var tracked in Settings.TrackedRoulettes)
                {
                    if (tracked.Completed == false && tracked.Tracked == true)
                    {
                        ImGui.TextColored(colors.IncompleteColor, $"{tracked.Type} Roulette");
                    }
                    else if (tracked.Completed == true && tracked.Tracked == true && showCompletedTasks)
                    {
                        ImGui.TextColored(colors.CompleteColor, $"{tracked.Type} Roulette");
                    }
                }
            }
        }

        public void SendNotification()
        {
            if (RemainingRoulettesCount() > 0 && Condition.IsBoundByDuty() == false)
            {
                Chat.Print(HeaderText, $"{RemainingRoulettesCount()} Roulettes Remaining", openDutyFinder);
            }
        }

        public void NotificationOptions()
        {
            ImGui.Checkbox("Single Todo Task", ref Settings.SingleTask);
            ImGuiComponents.HelpMarker("Display this module's status as a single task in the todo window instead of each roulette separate");

            Draw.OnLoginReminderCheckbox(Settings);

            Draw.OnTerritoryChangeCheckbox(Settings);
        }

        public void EditModeOptions()
        {
            EditGrid();
        }
    
        public void DisplayData()
        {
            ImGui.Text("Only detects that you started a duty roulette\n" +
                       "Does not detect successful completion");

            ImGui.Spacing();

            ImGui.TextColored(Colors.SoftRed, "Does not detect completion if started before reset\n" +
                                              "and subsequently completed after reset");

            DisplayTrackingGrid();

            ImGui.Spacing();

            DisplayRouletteStatus();
        }
    
        public void HandleZoneChange(object? sender, ushort e)
        {
            if (*selectedRoulette != 0 && Condition.IsBoundByDuty() == true)
            {
                var duty = Settings.TrackedRoulettes
                    .Where(t => (int) t.Type == *selectedRoulette)
                    .FirstOrDefault();

                if (duty != null)
                {
                    duty.Completed = true;
                    Service.Configuration.Save();
                }
            }
        }

        void IResettable.ResetThis()
        {
            foreach (var task in Settings.TrackedRoulettes)
            {
                task.Completed = false;
            }
        }

        //
        //  Implementation
        //
        private int RemainingRoulettesCount()
        {
            return Settings.TrackedRoulettes
                .Where(r => r.Tracked == true)
                .Count(r => !r.Completed);
        }

        private void DisplayRouletteStatus()
        {
            ImGui.Text("Tracked Roulette Status");

            if (ImGui.BeginTable($"##{HeaderText}Status", 2))
            {
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 125f * ImGuiHelpers.GlobalScale);
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 100f * ImGuiHelpers.GlobalScale);
            
                foreach (var tracked in Settings.TrackedRoulettes)
                {
                    if (tracked.Tracked == true)
                    {
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.Text(tracked.Type.ToString());

                        ImGui.TableNextColumn();
                        Draw.CompleteIncomplete(tracked.Completed);
                    }
                }

                ImGui.EndTable();
            }
        }

        private void DisplayTrackingGrid()
        {
            ImGui.Text("Select the Roulettes to track");

            var contentWidth = ImGui.GetContentRegionAvail().X;

            if (ImGui.BeginTable($"##{HeaderText}Table", (int)(contentWidth / 125.0f)))
            {

                foreach (var roulette in Settings.TrackedRoulettes)
                {

                    ImGui.TableNextColumn();
                    ImGui.Checkbox($"{roulette.Type}##{HeaderText}", ref roulette.Tracked);

                    if (roulette.Type == RouletteType.Mentor)
                    {
                        ImGui.SameLine();
                        ImGuiComponents.HelpMarker("You know it's going to be an extreme... right?");
                    }
                }

                ImGui.EndTable();
            }
        }
        private void EditGrid()
        {
            ImGui.Text("Set Roulette Status");

            if (ImGui.BeginTable($"##{HeaderText}EditTable", 2))
            {
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 125f * ImGuiHelpers.GlobalScale);
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 100f * ImGuiHelpers.GlobalScale);

                foreach (var roulette in Settings.TrackedRoulettes)
                {
                    if (roulette.Tracked)
                    {
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.Text(roulette.Type.ToString());

                        ImGui.TableNextColumn();

                        if (roulette.Completed == false)
                        {
                            if (ImGui.Button($"Complete##{roulette.Type}{HeaderText}"))
                            {
                                roulette.Completed = true;
                                Service.Configuration.Save();
                            }
                        }
                        else
                        {
                            if (ImGui.Button($"Incomplete##{roulette.Type}{HeaderText}"))
                            {
                                roulette.Completed = false;
                                Service.Configuration.Save();
                            }
                        }
                    }
                }

                ImGui.EndTable();
            }
        }

        private byte GetFirstMissingRoulette()
        {
            foreach (var trackedRoulette in Settings.TrackedRoulettes)
            {
                if (trackedRoulette is {Completed: false, Tracked: true})
                {
                    return (byte) trackedRoulette.Type;
                }
            }

            return (byte)RouletteType.Leveling;
        }
    }
}
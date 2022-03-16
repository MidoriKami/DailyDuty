using System;
using System.Linq;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.DutyRoulette;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Daily;
using DailyDuty.Data.SettingsObjects.Windows.SubComponents;
using DailyDuty.Data.Structs;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Logging;
using Dalamud.Utility;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Condition = DailyDuty.Utilities.Condition;

namespace DailyDuty.Modules.Daily
{
    internal unsafe class DutyRoulette : 
        IConfigurable,
        ILoginNotification,
        IZoneChangeThrottledNotification,
        ICompletable,
        IDailyResettable,
        IUpdateable
    {
        private DutyRouletteSettings Settings => Service.Configuration.Current().DutyRoulette;
        public CompletionType Type => CompletionType.Daily;
        public string HeaderText => "Duty Roulette";
        public GenericSettings GenericSettings => Settings;

        [Signature("E9 ?? ?? ?? ?? 8B 93 ?? ?? ?? ?? 48 83 C4 20")]
        private readonly delegate* unmanaged<AgentInterface*, byte, byte, IntPtr> openRouletteDuty = null!;

        [Signature("48 83 EC 28 84 D2 75 07 32 C0", ScanType = ScanType.Text)]
        private readonly delegate* unmanaged<IntPtr, byte, byte> rouletteIncomplete = null;

        [Signature("48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 84 C0 74 0C 48 8D 4C 24", ScanType = ScanType.StaticAddress)]
        private readonly IntPtr rouletteBasePointer = IntPtr.Zero;

        private readonly DalamudLinkPayload openDutyFinder;

        public DateTime NextReset
        {
            get => Settings.NextReset;
            set => Settings.NextReset = value;
        }

        public DutyRoulette()
        {
            SignatureHelper.Initialise(this);

            openDutyFinder = Service.PluginInterface.AddChatLinkHandler((uint) FunctionalPayloads.OpenRouletteDutyFinder, OpenRouletteDutyFinder);
        }
        
        public void Dispose()
        {
            Service.PluginInterface.RemoveChatLinkHandler((uint) FunctionalPayloads.OpenRouletteDutyFinder);
        }

        private void OpenRouletteDutyFinder(uint arg1, SeString arg2)
        {
            var agent = GetAgentContentsFinder();
            openRouletteDuty(agent, GetFirstMissingRoulette(), 0);
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
            Draw.Checkbox("Single Todo Task", ref Settings.SingleTask, "Display this module's status as a single task in the todo window instead of each roulette separate");

            Draw.OnLoginReminderCheckbox(Settings);

            Draw.OnTerritoryChangeCheckbox(Settings);
        }

        public void EditModeOptions()
        {

        }
    
        public void DisplayData()
        {
            DisplayTrackingGrid();

            ImGui.Spacing();

            DisplayRouletteStatus();
        }
        
        public void Update()
        {
            var dataChanged = false;

            foreach (var trackedRoulette in Settings.TrackedRoulettes)
            {
                var rouletteStatus = rouletteIncomplete(rouletteBasePointer, (byte) trackedRoulette.Type) == 0;

                if (trackedRoulette.Completed != rouletteStatus)
                {
                    trackedRoulette.Completed = rouletteStatus;
                    dataChanged = true;
                }
            }

            if (dataChanged == true)
            {
                Service.Configuration.Save();
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
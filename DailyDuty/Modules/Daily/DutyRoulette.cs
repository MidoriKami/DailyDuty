using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.DutyRoulette;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.DailySettings;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Condition = DailyDuty.Utilities.Condition;

namespace DailyDuty.Modules.Daily;

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

    public void SendNotification()
    {
        if (RemainingRoulettesCount() > 0 && Condition.IsBoundByDuty() == false)
        {
            Chat.Print(HeaderText, $"{RemainingRoulettesCount()} Roulettes Remaining", openDutyFinder);
        }
    }

    public void NotificationOptions()
    {
        Draw.OnLoginReminderCheckbox(Settings, HeaderText);

        Draw.OnTerritoryChangeCheckbox(Settings, HeaderText);
    }

    public void EditModeOptions()
    {
        EditGrid();
    }
    
    public void DisplayData()
    {
        ImGui.Text("Only detects that you started a duty roulette\n" +
                   "Does not detect successful completion");

        DisplayTrackingGrid();

        ImGui.Spacing();

        DisplayRouletteStatus();
    }
    
    public void HandleZoneChange(object? sender, ushort e)
    {
        if (*selectedRoulette != 0)
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

    void IResettable.ResetThis(CharacterSettings settings)
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
                    Draw.PrintCompleteIncomplete(tracked.Completed);
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
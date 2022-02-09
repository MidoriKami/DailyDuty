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
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

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

    public DateTime NextReset
    {
        get => Settings.NextReset;
        set => Settings.NextReset = value;
    }

    private readonly byte* selectedRoulette;

    public DutyRoulette()
    {
        // UI State Pointer
        var clientStruct = FFXIVClientStructs.FFXIV.Client.Game.UI.UIState.Instance();

        // Offset to Client::Game::UI::InstanceContent (maybe?)
        //var offset = (byte*)clientStruct + 0x118A8;

        // +330 Bytes from UIState
        selectedRoulette = (byte*)clientStruct + 0x119F2;
    }

    public bool IsCompleted()
    {
        return RemainingRoulettesCount() == 0;
    }

    public void SendNotification()
    {
        if (RemainingRoulettesCount() > 0)
        {
            Chat.Print(HeaderText, $"{RemainingRoulettesCount()} Roulettes remaining");
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

    public void Dispose()
    {
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
}
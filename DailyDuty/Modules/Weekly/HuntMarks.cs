using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.HuntMarks;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.WeeklySettings;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;
using Dalamud.Utility.Signatures;
using ImGuiNET;
#pragma warning disable CS0649

namespace DailyDuty.Modules.Weekly;

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
    private EliteHuntStruct* huntData;

    public HuntMarks()
    {
        SignatureHelper.Initialise(this);
    }

    public bool IsCompleted()
    {
        return CountUnclaimed() == 0;
    }

    public void SendNotification()
    {
        if (Condition.IsBoundByDuty() == true) return;

        if (Settings.Enabled)
        {
            DisplayNotification();
        }
    }
    
    public void NotificationOptions()
    {
        Draw.OnLoginReminderCheckbox(Settings, HeaderText);

        Draw.OnTerritoryChangeCheckbox(Settings, HeaderText);
    }

    public void EditModeOptions()
    {
        EditHuntStatus();
    }
    
    public void DisplayData()
    {
        DisplayHuntStatus();
    }
    
    public void Dispose()
    {

    }

    public void Update()
    {
        foreach (var hunt in Settings.TrackedHunts)
        {
            var obtained = huntData->Obtained(hunt.Expansion);

            if (obtained == true && hunt.Obtained == false)
            {
                hunt.Obtained = true;
                Service.Configuration.Save();
            }
        } 
    }

    void IResettable.ResetThis(CharacterSettings settings)
    {
        foreach (var hunt in settings.HuntMarks.TrackedHunts)
        {
            hunt.Obtained = false;
        }
    }

    private int CountUnclaimed()
    {
        var count = 0;

        foreach (var hunt in Settings.TrackedHunts)
        {
            var obtained = huntData->Obtained(hunt.Expansion) || hunt.Obtained;

            if (!obtained && hunt.Tracked)
                count++;
        }

        return count;
    }

    //
    // Implementation
    //
    private void DisplayNotification()
    {
        var unclaimed = CountUnclaimed();

        if (unclaimed > 0)
        {
            Chat.Print(HeaderText, $"{unclaimed} Elite Marks Unclaimed");
        }
    }

    private void DisplayHuntStatus()
    {
        ImGui.Text("Only the checked lines will be evaluated for notifications");
        ImGui.Spacing();

        if (ImGui.BeginTable($"##EditTable{HeaderText}", 2))
        {
            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 150f * ImGuiHelpers.GlobalScale);
            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 200f * ImGuiHelpers.GlobalScale);

            foreach (var hunt in Settings.TrackedHunts)
            {
                var label = hunt.Expansion.ToString();

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                if (ImGui.Checkbox($"##{hunt.Expansion}{HeaderText}", ref hunt.Tracked))
                {
                    Service.Configuration.Save();
                }
                ImGui.SameLine();
                ImGui.Text(label);

                ImGui.TableNextColumn();
                Draw.DrawConditionalText(huntData->Obtained(hunt.Expansion) || hunt.Obtained, "Mark Obtained", "Hunt Mark Available");
            }

            ImGui.EndTable();
        }
    }

    private void EditHuntStatus()
    {
        if (ImGui.BeginTable($"##EditTable{HeaderText}", 3))
        {
            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 100f * ImGuiHelpers.GlobalScale);
            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 120f * ImGuiHelpers.GlobalScale);
            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 120f * ImGuiHelpers.GlobalScale);

            foreach (var hunt in Settings.TrackedHunts)
            {
                var label = hunt.Expansion.ToString();

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text(label);

                ImGui.TableNextColumn();
                if (ImGui.Button($"Collected##{label}{HeaderText}", ImGuiHelpers.ScaledVector2(100, 25)))
                {
                    hunt.Obtained = true;
                    Service.Configuration.Save();
                }

                ImGui.TableNextColumn();
                if (ImGui.Button($"Not Collected##{label}{HeaderText}", ImGuiHelpers.ScaledVector2(100, 25)))
                {
                    hunt.Obtained = false;
                    Service.Configuration.Save();
                }

            }

            ImGui.EndTable();
        }
    }
}
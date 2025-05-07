﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Models;
using DailyDuty.Localization;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.UI;
using ImGuiNET;
using KamiLib.Classes;
using KamiLib.Window.SelectionWindows;
using Lumina.Excel.Sheets;
using ClientStructs = FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace DailyDuty.Modules;

public class ChallengeLogConfig : ModuleTaskConfig<ContentsNote> {

    // ContentNote rowId's to warn the user about when they open the Duty Finder
    public bool EnableContentFinderWarning = true;
    public bool EnableWarningSound = true;
    
    public HashSet<uint> WarningEntries = [];
    
    protected override bool DrawModuleConfig() {
        var configChanged = false;

        using var tabBar = ImRaii.TabBar("##SubOptionsTabBar");
        if (tabBar) {
            using (var entryTracking = ImRaii.TabItem("Entry Tracking##entryTracking")) {
                if (entryTracking) {
                    configChanged |= DrawEntryTrackingOptions();
                }
            }
                
            using (var dutyFinderWarningTab = ImRaii.TabItem("Duty Finder Warning##dutyFinderFeature")) {
                if (dutyFinderWarningTab) {
                    configChanged = DrawDutyFinderWarnings();
                        

                }
            }
        }

        return configChanged;
    }

    private bool DrawEntryTrackingOptions() {
        var configChanged = false;
        
        if (ImGui.Button("Add Tasks", new Vector2(ImGui.GetContentRegionAvail().X, 24.0f * ImGuiHelpers.GlobalScale))) {
            System.WindowManager.AddWindow(new ContentsNoteSelectionWindow(Service.PluginInterface) {
                MultiSelectionCallback = selections => {
                    foreach (var selection in selections) {
                        var matchingTask = TaskConfig.ConfigList.FirstOrDefault(tasks => tasks.RowId == selection.RowId);
                        if (matchingTask is not null) {
                            matchingTask.Enabled = true;
                        }
                    }

                    configChanged = true;
                },
            });
        }
        
        ImGui.Spacing();

        var enabledTasks = TaskConfig.ConfigList.Where(entry => entry.Enabled).ToList();

        foreach (var task in enabledTasks) {
            if (ImGuiTweaks.IconButtonWithSize(Service.PluginInterface.UiBuilder.IconFontFixedWidthHandle, FontAwesomeIcon.Trash, $"remove_task##{task.RowId}", ImGuiHelpers.ScaledVector2(24.0f))) {
                task.Enabled = false;
                configChanged = true;
            }
            
            ImGui.SameLine();
            
            ImGui.Text(task.Label());
        }

        if (enabledTasks.Count == 0) {
            ImGuiTweaks.CenteredWarning("No tasks are currently tracked");
        }

        ImGui.Spacing();
        
        ImGuiTweaks.DisabledButton("Clear All", () => {
            TaskConfig.ConfigList.ForEach(task => task.Enabled = false);
        });
        
        return configChanged;
    }

    private bool DrawDutyFinderWarnings() {
        var configChanged = false;
        
        ImGui.Checkbox("Enable Duty Finder Warnings", ref EnableContentFinderWarning);
        if (!EnableContentFinderWarning) return false;

        ImGui.TextWrapped("Post a warning to chat upon opening duty finder when any of the following challenges are incomplete");

        ImGui.Spacing();
        
        if (ImGui.Button("Add Warning", new Vector2(ImGui.GetContentRegionAvail().X, 24.0f * ImGuiHelpers.GlobalScale))) {
            System.WindowManager.AddWindow(new ContentsNoteSelectionWindow(Service.PluginInterface) {
                MultiSelectionCallback = selections => {
                    foreach (var selection in selections) {
                        WarningEntries.Add(selection.RowId);
                    }

                    configChanged = true;
                },
            });
        }
        
        var enabledWarnings = WarningEntries.Select(warning => Service.DataManager.GetExcelSheet<ContentsNote>().GetRow(warning)).ToList();
        
        foreach (var task in enabledWarnings) {
            if (ImGuiTweaks.IconButtonWithSize(Service.PluginInterface.UiBuilder.IconFontFixedWidthHandle, FontAwesomeIcon.Trash, $"remove_task##{task.RowId}", ImGuiHelpers.ScaledVector2(24.0f))) {
                WarningEntries.Remove(task.RowId);
                configChanged = true;
            }
            
            ImGui.SameLine();
            
            ImGui.Text(task.Name.ExtractText());
        }

        if (enabledWarnings.Count == 0) {
            ImGuiTweaks.CenteredWarning("No warnings are currently active");
        }
        
        ImGui.Spacing();

        ImGuiTweaks.DisabledButton("Clear All", WarningEntries.Clear);
        
        return configChanged;
    }
}

public unsafe class ChallengeLog : Modules.WeeklyTask<ModuleTaskData<ContentsNote>, ChallengeLogConfig, ContentsNote> {
    public override ModuleName ModuleName => ModuleName.ChallengeLog;
    
    public override bool HasClickableLink => true;
    
    public override PayloadId ClickableLinkPayloadId => PayloadId.OpenChallengeLog;
    
    private readonly Stopwatch warningThrottleTimer = new();

    protected override void UpdateTaskLists() {
        var luminaUpdater = new LuminaTaskUpdater<ContentsNote>(this, row => row.RequiredAmount is not 0);
        luminaUpdater.UpdateConfig(Config.TaskConfig);
        luminaUpdater.UpdateData(Data.TaskData);
    }

    public override void Update() {
        Data.TaskData.Update(ref DataChanged, rowId => ClientStructs.ContentsNote.Instance()->IsContentNoteComplete((int) rowId));

        base.Update();
    }

    public override void Load() {
        base.Load();
        
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "ContentsFinder", OnContentsFinderOpen);
    }

    public override void Unload() {
        base.Unload();
        
        Service.AddonLifecycle.UnregisterListener(OnContentsFinderOpen);
    }

    private void OnContentsFinderOpen(AddonEvent type, AddonArgs args) {
        if (!Config.EnableContentFinderWarning) return;
        
        // Don't allow a warning more frequently than once every 5 mins
        if (warningThrottleTimer is { IsRunning: true, Elapsed.TotalSeconds: <= 300 }) {
            Service.Log.Info($"Suppressing Duty Finder Warning, time elapsed: {warningThrottleTimer.Elapsed} :: Unlocked at 5mins");
            return;
        }

        var anyWarningGenerated = false;

        foreach (var warningId in Config.WarningEntries) {
            var matchingTaskData = Data.TaskData.FirstOrDefault(task => task.RowId == warningId);
            if (matchingTaskData == null) continue;

            if (!matchingTaskData.Complete) {
                var taskInfo = Service.DataManager.GetExcelSheet<ContentsNote>().GetRow(warningId);
                
                StatusMessage.PrintTaggedMessage($"{taskInfo.Name.ExtractText()} is still incomplete!", "ChallengeLog");
                anyWarningGenerated = true;
            }
        }

        if (anyWarningGenerated && Config.EnableWarningSound) {
            UIGlobals.PlayChatSoundEffect(11);
            warningThrottleTimer.Restart();
        }
    }

    public override void Reset() {
        Data.TaskData.Reset();
        
        base.Reset();
    }

    protected override ModuleStatus GetModuleStatus()
        => IncompleteTaskCount is 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    protected override StatusMessage GetStatusMessage() => new() {
        Message = $"{IncompleteTaskCount} {Strings.TasksIncomplete}",
    };
}
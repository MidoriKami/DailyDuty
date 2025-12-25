using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.ConfigurationWindow.AutoConfig;
using DailyDuty.Enums;
using DailyDuty.Extensions;
using DailyDuty.Utilities;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Addons;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Features.ChallengeLog;

public class ChallengeLog : Module<ChallengeLogConfig, DataBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Challenge Log",
        FileName = "ChallengeLog",
        Type = ModuleType.Daily,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Achievements", "Exp" ],
    };

    private Stopwatch? contentsFinderStopwatch;
    private ConfigAddon? configWindow;
    private LuminaSearchAddon<ContentsNote>? contentsNoteSearch; // todo: what?
    
    public override UpdatableNode GetDataNode() => new DataNode(this);
    public override SimpleComponentNode GetConfigNode() => new ConfigNode(this);

    protected override void OnEnable() {
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostOpen, "ContentsFinder", OnContentsFinderOpen);

        configWindow = new ConfigAddon {
            InternalName = "ChallengeLogConfig",
            Title = "Challenge Log Config",
            Size = new Vector2(300.0f, 400.0f),
            Config = ModuleConfig,
        };

        configWindow.AddCategory("Duty Finder Warnings")
            .AddCheckbox("Enable Duty Finder Warning", nameof(ModuleConfig.EnableContentFinderWarning))
            .AddCheckbox("Enable Warning Sound", nameof(ModuleConfig.EnableWarningSound));

        OpenConfigAction = configWindow.Toggle;
    }

    protected override void OnDisable() {
        Services.AddonLifecycle.UnregisterListener(OnContentsFinderOpen);

        contentsFinderStopwatch = null;
    }

    private void OnContentsFinderOpen(AddonEvent type, AddonArgs args) {
        if (ModuleConfig is not { EnableContentFinderWarning: true } config) return;

        if (contentsFinderStopwatch is { IsRunning: true, Elapsed.TotalSeconds: <= 300 }) {
            Services.PluginLog.Info($"Suppressing Duty Finder Warning, time elapsed: {contentsFinderStopwatch.Elapsed} :: Unlocked at 5 mins");
            return;
        }

        contentsFinderStopwatch ??= Stopwatch.StartNew();
        contentsFinderStopwatch.Restart();

        var anyWarningGenerated = false;

        foreach (var warningId in config.WarningEntries) {
            if (!IsContentNoteComplete(warningId)) {
                if (!Services.DataManager.GetExcelSheet<ContentsNote>().TryGetRow(warningId, out var contentNote)) continue;

                Services.ChatGui.PrintTaggedMessage($"{contentNote.Name.ToString()} is still incomplete!", "ChallengeLog");
                anyWarningGenerated = true;
            }
        }

        if (anyWarningGenerated && config.EnableWarningSound) {
            UIGlobals.PlayChatSoundEffect(11);
            contentsFinderStopwatch?.Restart();
        }
    }
        
    public override DateTime GetNextResetDateTime()
        => Time.NextWeeklyReset();

    public override void Reset() { }

    protected override CompletionStatus GetCompletionStatus()
        => ModuleConfig.TrackedEntries.All(IsContentNoteComplete) ? CompletionStatus.Complete : CompletionStatus.Incomplete;

    public override ReadOnlySeString GetStatusMessage()
        => $"{ModuleConfig.TrackedEntries.Count - ModuleConfig.TrackedEntries.Count(IsContentNoteComplete)} Challenge Log Entries Incomplete";

    private static unsafe bool IsContentNoteComplete(uint rowId)
        => FFXIVClientStructs.FFXIV.Client.Game.UI.ContentsNote.Instance()->IsContentNoteComplete((int)rowId);
}

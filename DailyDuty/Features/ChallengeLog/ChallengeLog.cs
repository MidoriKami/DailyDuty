using System;
using System.Diagnostics;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.ChallengeLog;

public class ChallengeLog : Module<Config, DataBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Challenge Log",
        FileName = "ChallengeLog",
        Type = ModuleType.Weekly,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Achievements", "Exp" ],
    };

    private Stopwatch? contentsFinderStopwatch;
    public override DataNodeBase DataNode => new DataNode(this);
    public override ConfigNodeBase ConfigNode => new ConfigNode(this);

    protected override void OnEnable() {
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostOpen, "ContentsFinder", OnContentsFinderOpen);
    }

    protected override void OnDisable() {
        Services.AddonLifecycle.UnregisterListener(OnContentsFinderOpen);
        contentsFinderStopwatch = null;
    }

    public override DateTime GetNextResetDateTime()
        => Time.NextWeeklyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(7);

    protected override CompletionStatus GetCompletionStatus()
        => ModuleConfig.TrackedEntries.All(IsContentNoteComplete) ? CompletionStatus.Complete : CompletionStatus.Incomplete;

    protected override StatusMessage GetStatusMessage() => new() {
        Message = $"{ModuleConfig.TrackedEntries.Count - ModuleConfig.TrackedEntries.Count(IsContentNoteComplete)} Challenge Log entrie(s) Incomplete",
        PayloadId = PayloadId.OpenChallengeLog,
    };

    private static unsafe bool IsContentNoteComplete(uint rowId)
        => FFXIVClientStructs.FFXIV.Client.Game.UI.ContentsNote.Instance()->IsContentNoteComplete((int)rowId);
    
    private void OnContentsFinderOpen(AddonEvent type, AddonArgs args) {
        if (ModuleConfig is not { EnableContentFinderWarning: true } config) return;

        if (contentsFinderStopwatch is { IsRunning: true, Elapsed.TotalSeconds: <= 300 }) {
            Services.PluginLog.Info($"Suppressing Duty Finder Warning, time elapsed: {contentsFinderStopwatch.Elapsed} :: Unlocked at 5 mins");
            return;
        }

        contentsFinderStopwatch ??= Stopwatch.StartNew();
        contentsFinderStopwatch.Restart();

        var anyWarningGenerated = false;

        foreach (var warningId in config.WarningEntries.Where(warningId => !IsContentNoteComplete(warningId))) {
            if (!Services.DataManager.GetExcelSheet<ContentsNote>().TryGetRow(warningId, out var contentNote)) continue;

            Services.ChatGui.PrintTaggedMessage($"{contentNote.Name.ToString()} is still incomplete!", "ChallengeLog");
            anyWarningGenerated = true;
        }

        if (anyWarningGenerated && config.EnableWarningSound) {
            UIGlobals.PlayChatSoundEffect(11);
            contentsFinderStopwatch?.Restart();
        }
    }
}

using System.Diagnostics;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Extensions;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit;
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

    protected override void OnEnable() {
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostOpen, "ContentsFinder", OnContentsFinderOpen);
    }

    protected override void OnDisable() {
        Services.AddonLifecycle.UnregisterListener(OnContentsFinderOpen);

        contentsFinderStopwatch = null;
    }

    public override NodeBase GetStatusDisplayNode() => new ChallengeLogStatusNode(ModuleData);

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

                Services.Chat.PrintTaggedMessage($"{contentNote.Name.ToString()} is still incomplete!", "ChallengeLog");
                anyWarningGenerated = true;
            }
        }

        if (anyWarningGenerated && config.EnableWarningSound) {
            UIGlobals.PlayChatSoundEffect(11);
            contentsFinderStopwatch?.Restart();
        }
    }

    public override CompletionStatus? GetModuleStatus() {
        if (ModuleConfig is not { } config) return null;
        return config.TrackedEntries.All(IsContentNoteComplete) ? CompletionStatus.Complete : CompletionStatus.Incomplete;
    }

    public override ReadOnlySeString? GetStatusMessage() {
        if (ModuleConfig is not { } config) return null;
        var incompleteCount = config.TrackedEntries.Count - config.TrackedEntries.Count(IsContentNoteComplete);
        return $"{incompleteCount} Challenge Log Entries Incomplete";
    }

    private static unsafe bool IsContentNoteComplete(uint rowId)
        => FFXIVClientStructs.FFXIV.Client.Game.UI.ContentsNote.Instance()->IsContentNoteComplete((int)rowId);
}

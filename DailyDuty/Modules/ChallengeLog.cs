using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Nodes;
using Dalamud.Excel.Sheets;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.DutyState;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit.Components.Builders;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Modules;

public class ChallengeLog : Module<ChallengeLogData, ChallengeLogConfig>, IConfigurable, IWeeklyResettable, IClickableTooltip {

    public override string Name => Strings.ChallengeLog_DisplayName;

    public override ChallengeLogData Data { get; protected set; } = null!;
    public override ChallengeLogConfig Config { get; protected set; } = null!;

    public override async Task EnableAsync() {
        Data = await ChallengeLogData.Load();
        Config = await ChallengeLogConfig.Load();

        openChallengeLog = new ChatLinkPayload(() => {
            unsafe {
                UIModule.Instance()->ExecuteMainCommand(60);
            }
        });

        IAddonLifecycle.Get().RegisterListener(AddonEvent.PostOpen, "ContentsFinder", OnContentsFinderOpen);
        IDutyState.Get().DutyCompleted += OnDutyCompleted;

        contentsFinderStopwatch = new Stopwatch();
    }

    public override Task DisableAsync() {
        contentsFinderStopwatch = null;

        IDutyState.Get().DutyCompleted -= OnDutyCompleted;
        IAddonLifecycle.Get().UnregisterListener(OnContentsFinderOpen);

        openChallengeLog?.Dispose();
        openChallengeLog = null;

        return Task.CompletedTask;
    }

    public override void BuildDataUi(NodeBuilder builder) => builder
        .List([.. ValidContentsNotes], out dataListNode);

    public override void UpdateDataUi()
        => dataListNode?.Update();

    public void BuildConfigUi(NodeBuilder nodeBuilder) => nodeBuilder
        .TabbedContent(tabbedContentBuilder => tabbedContentBuilder
            .AddTab("Tracking", trackingTabBuilder => trackingTabBuilder
                .Spacer(5.0f)
                .TextWrapped("This tab controls which entries are required to be completed to mark this module as completed.")
                .Spacer(5.0f)
                .List<ContentsNote, LuminaConfigNode<ContentsNote>>([.. ValidContentsNotes], OnTrackingDataChanged, listConfigure => {
                    listConfigure.NodeReferenceData = Config.TrackedEntries;
                }))
            .AddTab("Warnings", warningsTabBuilder => warningsTabBuilder
                .Spacer(5.0f)
                .TextWrapped("This tab allows you to select entries that you want to be warned about when opening duty finder or completing a duty.")
                .Spacer(5.0f)
                .Checkbox(Strings.ChallengeLog_DutyFinderWarningSound, Config.EnableContentFinderWarning, newValue => {
                    Config.EnableContentFinderWarning = newValue;
                    Config.Save();
                })
                .Checkbox("Enable Duty Complete Warning Sound", Config.EnableDutyCompleteWarning, newValue => {
                    Config.EnableDutyCompleteWarning = newValue;
                    Config.Save();
                })
                .List<ContentsNote, LuminaConfigNode<ContentsNote>>([.. ValidContentsNotes], OnWarningDataChanged, listConfigure => {
                    listConfigure.NodeReferenceData = Config.WarningEntries;
                })));

    public ReadOnlySeString GetTooltip() {
        using var stringBuilder = new RentedSeStringBuilder();

        return stringBuilder
            .AppendJoined<ContentsNote>(Config.TrackedEntries.Where(id => !IsContentNoteComplete(id)))
            .ToReadOnlySeString();
    }

    public void OnTooltipClicked()
        => openChallengeLog?.ClickAction();

    protected override Task ResetAsync()
        => Task.CompletedTask;

    protected override CompletionStatus GetCompletionStatus()
        => Config.TrackedEntries.All(IsContentNoteComplete) ? CompletionStatus.Complete : CompletionStatus.Incomplete;

    protected override void SendStatusMessage(ReadOnlySeString? defaultMessage) => IChatGui.Get().PrintPayloadMessage(
        Config.MessageChatChannel, openChallengeLog, Name,
        defaultMessage ?? $"{Config.TrackedEntries.Count - Config.TrackedEntries.Count(IsContentNoteComplete)} {Strings.StatusMessages_ChallengeLogIncomplete}"
    );

    protected override void SendResetMessage(ReadOnlySeString? defaultMessage) => IChatGui.Get().PrintPayloadMessage(
        Config.MessageChatChannel, openChallengeLog, Name,
        defaultMessage ?? "Module has been reset."
    );

    private Stopwatch? contentsFinderStopwatch;
    private ChatLinkPayload? openChallengeLog;
    private ListNode<ContentsNote, LuminaStatusNode<ContentsNote>>? dataListNode;

    private static IEnumerable<ContentsNote> ValidContentsNotes =>
        IDataManager.Get().GetExcelSheet<ContentsNote>()
            .Where(contentNode => contentNode.RowId is not 0 && !contentNode.Name.IsEmpty);

    private void OnContentsFinderOpen(AddonEvent type, AddonArgs args) {
        if (contentsFinderStopwatch is null) return;

        if (contentsFinderStopwatch is { IsRunning: true, Elapsed.TotalSeconds: <= 300 }) {
            IPluginLog.Get().Info($"Suppressing Duty Finder Warning, time elapsed: {contentsFinderStopwatch.Elapsed} :: Unlocked at 5 mins");
            return;
        }

        contentsFinderStopwatch.Restart();

        if (Config.EnableContentFinderWarning) {
            PrintOutstandingEntries();
        }
    }

    private void OnDutyCompleted(IDutyStateEventArgs args) {
        if (Config.EnableDutyCompleteWarning) {
            PrintOutstandingEntries();
        }
    }

    private void OnEntryChanged(LuminaConfigNode<ContentsNote> node, ContentsNote data, HashSet<uint> entries) {
        if (node.IsChecked) {
            entries.Add(data.RowId);
        }
        else {
            entries.Remove(data.RowId);
        }

        Config.Save();
    }

    private void OnTrackingDataChanged(LuminaConfigNode<ContentsNote> node, ContentsNote data)
        => OnEntryChanged(node, data, Config.TrackedEntries);

    private void OnWarningDataChanged(LuminaConfigNode<ContentsNote> node, ContentsNote data)
        => OnEntryChanged(node, data, Config.WarningEntries);

    private void PrintOutstandingEntries() {
        var anyWarningGenerated = false;

        foreach (var warningId in Config.WarningEntries.Where(warningId => !IsContentNoteComplete(warningId))) {
            if (!IDataManager.Get().GetExcelSheet<ContentsNote>().TryGetRow(warningId, out var contentNote)) continue;

            IChatGui.Get().PrintPayloadMessage(Config.MessageChatChannel, openChallengeLog, Name,
                $"{contentNote.Name.ToString()} is still incomplete!"
            );

            anyWarningGenerated = true;
        }

        if (anyWarningGenerated && Config.EnableWarningSound) {
            UIGlobals.PlayChatSoundEffect(11);
        }
    }

    private static unsafe bool IsContentNoteComplete(uint rowId)
        => FFXIVClientStructs.FFXIV.Client.Game.UI.ContentsNote.Instance()->IsContentNoteComplete((int)rowId);
}

public class ChallengeLogData : ModuleDataFile<ChallengeLogData> {
    protected override string FileName => "ChallengeLog.data.json";
}

public class ChallengeLogConfig : ModuleConfigFile<ChallengeLogConfig> {
    protected override string FileName => "ChallengeLog.config.json";

    public bool EnableContentFinderWarning { get; set; } = true;
    public bool EnableDutyCompleteWarning { get; set; } = true;
    public bool EnableWarningSound { get; set; } = true;

    public HashSet<uint> WarningEntries { get; set; } = [];
    public HashSet<uint> TrackedEntries { get; set; } = [];
}

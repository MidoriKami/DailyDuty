using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyDuty.Data;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.UI;
using Lumina.Excel.Sheets;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using KamiToolKit.Controls.Builders;
using KamiToolKit.UI.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Modules;

public class MaskedCarnivale : Module<MaskedCarnivaleData, MaskedCarnivaleConfig>, IWeeklyResettable, IConfigurable, IFrameworkUpdatable, IClickableTooltip {

    public override string DisplayName => Strings.MaskedCarnivale_DisplayName;

    public override MaskedCarnivaleData Data { get; protected set; } = null!;
    public override MaskedCarnivaleConfig Config { get; protected set; } = null!;

    private TextNode? noviceNode;
    private TextNode? moderateNode;
    private TextNode? advancedNode;
    private ChatLinkPayload? uldahTeleportPayload;

    public override async Task EnableAsync() {
        Data = await MaskedCarnivaleData.Load();
        Config = await MaskedCarnivaleConfig.Load();

        uldahTeleportPayload = ChatLinkPayload.TeleportToUldah();

        IAddonLifecycle.Get().RegisterListener(AddonEvent.PostSetup, "AOZContentResult", AozContentResultPostSetup);
    }

    public override Task DisableAsync() {
        IAddonLifecycle.Get().UnregisterListener(AozContentResultPostSetup);

        uldahTeleportPayload?.Dispose();
        uldahTeleportPayload = null;

        noviceNode = null;
        moderateNode = null;
        advancedNode = null;

        return Task.CompletedTask;
    }

    public override void BuildDataUi(NodeBuilder builder) {
        var addonSheet = IDataManager.Get().GetExcelSheet<Addon>();
        var advancedText = addonSheet.GetRow(12447u).Text.ToString();
        var moderateText = addonSheet.GetRow(12448u).Text.ToString();
        var noviceText = addonSheet.GetRow(12449u).Text.ToString();

        builder
            .DataRow(noviceText, Strings.CompletionStatus_NotStarted, out noviceNode)
            .DataRow(moderateText, Strings.CompletionStatus_NotStarted, out moderateNode)
            .DataRow(advancedText, Strings.CompletionStatus_NotStarted, out advancedNode);
    }

    public override void UpdateDataUi() {
        if (Data.TaskData.TryGetValue(12449, out var noviceComplete)) {
            noviceNode?.String = noviceComplete ? Strings.CompletionStatus_Complete : Strings.CompletionStatus_Incomplete;
        }

        if (Data.TaskData.TryGetValue(12448, out var moderateComplete)) {
            moderateNode?.String = moderateComplete ? Strings.CompletionStatus_Complete : Strings.CompletionStatus_Incomplete;
        }

        if (Data.TaskData.TryGetValue(12447, out var advancedComplete)) {
            advancedNode?.String = advancedComplete ? Strings.CompletionStatus_Complete : Strings.CompletionStatus_Incomplete;
        }
    }

    public void BuildConfigUi(NodeBuilder builder) {
        var addonSheet = IDataManager.Get().GetExcelSheet<Addon>();
        var difficulties = new[] {
            (Text: addonSheet.GetRow(12449u).Text.ToString(), RowId: 12449u),
            (Text: addonSheet.GetRow(12448u).Text.ToString(), RowId: 12448u),
            (Text: addonSheet.GetRow(12447u).Text.ToString(), RowId: 12447u),
        };

        foreach (var (text, rowId) in difficulties) {
            builder.Checkbox(text, Config.TrackedTasks.Contains(rowId), isChecked => {

                if (isChecked) {
                    Config.TrackedTasks.Add(rowId);
                }
                else {
                    Config.TrackedTasks.Remove(rowId);
                }

                Config.Save();
            });
        }
    }

    protected override async Task ResetAsync() {
        foreach (var key in Data.TaskData.Keys) {
            Data.TaskData[key] = false;
        }
        await Data.Save();
    }

    protected override CompletionStatus GetCompletionStatus()
        => GetIncompleteCount() is not 0 ? CompletionStatus.Incomplete : CompletionStatus.Complete;

    public ReadOnlySeString GetTooltip() {
        var addonSheet = IDataManager.Get().GetExcelSheet<Addon>();

        var incompleteTasks = Config.TrackedTasks
            .Where(taskId => !Data.TaskData.GetValueOrDefault((int)taskId))
            .Select(taskId => addonSheet.GetRow(taskId).Text.ToString());

        return string.Join("\n", incompleteTasks);
    }

    public void OnTooltipClicked()
        => uldahTeleportPayload?.ClickAction();

    private int GetIncompleteCount() {
        var count = 0;
        foreach (var tracked in Config.TrackedTasks) {
            if (!Data.TaskData.TryGetValue((int)tracked, out var taskCompleted) || !taskCompleted) {
                count++;
            }
        }
        return count;
    }

    void IFrameworkUpdatable.Update() {
        unsafe {
            if (AgentAozContentBriefing.Instance() is not null && AgentAozContentBriefing.Instance()->IsAgentActive()) {
                Data.TaskData.TryAdd(12449, false);
                Data.TaskData.TryAdd(12448, false);
                Data.TaskData.TryAdd(12447, false);

                foreach (var addonId in Data.TaskData.Keys) {
                    var status = addonId switch {
                        12449 => AgentAozContentBriefing.Instance()->IsWeeklyChallengeComplete(AozWeeklyChallenge.Novice),
                        12448 => AgentAozContentBriefing.Instance()->IsWeeklyChallengeComplete(AozWeeklyChallenge.Moderate),
                        12447 => AgentAozContentBriefing.Instance()->IsWeeklyChallengeComplete(AozWeeklyChallenge.Advanced),
                        _ => false,
                    };

                    if (Data.TaskData[addonId] != status) {
                        Data.TaskData[addonId] = status;
                        Data.Save();
                    }
                }
            }
        }
    }

    protected override void SendStatusMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        uldahTeleportPayload,
        DisplayName,
        message ?? $"{GetIncompleteCount()} {Strings.MaskedCarnivale_StatusMessageIncomplete}"
    );

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        DisplayName,
        message ?? Strings.MaskedCarnivale_StatusMessageReset
    );

    private void AozContentResultPostSetup(AddonEvent eventType, AddonArgs addonInfo) {
        try {
            var atkValues = addonInfo.Addon.AtkValueSpan;
            if (!atkValues[112].TryGet(out uint completionIndex)) return;
            if (!atkValues[114].TryGet(out byte completionStatus)) return;

            var addonId = completionIndex switch {
                0 => 12449,
                1 => 12448,
                2 => 12447,
                _ => -1,
            };

            if (addonId < 0) return;

            Data.TaskData.TryAdd(addonId, completionStatus != 0);
            Data.TaskData[addonId] = completionStatus != 0;
            Data.Save();
        }
        catch (Exception e) {
            IPluginLog.Get().Error(e, "Exception processing MaskedCarnivale addon event");
        }
    }
}

public class MaskedCarnivaleData : ModuleDataFile<MaskedCarnivaleData> {
    protected override string FileName => "MaskedCarnivale.data.json";

    public Dictionary<int, bool> TaskData { get; set; } = [];
}

public class MaskedCarnivaleConfig : ModuleConfigFile<MaskedCarnivaleConfig> {
    protected override string FileName => "MaskedCarnivale.config.json";

    public List<uint> TrackedTasks { get; set; } = [12447, 12448, 12449];
}

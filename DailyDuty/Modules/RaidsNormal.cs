using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using DailyDuty.Data;
using DailyDuty.UI;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Nodes.Config;
using DailyDuty.Nodes.Display;
using Dalamud.Game.Inventory;
using Dalamud.Game.Inventory.InventoryEventArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiToolKit.Controls.Builders;
using KamiToolKit.UI.Nodes.Layout;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Modules;

public class RaidsNormal : Module<RaidsNormalData, RaidsNormalConfig>, IWeeklyResettable, IConfigurable, IClickableTooltip {
    public override string DisplayName => Strings.RaidsNormal_DisplayName;

    public override RaidsNormalData Data { get; protected set; } = null!;
    public override RaidsNormalConfig Config { get; protected set; } = null!;

    private ListNode<RaidEntry, RaidStatusNode>? dataListNode;

    private VerticalListNode? dataWarningPage;

    private ChatLinkPayload? openDutyFinderPayload;

    public override async Task EnableAsync() {
        Data = await RaidsNormalData.Load();
        Config = await RaidsNormalConfig.Load();

        var validRaids = IDataManager.Get().LimitedNormalRaidDuties.Select(d => d.RowId).ToHashSet();
        Config.TrackedRaids.RemoveAll(e => !validRaids.Contains(e.Id));
        await Config.Save();

        openDutyFinderPayload = ChatLinkPayload.OpenDutyFinderRaid();

        IGameInventory.Get().ItemAdded += OnItemEvent;
        IGameInventory.Get().ItemChanged += OnItemEvent;
    }

    public override Task DisableAsync() {
        IGameInventory.Get().ItemAdded -= OnItemEvent;
        IGameInventory.Get().ItemChanged -= OnItemEvent;

        openDutyFinderPayload?.Dispose();
        openDutyFinderPayload = null;

        dataListNode = null;
        dataWarningPage = null;

        return Task.CompletedTask;
    }

    public override void BuildDataUi(NodeBuilder builder) => builder
        .FittedContent(inner => inner
            .VerticalList(list => list
                .TextWarningFilled(Strings.Raids_NoneTracked), out dataWarningPage)
            .List(Config.TrackedRaids, out dataListNode));

    public override void UpdateDataUi() {
        dataListNode?.Update();

        dataWarningPage?.IsVisible = Config.TrackedRaids.Count is 0;
        dataListNode?.IsVisible = Config.TrackedRaids.Count > 0;
    }

    public void BuildConfigUi(NodeBuilder builder) {
        var raids = IDataManager.Get().LimitedNormalRaidDuties.ToList();
        if (raids.Count is 0) {
            builder.TextWarningFilled(Strings.Raids_NoneLimited);
            return;
        }

        builder.List<ContentFinderCondition, LuminaConfigNode<ContentFinderCondition>>(raids, OnTrackedEntryChanged, configureList => {
            configureList.NodeReferenceData = Config.TrackedRaids;
        });
    }

    private void OnTrackedEntryChanged(LuminaConfigNode<ContentFinderCondition> node, ContentFinderCondition data) {
        if (node.IsChecked) {
            Config.TrackedRaids.Add(new RaidEntry(data.RowId, false));
        }
        else {
            Config.TrackedRaids.RemoveAll(e => e.Id == data.RowId);
        }
        Config.Save();
    }

    public ReadOnlySeString GetTooltip()
        => new(string.Join("\n", GetIncompleteTasks()));

    public void OnTooltipClicked()
        => openDutyFinderPayload?.ClickAction();

    protected override async Task ResetAsync() {
        foreach (var entry in Config.TrackedRaids) {
            Config.TrackedRaids[Config.TrackedRaids.IndexOf(entry)] = entry with { Complete = false };
        }

        await Config.Save();
    }

    protected override CompletionStatus GetCompletionStatus()
        => GetIncompleteCount() is 0 ? CompletionStatus.Complete : CompletionStatus.Incomplete;

    protected override void SendStatusMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        openDutyFinderPayload,
        DisplayName,
        message ?? $"{GetIncompleteCount()} {Strings.StatusMessages_NormalRaidIncomplete}"
    );

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        openDutyFinderPayload,
        DisplayName,
        message ?? "Module has been reset."
    );

    private int GetIncompleteCount()
        => Config.TrackedRaids.Count(e => !e.Complete);

    private unsafe void OnItemEvent(GameInventoryEvent type, InventoryEventArgs data) {
        if (data.Item.ContainerType is not (GameInventoryType.Inventory1 or GameInventoryType.Inventory2 or GameInventoryType.Inventory3 or GameInventoryType.Inventory4)) return;

        var currentDuty = GameMain.Instance()->CurrentContentFinderConditionId;
        var index = Config.TrackedRaids.FindIndex(e => e.Id == currentDuty);
        if (index < 0) return;

        var item = IDataManager.Get().GetExcelSheet<Item>().GetRow(data.Item.ItemId);
        if (item.RowId is 0) return;

        IPluginLog.Get().Debug($"InventoryEvent: {type}: {item.Name}");

        switch (item.ItemUICategory.RowId) {
            case 34:                                 // Head
            case 35:                                 // Body
            case 36:                                 // Legs
            case 37:                                 // Hands
            case 38:                                 // Feet
            case 61 when item.ItemAction.RowId == 0: // Miscellany with no itemAction
                Config.TrackedRaids[index] = Config.TrackedRaids[index] with { Complete = true };
                Config.Save();
                break;
        }
    }

    private IEnumerable<string> GetIncompleteTasks() => Config.TrackedRaids
        .Where(e => !e.Complete)
        .Select(e => IDataManager.Get().GetExcelSheet<ContentFinderCondition>().GetRow(e.Id).Name.ToString());
}

public class RaidsNormalData : ModuleDataFile<RaidsNormalData> {
    protected override string FileName => "RaidsNormal.data.json";
}

public class RaidsNormalConfig : ModuleConfigFile<RaidsNormalConfig> {
    protected override string FileName => "RaidsNormal.config.json";
    public override int Version => 2;

    public List<RaidEntry> TrackedRaids { get; set; } = [];

    protected override bool TryMigrateStep(ref int fileVersion, JsonObject fileData) {
        switch (fileVersion) {
            case 1:
                var trackedTasks = fileData["TrackedTasks"]?.Deserialize<Dictionary<uint, bool>>() ?? [];
                var trackedRaids = trackedTasks
                    .Where(kv => kv.Value)
                    .Select(kv => new RaidEntry(kv.Key, false))
                    .ToList();

                fileData[nameof(TrackedRaids)] = JsonSerializer.SerializeToNode(trackedRaids);
                fileData.Remove("TrackedTasks");
                fileVersion = 2;
                return true;
        }

        return base.TryMigrateStep(ref fileVersion, fileData);
    }
}

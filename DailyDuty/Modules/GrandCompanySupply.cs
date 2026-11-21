using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using KamiToolKit.Components.Builders;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Modules;

public class GrandCompanySupply : Module<GrandCompanySupplyData, GrandCompanySupplyConfig>, IDailyResettable, IConfigurable, IFrameworkUpdatable {

    public override string DisplayName => Strings.GrandCompanySupply_DisplayName;

    public override GrandCompanySupplyData Data { get; protected set; } = null!;
    public override GrandCompanySupplyConfig Config { get; protected set; } = null!;

    private Dictionary<uint, TextNode>? statusNodes;

    private ChatLinkPayload? grandCompanyTeleportPayload;

    public override async Task EnableAsync() {
        Data = await GrandCompanySupplyData.Load();
        Config = await GrandCompanySupplyConfig.Load();

        grandCompanyTeleportPayload = ChatLinkPayload.TeleportToGrandCompany();

        statusNodes = [];
    }

    public override Task DisableAsync() {
        grandCompanyTeleportPayload?.Dispose();
        grandCompanyTeleportPayload = null;

        statusNodes = null;

        return Task.CompletedTask;
    }

    public override void BuildDataUi(NodeBuilder builder) {
        if (statusNodes == null) return;

        statusNodes.Clear();
        var classJobSheet = IDataManager.Get().GetExcelSheet<ClassJob>();

        foreach (var (job, _) in Data.ClassJobStatus) {
            var classJob = classJobSheet.GetRow(job);

            builder.DataRow(classJob.NameEnglish.ToString(), job.ToString(), out var statusNode);
            statusNodes.TryAdd(job, statusNode);
        }
    }

    public override void UpdateDataUi() {
        if (statusNodes == null) return;

        foreach (var (job, node) in statusNodes) {
            node?.String = Data.ClassJobStatus[job] ? Strings.CompletionStatus_Complete : Strings.CompletionStatus_Incomplete;
        }
    }

    public unsafe void Update() {
        var agent = AgentGrandCompanySupply.Instance();
        if (!agent->IsAgentActive()) return;
        if (agent->NumItems < 11) return;

        var supplyLength = agent->SupplyProvisioningData->SupplyData.Length;
        var supplySpan = new Span<GrandCompanyItem>(agent->ItemArray, supplyLength);

        foreach (var item in supplySpan) {
            if (item is { ItemId: 0 }) continue;

            // Offset ClassJob by 8, as there's 8 non-DoH/DoL jobs at the start of the sheet that native omits
            var classJobPosition = (uint)(item.Position + 8);

            if (Data.ClassJobStatus.TryGetValue(classJobPosition, out var value) && value != !item.IsTurnInAvailable) {
                Data.ClassJobStatus[classJobPosition] = !item.IsTurnInAvailable;
                Data.Save();
            }
        }
    }

    protected override Task ResetAsync() {
        foreach (var entry in Data.ClassJobStatus.Keys) {
            Data.ClassJobStatus[entry] = false;
        }
        Data.Save();
        return Task.CompletedTask;
    }

    protected override CompletionStatus GetCompletionStatus() {
        foreach (var job in Config.TrackedClasses.Where(pair => pair.Value)) {
            if (!Data.ClassJobStatus.TryGetValue(job.Key, out var value) || !value)
                return CompletionStatus.Incomplete;
        }

        return CompletionStatus.Complete;
    }

    protected override void SendStatusMessage(ReadOnlySeString? message) {
        var incompleteCount = Config.TrackedClasses.Count(job => job.Value && !Data.ClassJobStatus.GetValueOrDefault(job.Key));

        IChatGui.Get().PrintMessage(
            Config.MessageChatChannel,
            grandCompanyTeleportPayload,
            DisplayName,
            message ?? $"{incompleteCount} {Strings.StatusMessages_DeliveryIncomplete}"
        );
    }

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        grandCompanyTeleportPayload,
        DisplayName,
        message ?? "Module has been reset."
    );

    public void BuildConfigUi(NodeBuilder builder) {
        var classJobSheet = IDataManager.Get().GetExcelSheet<ClassJob>();

        foreach (var (job, _) in Data.ClassJobStatus) {
            var classJob = classJobSheet.GetRow(job);

            builder.Checkbox(classJob.NameEnglish.ToString(), Config.TrackedClasses.GetValueOrDefault(job, false), isChecked => {
                Config.TrackedClasses[job] = isChecked;
                Config.Save();
            });
        }
    }
}

public class GrandCompanySupplyData : ModuleDataFile<GrandCompanySupplyData> {
    protected override string FileName => "GrandCompanySupply.data.json";

    public Dictionary<uint, bool> ClassJobStatus = new() {
        [8] = false,
        [9] = false,
        [10] = false,
        [11] = false,
        [12] = false,
        [13] = false,
        [14] = false,
        [15] = false,
    };
}

public class GrandCompanySupplyConfig : ModuleConfigFile<GrandCompanySupplyConfig> {
    protected override string FileName => "GrandCompanySupply.config.json";

    public Dictionary<uint, bool> TrackedClasses = new() {
        [8] = true,
        [9] = true,
        [10] = true,
        [11] = true,
        [12] = true,
        [13] = true,
        [14] = true,
        [15] = true,
    };
}

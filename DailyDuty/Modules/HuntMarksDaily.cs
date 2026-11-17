using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Nodes;
using DailyDuty.Utilities;
using Dalamud.Excel.Sheets;
using Dalamud.Plugin.Services;
using KamiToolKit.Components.Builders;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Modules;

public class HuntMarksDaily : Module<HuntMarksDailyData, HuntMarksDailyConfig>, IDailyResettable, IConfigurable {

    public override string Name => Strings.HuntMarksDaily_DisplayName;

    public override HuntMarksDailyData Data { get; protected set; } = null!;
    public override HuntMarksDailyConfig Config { get; protected set; } = null!;

    private ListNode<MobHuntOrderType, LuminaStatusNode<MobHuntOrderType>>? dataListNode;

    public override async Task EnableAsync() {
        Data = await HuntMarksDailyData.Load();
        Config = await HuntMarksDailyConfig.Load();
    }

    public override Task DisableAsync() {
        dataListNode = null;
        return Task.CompletedTask;
    }

    public override void BuildDataUi(NodeBuilder builder) => builder
        .List([.. MobHuntExtensions.GetValidHuntMarks(1).Where(m => Config.TrackedHuntMarks.Contains(m.RowId))], out dataListNode);

    public override void UpdateDataUi()
        => dataListNode?.Update();

    public void BuildConfigUi(NodeBuilder builder) => builder
        .List<MobHuntOrderType, LuminaConfigNode<MobHuntOrderType>>([.. MobHuntExtensions.GetValidHuntMarks(1)], OnTrackingDataChanged, listConfigure => {
            listConfigure.NodeReferenceData = Config.TrackedHuntMarks;
        });

    protected override Task ResetAsync()
        => Task.CompletedTask;

    protected override CompletionStatus GetCompletionStatus()
        => Config.TrackedHuntMarks.All(Excel.IsComplete<MobHuntOrderType>) ? CompletionStatus.Complete : CompletionStatus.Incomplete;

    private void OnTrackingDataChanged(LuminaConfigNode<MobHuntOrderType> node, MobHuntOrderType data) {
        if (node.IsChecked) {
            Config.TrackedHuntMarks.Add(data.RowId);
        }
        else {
            Config.TrackedHuntMarks.Remove(data.RowId);
        }

        Config.Save();
    }

    protected override void SendStatusMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(Config.MessageChatChannel, Name,
        message ?? $"{Config.TrackedHuntMarks.Count - Config.TrackedHuntMarks.Count(Excel.IsComplete<MobHuntOrderType>)} {Strings.StatusMessages_HuntBillIncomplete}"
    );

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(Config.MessageChatChannel, Name,
        message ?? "Module has been reset."
    );
}

public class HuntMarksDailyData : ModuleDataFile<HuntMarksDailyData> {
    protected override string FileName => "HuntMarksDaily.data.json";
}

public class HuntMarksDailyConfig : ModuleConfigFile<HuntMarksDailyConfig> {
    protected override string FileName => "HuntMarksDaily.config.json";

    public HashSet<uint> TrackedHuntMarks { get; set; } = [];
}

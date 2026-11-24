using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyDuty.Data;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Nodes.Config;
using DailyDuty.Nodes.Display;
using DailyDuty.Utilities;
using Lumina.Excel.Sheets;
using Dalamud.Plugin.Services;
using KamiToolKit.Controls.Builders;
using KamiToolKit.UI.Nodes.Layout;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Modules;

public class HuntMarksWeekly : Module<HuntMarksWeeklyData, HuntMarksWeeklyConfig>, IWeeklyResettable, IConfigurable {

    public override string DisplayName => Strings.HuntMarksWeekly_DisplayName;

    public override HuntMarksWeeklyData Data { get; protected set; } = null!;
    public override HuntMarksWeeklyConfig Config { get; protected set; } = null!;

    private ListNode<MobHuntOrderType, LuminaStatusNode<MobHuntOrderType>>? dataListNode;

    public override async Task EnableAsync() {
        Data = await HuntMarksWeeklyData.Load();
        Config = await HuntMarksWeeklyConfig.Load();
    }

    public override Task DisableAsync() {
        dataListNode = null;
        return Task.CompletedTask;
    }

    public override void BuildDataUi(NodeBuilder builder) => builder
        .List([.. MobHuntExtensions.GetValidHuntMarks(2).Where(m => Config.TrackedHuntMarks.Contains(m.RowId))], out dataListNode);

    public override void UpdateDataUi()
        => dataListNode?.Update();

    public void BuildConfigUi(NodeBuilder builder) => builder
        .List<MobHuntOrderType, LuminaConfigNode<MobHuntOrderType>>([.. MobHuntExtensions.GetValidHuntMarks(2)], OnTrackingDataChanged, listConfigure => {
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

    protected override void SendStatusMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        DisplayName,
        message ?? $"{Config.TrackedHuntMarks.Count - Config.TrackedHuntMarks.Count(Excel.IsComplete<MobHuntOrderType>)} {Strings.StatusMessages_HuntBillIncomplete}"
    );

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        DisplayName,
        message ?? "Module has been reset."
    );
}

public class HuntMarksWeeklyData : ModuleDataFile<HuntMarksWeeklyData> {
    protected override string FileName => "HuntMarksWeekly.data.json";
}

public class HuntMarksWeeklyConfig : ModuleConfigFile<HuntMarksWeeklyConfig> {
    protected override string FileName => "HuntMarksWeekly.config.json";

    public HashSet<uint> TrackedHuntMarks { get; set; } = [];
}

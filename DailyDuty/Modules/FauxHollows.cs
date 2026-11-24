using System.Threading.Tasks;
using DailyDuty.Data;
using DailyDuty.UI;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using KamiToolKit.Controls.Builders;
using KamiToolKit.UI.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Modules;

public class FauxHollows : Module<FauxHollowsData, FauxHollowsConfig>, IWeeklyResettable, IConfigurable {

    public override string DisplayName => Strings.FauxHollows_DisplayName;

    public override FauxHollowsData Data { get; protected set; } = null!;
    public override FauxHollowsConfig Config { get; protected set; } = null!;

    private TextNode? completionText;

    private ChatLinkPayload? idyllShireTeleportPayload;

    public override async Task EnableAsync() {
        Data = await FauxHollowsData.Load();
        Config = await FauxHollowsConfig.Load();

        idyllShireTeleportPayload = ChatLinkPayload.TeleportToIdyllshire();

        IAddonLifecycle.Get().RegisterListener(AddonEvent.PreSetup, "WeeklyPuzzle", WeeklyPuzzlePreSetup);
    }

    public override Task DisableAsync() {
        IAddonLifecycle.Get().UnregisterListener(WeeklyPuzzlePreSetup);

        idyllShireTeleportPayload?.Dispose();
        idyllShireTeleportPayload = null;

        completionText = null;

        return Task.CompletedTask;
    }

    public override void BuildDataUi(NodeBuilder builder) => builder
        .DataRow(Strings.FauxHollows_CompletionCount, "COMPLETIONS", out completionText);

    public override void UpdateDataUi()
        => completionText?.String = Data.FauxHollowsCompletions.ToString();

    public void BuildConfigUi(NodeBuilder builder) => builder
        .Checkbox(Strings.FauxHollows_IncludeRetelling, Config.IncludeRetelling, val => {
            Config.IncludeRetelling = val;
            Config.Save();
        });

    protected override async Task ResetAsync() {
        Data.FauxHollowsCompletions = 0;
        await Data.Save();
    }

    protected override CompletionStatus GetCompletionStatus() => Config.IncludeRetelling switch {
        true when Data.FauxHollowsCompletions >= 2 => CompletionStatus.Complete,
        false when Data.FauxHollowsCompletions >= 1 => CompletionStatus.Complete,
        _ => CompletionStatus.Incomplete,
    };

    protected override void SendStatusMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        idyllShireTeleportPayload,
        DisplayName,
        message ?? "Unreal Trial for Faux Hollows Incomplete"
    );

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        idyllShireTeleportPayload,
        DisplayName,
        message ?? "Module has been reset."
    );

    private void WeeklyPuzzlePreSetup(AddonEvent type, AddonArgs args) {
        Data.FauxHollowsCompletions++;
        Data.Save();
    }
}

public class FauxHollowsData : ModuleDataFile<FauxHollowsData> {
    protected override string FileName => "FauxHollows.data.json";

    public int FauxHollowsCompletions { get; set; }
}

public class FauxHollowsConfig : ModuleConfigFile<FauxHollowsConfig> {
    protected override string FileName => "FauxHollows.config.json";

    public bool IncludeRetelling { get; set; } = true;
}

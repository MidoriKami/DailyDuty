using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using KamiLib.ChatCommands;
using KamiLib.Configuration;
using KamiLib.Drawing;

namespace DailyDuty.Modules;

public class FauxHollowsSettings : GenericSettings
{
    public Setting<bool> EnableClickableLink = new(true);
    public Setting<bool> IncludeRetelling = new(true);
    public int FauxHollowsCompleted;
}

public class FauxHollows : AbstractModule
{
    public override ModuleName Name => ModuleName.UnrealTrial;
    public override CompletionType CompletionType => CompletionType.Weekly;

    private static FauxHollowsSettings Settings => Service.ConfigurationManager.CharacterConfiguration.FauxHollows;
    public override GenericSettings GenericSettings => Settings;
    public override DalamudLinkPayload? DalamudLinkPayload { get; }
    public override bool LinkPayloadActive => Settings.EnableClickableLink;
    
    public FauxHollows()
    {
        DalamudLinkPayload = ChatPayloadManager.Instance.AddChatLink(ChatPayloads.OpenPartyFinder, OpenPartyFinder);

        WeeklyPuzzleAddon.Instance.Show += OnShow;
    }
        
    public override void Dispose()
    {
        WeeklyPuzzleAddon.Instance.Show -= OnShow;
    }

    private void OnShow(object? sender, nint e)
    {
        if (!Settings.Enabled) return;

        Settings.FauxHollowsCompleted += 1;
        Service.ConfigurationManager.Save();
    }

    private void OpenPartyFinder(uint arg1, SeString arg2) => Service.ChatManager.SendCommandUnsafe("partyfinder");
    public override string GetStatusMessage() => $"{Strings.FauxHollows_TrialAvailable}";
    public override void DoReset() => Settings.FauxHollowsCompleted = 0;
    public override ModuleStatus GetModuleStatus() => Settings.FauxHollowsCompleted >= GetRequiredCompletionCount() ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    private static int GetRequiredCompletionCount() => Settings.IncludeRetelling ? 2 : 1;

    protected override void DrawConfiguration()
    {
        InfoBox.Instance.DrawGenericSettings(this);

        InfoBox.Instance
            .AddTitle(Strings.FauxHollows_Retelling)
            .AddConfigCheckbox(Strings.FauxHollows_Retelling, Settings.IncludeRetelling, Strings.FauxHollows_Retelling_Info)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Common_ClickableLink)
            .AddString(Strings.PartyFinder_ClickableLink)
            .AddConfigCheckbox(Strings.Common_Enabled, Settings.EnableClickableLink)
            .Draw();

        InfoBox.Instance.DrawNotificationOptions(this);
    }

    protected override void DrawStatus()
    {
        InfoBox.Instance.DrawGenericStatus(this);

        InfoBox.Instance
            .AddTitle(Strings.Status_ModuleData)
            .BeginTable()
            .BeginRow()
            .AddString(Strings.Common_Completions)
            .AddString($"{Settings.FauxHollowsCompleted} / {GetRequiredCompletionCount()}", GetModuleStatus().GetStatusColor())
            .EndRow()
            .EndTable()
            .Draw();
            
        InfoBox.Instance.DrawSuppressionOption(this);
    }
}
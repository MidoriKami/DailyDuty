using DailyDuty.Interfaces;
using System.Linq;
using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Teleporter;

namespace DailyDuty.Modules;

public class MaskedCarnivaleSettings : GenericSettings
{
    public TrackedMaskedCarnivale[] TrackedTasks =
    {
        new(CarnivaleTask.Novice, new Setting<bool>(true), false),
        new(CarnivaleTask.Moderate, new Setting<bool>(true), false),
        new(CarnivaleTask.Advanced, new Setting<bool>(true), false)
    };

    public Setting<bool> EnableClickableLink = new(true);
}

public unsafe class MaskedCarnivale : AbstractModule
{
    public override ModuleName Name => ModuleName.MaskedCarnivale;
    public override CompletionType CompletionType => CompletionType.Weekly;

    private static MaskedCarnivaleSettings Settings => Service.ConfigurationManager.CharacterConfiguration.MaskedCarnivale;
    public override GenericSettings GenericSettings => Settings;

    public override DalamudLinkPayload DalamudLinkPayload => TeleportManager.Instance.GetPayload(TeleportLocation.UlDah);
    public override bool LinkPayloadActive => Settings.EnableClickableLink;
    private AgentInterface* AozContentBriefingAgentInterface => AgentModule.Instance()->GetAgentByInternalId(AgentId.AozContentBriefing);

    private delegate byte IsWeeklyCompleteDelegate(AgentInterface* agent, byte index);
    [Signature("4C 8B C1 80 FA 03")] private readonly IsWeeklyCompleteDelegate isWeeklyCompleted = null!;
    
    public MaskedCarnivale()
    {
        SignatureHelper.Initialise(this);

        AOZContentResultAddon.Instance.Setup += OnSetup;
        Service.Framework.Update += OnFrameworkUpdate;
    }

    public override void Dispose()
    {
        AOZContentResultAddon.Instance.Setup -= OnSetup;
        Service.Framework.Update -= OnFrameworkUpdate;
    }
        
    private void OnSetup(object? sender, AOZContentResultArgs e)
    {
        switch (e.CompletionType)
        {
            // Novice
            case 0 when e.Successful:
                SetTaskState(CarnivaleTask.Novice, true);
                break;
                
            // Moderate 
            case 1 when e.Successful:
                SetTaskState(CarnivaleTask.Moderate, true);
                break;
                
            // Advanced
            case 2 when e.Successful:
                SetTaskState(CarnivaleTask.Advanced, true);
                break;
                    
            // Other
            case 3:
                break;
        }
    }
        
    private void OnFrameworkUpdate(Dalamud.Game.Framework framework)
    {
        if (!Settings.Enabled) return;
        if (!AozContentBriefingAgentInterface->IsAgentActive()) return;
            
        foreach (var task in Settings.TrackedTasks)
        {
            var completed = isWeeklyCompleted(AozContentBriefingAgentInterface, (byte) task.Task) != 0;

            if (task.State != completed)
            {
                task.State = completed;
                Service.ConfigurationManager.Save();
            }
        }
    }

    public override void DoReset()
    {
        foreach (var task in Settings.TrackedTasks)
        {
            task.State = false;
        }
    }

    private static void SetTaskState(CarnivaleTask task, bool completedState)
    {
        foreach (var trackedTask in Settings.TrackedTasks)
        {
            if (trackedTask.Task == task)
            {
                trackedTask.State = completedState;
                Service.ConfigurationManager.Save();
            }
        }
    }
    
    public override string GetStatusMessage() => $"{GetIncompleteCount()} {Strings.Common_AllowancesRemaining}";
    public override ModuleStatus GetModuleStatus() => GetIncompleteCount() == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    private static int GetIncompleteCount() => Settings.TrackedTasks.Where(task => task.Tracked && !task.State).Count();

    protected override void DrawConfiguration()
    {
        InfoBox.Instance.DrawGenericSettings(this);

        InfoBox.Instance
            .AddTitle(Strings.GrandCompany_Tracked)
            .AddList(Settings.TrackedTasks)
            .Draw();
            
        InfoBox.Instance
            .AddTitle(Strings.Common_ClickableLink)
            .AddString(Strings.UlDah_ClickableLink)
            .AddConfigCheckbox(Strings.Common_Enabled, Settings.EnableClickableLink)
            .Draw();
            
        InfoBox.Instance.DrawNotificationOptions(this);
    }

    protected override void DrawStatus()
    {
        InfoBox.Instance.DrawGenericStatus(this);
            
        if (Settings.TrackedTasks.Any(row => row.Tracked))
        {
            InfoBox.Instance
                .AddTitle(Strings.Status_ModuleData)
                .BeginTable()
                .AddDataRows(Settings.TrackedTasks.Where(row => row.Tracked))
                .EndTable()
                .Draw();
        }
        else
        {
            InfoBox.Instance
                .AddTitle(Strings.Status_ModuleData, out var innerWidth)
                .AddStringCentered(Strings.MaskedCarnivale_NothingTracked, innerWidth, Colors.Orange)
                .Draw();
        }
            
        InfoBox.Instance.DrawSuppressionOption(this);
    }
}
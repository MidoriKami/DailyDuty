using System;
using System.Linq;
using DailyDuty.DataModels;
using DailyDuty.DataStructures;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Misc;

namespace DailyDuty.Modules;

public class GrandCompanySupplySettings : GenericSettings
{
    public TrackedGrandCompanySupplyProvisioning[] TrackedSupply =
    {
        new(8, new Setting<bool>(true), false),
        new(9, new Setting<bool>(true), false),
        new(10, new Setting<bool>(true), false),
        new(11, new Setting<bool>(true), false),
        new(12, new Setting<bool>(true), false),
        new(13, new Setting<bool>(true), false),
        new(14, new Setting<bool>(true), false),
        new(15, new Setting<bool>(true), false),
    };
}

public unsafe class GrandCompanySupply : Module
{
    public override ModuleName Name => ModuleName.GrandCompanySupply;
    public override CompletionType CompletionType => CompletionType.Daily;

    private static GrandCompanySupplySettings Settings => Service.ConfigurationManager.CharacterConfiguration.GrandCompanySupply;
    public override GenericSettings GenericSettings => Settings;
    private AgentInterface* GrandCompanySupplyAgent => AgentModule.Instance()->GetAgentByInternalId(AgentId.GrandCompanySupply);

    public GrandCompanySupply()
    {
        Service.Framework.Update += OnFrameworkUpdate;
    }
        
    public override void Dispose()
    {
        Service.Framework.Update -= OnFrameworkUpdate;
    }

    private void OnFrameworkUpdate(Dalamud.Game.Framework framework)
    {
        if (!Settings.Enabled) return;
        if (GrandCompanySupplyAgent == null) return;
        if (!GrandCompanySupplyAgent->IsAgentActive()) return;

        var dataArray = new GrandCompanyDataArray(new nint(GrandCompanySupplyAgent));

        foreach (var tracked in Settings.TrackedSupply)
        {
            var dataRow = dataArray.GetRowForJob(tracked.ClassJobID);

            var turnInState = !dataRow.IsTurnInAvailable;

            if (turnInState != tracked.State)
            {
                tracked.State = turnInState;
                Service.ConfigurationManager.Save();
            }
        }
    }

    public override string GetStatusMessage() => $"{GetIncompleteJobs()} {Strings.Common_AllowancesAvailable}";
    protected override DateTime GetModuleReset() => Time.NextGrandCompanyReset();
    public override ModuleStatus GetModuleStatus() => GetIncompleteJobs() == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    public override void DoReset()
    {
        foreach (var trackedJobs in Settings.TrackedSupply)
        {
            trackedJobs.State = false;
        }
    }

    private static int GetIncompleteJobs()
    {
        return Settings.TrackedSupply
            .Where(r => r.Tracked)
            .Count(r => r.State == false);
    }

    private static string GetNextGrandCompanyReset()
    {
        var span = Time.NextGrandCompanyReset() - DateTime.UtcNow;

        return span.FormatTimespan(Settings.TimerSettings.TimerStyle.Value);
    }

    protected override void DrawConfiguration()
    {
        InfoBox.Instance.DrawGenericSettings(this);
            
        InfoBox.Instance
            .AddTitle(Strings.GrandCompany_Tracked)
            .AddList(Settings.TrackedSupply)
            .Draw();

        InfoBox.Instance.DrawNotificationOptions(this);
    }

    protected override void DrawStatus()
    {
        InfoBox.Instance.DrawGenericStatus(this);

        if (Settings.TrackedSupply.Any(row => row.Tracked))
        {
            InfoBox.Instance
                .AddTitle(Strings.Status_ModuleData)
                .BeginTable()
                .AddDataRows(Settings.TrackedSupply.Where(row => row.Tracked))
                .EndTable()
                .Draw();
        }
        else
        {
            InfoBox.Instance
                .AddTitle(Strings.Status_ModuleData, out var innerWidth)
                .AddStringCentered(Strings.GrandCompany_NothingTracked, innerWidth, Colors.Orange)
                .Draw();
        }
            
        InfoBox.Instance
            .AddTitle(Strings.Common_NextReset)
            .BeginTable()
            .BeginRow()
            .AddString(Strings.Common_NextReset)
            .AddString(GetNextGrandCompanyReset())
            .EndRow()
            .EndTable()
            .Draw();
            
        InfoBox.Instance.DrawSuppressionOption(this);
    }
}
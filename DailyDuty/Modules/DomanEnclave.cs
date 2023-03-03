using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Teleporter;

namespace DailyDuty.Modules;

public class DomanEnclaveSettings : GenericSettings
{
    public int DonatedThisWeek;
    public int WeeklyAllowance;

    public Setting<bool> EnableClickableLink = new(true);
}

public unsafe class DomanEnclave : Module
{
    public override ModuleName Name => ModuleName.DomanEnclave;
    public override CompletionType CompletionType => CompletionType.Weekly;
    
    private static DomanEnclaveSettings Settings => Service.ConfigurationManager.CharacterConfiguration.DomanEnclave;
    public override GenericSettings GenericSettings => Settings;
    public override DalamudLinkPayload DalamudLinkPayload => TeleportManager.Instance.GetPayload(TeleportLocation.DomanEnclave);
    public override bool LinkPayloadActive => Settings.EnableClickableLink;
    
    public DomanEnclave()
    {
        Service.Framework.Update += OnFrameworkUpdate;
    }

    public override void Dispose()
    {
        Service.Framework.Update -= OnFrameworkUpdate;
    }

    private void OnFrameworkUpdate(Framework framework)
    {
        if (!Service.ConfigurationManager.CharacterDataLoaded) return;
        if (!DataAvailable()) return;

        UpdateWeeklyAllowance();
        UpdateDonatedThisWeek();
    }

    public override string GetStatusMessage()
    {
        if (GetModuleStatus() == ModuleStatus.Unknown) return Strings.DomanEnclave_StatusUnknown;

        return $"{GetRemainingBudget()} {Strings.DomanEnclave_GilRemaining}";
    }

    public override void DoReset() => Settings.DonatedThisWeek = 0;

    public override ModuleStatus GetModuleStatus()
    {
        if (!ModuleInitialized()) return ModuleStatus.Unknown;

        return GetRemainingBudget() == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }

    private void UpdateWeeklyAllowance()
    {
        var allowance = GetWeeklyAllowance();

        if (Settings.WeeklyAllowance != allowance)
        {
            Settings.WeeklyAllowance = allowance;
            Service.ConfigurationManager.Save();
        }
    }
    private void UpdateDonatedThisWeek()
    {
        var donatedThisWeek = GetDonatedThisWeek();

        if (Settings.DonatedThisWeek != donatedThisWeek)
        {
            Settings.DonatedThisWeek = donatedThisWeek;
            Service.ConfigurationManager.Save();
        }
    }

    private static int GetRemainingBudget() => Settings.WeeklyAllowance - Settings.DonatedThisWeek;
    private ushort GetDonatedThisWeek() => ReconstructionBoxManager.Instance()->ReconstructionBoxData->Donated;
    private ushort GetWeeklyAllowance() => ReconstructionBoxManager.Instance()->ReconstructionBoxData->Allowance;
    private bool DataAvailable() => GetWeeklyAllowance() != 0;
    private static bool ModuleInitialized() => Settings.WeeklyAllowance != 0;

    protected override void DrawConfiguration()
    {
        InfoBox.Instance.DrawGenericSettings(this);

        InfoBox.Instance
            .AddTitle(Strings.Common_ClickableLink)
            .AddString(Strings.DomanEnclave_ClickableLink)
            .AddConfigCheckbox(Strings.Common_ClickableLink, Settings.EnableClickableLink)
            .Draw();
            
        InfoBox.Instance.DrawNotificationOptions(this);
    }

    protected override void DrawStatus()
    {
        var moduleStatus = GetModuleStatus();

        InfoBox.Instance.DrawGenericStatus(this);
            
        InfoBox.Instance
            .AddTitle(Strings.Status_ModuleData)
            .BeginTable()
            .BeginRow()
            .AddString(Strings.DomanEnclave_BudgetRemaining)
            .AddString(GetRemainingBudget().ToString(), GetRemainingBudget() == 0 ? Colors.Green : Colors.Orange)
            .EndRow()
            .BeginRow()
            .AddString(Strings.DomanEnclave_CurrentAllowance)
            .AddString(Settings.WeeklyAllowance.ToString())
            .EndRow()
            .EndTable()
            .Draw();
                
        if (moduleStatus == ModuleStatus.Unknown)
        {
            InfoBox.Instance
                .AddTitle(Strings.DomanEnclave_StatusUnknown, out var innerWidth)
                .AddStringCentered(Strings.DomanEnclave_StatusUnknown_Info, innerWidth, Colors.Orange)
                .Draw();
        }
            
        InfoBox.Instance.DrawSuppressionOption(this);
    }
}
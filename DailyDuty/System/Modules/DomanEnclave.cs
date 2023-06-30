using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.Models.ModuleData;
using DailyDuty.System.Localization;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace DailyDuty.System;

public unsafe class DomanEnclave : Module.WeeklyModule
{
    public override ModuleName ModuleName => ModuleName.DomanEnclave;

    public override IModuleDataBase ModuleData { get; protected set; } = new DomanEnclaveData();
    public override IModuleConfigBase ModuleConfig { get; protected set; } = new DomanEnclaveConfig();
    private DomanEnclaveConfig Config => ModuleConfig as DomanEnclaveConfig ?? new DomanEnclaveConfig();
    private DomanEnclaveData Data => ModuleData as DomanEnclaveData ?? new DomanEnclaveData();
    
    public override bool HasClickableLink => true;
    public override PayloadId ClickableLinkPayloadId => PayloadId.DomanEnclaveTeleport;

    public override void Update()
    {
        var reconstructionBoxData = ReconstructionBoxManager.Instance()->ReconstructionBoxData;

        if (reconstructionBoxData->Allowance is not 0)
        {
            Data.WeeklyAllowance = TryUpdateData(Data.WeeklyAllowance, reconstructionBoxData->Allowance);
            Data.DonatedThisWeek = TryUpdateData(Data.DonatedThisWeek, reconstructionBoxData->Donated);
            Data.RemainingAllowance = TryUpdateData(Data.RemainingAllowance, Data.WeeklyAllowance - Data.DonatedThisWeek);
        }
        
        base.Update();
    }

    public override void Reset()
    {
        Data.RemainingAllowance = Data.WeeklyAllowance;
        Data.DonatedThisWeek = 0;
        
        base.Reset();
    }

    protected override ModuleStatus GetModuleStatus()
    {
        if (Data.WeeklyAllowance is 0) return ModuleStatus.Unknown;

        return Data.RemainingAllowance is 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }
    
    protected override StatusMessage GetStatusMessage()
    {
        var message = GetModuleStatus() is ModuleStatus.Unknown ? Strings.StatusUnknown : $"{Data.RemainingAllowance} {Strings.GilRemaining}";

        return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.DomanEnclaveTeleport);
    }
}
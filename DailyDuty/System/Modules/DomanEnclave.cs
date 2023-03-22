using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Localization;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace DailyDuty.System;

public class DomanEnclaveConfig : ModuleConfigBase
{
    [ClickableLink("DomanEnclaveTeleport")] 
    public bool ClickableLink = true;
}

public class DomanEnclaveData : ModuleDataBase
{
    [DataDisplay("WeeklyAllowance")]
    public int WeeklyAllowance;
    
    [DataDisplay("DonatedThisWeek")]
    public int DonatedThisWeek;
    
    [DataDisplay("BudgetRemaining")]
    public int RemainingAllowance;
}

public unsafe class DomanEnclave : Module.WeeklyModule
{
    public override ModuleName ModuleName => ModuleName.DomanEnclave;

    public override ModuleDataBase ModuleData { get; protected set; } = new DomanEnclaveData();
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new DomanEnclaveConfig();
    private DomanEnclaveConfig Config => ModuleConfig as DomanEnclaveConfig ?? new DomanEnclaveConfig();
    private DomanEnclaveData Data => ModuleData as DomanEnclaveData ?? new DomanEnclaveData();

    public override void Update()
    {
        var reconstructionBoxData = ReconstructionBoxManager.Instance()->ReconstructionBoxData;

        if (reconstructionBoxData->Allowance != 0)
        {
            TryUpdateData(ref Data.WeeklyAllowance, reconstructionBoxData->Allowance);
            TryUpdateData(ref Data.DonatedThisWeek, reconstructionBoxData->Donated);
            TryUpdateData(ref Data.RemainingAllowance, Data.WeeklyAllowance - Data.DonatedThisWeek);
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

        return Data.RemainingAllowance == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }
    
    protected override StatusMessage GetStatusMessage()
    {
        var message = GetModuleStatus() == ModuleStatus.Unknown ? Strings.StatusUnknown : $"{Data.RemainingAllowance} {Strings.GilRemaining}";

        return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.DomanEnclaveTeleport);
    }
}
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
    public bool ClickableLink = false;
}

public class DomanEnclaveData : ModuleDataBase
{
    [DataDisplay("WeeklyAllowance")]
    public int WeeklyAllowance;
    
    [DataDisplay("DonatedThisWeek")]
    public int DonatedThisWeek;
    
    [DataDisplay("RemainingAllowance")]
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

        var weeklyAllowance = reconstructionBoxData->Allowance;
        var donatedThisWeek = reconstructionBoxData->Donated;

        if (weeklyAllowance != 0)
        {
            if (Data.WeeklyAllowance != weeklyAllowance)
            {
                Data.WeeklyAllowance = weeklyAllowance;
                SaveData();
            }

            if (Data.DonatedThisWeek != donatedThisWeek)
            {
                Data.DonatedThisWeek = donatedThisWeek;
                SaveData();
            }

            if (Data.RemainingAllowance != weeklyAllowance - donatedThisWeek)
            {
                Data.RemainingAllowance = weeklyAllowance - donatedThisWeek;
                SaveData();
            }
        }
    }

    protected override ModuleStatus GetModuleStatus()
    {
        if (Data.WeeklyAllowance is 0) return ModuleStatus.Unknown;

        return Data.RemainingAllowance == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }
    
    protected override StatusMessage GetStatusMessage()
    {
        var message = GetModuleStatus() == ModuleStatus.Unknown ? Strings.StatusUnknown : $"{Data.RemainingAllowance} {Strings.GilRemaining}";

        if (Config.ClickableLink)
        {
            return new LinkedStatusMessage
            {
                Message = message,
                Payload = PayloadId.DomanEnclaveTeleport,
            };
        }
        else
        {
            return new StatusMessage
            {
                Message = message
            };
        }
    }
}
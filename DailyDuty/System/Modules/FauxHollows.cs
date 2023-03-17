using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;

namespace DailyDuty.System;

public class FauxHollowsConfig : ModuleConfigBase
{
    [ClickableLink("IdyllshireTeleport")]
    public bool ClickableLink = true;

    [ConfigOption("IncludeRetelling")]
    public bool IncludeRetelling = true;
}

public class FauxHollowsData : ModuleDataBase
{
    [DataDisplay("FauxHollowsCompletions")]
    public int FauxHollowsCompletions;
}

public class FauxHollows : Module.DailyModule
{
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new FauxHollowsConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new FauxHollowsData();
    private FauxHollowsConfig Config => ModuleConfig as FauxHollowsConfig ?? new FauxHollowsConfig();
    private FauxHollowsData Data => ModuleData as FauxHollowsData ?? new FauxHollowsData();
    public override ModuleName ModuleName => ModuleName.FauxHollows;

    public override void AddonSetup(SetupAddonArgs addonInfo)
    {
        if (addonInfo.AddonName != "WeeklyPuzzle") return;

        Data.FauxHollowsCompletions += 1;
        DataChanged = true;
    }

    public override void Reset()
    {
        Data.FauxHollowsCompletions = 0;
        
        base.Reset();
    }

    protected override ModuleStatus GetModuleStatus()
    {
        if (Config.IncludeRetelling)
        {
            return Data.FauxHollowsCompletions == 2 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
        }

        return Data.FauxHollowsCompletions == 1 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }

    protected override StatusMessage GetStatusMessage()
    {
        var message = "Unreal Trial Available";

        return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.OpenPartyFinder);
    }
}
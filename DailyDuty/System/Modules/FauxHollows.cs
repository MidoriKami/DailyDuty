using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.Models.ModuleData;
using DailyDuty.System.Localization;

namespace DailyDuty.System;

public class FauxHollows : Module.WeeklyModule
{
    public override ModuleName ModuleName => ModuleName.FauxHollows;

    public override IModuleConfigBase ModuleConfig { get; protected set; } = new FauxHollowsConfig();
    public override IModuleDataBase ModuleData { get; protected set; } = new FauxHollowsData();
    private FauxHollowsConfig Config => ModuleConfig as FauxHollowsConfig ?? new FauxHollowsConfig();
    private FauxHollowsData Data => ModuleData as FauxHollowsData ?? new FauxHollowsData();

    public override bool HasClickableLink => true;
    public override PayloadId ClickableLinkPayloadId => PayloadId.IdyllshireTeleport;
    
    public override void AddonPreSetup(AddonArgs addonInfo)
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

    protected override ModuleStatus GetModuleStatus() => Config.IncludeRetelling switch
    {
        true when Data.FauxHollowsCompletions is 2 => ModuleStatus.Complete,
        false when Data.FauxHollowsCompletions is 1 => ModuleStatus.Complete,
        _ => ModuleStatus.Incomplete,
    };

    protected override StatusMessage GetStatusMessage() => 
        ConditionalStatusMessage.GetMessage(Config.ClickableLink, Strings.UnrealTrialAvailable, PayloadId.OpenPartyFinder);
}
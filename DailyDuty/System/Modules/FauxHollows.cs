using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.System.Localization;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.System;

public class FauxHollowsConfig : ModuleConfigBase
{
    [BoolConfigOption("IncludeRetelling", "ModuleConfiguration", 1)]
    public bool IncludeRetelling = true;
    
    [BoolDescriptionConfigOption("Enable", "ClickableLink", 2, "IdyllshireTeleport")] 
    public bool ClickableLink = true;
}

public class FauxHollowsData : ModuleDataBase
{
    [IntDisplay("FauxHollowsCompletions", "ModuleData", 1)]
    public int FauxHollowsCompletions;
}

public class FauxHollows : Module.WeeklyModule
{
    public override ModuleName ModuleName => ModuleName.FauxHollows;

    public override ModuleConfigBase ModuleConfig { get; protected set; } = new FauxHollowsConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new FauxHollowsData();
    private FauxHollowsConfig Config => ModuleConfig as FauxHollowsConfig ?? new FauxHollowsConfig();
    private FauxHollowsData Data => ModuleData as FauxHollowsData ?? new FauxHollowsData();

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
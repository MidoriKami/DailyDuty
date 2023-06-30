using DailyDuty.Abstracts;
using KamiLib.AutomaticUserInterface;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Models;

[Category("ModuleConfiguration", 1)]
public interface IDutyRouletteModuleConfiguration
{
    [BoolConfig("CompleteWhenTomeCapped", "CompleteWhenTomeCappedHelp")]
    public bool CompleteWhenCapped { get; set; }
}

[Category("ModuleConfiguration", 4)]
public interface IDutyRouletteClickableLinkConfiguration
{
    [BoolDescriptionConfig("Enable", "DutyRouletteOpenDutyFinder")]
    public bool ClickableLink { get; set; }
}

public class DutyRouletteConfig : ModuleTaskConfigBase<ContentRoulette>, IDutyRouletteModuleConfiguration, IDutyRouletteClickableLinkConfiguration
{
    public bool CompleteWhenCapped { get; set; } = false;
    public bool ClickableLink { get; set; } = true;
}
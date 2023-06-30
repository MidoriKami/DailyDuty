using DailyDuty.Abstracts;
using KamiLib.AutomaticUserInterface;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Models;

[Category("ClickableLink", 4)]
public interface IMaskedCarnivaleClickableLink
{
    [BoolDescriptionConfig("Enable", "UldahTeleport")] 
    public bool ClickableLink { get; set; }
}

public class MaskedCarnivaleConfig : ModuleTaskConfigBase<Addon>, IMaskedCarnivaleClickableLink
{
    public bool ClickableLink { get; set; } = true;
}

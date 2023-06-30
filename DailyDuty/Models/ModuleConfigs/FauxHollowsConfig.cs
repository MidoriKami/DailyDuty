using System.Numerics;
using DailyDuty.Abstracts;
using Dalamud.Game.Text;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

[Category("ClickableLink", 2)]
public interface IFauxHollowsClickableLink
{
    [BoolDescriptionConfig("Enable", "IdyllshireTeleport")] 
    public bool ClickableLink { get; set; }
}

[Category("ModuleConfiguration", 1)]
public interface IFauxHollowsModuleConfig
{
    [BoolConfig("IncludeRetelling")]
    public bool IncludeRetelling { get; set; }
}

public class FauxHollowsConfig : IModuleConfigBase, IFauxHollowsClickableLink, IFauxHollowsModuleConfig
{
    // IModuleEnable
    public bool ModuleEnabled { get; set; } = false;
    
    // INotificationOptions
    public bool OnLoginMessage { get; set; } = true;
    public bool OnZoneChangeMessage { get; set; } = true;
    public bool ResetMessage { get; set; } = false;
    
    // ITodoConfig
    public bool TodoEnabled { get; set; } = true;
    public bool UseCustomTodoLabel { get; set; } = false;
    public string CustomTodoLabel { get; set; } = string.Empty;
    public bool OverrideTextColor { get; set; } = false;
    public Vector4 TodoTextColor { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);
    public Vector4 TodoTextOutline { get; set; } = new(0.0f, 0.0f, 0.0f, 1.0f);
    public bool StyleChanged { get; set; } = true;
    
    // INotificationCustomization
    public bool UseCustomChannel { get; set; } = false;
    public XivChatType MessageChatChannel { get; set; } = Service.PluginInterface.GeneralChatType;
    public bool UseCustomStatusMessage { get; set; } = false;
    public string CustomStatusMessage { get; set; } = string.Empty;
    public bool UseCustomResetMessage { get; set; } = false;
    public string CustomResetMessage { get; set; } = string.Empty;
    
    // Suppression
    public bool Suppressed { get; set; }
    
    // Module Configuration
    public bool IncludeRetelling { get; set; } = true;
    
    // Clickable Link
    public bool ClickableLink { get; set; } = true;
}

using System.Numerics;
using DailyDuty.Abstracts;
using Dalamud.Game.Text;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

[Category("ClickableLink", 1)]
public interface IMiniCactpotClickableLink
{
    [BoolDescriptionConfig("Enable", "GoldSaucerTeleport")] 
    public bool ClickableLink { get; set; }
}

public class MiniCactpotConfig : IModuleConfigBase, IMiniCactpotClickableLink
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
    
    public bool ClickableLink { get; set; } = true;
}
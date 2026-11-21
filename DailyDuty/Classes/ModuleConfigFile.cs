using Dalamud.Game.Text;

namespace DailyDuty.Classes;

public abstract class ModuleConfigFile<T> : FileBase<T> where T : ModuleConfigFile<T>, new() {

    /// <summary>
    /// Gets whether status messages should be sent on login.
    /// </summary>
    public bool OnLoginMessage { get; set; } = true;

    /// <summary>
    /// Gets whether status messages should be sent when changing zones.
    /// </summary>
    public bool OnZoneChangeMessage { get; set; } = true;

    /// <summary>
    /// Gets whether reset messages should be sent when resetting modules.
    /// </summary>
    public bool ResetMessages { get; set; }

    /// <summary>
    /// Gets what chat channel to use for this module, defaults to Dalamud's set channel on initial creation.
    /// </summary>
    public XivChatType MessageChatChannel { get; set; } = DailyDuty.PluginInterface.GeneralChatType;

    /// <summary>
    /// Gets a custom status message string that the user can set.
    /// </summary>
    public string CustomStatusMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets a custom reset message string that the user can set.
    /// </summary>
    public string CustomResetMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets whether this module's status is being forced to complete.
    /// </summary>
    public bool Suppressed { get; set; }
}

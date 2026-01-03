using System.Linq;
using Dalamud.Game.Text;
using Newtonsoft.Json.Linq;

namespace DailyDuty.Features.ChallengeLog;

public static class Migration {
    public static Config Migrate(JObject data) => new() {
        EnableContentFinderWarning = data["EnableContentFinderWarning"]?.ToObject<bool>() ?? true,
        EnableWarningSound = data["EnableWarningSound"]?.ToObject<bool>() ?? true,
        WarningEntries = data["WarningEntries"]?.Where(token => token.Value<bool>("Enabled")).Select(token => token.Value<uint>("RowId")).ToHashSet() ?? [],
        TrackedEntries = data["TaskConfig"]?.Where(token => token.Value<bool>("Enabled")).Select(token => token.Value<uint>("RowId")).ToHashSet() ?? [],
        
        // Base Properties
        OnLoginMessage = data["OnLoginMessage"]?.ToObject<bool>() ?? true,
        OnZoneChangeMessage = data["OnZoneChangeMessage"]?.ToObject<bool>() ?? true,
        ResetMessage = data["ResetMessage"]?.ToObject<bool>() ?? true,
        MessageChatChannel = data["MessageChatChannel"]?.ToObject<XivChatType>() ?? Services.PluginInterface.GeneralChatType,
        CustomStatusMessage = data["CustomStatusMessage"]?.ToObject<string>() ?? string.Empty,
        CustomResetMessage = data["CustomResetMessage"]?.ToObject<string>() ?? string.Empty,
        Suppressed = data["Suppressed"]?.ToObject<bool>() ?? false,
    };
}

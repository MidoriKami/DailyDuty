using System.Linq;
using Dalamud.Game.Text;
using Newtonsoft.Json.Linq;

namespace DailyDuty.Features.RaidsNormal;

public static class RaidsNormalMigration {
    public static RaidsNormalConfig Migrate(JObject data) => new() {
        TrackedTasks = data["TaskConfig"]?.ToDictionary(token => token.Value<uint>("RowId"), token => token.Value<bool>("Enabled")) ?? [],
        
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

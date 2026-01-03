using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Text;
using Newtonsoft.Json.Linq;

namespace DailyDuty.Features.GrandCompanyProvision;

public static class Migration {
    public static Config Migrate(JObject data) => new() {
        TrackedClasses = data["TaskConfig"]?.ToDictionary(token => token.Value<uint>("RowId"), token => token.Value<bool>("Enabled")) ?? new Dictionary<uint, bool> {
            [16] = true,
            [17] = true,
            [18] = true,
        },
        
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

using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Text;
using Newtonsoft.Json.Linq;

namespace DailyDuty.Features.GrandCompanySupply;

public static class Migration {
    public static Config Migrate(JObject data) => new() {
        TrackedClasses = data["TaskConfig"]?.ToDictionary(token => token.Value<uint>("RowId"), token => token.Value<bool>("Enabled")) ?? new Dictionary<uint, bool> {
            [8] = true,
            [9] = true,
            [10] = true,
            [11] = true,
            [12] = true,
            [13] = true,
            [14] = true,
            [15] = true,
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

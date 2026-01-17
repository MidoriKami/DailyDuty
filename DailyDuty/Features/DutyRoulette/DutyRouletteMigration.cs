using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Newtonsoft.Json.Linq;

namespace DailyDuty.Features.DutyRoulette;

public static class DutyRouletteMigration {
    public static DutyRouletteConfig Migrate(JObject data) => new() {
        CompleteWhenCapped = data["CompleteWhenCapped"]?.ToObject<bool>() ?? true,
        ColorContentFinder = data["ColorContentFinder"]?.ToObject<bool>() ?? true,
        CompleteColor = data["CompleteColor"]?.ToObject<Vector4>() ?? KnownColor.LimeGreen.Vector(),
        IncompleteColor = data["IncompleteColor"]?.ToObject<Vector4>() ?? KnownColor.OrangeRed.Vector(),
        TrackedRoulettes = data["TaskConfig"]?.Where(token => token.Value<bool>("Enabled")).Select(token => token.Value<uint>("RowId")).ToList() ?? [],
        
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

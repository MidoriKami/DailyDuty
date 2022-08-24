using Dalamud.Game.Text.SeStringHandling.Payloads;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Configuration.Components;
internal enum TeleportLocation
{
    GoldSaucer,
    Idyllshire,
    DomanEnclave
}

internal record TeleportInfo(uint CommandID, TeleportLocation Target, Aetheryte Aetherite);

internal record TeleportLinkPayloads(TeleportLocation Location, DalamudLinkPayload Payload);

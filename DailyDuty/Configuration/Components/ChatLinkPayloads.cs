using DailyDuty.Configuration.Enums;
using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace DailyDuty.Configuration.Components;

internal record ChatLinkPayloads(TeleportLocation Location, DalamudLinkPayload Payload);

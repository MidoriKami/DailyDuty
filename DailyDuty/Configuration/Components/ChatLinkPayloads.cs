using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace DailyDuty.Configuration.Components;

public enum ChatPayloads
{
    OpenDutyFinder,
    OpenWondrousTails
}

internal record ChatLinkPayload(uint CommandID, ChatPayloads Type, DalamudLinkPayload Payload);
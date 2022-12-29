using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace DailyDuty.DataModels;

public enum ChatPayloads
{
    DutyRouletteDutyFinder,
    OpenWondrousTails,
    NormalRaidsDutyFinder,
    AllianceRaidsDutyFinder,
    OpenPartyFinder
}

internal record ChatLinkPayload(uint CommandID, ChatPayloads Type, DalamudLinkPayload Payload);
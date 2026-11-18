using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace DailyDuty.Classes;

public static class ChatLinkPayloads {
    public static unsafe ChatLinkPayload TeleportToGrandCompany() => new(() => {
        uint? aetheryteId = IPlayerState.Get().GrandCompany.RowId switch {
            0 => 8, // Maelstorm - Limsa
            1 => 2, // Twin Adder - Gridania
            2 => 9, // Immortal Flames - Ul'dah
            _ => null,
        };

        if (aetheryteId is { } realAetheryteId) {
            Telepo.Instance()->Teleport(realAetheryteId, 0);
        }
    });

    public static unsafe ChatLinkPayload TeleportToGoldSaucer() => new(() =>
        Telepo.Instance()->Teleport(62, 0));

    public static unsafe ChatLinkPayload TeleportToDomanEnclave() => new(() =>
        Telepo.Instance()->Teleport(127, 0));

    public static unsafe ChatLinkPayload TeleportToIdyllshire() => new(() =>
        Telepo.Instance()->Teleport(75, 0));

    public static unsafe ChatLinkPayload OpenChallengeLog() => new(() =>
        UIModule.Instance()->ExecuteMainCommand(60));

    public static unsafe ChatLinkPayload OpenDutyRoulette() => new(() =>
        UIModule.Instance()->ExecuteMainCommand(2));
}

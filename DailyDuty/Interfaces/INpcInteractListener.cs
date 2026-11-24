using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace DailyDuty.Interfaces;

/// <summary>
/// Interface for listening for network messages containing NPC interact data.
/// </summary>
public interface INpcInteractListener {
    unsafe void OnNpcInteract(EventFramework* thisPtr, GameObject* gameObject, EventId eventId, short scene, ulong sceneFlags, uint* sceneData, byte sceneDataCount);
}

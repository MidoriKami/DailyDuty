using System;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using KamiLib.Classes;

namespace DailyDuty.Classes;

public unsafe class GoldSaucerEventArgs(int* data, byte eventId) : EventArgs {
    public int* Data = data;
    public byte EventId = eventId;
}

public unsafe class GoldSaucerMessageController : IDisposable {
    private delegate void* GoldSaucerUpdateDelegate(void* a1, byte* a2, uint a3, ushort a4, void* a5, int* data, byte eventId);

    // Note this seems to be a generic content director interact with npc method.
    [Signature("E8 ?? ?? ?? ?? EB 07 48 8D 9F", DetourName = nameof(ProcessNetworkPacket))] 
    private readonly Hook<GoldSaucerUpdateDelegate>? goldSaucerUpdateHook = null;
    
    public event Action<GoldSaucerEventArgs>? GoldSaucerUpdate;
    
    public GoldSaucerMessageController() {
        Service.Hooker.InitializeFromAttributes(this);
        goldSaucerUpdateHook?.Enable();
    }

    public void Dispose() {
        goldSaucerUpdateHook?.Dispose();
    }
    
    private void* ProcessNetworkPacket(void* a1, byte* a2, uint a3, ushort a4, void* a5, int* data, byte eventId) {
        HookSafety.ExecuteSafe(() => {
            // Service.Log.Debug("[GoldSaucerMessage]\n" +
            //                 $"A1: {new nint(a1):X8}\n" +
            //                 $"A2: {new nint(a2):X8}\n" +
            //                 $"A3: {a3}\n" +
            //                 $"A4: {a4}\n" +
            //                 $"A5: {new nint(a5):X8}\n" +
            //                 $"A6: {new nint(data):X8}\n" +
            //                 $"A7: {eventID}");
            
            GoldSaucerUpdate?.Invoke(new GoldSaucerEventArgs(data, eventId));
        }, Service.Log);

        return goldSaucerUpdateHook!.Original(a1, a2, a3, a4, a5, data, eventId);
    }
}
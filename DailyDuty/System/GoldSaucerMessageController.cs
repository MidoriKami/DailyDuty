using System;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using KamiLib.Hooking;

namespace DailyDuty.System;

public unsafe class GoldSaucerEventArgs : EventArgs
{
    public GoldSaucerEventArgs(int* data, byte eventID)
    {
        Data = data;
        EventId = eventID;
    }
    
    public int* Data;
    public byte EventId;
}

public unsafe class GoldSaucerMessageController : IDisposable
{
    private delegate void* GoldSaucerUpdateDelegate(void* a1, byte* a2, uint a3, ushort a4, void* a5, int* data, byte eventID);

    [Signature("E8 ?? ?? ?? ?? 80 A7 ?? ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? 44 89 AF", DetourName = nameof(ProcessNetworkPacket))]
    private readonly Hook<GoldSaucerUpdateDelegate>? goldSaucerUpdateHook = null;
    
    public event EventHandler<GoldSaucerEventArgs>? GoldSaucerUpdate;
    
    public GoldSaucerMessageController()
    {
        Service.Hooker.InitializeFromAttributes(this);
        goldSaucerUpdateHook?.Enable();
    }

    public void Dispose()
    {
        goldSaucerUpdateHook?.Dispose();
    }
    
    private void* ProcessNetworkPacket(void* a1, byte* a2, uint a3, ushort a4, void* a5, int* data, byte eventID)
    {
        Safety.ExecuteSafe(() =>
        {
            // Service.Log.Debug("[GoldSaucerMessage]\n" +
            //                 $"A1: {new nint(a1):X8}\n" +
            //                 $"A2: {new nint(a2):X8}\n" +
            //                 $"A3: {a3}\n" +
            //                 $"A4: {a4}\n" +
            //                 $"A5: {new nint(a5):X8}\n" +
            //                 $"A6: {new nint(data):X8}\n" +
            //                 $"A7: {eventID}");
            
            GoldSaucerUpdate?.Invoke(this, new GoldSaucerEventArgs(data, eventID));
        });

        return goldSaucerUpdateHook!.Original(a1, a2, a3, a4, a5, data, eventID);
    }
}
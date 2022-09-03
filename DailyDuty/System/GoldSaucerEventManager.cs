using System;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;

namespace DailyDuty.System;

internal unsafe class GoldSaucerEventArgs : EventArgs
{
    public GoldSaucerEventArgs(int* data, byte eventID)
    {
        Data = data;
        EventID = eventID;
    }

    public int* Data;
    public byte EventID;
}

internal unsafe class GoldSaucerEventManager : IDisposable
{
    private delegate void* GoldSaucerUpdateDelegate(void* a1, byte* a2, uint a3, ushort a4, void* a5, int* data, byte eventID);

    [Signature("E8 ?? ?? ?? ?? 80 A7 ?? ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? 44 89 AF", DetourName = nameof(GoldSaucerUpdate))]
    private readonly Hook<GoldSaucerUpdateDelegate>? goldSaucerUpdateHook = null;

    public event EventHandler<GoldSaucerEventArgs>? OnGoldSaucerUpdate;

    public GoldSaucerEventManager()
    {
        SignatureHelper.Initialise(this);

        goldSaucerUpdateHook?.Enable();
    }

    public void Dispose()
    {
        goldSaucerUpdateHook?.Dispose();
    }

    private void* GoldSaucerUpdate(void* a1, byte* a2, uint a3, ushort a4, void* a5, int* data, byte eventID)
    {
        try
        {
            OnGoldSaucerUpdate?.Invoke(this, new GoldSaucerEventArgs(data, eventID));
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Unable to get data from Gold Saucer Update");
        }

        return goldSaucerUpdateHook!.Original(a1, a2, a3, a4, a5, data, eventID);
    }
}
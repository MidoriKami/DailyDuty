using System;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;

namespace DailyDuty.Addons;

internal unsafe class GoldSaucerEventArgs : EventArgs
{
    public GoldSaucerEventArgs(void* a1, byte* a2, uint a3, ushort a4, void* a5, int* data, byte eventID)
    {
        Data = data;
        EventID = eventID;
        A1 = a1;
        A2 = a2;
        A3 = a3;
        A4 = a4;
        A5 = a5;
    }

    public void* A1;
    public byte* A2;
    public uint A3;
    public ushort A4;
    public void* A5;
    public int* Data;
    public byte EventID;

    public void Print()
    {
        PluginLog.Verbose($"A1: {(IntPtr)A1:X8}");
        PluginLog.Verbose($"A2: {(IntPtr)A2:X8}");
        PluginLog.Verbose($"A3: {A3}");
        PluginLog.Verbose($"A4: {A4}");
        PluginLog.Verbose($"A5: {(IntPtr)A5:X8}");
        PluginLog.Verbose($"Data: {(IntPtr)Data:X8}");
        PluginLog.Verbose($"EventID: {EventID}");
    }
}

internal unsafe class GoldSaucerAddon : IDisposable
{
    private delegate void* GoldSaucerUpdateDelegate(void* a1, byte* a2, uint a3, ushort a4, void* a5, int* data, byte eventID);

    [Signature("E8 ?? ?? ?? ?? 80 A7 ?? ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? 44 89 AF", DetourName = nameof(ProcessNetworkPacket))]
    private readonly Hook<GoldSaucerUpdateDelegate>? goldSaucerUpdateHook = null;

    public event EventHandler<GoldSaucerEventArgs>? OnGoldSaucerUpdate;

    public GoldSaucerAddon()
    {
        SignatureHelper.Initialise(this);

        goldSaucerUpdateHook?.Enable();
    }

    public void Dispose()
    {
        goldSaucerUpdateHook?.Dispose();
    }

    private void* ProcessNetworkPacket(void* a1, byte* a2, uint a3, ushort a4, void* a5, int* data, byte eventID)
    {
        try
        {
            OnGoldSaucerUpdate?.Invoke(this, new GoldSaucerEventArgs(a1, a2, a3, a4, a5, data, eventID));
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Unable to get data from Gold Saucer Update");
        }

        return goldSaucerUpdateHook!.Original(a1, a2, a3, a4, a5, data, eventID);
    }
}
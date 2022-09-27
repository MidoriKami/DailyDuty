using DailyDuty.Configuration;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using DailyDuty.Utilities;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace DailyDuty.System;

// Taken from XivCommon to prevent dependency
// User input is ***never*** sent by DailyDuty, only pre-formatted commands
[StructLayout(LayoutKind.Explicit)]
[SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
internal readonly struct ChatPayload : IDisposable 
{
    [FieldOffset(0)]
    private readonly IntPtr textPtr;

    [FieldOffset(16)]
    private readonly ulong textLen;

    [FieldOffset(8)]
    private readonly ulong unk1;

    [FieldOffset(24)]
    private readonly ulong unk2;

    internal ChatPayload(byte[] stringBytes) {
        textPtr = Marshal.AllocHGlobal(stringBytes.Length + 30);
        Marshal.Copy(stringBytes, 0, textPtr, stringBytes.Length);
        Marshal.WriteByte(textPtr + stringBytes.Length, 0);

        textLen = (ulong) (stringBytes.Length + 1);

        unk1 = 64;
        unk2 = 0;
    }

    public void Dispose() {
        Marshal.FreeHGlobal(textPtr);
    }
}

internal class ChatManager : IDisposable
{
    public event EventHandler? OnLoginMessage;
    public event EventHandler? OnZoneChangeMessage;

    private delegate void ProcessChatBoxDelegate(IntPtr uiModule, IntPtr message, IntPtr unused, byte a4);

    [Signature("48 89 5C 24 ?? 57 48 83 EC 20 48 8B FA 48 8B D9 45 84 C9")]
    private readonly ProcessChatBoxDelegate? processChatBox = null!;

    private readonly Stopwatch stopwatch = new();

    public ChatManager()
    {
        SignatureHelper.Initialise(this);

        Service.ClientState.TerritoryChanged += OnTerritoryChanged;
        Service.ConfigurationManager.OnCharacterDataAvailable += OnCharacterDataLoaded;
        
        foreach (var module in Service.ModuleManager.GetLogicComponents())
        {
            OnLoginMessage += module.OnLoginMessage;
            OnZoneChangeMessage += module.OnZoneChangeMessage;
        }
    }

    public void Dispose()
    {
        Service.ClientState.TerritoryChanged -= OnTerritoryChanged;
        Service.ConfigurationManager.OnCharacterDataAvailable -= OnCharacterDataLoaded;
    }

    private void OnCharacterDataLoaded(object? sender, CharacterConfiguration e)
    {
        ResetManager.ResetModules();

        OnLoginMessage?.Invoke(this, EventArgs.Empty);
        stopwatch.Restart();
    }
    
    private void OnTerritoryChanged(object? sender, ushort e)
    {
        if (stopwatch.Elapsed.Minutes >= 5 || stopwatch.IsRunning == false)
        {
            stopwatch.Restart();
            OnZoneChangeMessage?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            var lockoutRemaining = TimeSpan.FromMinutes(5) - stopwatch.Elapsed;
            Log.Verbose($"Zone Change Messages Suppressed, '{lockoutRemaining}' Remaining");
        }
    }

    public void SendMessages() => OnZoneChangeMessage?.Invoke(this, EventArgs.Empty);


    // User input is NEVER sent via this function call
    // Forces a forward slash, data should NEVER be sent to the server
    public unsafe void SendCommandUnsafe(string command)
    {
        if (processChatBox == null) {
            throw new InvalidOperationException("Could not find signature for chat sending");
        }

        var uiModule = (IntPtr) Framework.Instance()->GetUiModule();

        using var payload = new ChatPayload(Encoding.UTF8.GetBytes($"/{command}"));
        var mem1 = Marshal.AllocHGlobal(400);
        Marshal.StructureToPtr(payload, mem1, false);

        processChatBox(uiModule, mem1, IntPtr.Zero, 0);

        Marshal.FreeHGlobal(mem1);
    }
}
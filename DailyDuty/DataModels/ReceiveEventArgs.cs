using System;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.DataModels;

public unsafe class ReceiveEventArgs : EventArgs
{
    public ReceiveEventArgs(AgentInterface* agentInterface, void* rawData, AtkValue* eventArgs, uint eventArgsCount, ulong senderID)
    {
        AgentInterface = agentInterface;
        RawData = rawData;
        EventArgs = eventArgs;
        EventArgsCount = eventArgsCount;
        SenderID = senderID;
    }

    public AgentInterface* AgentInterface;
    public void* RawData;
    public AtkValue* EventArgs;
    public uint EventArgsCount;
    public ulong SenderID;

    public void PrintData()
    {
        PluginLog.Verbose("ReceiveEvent Argument Printout --------------");
        PluginLog.Verbose($"AgentInterface: {(IntPtr)AgentInterface:X8}");
        PluginLog.Verbose($"RawData: {(IntPtr)RawData:X8}");
        PluginLog.Verbose($"EventArgs: {(IntPtr)EventArgs:X8}");
        PluginLog.Verbose($"EventArgsCount: {EventArgsCount}");
        PluginLog.Verbose($"SenderID: {SenderID}");

        for (var i = 0; i < EventArgsCount; i++)
        {
            PluginLog.Verbose($"[{i}] {EventArgs[i].Int}, {EventArgs[i].Type}");
        }

        PluginLog.Verbose("End -----------------------------------------");
    }
}
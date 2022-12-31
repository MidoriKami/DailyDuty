using System;
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
}
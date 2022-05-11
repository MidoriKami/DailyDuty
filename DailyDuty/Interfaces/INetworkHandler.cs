using System;
using Dalamud.Game.Network;

namespace DailyDuty.Interfaces
{
    internal interface INetworkHandler
    {
        void OnNetworkMessage(IntPtr dataPtr, ushort opcode, uint sourceActorID, uint targetActorID, NetworkMessageDirection direction);
    }
}

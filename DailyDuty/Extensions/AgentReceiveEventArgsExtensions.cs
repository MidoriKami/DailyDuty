using System;
using Dalamud.Game.Agent.AgentArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Extensions;

public static unsafe class AgentReceiveEventArgsExtensions {
    extension(AgentReceiveEventArgs eventArgs) {
        public Span<AtkValue> AtkValueSpan => new Span<AtkValue>((AtkValue*) eventArgs.AtkValues, (int) eventArgs.ValueCount);
    }
}

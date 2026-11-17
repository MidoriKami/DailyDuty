using System;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Agent.AgentArgTypes;
using Dalamud.Game.NativeWrapper;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Extensions;

public static class AtkValueExtensions {
    extension(AgentReceiveEventArgs args) {
        public unsafe Span<AtkValuePtr> AtkValueSpan
            => new((AtkValuePtr*)args.AtkValues, (int)args.ValueCount);
    }

    extension(AddonArgs addon) {
        public Span<AtkValuePtr> AtkValueSpan
            => addon.Addon.AtkValueSpan;
    }

    extension(AtkUnitBasePtr addon) {
        public unsafe Span<AtkValuePtr> AtkValueSpan
            => new((AtkValuePtr*)((AtkUnitBase*)addon.Address)->AtkValues, addon.AtkValuesCount);
    }
}

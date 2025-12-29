using System;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Features.MiniCactpot;

public unsafe class MiniCactpot : Module<ConfigBase, Data> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Mini Cactpot",
        FileName = "MiniCactpot",
        Type = ModuleType.Daily,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "DoH", "DoL", "Exp" ],
    };
    private Hook<EventFramework.Delegates.ProcessEventPlay>? frameworkEventHook;

    public override DataNodeBase DataNode => new DataNode(this);

    protected override void OnEnable() {
         Services.AddonLifecycle.RegisterListener(AddonEvent.PreSetup, "LotteryDaily", LotteryDailyPreSetup);
        
        frameworkEventHook = Services.Hooker.HookFromAddress<EventFramework.Delegates.ProcessEventPlay>(EventFramework.MemberFunctionPointers.ProcessEventPlay, OnFrameworkEvent);
        frameworkEventHook?.Enable();
    }

    protected override void OnDisable() {
        frameworkEventHook?.Dispose();
        frameworkEventHook = null;
        
         Services.AddonLifecycle.UnregisterListener(LotteryDailyPreSetup);
    }

    protected override ReadOnlySeString GetStatusMessage()
        => $"{ModuleData.AllowancesRemaining} Attempts Remaining";

    public override DateTime GetNextResetDateTime()
        => Time.NextDailyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(1);

    protected override CompletionStatus GetCompletionStatus()
        => ModuleData.AllowancesRemaining is 0 ? CompletionStatus.Complete : CompletionStatus.Incomplete;
    
    private void LotteryDailyPreSetup(AddonEvent eventType, AddonArgs addonInfo) {
        ModuleData.AllowancesRemaining -= 1;
        ModuleData.MarkDirty();
    }
    
    private void OnFrameworkEvent(EventFramework* thisPtr, GameObject* gameObject, EventId eventId, short scene, ulong sceneFlags, uint* sceneData, byte sceneDataCount) {
        frameworkEventHook!.Original(thisPtr, gameObject, eventId, scene, sceneFlags, sceneData, sceneDataCount);
        
        if (gameObject->BaseId is not 1010445) return;
        
        // Services.PluginLog.Debug($"[FrameworkEvent] Scene: {scene}, Flags: {sceneFlags}, EventId: {eventId.ContentId}-{eventId.EntryId}-{eventId.EntryId}");
        // Services.PluginLog.Debug($"[FrameworkEvent] DataCount: {sceneDataCount} Data: {string.Join(", ", Enumerable.Range(0, sceneDataCount).Select(index => sceneData[index]))}");
        
        if (sceneDataCount is 5) {
            ModuleData.AllowancesRemaining = (int) sceneData[4];
        }
        else {
            ModuleData.AllowancesRemaining = 0;
        }

        ModuleData.MarkDirty();
    }
}

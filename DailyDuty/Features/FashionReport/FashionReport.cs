using System;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Features.FashionReport;

public unsafe class FashionReport : Module<FashionReportConfig, FashionReportData> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Fashion Report",
        FileName = "FashionReport",
        Type = ModuleType.Special,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Gold Saucer", "Gold", "Saucer", "MGP" ],
        MessageClickAction = PayloadId.GoldSaucerTeleport, 
    };

    private Hook<EventFramework.Delegates.ProcessEventPlay>? goldSaucerUpdateHook;
    public override DataNodeBase DataNode => new DataNode(this);
    public override ConfigNodeBase ConfigNode => new ConfigNode(this);

    protected override void OnEnable() {
        goldSaucerUpdateHook = Services.Hooker.HookFromAddress<EventFramework.Delegates.ProcessEventPlay>(EventFramework.MemberFunctionPointers.ProcessEventPlay, OnFrameworkEvent);
        goldSaucerUpdateHook.Enable();
    }

    protected override void OnDisable() {
        goldSaucerUpdateHook?.Dispose();
        goldSaucerUpdateHook = null;
    }

    protected override ReadOnlySeString GetStatusMessage() => ModuleConfig.CompletionMode switch {
        FashionReportMode.All => $"{ModuleData.AllowancesRemaining} Allowances Available",
        FashionReportMode.Single when ModuleData.AllowancesRemaining is 4 => $"{ModuleData.AllowancesRemaining} Allowances Available",
        FashionReportMode.Plus80 when ModuleData.HighestWeeklyScore <= 80 => $"{ModuleData.HighestWeeklyScore} Highest Score",
        _ => throw new ArgumentOutOfRangeException(),
    };

    public override DateTime GetNextResetDateTime() 
        => Time.NextFashionReportReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(7);

    public override void Reset() {
        ModuleData.AllowancesRemaining = 4;
        ModuleData.HighestWeeklyScore = 0;
    }

    protected override CompletionStatus GetCompletionStatus() => ModuleConfig.CompletionMode switch {
        _ when !IsFashionReportAvailable => CompletionStatus.Unavailable,
        FashionReportMode.Single when ModuleData.AllowancesRemaining < 4 => CompletionStatus.Complete,
        FashionReportMode.All when ModuleData.AllowancesRemaining is 0 => CompletionStatus.Complete,
        FashionReportMode.Plus80 when ModuleData.HighestWeeklyScore >= 80 => CompletionStatus.Complete,
        _ => CompletionStatus.Incomplete,
    };
    
    private void OnFrameworkEvent(EventFramework* thisPtr, GameObject* gameObject, EventId eventId, short scene, ulong sceneFlags, uint* sceneData, byte sceneDataCount) {
        goldSaucerUpdateHook!.Original(thisPtr, gameObject, eventId, scene, sceneFlags, sceneData, sceneDataCount);
        
        if (gameObject->BaseId is not 1025176) return;
        
        // Services.PluginLog.Debug($"[FrameworkEvent] Scene: {scene}, Flags: {sceneFlags}, EventId: {eventId.ContentId}-{eventId.EntryId}-{eventId.EntryId}");
        // Services.PluginLog.Debug($"[FrameworkEvent] DataCount: {sceneDataCount} Data: {string.Join(", ", Enumerable.Range(0, sceneDataCount).Select(index => sceneData[index]))}");

        switch (scene) {
            case 1:
                ModuleData.AllowancesRemaining = (int) sceneData[1];
                ModuleData.HighestWeeklyScore = (int) sceneData[0];
                ModuleData.SavePending = true;
                break;
            
            case 2:
                ModuleData.HighestWeeklyScore = Math.Max((int) sceneData[0], ModuleData.HighestWeeklyScore);
                ModuleData.SavePending = true;
                break;
            
            case 5:
                ModuleData.AllowancesRemaining = (int) sceneData[0];
                ModuleData.SavePending = true;
                break;
        }
    }

    private static bool IsFashionReportAvailable 
        => DateTime.UtcNow > Time.NextWeeklyReset().AddDays(-4) && DateTime.UtcNow < Time.NextWeeklyReset();
}

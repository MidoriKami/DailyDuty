using System;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace DailyDuty.Features.WondrousTails;

public unsafe class DutyController : IDisposable {
    private readonly WondrousTails module;

    public DutyController(WondrousTails module) {
        this.module = module;
        
        Services.DutyState.DutyStarted += OnDutyStarted;
        Services.DutyState.DutyCompleted += OnDutyCompleted;

        if (Services.DutyState.IsDutyStarted) {
            OnDutyStarted(null, Services.ClientState.TerritoryType);
        }
    }

    public void Dispose() {
        Services.DutyState.DutyStarted -= OnDutyStarted;
        Services.DutyState.DutyCompleted -= OnDutyCompleted;
    }
    
    private void OnDutyStarted(object? sender, ushort e) {
    	if (!module.ModuleConfig.InstanceNotifications) return;
        if (!module.PlayerHasBook || module.IsBookExpired) return;
    	if (module.ModuleStatus is CompletionStatus.Complete) return;

    	var taskState = GetStatusForTerritory(e);

    	switch (taskState) {
    		case PlayerState.WeeklyBingoTaskStatus.Claimed when module is { PlacedStickers: > 0, SecondChancePoints: > 0 }:
                PrintMessage($"{module.SecondChancePoints} Rerolls Available");
    			break;
            
    		case PlayerState.WeeklyBingoTaskStatus.Claimable:
    			PrintMessage("Sticker is already available for this duty, be sure to claim it!");
    			break;
            
    		case PlayerState.WeeklyBingoTaskStatus.Open:
    			PrintMessage("Completing this duty will reward you with a sticker");
    			break;
    	}
    }

    private void OnDutyCompleted(object? sender, ushort e) {
        if (!module.ModuleConfig.InstanceNotifications) return;
        if (!module.PlayerHasBook || module.IsBookExpired) return;
        if (module.ModuleStatus is CompletionStatus.Complete) return;

        var taskState = GetStatusForTerritory(e);

    	switch (taskState) {
    		case PlayerState.WeeklyBingoTaskStatus.Claimable:
    		case PlayerState.WeeklyBingoTaskStatus.Open:
    			PrintMessage("Stickers Claimable");
    			break;
    	}
    }
    
    private static PlayerState.WeeklyBingoTaskStatus? GetStatusForTerritory(uint territory) {
    	foreach (var index in Enumerable.Range(0, 16)) {
    		var territoriesForSlot = Services.DataManager.GetTerritoriesForOrderData(PlayerState.Instance()->WeeklyBingoOrderData[index]);

    		if (territoriesForSlot.Any(terr => terr.RowId == territory)) {
    			return PlayerState.Instance()->GetWeeklyBingoTaskStatus(index);
    		}
    	}

    	return null;
    }
    
    private void PrintMessage(string message)
        => Services.ChatGui.PrintPayloadMessage(
            module.ModuleConfig.MessageChatChannel, 
            PayloadId.OpenWondrousTailsBook, 
            "WondrousTails", 
            message);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.Models.ModuleData;
using DailyDuty.System.Localization;
using Dalamud;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public unsafe class WondrousTails : Module.WeeklyModule
{
    public override ModuleName ModuleName => ModuleName.WondrousTails;

    public override IModuleConfigBase ModuleConfig { get; protected set; } = new WondrousTailsConfig();
    public override IModuleDataBase ModuleData { get; protected set; } = new WondrousTailsData();
    private WondrousTailsConfig Config => ModuleConfig as WondrousTailsConfig ?? new WondrousTailsConfig();
    private WondrousTailsData Data => ModuleData as WondrousTailsData ?? new WondrousTailsData();
    
    public override bool HasClickableLink => true;
    public override PayloadId ClickableLinkPayloadId => Data.NewBookAvailable ? PayloadId.IdyllshireTeleport : PayloadId.OpenWondrousTailsBook;

    public WondrousTails()
    {
        Service.DutyState.DutyStarted += OnDutyStarted;
        Service.DutyState.DutyCompleted += OnDutyCompleted;
    }

    public override void Dispose()
    {
        Service.DutyState.DutyStarted -= OnDutyStarted;
        Service.DutyState.DutyCompleted -= OnDutyCompleted;
    }

    public override void Update()
    {
        Data.PlacedStickers = TryUpdateData(Data.PlacedStickers, PlayerState.Instance()->WeeklyBingoNumPlacedStickers);
        Data.SecondChance = TryUpdateData(Data.SecondChance, PlayerState.Instance()->WeeklyBingoNumSecondChancePoints);
        Data.Deadline = TryUpdateData(Data.Deadline, DateTimeOffset.FromUnixTimeSeconds(PlayerState.Instance()->GetWeeklyBingoExpireUnixTimestamp()).DateTime);
        Data.PlayerHasBook = TryUpdateData(Data.PlayerHasBook, PlayerState.Instance()->HasWeeklyBingoJournal);
        Data.NewBookAvailable = TryUpdateData(Data.NewBookAvailable, DateTime.UtcNow > Data.Deadline - TimeSpan.FromDays(7));
        Data.BookExpired = TryUpdateData(Data.BookExpired, PlayerState.Instance()->IsWeeklyBingoExpired());
        
        var timeRemaining = Data.Deadline - DateTime.UtcNow;
        Data.TimeRemaining = timeRemaining > TimeSpan.Zero ? timeRemaining : TimeSpan.Zero;
        Data.DistanceToKhloe = 0.0f;
        var lastNearKhloe = Data.CloseToKhloe;
        var lastCastingTeleport = Data.CastingTeleport;
        Data.CloseToKhloe = false;
        Data.CastingTeleport = false;
        
        const int idyllshireTerritoryType = 478;
        const uint khloeAliapohDataId = 1017653;
        if (Service.ClientState.TerritoryType is idyllshireTerritoryType && Config.ModuleEnabled)
        {
            var khloe = Service.ObjectTable.FirstOrDefault(obj => obj.DataId is khloeAliapohDataId);

            if (khloe is not null && Service.ClientState.LocalPlayer is { Position: var playerPosition })
            {
                Data.DistanceToKhloe = Vector3.Distance(playerPosition, khloe.Position);
                Data.CloseToKhloe = Data.DistanceToKhloe < 10.0f;
                Data.CastingTeleport = Service.ClientState.LocalPlayer is { IsCasting: true, CastActionId: 5 or 6 };

                var noLongerNearKhloe = lastNearKhloe && !Data.CloseToKhloe;
                var startedTeleportingAway = lastNearKhloe && !lastCastingTeleport && Data.CastingTeleport;
                
                if ((noLongerNearKhloe || startedTeleportingAway) && Data is { PlayerHasBook: false, NewBookAvailable: true })
                {
                    PrintMessage(Strings.ForgotBookWarning);
                    UIModule.PlayChatSoundEffect(11);
                }
            }
        }

        base.Update();
    }

    private void OnDutyStarted(object? sender, ushort e)
    {
        if (!Config.ModuleEnabled) return;
        if (!Config.InstanceNotifications) return;
        if (Data is not { PlayerHasBook: true, BookExpired: false }) return;
        if (GetModuleStatus() == ModuleStatus.Complete) return;
        if (!PlayerState.Instance()->HasWeeklyBingoJournal) return;

        var taskState = GetStatusForTerritory(e);

        switch (taskState)
        {
            case PlayerState.WeeklyBingoTaskStatus.Claimed when Data is { PlacedStickers: > 0, SecondChance: > 0}:
                PrintMessage(Strings.RerollNotice);
                PrintMessage(string.Format(Strings.RerollsAvailable, Data.SecondChance), true);
                break;
            
            case PlayerState.WeeklyBingoTaskStatus.Claimable:
                PrintMessage(Strings.StampAlreadyAvailable, true);
                break;
            
            case PlayerState.WeeklyBingoTaskStatus.Open:
                PrintMessage(Strings.CompletionAvailable);
                break;
        }
    }
    
    private void OnDutyCompleted(object? sender, ushort e)
    {
        if (!Config.ModuleEnabled) return;
        if (!Config.InstanceNotifications) return;
        if (Data is not { PlayerHasBook: true, BookExpired: false }) return;
        if (GetModuleStatus() == ModuleStatus.Complete) return;
        if (!PlayerState.Instance()->HasWeeklyBingoJournal) return;

        var taskState = GetStatusForTerritory(e);

        switch (taskState)
        {
            case PlayerState.WeeklyBingoTaskStatus.Claimable:
            case PlayerState.WeeklyBingoTaskStatus.Open:
                PrintMessage(Strings.StampClaimable, true);
                break;
        }
    }

    protected override ModuleStatus GetModuleStatus()
    {
        if (Config.UnclaimedBookWarning && Data.NewBookAvailable) return ModuleStatus.Incomplete;

        return Data.PlacedStickers == 9 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }
    
    protected override StatusMessage GetStatusMessage() => Data switch
    {
        { PlayerHasBook: true, BookExpired: false } when Config.StickerAvailableNotice && AnyTaskAvailableForSticker() => 
            ConditionalStatusMessage.GetMessage(Config.ClickableLink, Strings.StickerAvailable, PayloadId.OpenWondrousTailsBook),

        { SecondChance: > 7, PlacedStickers: >= 3 and <= 7, PlayerHasBook: true, BookExpired: false } when Config.ShuffleAvailableNotice => 
            ConditionalStatusMessage.GetMessage(Config.ClickableLink, Strings.ShuffleAvailable, PayloadId.OpenWondrousTailsBook),

        { NewBookAvailable: true } when Config.UnclaimedBookWarning => 
            ConditionalStatusMessage.GetMessage(Config.ClickableLink, Strings.NewBookAvailable, PayloadId.IdyllshireTeleport),

        _ => ConditionalStatusMessage.GetMessage(Config.ClickableLink, string.Format(Strings.StickersRemaining, 9 - Data.PlacedStickers), PayloadId.OpenWondrousTailsBook)
    };

    private PlayerState.WeeklyBingoTaskStatus? GetStatusForTerritory(uint territory)
    {
        foreach (var index in Enumerable.Range(0, 16))
        {
            var dutyListForSlot = TaskLookup.GetInstanceListFromID(PlayerState.Instance()->WeeklyBingoOrderData[index]);

            if (dutyListForSlot.Contains(territory))
            {
                return PlayerState.Instance()->GetWeeklyBingoTaskStatus(index);
            }
        }

        return null;
    }

    private bool AnyTaskAvailableForSticker() => Enumerable.Range(0, 16).Select(index => PlayerState.Instance()->GetWeeklyBingoTaskStatus(index)).Any(taskStatus => taskStatus == PlayerState.WeeklyBingoTaskStatus.Claimable);

    private void PrintMessage(string message, bool withPayload = false)
    {
        if (withPayload)
        {
            var conditionalMessage = ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.OpenWondrousTailsBook);
            conditionalMessage.MessageChannel = GetChatChannel();
            conditionalMessage.SourceModule = ModuleName;
            conditionalMessage.PrintMessage();
        }
        else
        {
            var statusMessage = new StatusMessage
            {
                Message = message, 
                MessageChannel = GetChatChannel(),
                SourceModule = ModuleName,
            };
            
            statusMessage.PrintMessage();
        }
    }
}

internal static class TaskLookup
{
    public static List<uint> GetInstanceListFromID(uint orderDataId)
    {
        var bingoOrderData = LuminaCache<WeeklyBingoOrderData>.Instance.GetRow(orderDataId);
        if (bingoOrderData is null) return new List<uint>();
        
        switch (bingoOrderData.Type)
        {
            // Specific Duty
            case 0:
                return LuminaCache<ContentFinderCondition>.Instance
                    .Where(c => c.Content == bingoOrderData.Data)
                    .OrderBy(row => row.SortKey)
                    .Select(c => c.TerritoryType.Row)
                    .ToList();
            
            // Specific Level Dungeon
            case 1:
                return LuminaCache<ContentFinderCondition>.Instance
                    .Where(m => m.ContentType.Row is 2)
                    .Where(m => m.ClassJobLevelRequired == bingoOrderData.Data)
                    .OrderBy(row => row.SortKey)
                    .Select(m => m.TerritoryType.Row)
                    .ToList();
            
            // Level Range Dungeon
            case 2:
                return LuminaCache<ContentFinderCondition>.Instance
                    .Where(m => m.ContentType.Row is 2)
                    .Where(m => m.ClassJobLevelRequired >= bingoOrderData.Data - (bingoOrderData.Data > 50 ? 9 : 49) && m.ClassJobLevelRequired <= bingoOrderData.Data - 1)
                    .OrderBy(row => row.SortKey)
                    .Select(m => m.TerritoryType.Row)
                    .ToList();
            
            // Special categories
            case 3:
                return bingoOrderData.Unknown5 switch
                {
                    // Treasure Map Instances are Not Supported
                    1 => new List<uint>(),
                    
                    // PvP Categories are Not Supported
                    2 => new List<uint>(),
                    
                    // Deep Dungeons
                    3 => LuminaCache<ContentFinderCondition>.Instance
                        .Where(m => m.ContentType.Row is 21)
                        .OrderBy(row => row.SortKey)
                        .Select(m => m.TerritoryType.Row)
                        .ToList(),
                    
                    _ => new List<uint>()
                };
            
            // Multi-instance raids
            case 4:
                var raidIndex = (int)(bingoOrderData.Data - 11) * 2;
                
                return bingoOrderData.Data switch
                {
                    // Binding Coil, Second Coil, Final Coil
                    2 => new List<uint> { 241, 242, 243, 244, 245 },
                    3 => new List<uint> { 355, 356, 357, 358 },
                    4 => new List<uint> { 193, 194, 195, 196 },
                    
                    // Gordias, Midas, The Creator
                    5 => new List<uint> { 442, 443, 444, 445 },
                    6 => new List<uint> { 520, 521, 522, 523},
                    7 => new List<uint> { 580, 581, 582, 583 },
                    
                    // Deltascape, Sigmascape, Alphascape
                    8 => new List<uint> { 691, 692, 693, 694 },
                    9 => new List<uint> { 748, 749, 750, 751 },
                    10 => new List<uint> { 798, 799, 800, 801 },
                    
                    > 10 => LuminaCache<ContentFinderCondition>.Instance
                        .OfLanguage(ClientLanguage.English)
                        .Where(row => row.ContentType.Row is 5)
                        .Where(row => row.ContentMemberType.Row is 3)
                        .Where(row => !row.Name.ToDalamudString().TextValue.Contains("Savage"))
                        .Where(row => row.ItemLevelRequired >= 425)
                        .OrderBy(row => row.SortKey)
                        .Select(row => row.TerritoryType.Row)
                        .ToArray()[raidIndex..(raidIndex + 2)]
                        .ToList(),
                    
                    _ => new List<uint>()
                };
        }
        
        Service.Log.Information($"[WondrousTails] Unrecognized ID: {orderDataId}");
        return new List<uint>();
    }
}
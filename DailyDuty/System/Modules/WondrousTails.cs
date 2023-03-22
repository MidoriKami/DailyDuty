using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using Dalamud;
using Dalamud.Logging;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public class WondrousTailsConfig : ModuleConfigBase
{
    [ConfigOption("InstanceNotifications")]
    public bool InstanceNotifications = true;
    
    [ClickableLink("WondrousTailsClickableLink")]
    public bool ClickableLink = true;

    [ConfigOption("StickerAvailableNotice")]
    public bool StickerAvailableNotice = true;
    
    [ConfigOption("UnclaimedBookWarning")]
    public bool UnclaimedBookWarning = true;

    [ConfigOption("ShuffleAvailableNotice", "ShuffleAvailableNoticeHelp")]
    public bool ShuffleAvailableNotice = false;
}

public class WondrousTailsData : ModuleDataBase
{
    [DataDisplay("PlacedStickers")]
    public int PlacedStickers;

    [DataDisplay("SecondChancePoints")] 
    public uint SecondChance;

    [DataDisplay("NewBookAvailable")]
    public bool NewBookAvailable;

    [DataDisplay("PlayerHasBook")]
    public bool PlayerHasBook;
    
    [DataDisplay("Deadline")]
    public DateTime Deadline;

    [DataDisplay("TimeRemaining")]
    public TimeSpan TimeRemaining;
}

public unsafe class WondrousTails : Module.WeeklyModule
{
    public override ModuleName ModuleName => ModuleName.WondrousTails;

    public override ModuleConfigBase ModuleConfig { get; protected set; } = new WondrousTailsConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new WondrousTailsData();
    private WondrousTailsConfig Config => ModuleConfig as WondrousTailsConfig ?? new WondrousTailsConfig();
    private WondrousTailsData Data => ModuleData as WondrousTailsData ?? new WondrousTailsData();

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
        TryUpdateData(ref Data.PlacedStickers, PlayerState.Instance()->WeeklyBingoNumPlacedStickers);
        TryUpdateData(ref Data.SecondChance, PlayerState.Instance()->WeeklyBingoNumSecondChancePoints);
        TryUpdateData(ref Data.Deadline, DateTimeOffset.FromUnixTimeSeconds(PlayerState.Instance()->GetWeeklyBingoExpireUnixTimestamp()).DateTime);
        TryUpdateData(ref Data.PlayerHasBook, PlayerState.Instance()->HasWeeklyBingoJournal);
        TryUpdateData(ref Data.NewBookAvailable, DateTime.UtcNow > Data.Deadline - TimeSpan.FromDays(7));
        
        Data.TimeRemaining = Data.Deadline - DateTime.UtcNow;

        base.Update();
    }

    private void OnDutyStarted(object? sender, ushort e)
    {
        if (!Config.InstanceNotifications) return;
        if (GetModuleStatus() == ModuleStatus.Complete) return;
        if (!PlayerState.Instance()->HasWeeklyBingoJournal) return;

        var taskState = GetStatusForTerritory(e);

        switch (taskState)
        {
            case PlayerState.WeeklyBingoTaskStatus.Claimed when Data is {PlacedStickers: > 0, SecondChance: > 0}:
                PrintMessage("This instance is available for a stamp if you re-roll it");
                PrintMessage($"You have {Data.SecondChance} re-rolls available", true);
                break;
            
            case PlayerState.WeeklyBingoTaskStatus.Claimable:
                PrintMessage("A stamp is already available for this instance", true);
                break;
            
            case PlayerState.WeeklyBingoTaskStatus.Open:
                PrintMessage("Completing this instance will reward you with a stamp");
                break;
        }
    }
    
    private void OnDutyCompleted(object? sender, ushort e)
    {
        if (!Config.InstanceNotifications) return;
        if (GetModuleStatus() == ModuleStatus.Complete) return;
        if (!PlayerState.Instance()->HasWeeklyBingoJournal) return;

        var taskState = GetStatusForTerritory(e);

        switch (taskState)
        {
            case PlayerState.WeeklyBingoTaskStatus.Claimable:
            case PlayerState.WeeklyBingoTaskStatus.Open:
                PrintMessage("You can claim a stamp for this duty!", true);
                break;
        }
    }

    protected override ModuleStatus GetModuleStatus()
    {
        if (Config.UnclaimedBookWarning && Data.NewBookAvailable) return ModuleStatus.Incomplete;

        return Data.PlacedStickers == 9 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }
    
    protected override StatusMessage GetStatusMessage()
    {
        if (Config.StickerAvailableNotice && AnyTaskAvailableForSticker())
            return ConditionalStatusMessage.GetMessage(Config.ClickableLink, "Sticker Available", PayloadId.OpenWondrousTailsBook);

        if (Config.ShuffleAvailableNotice && Data is { SecondChance: > 7, PlacedStickers: > 3 and < 7 }) 
            return ConditionalStatusMessage.GetMessage(Config.ClickableLink, "Shuffle Available", PayloadId.OpenWondrousTailsBook);
        
        if (Config.UnclaimedBookWarning && Data.NewBookAvailable) 
            return ConditionalStatusMessage.GetMessage(Config.ClickableLink, "New Book Available", PayloadId.IdyllshireTeleport);

        return ConditionalStatusMessage.GetMessage(Config.ClickableLink, $"{9 - Data.PlacedStickers} Stickers Remaining", PayloadId.OpenWondrousTailsBook);
    }

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
                    6 => new List<uint> {520, 521, 522, 523},
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
        
        PluginLog.Information($"[WondrousTails] Unrecognized ID: {orderDataId}");
        return new List<uint>();
    }
}
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.NativeListItems;
using DailyDuty.Nodes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Components.Builders;
using KamiToolKit.Controllers;
using KamiToolKit.Enums;
using KamiToolKit.Extensions;
using KamiToolKit.Nodes;
using Dalamud.Utility;
using Lumina.Text.ReadOnly;
using InstanceContent = FFXIVClientStructs.FFXIV.Client.Game.UI.InstanceContent;
using ContentRoulette = Dalamud.Excel.Sheets.ContentRoulette;

namespace DailyDuty.Modules;

public class DutyRoulette : Module<DutyRouletteData, DutyRouletteConfig>, IDailyResettable, IConfigurable, IClickableTooltip {

    public override string Name => Strings.DutyRoulette_DisplayName;

    public override DutyRouletteData Data { get; protected set; } = null!;
    public override DutyRouletteConfig Config { get; protected set; } = null!;

    private NativeListController<AddonContentsFinder, ContentsFinderListItem>? listController;
    private AddonController<AddonContentsFinder>? addonController;
    private ChatLinkPayload? openRoulettePayload;
    private TextNode? infoTextNode;

    private ListNode<ContentRoulette, LuminaStatusNode<ContentRoulette>>? dataListNode;

    public override async Task EnableAsync() {
        Data = await DutyRouletteData.Load();
        Config = await DutyRouletteConfig.Load();

        openRoulettePayload = new ChatLinkPayload(() => {
            unsafe {
                UIModule.Instance()->ExecuteMainCommand(2);
            }
        });

        await IFramework.Get().Run(() => {
            unsafe {
                listController = new NativeListController<AddonContentsFinder, ContentsFinderListItem> {
                    AddonName = "ContentsFinder",
                    GetPopulatorNode = GetPopulatorMethod,
                    ShouldModifyElement = ShouldModifyElementMethod,
                    UpdateElement = UpdateElementMethod,
                    ResetElement = ResetElementMethod,
                };
                listController.Enable();

                addonController = new AddonController<AddonContentsFinder> {
                    AddonName = "ContentsFinder",
                    OnSetup = SetupContentsFinder,
                    OnRefresh = RefreshContentsFinder,
                    OnFinalize = FinalizeContentsFinder,
                };
                addonController.Enable();
            }
        });
    }

    public override async Task DisableAsync() {
        await listController.DisposeAsyncSafe();
        listController = null;

        await addonController.DisposeAsyncSafe();
        addonController = null;

        openRoulettePayload?.Dispose();
        openRoulettePayload = null;
    }

    public override void BuildDataUi(NodeBuilder builder) => builder
        .List([.. ValidRoulettes], out dataListNode, configureList => {
            configureList.ItemSpacing = 4.0f;
        });

    public override void UpdateDataUi()
        => dataListNode?.Update();

    public void BuildConfigUi(NodeBuilder nodeBuilder) => nodeBuilder
        .Checkbox(Strings.DutyRoulette_MarkCompleteTomecapped, Config.CompleteWhenCapped, newValue => {
            Config.CompleteWhenCapped = newValue;
            Config.Save();
        })
        .Checkbox(Strings.DutyRoulette_ColorRoulette, Config.ColorContentFinder, newValue => {
            Config.ColorContentFinder = newValue;
            Config.Save();
        })
        .ColorEdit4(Strings.DutyRoulette_IncompleteColor, Config.IncompleteColor, KnownColor.OrangeRed, color => {
            Config.IncompleteColor = color;
            Config.Save();
        })
        .ColorEdit4(Strings.DutyRoulette_CompleteColor, Config.CompleteColor, KnownColor.LimeGreen, color => {
            Config.CompleteColor = color;
            Config.Save();
        })
        .Spacer(5.0f)
        .TextCentered(Strings.DutyRoulette_TrackedEntries, 14)
        .Separator()
        .List<ContentRoulette, LuminaConfigNode<ContentRoulette>>([.. ValidRoulettes], OnTrackedEntryChanged, configureList => {
            configureList.NodeReferenceData = Config.TrackedRoulettes;
            configureList.ItemSpacing = 4.0f;
        });

    public ReadOnlySeString GetTooltip() {
        using var stringBuilder = new RentedSeStringBuilder();

        return stringBuilder
            .AppendJoined<ContentRoulette>(Config.TrackedRoulettes.Where(id => !IsRouletteComplete(id)))
            .ToReadOnlySeString();
    }

    public void OnTooltipClicked()
        => openRoulettePayload?.ClickAction();

    protected override Task ResetAsync()
        => Task.CompletedTask;

    protected override CompletionStatus GetCompletionStatus() => Config switch {
        { CompleteWhenCapped: true } when GetLimitedTomestonesCount() >= GetLimitedTomestonesLimit() => CompletionStatus.Complete,
        _ => Config.TrackedRoulettes.All(IsRouletteComplete) ? CompletionStatus.Complete : CompletionStatus.Incomplete,
    };

    protected override void SendStatusMessage(ReadOnlySeString? message) => IChatGui.Get().PrintPayloadMessage(
        Config.MessageChatChannel, openRoulettePayload, Name,
        message ?? $"{Config.TrackedRoulettes.Count - Config.TrackedRoulettes.Count(IsRouletteComplete)} {Strings.StatusMessages_RouletteIncomplete}"
    );

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintPayloadMessage(
        Config.MessageChatChannel, openRoulettePayload, Name,
        message ?? "Module has been reset."
    );

    private static IEnumerable<ContentRoulette> ValidRoulettes
        => IDataManager.Get().GetExcelSheet<ContentRoulette>()
            .Where(roulette => roulette is { DutyType.IsEmpty: false } );

    private void OnTrackedEntryChanged(LuminaConfigNode<ContentRoulette> node, ContentRoulette data) {
        if (node.IsChecked) {
            Config.TrackedRoulettes.Add(data.RowId);
        }
        else {
            Config.TrackedRoulettes.Remove(data.RowId);
        }

        Config.Save();
    }

    private static unsafe bool IsRouletteComplete(uint rowId)
        => InstanceContent.Instance()->IsRouletteComplete((byte)rowId);

    private static unsafe int GetLimitedTomestonesCount()
        => InventoryManager.Instance()->GetWeeklyAcquiredTomestoneCount();

    private static int GetLimitedTomestonesLimit()
        => InventoryManager.GetLimitedTomestoneWeeklyLimit();

    // Duty Finder Controller Methods

    private unsafe void SetupContentsFinder(AddonContentsFinder* addon) {
        var targetResNode = addon->GetNodeById(56);
        if (targetResNode is null) return;

        using var rentedStringBuilder = new RentedSeStringBuilder();

        infoTextNode = new TextNode {
            Position = new Vector2(16.0f, targetResNode->Position.Y + 2.0f),
            Size = new Vector2(250.0f, 18.0f),
            AlignmentType = AlignmentType.Center,
            String = rentedStringBuilder
                .PushColorRgba(Config.IncompleteColor)
                .Append(Strings.DutyRoulette_IncompleteTask)
                .PopColor()
                .Append("        ")
                .PushColorRgba(Config.CompleteColor)
                .Append(Strings.DutyRoulette_CompleteTask)
                .PopColor()
                .ToReadOnlySeString(),
            TextTooltip = Strings.DutyRoulette_DutyFinderTooltip,
            IsVisible = false,
        };
        infoTextNode.AttachNode(targetResNode, NodePosition.AfterTarget);
    }

    private unsafe void RefreshContentsFinder(AddonContentsFinder* addon) {
        infoTextNode?.IsVisible = listController?.ModifiedIndexes.Count != 0 && addon->SelectedRadioButton is 0;
        addon->UpdateCollisionNodeList(false);
    }

    private unsafe void FinalizeContentsFinder(AddonContentsFinder* _) {
        infoTextNode?.Dispose();
        infoTextNode = null;
    }

    private static unsafe AtkComponentListItemRenderer* GetPopulatorMethod(AddonContentsFinder* addonContentsFinder)
        => addonContentsFinder->DutyList->GetComponentItemRendererById(6);

    private unsafe bool ShouldModifyElementMethod(AddonContentsFinder* addonContentsFinder, ContentsFinderListItem listItem) {
        if (!Config.ColorContentFinder) return false;
        if (listItem.ContentType is not ContentsType.Roulette) return false;

        return Config.TrackedRoulettes.Contains(listItem.GetContentId().Id);
    }

    private unsafe void UpdateElementMethod(AddonContentsFinder* addonContentsFinder, ContentsFinderListItem listItem) {
        if (listItem.ContentType is not ContentsType.Roulette) return;

        var rouletteInfo = IDataManager.Get()
            .GetExcelSheet<ContentRoulette>()
            .GetRow(listItem.GetContentId().Id);

        if (IsRouletteComplete(rouletteInfo.RowId)) {
            listItem.DutyNameTextNode->TextColor = Config.CompleteColor.ToByteColor();
        }
        else {
            listItem.DutyNameTextNode->TextColor = Config.IncompleteColor.ToByteColor();
        }
    }

    private static unsafe void ResetElementMethod(AddonContentsFinder* addonContentsFinder, ContentsFinderListItem listItemInfo)
        => listItemInfo.DutyNameTextNode->TextColor = listItemInfo.LevelTextNode->TextColor;
}

public class DutyRouletteData : ModuleDataFile<DutyRouletteData> {
    protected override string FileName => "DutyRoulette.data.json";
}

public class DutyRouletteConfig : ModuleConfigFile<DutyRouletteConfig> {
    protected override string FileName => "DutyRoulette.config.json";

    public bool CompleteWhenCapped { get; set; }
    public bool ColorContentFinder { get; set; } = true;
    public Vector4 CompleteColor { get; set; } = new(0.196f, 0.804f, 0.196f, 1f);
    public Vector4 IncompleteColor { get; set; } = new(0.804f, 0.196f, 0.196f, 1f);

    public HashSet<uint> TrackedRoulettes { get; set; } = [];
}

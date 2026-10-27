using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using Dalamud.Excel.Sheets;

namespace DailyDuty.Classes;

public unsafe class ContentsFinderListItem : ListItemData {
    public AtkTextNode* DutyNameTextNode => GetNode<AtkTextNode>(3);
    public AtkTextNode* LevelTextNode => GetNode<AtkTextNode>(4);

    public Contents* GetDutyInfo()
        => AgentContentsFinder.Instance()->ContentList[GetNumber(1) - 1].Value;

    public ContentsId GetContentId()
        => GetDutyInfo()->Id;

    public ContentsType ContentType
        => GetContentId().ContentType;

    public ContentFinderCondition ContentsFinderCondition
        => IDataManager.Get().GetExcelSheet<ContentFinderCondition>().GetRow(GetContentId().Id);
}

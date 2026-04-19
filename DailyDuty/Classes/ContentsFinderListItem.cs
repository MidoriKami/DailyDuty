using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Classes;

public unsafe class ContentsFinderListItem : ListItemData {
    public AtkTextNode* DutyNameTextNode => GetNode<AtkTextNode>(3);
    public AtkTextNode* LevelTextNode => GetNode<AtkTextNode>(4);

    public Contents* GetDutyInfo()
        => AgentContentsFinder.Instance()->ContentList[GetNumber(1) - 1].Value;

    public ContentsId GetContentId()
        => GetDutyInfo()->Id;

    public ContentsId.ContentsType ContentType
        => GetContentId().ContentType;

    public ContentFinderCondition ContentsFinderCondition
        => Services.DataManager.GetExcelSheet<ContentFinderCondition>().GetRow(GetContentId().Id);
}

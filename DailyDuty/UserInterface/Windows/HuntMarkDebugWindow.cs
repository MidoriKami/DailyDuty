using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.DataStructures.HuntMarks;
using ImGuiNET;
using KamiLib;
using KamiLib.ChatCommands;
using KamiLib.Drawing;
using KamiLib.Interfaces;
using KamiLib.Windows;

namespace DailyDuty.UserInterface.Windows;

public class HuntMarkDebugWindow : SelectionWindow
{
    public HuntMarkDebugWindow() : base("HuntMark Debug Window")
    {
        KamiCommon.CommandManager.AddCommand(new OpenWindowCommand<HuntMarkDebugWindow>("hunts"));
        
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(650, 350),
            MaximumSize = new Vector2(9999, 9999)
        };
        
        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;
    }

    protected override IEnumerable<ISelectable> GetSelectables() => Enumerable.Range(0, 18).Select(index => new HuntSelectable((HuntMarkType) index));
}

public class HuntSelectable : ISelectable, IDrawable
{
    public IDrawable Contents { get; }
    public string ID { get; }

    private readonly HuntMarkType huntType;
    
    public HuntSelectable(HuntMarkType type)
    {
        huntType = type;
        ID = type.GetLabel();
        Contents = this;
    }
    
    public void DrawLabel() => ImGui.Text(huntType.GetLabel());

    public void Draw()
    {
        var huntData = HuntMarkData.Instance.GetHuntData(huntType);
        
        InfoBox.Instance
            .AddTitle("Basic Hunt Info")
            .BeginTable()
            .BeginRow()
            .AddString("ID")
            .AddString(huntData.HuntID.ToString())
            .EndRow()
            .BeginRow()
            .AddString("Type")
            .AddString(huntType.GetLabel())
            .EndRow()
            .BeginRow()
            .AddString("Is Elite")
            .AddString(huntData.IsElite.ToString())
            .EndRow()
            .BeginRow()
            .AddString("Obtained")
            .AddString(huntData.Obtained.ToString(), color: huntData.Obtained ? Colors.Green : Colors.Orange)
            .EndRow()
            .BeginRow()
            .AddString("Complete")
            .AddString(huntData.IsCompleted.ToString(), color: huntData.IsCompleted ? Colors.Green : Colors.Orange)
            .EndRow()
            .EndTable()
            .Draw();

        if (huntData.IsElite)
        {
            InfoBox.Instance
                .AddTitle("Target Info")
                .BeginTable()
                .BeginRow()
                .AddString(huntData.TargetInfo[0]?.GetTargetName() ?? string.Empty)
                .AddAction(() => DrawKillCounts(0))
                .EndRow()
                .EndTable()
                .Draw();
        }
        else
        {
            InfoBox.Instance
                .AddTitle("Target Info")
                .BeginTable()
                .BeginRow()
                .AddString(huntData.TargetInfo[0]?.GetTargetName() ?? string.Empty)
                .AddAction(() => DrawKillCounts(0))
                .EndRow()
                .BeginRow()
                .AddString(huntData.TargetInfo[1]?.GetTargetName() ?? string.Empty)
                .AddAction(() => DrawKillCounts(1))
                .EndRow()
                .BeginRow()
                .AddString(huntData.TargetInfo[2]?.GetTargetName() ?? string.Empty)
                .AddAction(() => DrawKillCounts(2))
                .EndRow()
                .BeginRow()
                .AddString(huntData.TargetInfo[3]?.GetTargetName() ?? string.Empty)
                .AddAction(() => DrawKillCounts(3))
                .EndRow()
                .BeginRow()
                .AddString(huntData.TargetInfo[4]?.GetTargetName() ?? string.Empty)
                .AddAction(() => DrawKillCounts(4))
                .EndRow()
                .EndTable()
                .Draw();
        }
    }

    private unsafe void DrawKillCounts(int index)
    {
        var data = HuntMarkData.Instance.GetHuntData(huntType);

        var currentCount = (*data.KillCounts)[index];
        var targetCount = data.TargetInfo[index]?.NeededKills ?? 0;

        var complete = currentCount == targetCount;
        
        ImGui.TextColored(complete ? Colors.Green : Colors.Orange, $"{currentCount} / {targetCount}");
    }
}
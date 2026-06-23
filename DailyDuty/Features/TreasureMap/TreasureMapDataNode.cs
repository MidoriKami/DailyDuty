using System;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.TreasureMap;

public class TreasureMapDataNode(TreasureMap module) : DataNodeBase<TreasureMap>(module) {

    private TextNode? lastGatheredTime;
    private readonly TreasureMap module = module;

    protected override NodeBase BuildDataNode() => new VerticalListNode {
        FitWidth = true,
        InitialNodes = [
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.Last_Map_Gathered,
                    },
                    lastGatheredTime = new TextNode {
                        String = Strings.Attempts_Not_Updated,
                    },
                ],
            },
        ],
    };

    protected override void SnoozeClicked() {
        base.SnoozeClicked();

        if (module.ModuleConfig.Suppressed) {
            module.ModuleData.LastMapGatheredTime = DateTime.UtcNow;
            module.ModuleData.NextReset = module.ModuleData.LastMapGatheredTime + TimeSpan.FromHours(18);
            module.ModuleData.MarkDirty();
        }
    }

    public override void Update() {
        base.Update();

        var dateTime = module.ModuleData.LastMapGatheredTime;
        var shortDate = dateTime.ToLocalTime().ToShortDateString();
        var shortTime = dateTime.ToLocalTime().ToShortTimeString();

        lastGatheredTime?.String = $"{shortDate} {shortTime}";
    }
}

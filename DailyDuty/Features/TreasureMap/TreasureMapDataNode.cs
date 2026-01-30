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
                        String = "Last Map Gathered",
                    },
                    lastGatheredTime = new TextNode {
                        String = "Attempts Not Updated",
                    },
                ],
            },
        ],
    };

    public override void Update() {
        base.Update();

        var dateTime = module.ModuleData.LastMapGatheredTime;
        var shortDate = dateTime.ToLocalTime().ToShortDateString();
        var shortTime = dateTime.ToLocalTime().ToShortTimeString();
        
        lastGatheredTime?.String = $"{shortDate} {shortTime}";
    }
}

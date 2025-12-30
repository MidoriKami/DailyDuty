using DailyDuty.Classes.Nodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.TreasureMap;

public class DataNode(TreasureMap module) : DataNodeBase<TreasureMap>(module) {
    
    private TextNode? lastGatheredTime;
    private readonly TreasureMap module = module;

    protected override void BuildNode(VerticalListNode container) {
        container.AddNode([
            new HorizontalFlexNode {
                Height = 28.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = "Last Map Gathered",
                        AlignmentType = AlignmentType.Left,
                    },
                    lastGatheredTime = new TextNode {
                        String = "Attempts Not Updated",
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            },
        ]);
    }

    public override void Update() {
        base.Update();

        var dateTime = module.ModuleData.LastMapGatheredTime;
        var shortDate = dateTime.ToLocalTime().ToShortDateString();
        var shortTime = dateTime.ToLocalTime().ToShortTimeString();
        
        lastGatheredTime?.String = $"{shortDate} {shortTime}";
    }
}

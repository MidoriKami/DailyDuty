using DailyDuty.Classes;
using DailyDuty.Enums;
using KamiToolKit;

namespace DailyDuty.Features.TodoOverlay;

public class TodoOverlay : FeatureBase {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Todo List Overlay",
        FileName = "MiniCactpot",
        Type = ModuleType.GeneralFeatures,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Tasks", "List" ],
    };

    public override NodeBase DisplayNode => new ConfigNode(this);

    protected override void OnFeatureLoad() {
        
    }

    protected override void OnFeatureUnload() {
        
    }

    protected override void OnFeatureEnable() {
        
    }

    protected override void OnFeatureDisable() {
        
    }
    
    protected override void OnFeatureUpdate() {
        
    }
}

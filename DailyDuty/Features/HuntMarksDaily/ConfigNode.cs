using DailyDuty.CustomNodes;
using DailyDuty.Windows;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.HuntMarksDaily;

public class ConfigNode(HuntMarksDaily module) : ConfigNodeBase<HuntMarksDaily>(module) {
    private readonly HuntMarksDaily module = module;

    private LuminaMultiSelectWindow<MobHuntOrderType>? luminaSelectionWindow;
    
    protected override void BuildNode(VerticalListNode container) {
        container.AddNode([
            new TextButtonNode {
                Height = 24.0f,
                String = "Edit Tracked Daily Hunt Bills",
                OnClick = OpenMainTrackingWindow,
            },
        ]);
    }
    
    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        base.Dispose(disposing, isNativeDestructor);
        
        luminaSelectionWindow?.Dispose();
        luminaSelectionWindow = null;
    }

    private void OpenMainTrackingWindow() {
        luminaSelectionWindow?.Dispose();
        luminaSelectionWindow = new LuminaMultiSelectWindow<MobHuntOrderType> {
            InternalName = "MobHuntOrder",
            Title = "Hunts Daily Selection",
            Options = module.ModuleConfig.TrackedHuntMarks,
            GetLabelFunc = item => item.EventItem.ValueNullable?.Name.ToString(),
            OnEdited = module.ModuleConfig.MarkDirty,
            FilterFunc = orderType => orderType.Type is 1,
        };
        
        luminaSelectionWindow.Open();
    }
}

using DailyDuty.CustomNodes;
using DailyDuty.Windows;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.MaskedCarnivale;

public class MaskedCarnivaleConfigNode(MaskedCarnivale module) : ConfigNodeBase<MaskedCarnivale>(module){
    private readonly MaskedCarnivale module = module;

    private LuminaMultiSelectWindow<Addon>? luminaSelectionWindow;
    
    protected override void BuildNode(ScrollingListNode container) {
        container.AddNode([
            new TextButtonNode {
                Height = 28.0f,
                String = "Edit Tracked Cranivale Entries",
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
        luminaSelectionWindow = new LuminaMultiSelectWindow<Addon> {
            InternalName = "CarnavaleSelection",
            Title = "Masked Carnival Selection",
            Options = module.ModuleConfig.TrackedTasks,
            GetLabelFunc = item => item.Text.ToString(),
            OnEdited = module.ModuleConfig.MarkDirty,
            FilterFunc = item => item.RowId is >= 12447 and <= 12449,
        };
        
        luminaSelectionWindow.Open();
    }
}

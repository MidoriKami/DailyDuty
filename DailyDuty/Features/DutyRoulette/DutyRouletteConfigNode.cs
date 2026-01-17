using DailyDuty.CustomNodes;
using DailyDuty.Windows;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.DutyRoulette;

public class DutyRouletteConfigNode(DutyRoulette module) : ConfigNodeBase<DutyRoulette>(module) {
    private readonly DutyRoulette module = module;
    private LuminaMultiSelectWindow<ContentRoulette>? luminaSelectionWindow;

    protected override void BuildNode(ScrollingListNode container) {
        container.AddNode([
            new CheckboxNode {
                Height = 28.0f,
                String = "Mark Complete When Weekly Tomecapped",
                IsChecked = module.ModuleConfig.CompleteWhenCapped,
                OnClick = newValue => {
                    module.ModuleConfig.CompleteWhenCapped = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 28.0f,
                String = "Color Duty Roulette",
                IsChecked = module.ModuleConfig.ColorContentFinder,
                OnClick = newValue => {
                    module.ModuleConfig.ColorContentFinder = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CategoryHeaderNode {
                String = "Tracked Duty Finder Entries",
            },
            new TextButtonNode {
                Height = 28.0f,
                String = "Edit Tracked Duty Roulettes",
                OnClick = OpenMainTrackingWindow,
            },
        ]);
    }
    
    private void OpenMainTrackingWindow() {
        luminaSelectionWindow?.Dispose();
        luminaSelectionWindow = new LuminaMultiSelectWindow<ContentRoulette> {
            InternalName = "ContentsNoteSelection",
            Title = "Duty Roulette Selection",
            Options = module.ModuleConfig.TrackedRoulettes,
            GetLabelFunc = item => item.Name.ToString(),
            FilterFunc = item => item.ContentRouletteRoleBonus.RowId is not 0,
            OnEdited = module.ModuleConfig.MarkDirty,
        };
        
        luminaSelectionWindow.Open();
    }
}

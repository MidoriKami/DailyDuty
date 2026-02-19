using System.Drawing;
using DailyDuty.CustomNodes;
using DailyDuty.Windows;
using Dalamud.Interface;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Color;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.DutyRoulette;

public class DutyRouletteConfigNode(DutyRoulette module) : ConfigNodeBase<DutyRoulette>(module) {
    private readonly DutyRoulette module = module;
    private LuminaMultiSelectWindow<ContentRoulette>? luminaSelectionWindow;

    protected override void BuildNode(ScrollingListNode container) {
        var originalIncompleteColor = module.ModuleConfig.IncompleteColor;
        var originalCompleteColor = module.ModuleConfig.CompleteColor;
        
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
            new ColorEditNode {
                Height = 28.0f,
                CurrentColor = originalIncompleteColor,
                DefaultColor = KnownColor.OrangeRed.Vector(),
                String = "Incomplete Color",
                OnColorCancelled = () => {
                    module.ModuleConfig.IncompleteColor = originalIncompleteColor;
                    module.ModuleConfig.MarkDirty();
                },
                OnColorConfirmed = color => {
                    module.ModuleConfig.IncompleteColor = color;
                    module.ModuleConfig.MarkDirty();
                },
                OnColorPreviewed = color => {
                    module.ModuleConfig.IncompleteColor = color;
                },
            },
            new ColorEditNode {
                Height = 28.0f,
                CurrentColor = originalCompleteColor,
                DefaultColor = KnownColor.LimeGreen.Vector(),
                String = "Complete Color",
                OnColorCancelled = () => {
                    module.ModuleConfig.CompleteColor = originalCompleteColor;
                    module.ModuleConfig.MarkDirty();
                },
                OnColorConfirmed = color => {
                    module.ModuleConfig.CompleteColor = color;
                    module.ModuleConfig.MarkDirty();
                },
                OnColorPreviewed = color => {
                    module.ModuleConfig.CompleteColor = color;
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
            FilterFunc = item => item.ContentType.RowId is 1,
            OnEdited = module.ModuleConfig.MarkDirty,
        };
        
        luminaSelectionWindow.Open();
    }
}

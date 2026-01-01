using DailyDuty.CustomNodes;
using DailyDuty.Windows;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.ChallengeLog;

public class ConfigNode(ChallengeLog module) : ConfigNodeBase<ChallengeLog>(module) {
    private readonly ChallengeLog module = module;

    private LuminaMultiSelectWindow<ContentsNote>? luminaSelectionWindow;
    
    protected override void BuildNode(VerticalListNode container) {
        container.AddNode([
            new CheckboxNode {
                Height = 24.0f,
                String = "Enable Duty Finder Warning",
                IsChecked = module.ModuleConfig.EnableContentFinderWarning,
                OnClick = newValue => {
                    module.ModuleConfig.EnableContentFinderWarning = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 24.0f,
                String = "Enable Duty Finder Warning Sound",
                IsChecked = module.ModuleConfig.EnableWarningSound,
                OnClick = newValue => {
                    module.ModuleConfig.EnableWarningSound = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CategoryHeaderNode {
                Label = "Tracked Challenge Log Entries",
                Height = 40.0f,
            },
            new TextButtonNode {
                Height = 24.0f,
                String = "Edit Tracked Challenge Log Entries",
                OnClick = OpenMainTrackingWindow,
            },
            new CategoryHeaderNode {
                Label = "Tracked Duty Finder Warning Entries",
                Height = 40.0f,
            },
            new TextButtonNode {
                Height = 24.0f,
                String = "Edit Duty Finder Warning Entries",
                OnClick = OpenDutyFinderWarningEntries,
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
        luminaSelectionWindow = new LuminaMultiSelectWindow<ContentsNote> {
            InternalName = "ContentsNoteSelection",
            Title = "Challenge Log Tracking Selection",
            Options = module.ModuleConfig.TrackedEntries,
            GetLabelFunc = item => item.Name.ToString(),
            OnEdited = module.ModuleConfig.MarkDirty,
        };
        
        luminaSelectionWindow.Open();
    }
    
    private void OpenDutyFinderWarningEntries() {
        luminaSelectionWindow?.Dispose();
        luminaSelectionWindow = new LuminaMultiSelectWindow<ContentsNote> {
            InternalName = "ContentsNoteSelection",
            Title = "Challenge Log Duty Finder Warning Selection",
            Options = module.ModuleConfig.WarningEntries,
            GetLabelFunc = item => item.Name.ToString(),
            OnEdited = module.ModuleConfig.MarkDirty,
        };
        
        luminaSelectionWindow.Open();
    }
}

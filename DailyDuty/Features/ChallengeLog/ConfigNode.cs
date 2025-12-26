using DailyDuty.Classes.Nodes;
using DailyDuty.Windows;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.ChallengeLog;

public class ConfigNode(ChallengeLog module) : ConfigNodeBase<ChallengeLog>(module) {
    private readonly ChallengeLog module = module;

    private LuminaMultiSelectWindow<ContentsNote>? luminaSelectionWindow;
    
    protected override void BuildNode(VerticalListNode container) {
        container.AddNode(new CheckboxNode {
            Height = 24.0f,
            String = "Enable Duty Finder Warning",
            IsChecked = module.ModuleConfig.EnableContentFinderWarning,
            OnClick = newValue => {
                module.ModuleConfig.EnableContentFinderWarning = newValue;
                module.ModuleConfig.SavePending = true;
            },
        });
        
        container.AddNode(new CheckboxNode {
            Height = 24.0f,
            String = "Enable Duty Finder Warning Sound",
            IsChecked = module.ModuleConfig.EnableWarningSound,
            OnClick = newValue => {
                module.ModuleConfig.EnableWarningSound = newValue;
                module.ModuleConfig.SavePending = true;
            },
        });
        
        container.AddNode(new ResNode{ Height = 4.0f });
        
        container.AddNode(new CategoryTextNode {
            String = "Tracked Challenge Log Entries",
            AlignmentType = AlignmentType.BottomLeft,
            Height = 24.0f,
        });
        
        container.AddNode(new HorizontalLineNode { Height = 4.0f });
        
        container.AddNode(new TextButtonNode {
            Height = 24.0f,
            String = "Edit Tracked Challenge Log Entries",
            OnClick = OpenMainTrackingWindow,
        });
        
        container.AddNode(new ResNode{ Height = 4.0f });
        
        container.AddNode(new CategoryTextNode {
            String = "Tracked Duty Finder Warning Entries",
            AlignmentType = AlignmentType.BottomLeft,
            Height = 24.0f,
        });
        
        container.AddNode(new HorizontalLineNode { Height = 4.0f });
        
        container.AddNode(new TextButtonNode {
            Height = 24.0f,
            String = "Edit Duty Finder Warning Entries",
            OnClick = OpenDutyFinderWarningEntries,
        });
    }

    private void OpenMainTrackingWindow() {
        luminaSelectionWindow?.Dispose();
        luminaSelectionWindow = new LuminaMultiSelectWindow<ContentsNote> {
            InternalName = "ContentsNoteSelection",
            Title = "Challenge Log Tracking Selection",
            Options = module.ModuleConfig.TrackedEntries,
            GetLabelFunc = item => item.Name.ToString(),
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
        };
        
        luminaSelectionWindow.Open();
    }
}

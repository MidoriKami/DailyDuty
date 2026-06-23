using Resources;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Windows;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.ChallengeLog;

public class ChallengeLogConfigNode(ChallengeLog module) : ConfigNodeBase<ChallengeLog>(module) {
    private readonly ChallengeLog module = module;

    private LuminaMultiSelectWindow<ContentsNote>? luminaSelectionWindow;

    protected override void BuildNode(ScrollingListNode container) {
        container.AddNode([
            new CheckboxNode {
                Height = 28.0f,
                String = Strings.ResourceManager.GetString("Enable Duty Finder Warning", Strings.Culture) ?? "Enable Duty Finder Warning",
                IsChecked = module.ModuleConfig.EnableContentFinderWarning,
                OnClick = newValue => {
                    module.ModuleConfig.EnableContentFinderWarning = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 28.0f,
                String = Strings.ResourceManager.GetString("Enable Duty Finder Warning Sound", Strings.Culture) ?? "Enable Duty Finder Warning Sound",
                IsChecked = module.ModuleConfig.EnableWarningSound,
                OnClick = newValue => {
                    module.ModuleConfig.EnableWarningSound = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CategoryHeaderNode {
                String = Strings.ResourceManager.GetString("Tracked Challenge Log Entries", Strings.Culture) ?? "Tracked Challenge Log Entries",
            },
            new TextButtonNode {
                Height = 28.0f,
                String = Strings.ResourceManager.GetString("Edit Tracked Challenge Log Entries", Strings.Culture) ?? "Edit Tracked Challenge Log Entries",
                OnClick = OpenMainTrackingWindow,
            },
            new CategoryHeaderNode {
                String = Strings.ResourceManager.GetString("Tracked Duty Finder Warning Entries", Strings.Culture) ?? "Tracked Duty Finder Warning Entries",
                Height = 40.0f,
            },
            new TextButtonNode {
                Height = 28.0f,
                String = Strings.ResourceManager.GetString("Edit Duty Finder Warning Entries", Strings.Culture) ?? "Edit Duty Finder Warning Entries",
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
            Title = Strings.ResourceManager.GetString("Challenge Log Tracking Selection", Strings.Culture) ?? "Challenge Log Tracking Selection",
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
            Title = Strings.ResourceManager.GetString("Challenge Log Duty Finder Warning Selection", Strings.Culture) ?? "Challenge Log Duty Finder Warning Selection",
            Options = module.ModuleConfig.WarningEntries,
            GetLabelFunc = item => item.Name.ToString(),
            OnEdited = module.ModuleConfig.MarkDirty,
        };

        luminaSelectionWindow.Open();
    }
}

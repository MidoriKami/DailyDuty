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
                String = Strings.ChallengeLog_DutyFinderWarning,
                IsChecked = module.ModuleConfig.EnableContentFinderWarning,
                OnClick = newValue => {
                    module.ModuleConfig.EnableContentFinderWarning = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 28.0f,
                String = Strings.ChallengeLog_DutyFinderWarningSound,
                IsChecked = module.ModuleConfig.EnableWarningSound,
                OnClick = newValue => {
                    module.ModuleConfig.EnableWarningSound = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CategoryHeaderNode {
                String = Strings.ChallengeLog_TrackedEntries,
            },
            new TextButtonNode {
                Height = 28.0f,
                String = Strings.ChallengeLog_EditTrackedEntries,
                OnClick = OpenMainTrackingWindow,
            },
            new CategoryHeaderNode {
                String = Strings.ChallengeLog_TrackedDutyEntries,
                Height = 40.0f,
            },
            new TextButtonNode {
                Height = 28.0f,
                String = Strings.ChallengeLog_EditDutyEntries,
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
            Title = Strings.ChallengeLog_TrackingSelection,
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
            Title = Strings.ChallengeLog_DutyWarningSelection,
            Options = module.ModuleConfig.WarningEntries,
            GetLabelFunc = item => item.Name.ToString(),
            OnEdited = module.ModuleConfig.MarkDirty,
        };

        luminaSelectionWindow.Open();
    }
}

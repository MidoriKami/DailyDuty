using System.Drawing;
using DailyDuty.CustomNodes;
using Dalamud.Interface;
using KamiToolKit.BaseTypes;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.DutyRoulette;

public class DutyRouletteConfigNode(DutyRoulette module) : ConfigNodeBase<DutyRoulette>(module) {
    private readonly DutyRoulette module = module;

    protected override NodeBase BuildNode() {
        var originalIncompleteColor = module.ModuleConfig.IncompleteColor;
        var originalCompleteColor = module.ModuleConfig.CompleteColor;

        return new VerticalListNode {
            FitWidth = true,
            ItemSpacing = 4.0f,
            InitialNodes = [
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.DutyRoulette_MarkCompleteTomecapped,
                    IsChecked = module.ModuleConfig.CompleteWhenCapped,
                    OnClick = newValue => {
                        module.ModuleConfig.CompleteWhenCapped = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.DutyRoulette_ColorRoulette,
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
                    String = Strings.DutyRoulette_IncompleteColor,
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
                    String = Strings.DutyRoulette_CompleteColor,
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
                    String = Strings.DutyRoulette_TrackedEntries,
                },
                new LuminaMultiSelectNode<ContentRoulette> {
                    GetLabelFunc = item => item.Name.ToString(),
                    FilterFunc = item => item.ContentType.RowId is 1,
                    OnEdited = module.ModuleConfig.MarkDirty,
                    Options = module.ModuleConfig.TrackedRoulettes,
                    Height = 190.0f,
                },
            ],
        };
    }
}

using System.Drawing;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.DomanEnclave;

public class DomanEnclaveDataNode(DomanEnclave module) : DataNodeBase<DomanEnclave>(module) {
    private readonly DomanEnclave module = module;

    private TextNode? allowanceText;
    private TextNode? donatedText;
    private TextNode? allowanceRemaining;
    private TextNode? warningText;

    protected override NodeBase BuildDataNode() => new VerticalListNode {
        FitWidth = true,
        InitialNodes = [
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.Current_Max_Allowance,
                    },
                    allowanceText = new TextNode {
                        String = Strings.Allowances_Not_Updated,
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.Donated_This_Week,
                    },
                    donatedText = new TextNode {
                        String = Strings.Donated_Not_Updated,
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.Allowance_Remaining,
                    },
                    allowanceRemaining = new TextNode {
                        String = Strings.Remaining_Not_Updated,
                    },
                ],
            },
            warningText = new TextNode {
                Height = 32.0f,
                MultiplyColor = KnownColor.Orange.Vector().Fade(0.40f).AsVector3(),
                String = Strings.Status_is_unavailable__visit_the_Doman_Enclave_to_update,
                AlignmentType = AlignmentType.Center,
            },
        ],
    };

    public override void Update() {
        base.Update();

        allowanceText?.String = Allowance.ToString();
        donatedText?.String = Donated.ToString();
        allowanceRemaining?.String = RemainingAllowance.ToString();

        warningText?.IsVisible = Allowance is 0;
    }

    private int Allowance => module.ModuleData.WeeklyAllowance;
    private int Donated => module.ModuleData.DonatedThisWeek;
    private int RemainingAllowance => Allowance - Donated;
}

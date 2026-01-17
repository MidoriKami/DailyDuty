using System.Drawing;
using System.Numerics;
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
                        String = "Current Max Allowance",
                        AlignmentType = AlignmentType.Left,
                    },
                    allowanceText = new TextNode {
                        AlignmentType = AlignmentType.Left,
                        String = "Allowances Not Updated",
                    },
                ], 
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = "Donated This Week",
                        AlignmentType = AlignmentType.Left,
                    },
                    donatedText = new TextNode {
                        AlignmentType = AlignmentType.Left,
                        String = "Donated Not Updated",
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = "Allowance Remaining",
                        AlignmentType = AlignmentType.Left,
                    },
                    allowanceRemaining = new TextNode {
                        AlignmentType = AlignmentType.Left,
                        String = "Remaining Not Updated",
                    },
                ],
            },
            warningText = new TextNode {
                Height = 32.0f,
                MultiplyColor = KnownColor.Orange.Vector().Fade(0.40f).AsVector3(),
                String = "Status is unavailable, visit the Doman Enclave to update",
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

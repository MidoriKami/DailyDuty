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
                        String = Strings.DomanEnclave_CurrentMaxAllowance,
                    },
                    allowanceText = new TextNode {
                        String = Strings.CustomDelivery_AllowancesNotUpdated,
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.DomanEnclave_DonatedThisWeek,
                    },
                    donatedText = new TextNode {
                        String = Strings.DomanEnclave_DonatedNotUpdated,
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.DomanEnclave_AllowanceRemaining,
                    },
                    allowanceRemaining = new TextNode {
                        String = Strings.DomanEnclave_RemainingNotUpdated,
                    },
                ],
            },
            warningText = new TextNode {
                Height = 32.0f,
                MultiplyColor = KnownColor.Orange.Vector().Fade(0.40f).AsVector3(),
                String = Strings.DomanEnclave_UnavailableStatus,
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

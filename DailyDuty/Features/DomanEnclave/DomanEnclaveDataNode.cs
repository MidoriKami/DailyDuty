using Resources;
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
                        String = Strings.ResourceManager.GetString("Current Max Allowance", Strings.Culture) ?? "Current Max Allowance",
                    },
                    allowanceText = new TextNode {
                        String = Strings.ResourceManager.GetString("Allowances Not Updated", Strings.Culture) ?? "Allowances Not Updated",
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.ResourceManager.GetString("Donated This Week", Strings.Culture) ?? "Donated This Week",
                    },
                    donatedText = new TextNode {
                        String = Strings.ResourceManager.GetString("Donated Not Updated", Strings.Culture) ?? "Donated Not Updated",
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.ResourceManager.GetString("Allowance Remaining", Strings.Culture) ?? "Allowance Remaining",
                    },
                    allowanceRemaining = new TextNode {
                        String = Strings.ResourceManager.GetString("Remaining Not Updated", Strings.Culture) ?? "Remaining Not Updated",
                    },
                ],
            },
            warningText = new TextNode {
                Height = 32.0f,
                MultiplyColor = KnownColor.Orange.Vector().Fade(0.40f).AsVector3(),
                String = Strings.ResourceManager.GetString("Status is unavailable, visit the Doman Enclave to update", Strings.Culture) ?? "Status is unavailable, visit the Doman Enclave to update",
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

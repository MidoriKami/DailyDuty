using DailyDuty.Utilities;
using Resources;
using System;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.FashionReport;

public class FashionReportDataNode(FashionReport module) : DataNodeBase<FashionReport>(module) {
    private readonly FashionReport module = module;

    private TextNode? allowancesRemaining;
    private TextNode? highestScore;
    private TextNode? fashionReportAvailable;

    protected override NodeBase BuildDataNode() => new VerticalListNode {
        FitWidth = true,
        InitialNodes = [
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.ResourceManager.GetString("Allowances Remaining", Strings.Culture) ?? "Allowances Remaining",
                    },
                    allowancesRemaining = new TextNode {
                        String = Strings.ResourceManager.GetString("Allowances Not Updated", Strings.Culture) ?? "Allowances Not Updated",
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.ResourceManager.GetString("Highest Score", Strings.Culture) ?? "Highest Score",
                    },
                    highestScore = new TextNode {
                        String = Strings.ResourceManager.GetString("Highest Score Not Updated", Strings.Culture) ?? "Highest Score Not Updated",
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        Size = new Vector2(225.0f, 28.0f),
                        String = Strings.ResourceManager.GetString("Fashion Report Available", Strings.Culture) ?? "Fashion Report Available",
                        Height = 32.0f,
                    },
                    fashionReportAvailable = new TextNode {
                        Size = new Vector2(225.0f, 32.0f),
                        String = Strings.ResourceManager.GetString("Available Not Updated", Strings.Culture) ?? "Available Not Updated",
                    },
                ],
            },
        ],
    };

    public override void Update() {
        base.Update();

        allowancesRemaining?.String = module.ModuleData.AllowancesRemaining.ToString();
        highestScore?.String = module.ModuleData.HighestWeeklyScore.ToString();
        fashionReportAvailable?.String = IsFashionReportAvailable ? Strings.ResourceManager.GetString("Available", Strings.Culture) ?? "Available" : Strings.ResourceManager.GetString("Not Available", Strings.Culture) ?? "Not Available";
    }

    private static bool IsFashionReportAvailable
        => DateTime.UtcNow > Time.NextWeeklyReset().AddDays(-4) && DateTime.UtcNow < Time.NextWeeklyReset();
}

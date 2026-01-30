using System;
using System.Numerics;
using DailyDuty.CustomNodes;
using DailyDuty.Utilities;
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
                        String = "Allowances Remaining",
                    },
                    allowancesRemaining = new TextNode {
                        String = "Allowances Not Updated",
                    },
                ], 
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = "Highest Score",
                    },
                    highestScore = new TextNode {
                        String = "Highest Score Not Updated",
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        Size = new Vector2(225.0f, 28.0f),
                        String = "Fashion Report Available",
                        Height = 32.0f,
                    },
                    fashionReportAvailable = new TextNode {
                        Size = new Vector2(225.0f, 32.0f),
                        String = "Available Not Updated",
                    },
                ],
            },
        ],
    };

    public override void Update() {
        base.Update();

        allowancesRemaining?.String = module.ModuleData.AllowancesRemaining.ToString();
        highestScore?.String = module.ModuleData.HighestWeeklyScore.ToString();
        fashionReportAvailable?.String = IsFashionReportAvailable ? "Available" : "Not Available";
    }
    
    private static bool IsFashionReportAvailable 
        => DateTime.UtcNow > Time.NextWeeklyReset().AddDays(-4) && DateTime.UtcNow < Time.NextWeeklyReset();
}

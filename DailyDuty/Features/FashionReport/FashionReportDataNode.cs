using DailyDuty.Utilities;
using System;
using System.Numerics;
using DailyDuty.CustomNodes;
using KamiToolKit.BaseTypes;
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
                        String = Strings.CustomDelivery_AllowancesRemaining,
                    },
                    allowancesRemaining = new TextNode {
                        String = Strings.CustomDelivery_AllowancesNotUpdated,
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.FashionReport_HighestScore,
                    },
                    highestScore = new TextNode {
                        String = Strings.FashionReport_ScoreNotUpdated,
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        Size = new Vector2(225.0f, 28.0f),
                        String = Strings.FashionReport_Available,
                        Height = 32.0f,
                    },
                    fashionReportAvailable = new TextNode {
                        Size = new Vector2(225.0f, 32.0f),
                        String = Strings.FashionReport_AvailableNotUpdated,
                    },
                ],
            },
        ],
    };

    public override void Update() {
        base.Update();

        allowancesRemaining?.String = module.ModuleData.AllowancesRemaining.ToString();
        highestScore?.String = module.ModuleData.HighestWeeklyScore.ToString();
        fashionReportAvailable?.String = IsFashionReportAvailable ? Strings.FashionReport_AvailableStatus : Strings.FashionReport_NotAvailable;
    }

    private static bool IsFashionReportAvailable
        => DateTime.UtcNow > Time.NextWeeklyReset().AddDays(-4) && DateTime.UtcNow < Time.NextWeeklyReset();
}

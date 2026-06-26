using System;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.GrandCompanySquadron;

public class GrandCompanySquadronDataNode(GrandCompanySquadron module) : DataNodeBase<GrandCompanySquadron>(module) {
    private readonly GrandCompanySquadron module = module;

    private TextNode? missionCompleted;
    private TextNode? missionStarted;
    private TextNode? missionCompleteTime;
    private TextNode? missionTimeRemaining;

    protected override NodeBase BuildDataNode() => new VerticalListNode {
        FitWidth = true,
        InitialNodes = [
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.GrandCompanySquadron_MissionCompleted,
                    },
                    missionCompleted = new TextNode(),
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.GrandCompanySquadron_MissionStarted,
                    },
                    missionStarted = new TextNode(),
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.GrandCompanySquadron_CompletionTime,
                    },
                    missionCompleteTime = new TextNode(),
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.GrandCompanySquadron_TimeRemaining,
                    },
                    missionTimeRemaining = new TextNode(),
                ],
            },
        ],
    };

    public override void Update() {
        base.Update();

        missionCompleted?.String = module.ModuleData.MissionCompleted.ToString();
        missionStarted?.String = module.ModuleData.MissionStarted.ToString();
        missionCompleteTime?.String = $"{module.ModuleData.MissionCompleteTime.ToShortDateString()} {module.ModuleData.MissionCompleteTime.ToShortTimeString()}";

        if (module.ModuleData.TimeUntilMissionComplete == TimeSpan.MinValue) {
            missionTimeRemaining?.String = Strings.GrandCompanySquadron_NotStarted;
        }
        else if (module.ModuleData.TimeUntilMissionComplete < TimeSpan.Zero) {
            missionTimeRemaining?.String = Strings.CompletionStatus_ResultsAvailable;
        }
        else {
            missionTimeRemaining?.String = (module.ModuleData.MissionCompleteTime - DateTime.UtcNow).FormatTimespan();
        }
    }
}

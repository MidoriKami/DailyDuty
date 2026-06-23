using Resources;
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
                        String = Strings.ResourceManager.GetString("Mission Completed", Strings.Culture) ?? "Mission Completed",
                    },
                    missionCompleted = new TextNode(),
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.ResourceManager.GetString("Mission Started", Strings.Culture) ?? "Mission Started",
                    },
                    missionStarted = new TextNode(),
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.ResourceManager.GetString("Mission Completion Time", Strings.Culture) ?? "Mission Completion Time",
                    },
                    missionCompleteTime = new TextNode(),
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.ResourceManager.GetString("Mission Time Remaining", Strings.Culture) ?? "Mission Time Remaining",
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
            missionTimeRemaining?.String = Strings.ResourceManager.GetString("Not Started", Strings.Culture) ?? "Not Started";
        }
        else if (module.ModuleData.TimeUntilMissionComplete < TimeSpan.Zero) {
            missionTimeRemaining?.String = Strings.ResourceManager.GetString("Results Available", Strings.Culture) ?? "Results Available";
        }
        else {
            missionTimeRemaining?.String = (module.ModuleData.MissionCompleteTime - DateTime.UtcNow).FormatTimespan();
        }
    }
}

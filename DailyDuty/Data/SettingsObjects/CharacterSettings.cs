using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.SettingsObjects.DailySettings;
using DailyDuty.Data.SettingsObjects.WeeklySettings;
using DailyDuty.Modules.Weekly;

namespace DailyDuty.Data.SettingsObjects;

[Serializable]
public class CharacterSettings
{
    public string CharacterName = "NameNotSet";

    //Daily
    public BeastTribeSettings BeastTribe = new();
    public DutyRouletteSettings DutyRoulette = new();
    public GrandCompanySettings GrandCompany = new();
    public LeviquestSettings Leviquest = new();
    public MiniCactpotSettings MiniCactpot = new();
    public TreasureMapSettings TreasureMap = new();

    //Weekly
    public BlueMageLogSettings BlueMageLog = new();
    public ChallengeLogSettings ChallengeLog = new();
    public CustomDeliverySettings CustomDelivery = new();
    public DomanEnclaveSettings DomanEnclave = new();
    public FashionReportSettings FashionReport = new();
    public HuntMarksSettings HuntMarks = new();
    public JumboCactpotSettings JumboCactpot = new();
    public MaskedCarnivalSettings MaskedCarnival = new();
    public WondrousTailsSettings WondrousTails = new();
}
using System.ComponentModel;

namespace DailyDuty.Classes;

public enum ModuleName {
    [Description("Unknown")]
    Unknown,
    
    [Description("TestModule")] 
    TestModule,
    
    [Description("ChallengeLog")]
    ChallengeLog,
    
    [Description("CustomDelivery")]
    CustomDelivery,
    
    [Description("DomanEnclave")]
    DomanEnclave,
    
    [Description("DutyRoulette")]
    DutyRoulette,
    
    [Description("MiniCactpot")]
    MiniCactpot,
    
    [Description("FashionReport")]
    FashionReport,
    
    [Description("FauxHollows")]
    FauxHollows,
    
    [Description("GrandCompanyProvision")]
    GrandCompanyProvision,
    
    [Description("GrandCompanySquadron")]
    GrandCompanySquadron,
    
    [Description("GrandCompanySupply")]
    GrandCompanySupply,
    
    [Description("HuntMarksDaily")]
    HuntMarksDaily,
    
    [Description("HuntMarksWeekly")]
    HuntMarksWeekly,
    
    [Description("TreasureMap")]
    TreasureMap,
    
    [Description("JumboCactpot")]
    JumboCactpot,
    
    [Description("Levequest")]
    Levequest,
    
    [Description("MaskedCarnivale")]
    MaskedCarnivale,
    
    [Description("RaidsAlliance")]
    RaidsAlliance,
    
    [Description("RaidsNormal")]
    RaidsNormal,
    
    [Description("TribalQuests")]
    TribalQuests,
    
    [Description("WondrousTails")]
    WondrousTails,
}
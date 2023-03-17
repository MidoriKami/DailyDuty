using DailyDuty.Models.Attributes;

namespace DailyDuty.Models.Enums;

public enum ModuleName
{
    [Label("Unknown")]
    Unknown,
    
    [Label("TestModule")] 
    TestModule,
    
    [Label("ChallengeLog")]
    ChallengeLog,
    
    [Label("CustomDelivery")]
    CustomDelivery,
    
    [Label("DomanEnclave")]
    DomanEnclave,
    
    [Label("DutyRoulette")]
    DutyRoulette,
    
    [Label("MiniCactpot")]
    MiniCactpot,
    
    [Label("FashionReport")]
    FashionReport,
    
    [Label("FauxHollows")]
    FauxHollows,
}
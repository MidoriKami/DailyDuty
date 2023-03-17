using DailyDuty.System;

namespace DailyDuty.Interfaces;

public interface IGoldSaucerMessageReceiver
{
    void GoldSaucerUpdate(object? sender, GoldSaucerEventArgs data);
}
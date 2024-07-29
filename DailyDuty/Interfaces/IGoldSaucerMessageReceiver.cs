using DailyDuty.Classes;

namespace DailyDuty.Interfaces;

public interface IGoldSaucerMessageReceiver {
	void GoldSaucerUpdate(GoldSaucerEventArgs data);
}
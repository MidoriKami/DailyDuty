namespace DailyDuty.Classes;

public interface IGoldSaucerMessageReceiver {
	void GoldSaucerUpdate(GoldSaucerEventArgs data);
}
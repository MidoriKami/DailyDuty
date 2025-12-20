using KamiLib.Configuration;

namespace DailyDuty.Classes;

public static class StyleFileHelper {
	public static string GetPath(string fileName) 
		=> Service.PluginInterface.GetCharacterFileInfo(Service.PlayerState.ContentId, fileName).FullName;
}
using System.IO;
using ImGuiScene;

namespace DailyDuty.Utilities
{
    internal static class Image
    {
        public static TextureWrap LoadImage(string imageName)
        {
            var assemblyLocation = Service.PluginInterface.AssemblyLocation.DirectoryName!;
            var imagePath = Path.Combine(assemblyLocation, $@"images\{imageName}.png");

            return Service.PluginInterface.UiBuilder.LoadImage(imagePath);
        }
    }
}

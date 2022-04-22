using System;
using System.IO;
using DailyDuty.Data;
using Dalamud.Game;
using Newtonsoft.Json;

namespace DailyDuty.Utilities
{
    internal static class Configuration
    {
        private static readonly CharacterConfiguration NullCharacterConfiguration = new();

        public static void Login(object? sender, EventArgs e)
        {
            Chat.Log("OnLogin", "Adding Login Listener");

            Service.Framework.Update += LoginLogic;
        }

        private static void LoginLogic(Framework framework)
        {
            if (Service.ClientState.LocalContentId != 0 && Service.ClientState.LocalPlayer != null)
            {
                Chat.Log("LoginLogic", $"Logging into Character '{(Service.ClientState.LocalPlayer?.Name.TextValue ?? "Null Local Player")}'");

                LoadCharacterConfiguration();

                Chat.Log("LoginLogic", "Removing Login Listener");

                Service.LoggedIn = true;

                Service.Framework.Update -= LoginLogic;
            }
        }

        public static void Logout(object? sender, EventArgs e)
        {
            Chat.Log("Logout", $"Logging out of '{Service.CharacterConfiguration.CharacterName}'");

            Service.LoggedIn = false;

            Service.CharacterConfiguration = NullCharacterConfiguration;
        }

        public static void Startup()
        {
            LoadSystemConfiguration();

            LoadCharacterConfiguration();
        }

        public static void Cleanup()
        {
            Service.Framework.Update -= LoginLogic;
        }

        //
        //  Helpers
        //

        private static CharacterConfiguration LoadConfiguration(string characterName)
        {
            var configFileInfo = GetConfigFileInfo(characterName);

            if (configFileInfo.Exists)
            {
                var fileText = File.ReadAllText(GetConfigFileInfo(characterName).FullName);

                var loadedCharacterConfiguration = JsonConvert.DeserializeObject<CharacterConfiguration>(fileText);

                return loadedCharacterConfiguration ?? new CharacterConfiguration();
            }
            else
            {
                return new CharacterConfiguration();
            }
        }

        public static FileInfo GetConfigFileInfo(string characterName)
        {
            var pluginConfigDirectory = Service.PluginInterface.ConfigDirectory;

            return new FileInfo(pluginConfigDirectory.FullName + $@"\{characterName}.json");
        }

        private static void LoadSystemConfiguration()
        {
            var systemConfiguration = Service.PluginInterface.GetPluginConfig();

            if (systemConfiguration != null)
            {
                Service.SystemConfiguration = (SystemConfiguration) systemConfiguration;
            }
            else
            {
                Service.SystemConfiguration = new SystemConfiguration();
                Service.SystemConfiguration.Save();
            }
        }

        private static void LoadCharacterConfiguration()
        {
            if (Service.ClientState.IsLoggedIn)
            {
                var playerData = Service.ClientState.LocalPlayer;

                if (playerData != null)
                {
                    var configFileInfo = GetConfigFileInfo(playerData.Name.TextValue);

                    if (configFileInfo.Exists)
                    {
                        Service.CharacterConfiguration = LoadConfiguration(playerData.Name.TextValue);
                    }
                    else
                    {
                        Service.CharacterConfiguration = new CharacterConfiguration
                        {
                            CharacterName = playerData.Name.TextValue,
                            LocalContentID = Service.ClientState.LocalContentId
                        };
                        Service.CharacterConfiguration.Save();
                    }

                    Service.LoggedIn = true;
                }
            }
        }
    }
}

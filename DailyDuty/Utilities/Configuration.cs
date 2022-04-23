using System;
using System.IO;
using DailyDuty.Data;
using DailyDuty.Data.Components;
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
                LoadCharacterLog();

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
            if (Service.ClientState.IsLoggedIn)
                Service.LoggedIn = true;

            LoadSystemConfiguration();

            LoadCharacterConfiguration();

            LoadCharacterLog();
        }

        public static void Cleanup()
        {
            Service.Framework.Update -= LoginLogic;
        }

        //
        //  Helpers
        //

        private static CharacterConfiguration LoadConfiguration(ulong contentID)
        {
            var configFileInfo = GetConfigFileInfo(contentID);

            if (configFileInfo.Exists)
            {
                var fileText = File.ReadAllText(configFileInfo.FullName);

                var loadedCharacterConfiguration = JsonConvert.DeserializeObject<CharacterConfiguration>(fileText);

                return loadedCharacterConfiguration ?? new CharacterConfiguration();
            }
            else
            {
                return new CharacterConfiguration();
            }
        }

        private static CharacterLogFile LoadCharacterLogFile(ulong contentID)
        {
            var characterLogFile = GetLogFileInfo(contentID);

            if (characterLogFile.Exists)
            {
                var fileText = File.ReadAllText(characterLogFile.FullName);

                var loadedCharacterLogFile = JsonConvert.DeserializeObject<CharacterLogFile>(fileText);

                return loadedCharacterLogFile ?? new CharacterLogFile();
            }
            else
            {
                return new CharacterLogFile();
            }
        }

        public static FileInfo GetConfigFileInfo(ulong contentID)
        {
            var pluginConfigDirectory = Service.PluginInterface.ConfigDirectory;
            
            return new FileInfo(pluginConfigDirectory.FullName + $@"\{contentID}.json");
        }

        public static FileInfo GetLogFileInfo(ulong contentID)
        {
            var pluginConfigDirectory = Service.PluginInterface.ConfigDirectory;
            
            return new FileInfo(pluginConfigDirectory.FullName + $@"\{contentID}.log");
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
                var contentId = Service.ClientState.LocalContentId;

                if (playerData != null && playerData.HomeWorld.GameData != null)
                {
                    var playerName = playerData.Name.TextValue;
                    var playerWorld = playerData.HomeWorld.GameData.Name.ToString();

                    var configFileInfo = GetConfigFileInfo(contentId);

                    if (configFileInfo.Exists)
                    {
                        Service.CharacterConfiguration = LoadConfiguration(contentId);
                        Service.CharacterConfiguration.CharacterName = playerName;
                        Service.CharacterConfiguration.World = playerWorld;
                    }
                    else
                    {
                        Service.CharacterConfiguration = new CharacterConfiguration
                        {
                            CharacterName = playerName,
                            LocalContentID = contentId,
                            World = playerWorld,
                        };
                        Service.CharacterConfiguration.Save();
                    }
                }
            }
        }

        private static void LoadCharacterLog()
        {
            if (Service.ClientState.IsLoggedIn)
            {
                var playerData = Service.ClientState.LocalPlayer;
                var contentId = Service.ClientState.LocalContentId;

                if (playerData != null && playerData.HomeWorld.GameData != null)
                {
                    var playerName = playerData.Name.TextValue;
                    var playerWorld = playerData.HomeWorld.GameData.Name.ToString();

                    var logFileInfo = GetLogFileInfo(contentId);

                    if (logFileInfo.Exists)
                    {
                        Service.LogManager.Log = LoadCharacterLogFile(contentId);
                        Service.LogManager.Log.CharacterName = playerName;
                        Service.LogManager.Log.World = playerWorld;
                    }
                    else
                    {
                        Service.LogManager.Log = new CharacterLogFile()
                        {
                            CharacterName = playerName,
                            LocalContentID = contentId,
                            World = playerWorld,
                        };
                        Service.LogManager.Save();
                    }
                }
            }
        }
    }
}

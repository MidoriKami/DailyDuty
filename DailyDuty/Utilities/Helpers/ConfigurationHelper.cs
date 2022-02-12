using DailyDuty.Data.SettingsObjects;
using Dalamud.Logging;

namespace DailyDuty.Utilities.Helpers
{
    internal static class ConfigurationHelper
    {
        private static readonly CharacterSettings NullCharacterSettings = new();
        private static ulong _currentCharacter = 0;

        public static CharacterSettings GetCurrentCharacterData()
        {
            var config = Service.Configuration;

            // If we are not logged-in
            if (Service.LoggedIn == false)
            {
                return NullCharacterSettings;
            }
            // If we are logged in, then we have a valid character id
            else
            {
                // If the character exists, fetch it
                if (CharacterExists(_currentCharacter))
                {
                    return config.CharacterSettingsMap[_currentCharacter];
                }

                // If the character doesn't exit, make it
                else
                {
                    AddCharacterToMap(_currentCharacter);
                    return config.CharacterSettingsMap[_currentCharacter];
                }
            }
        }

        private static void AddCharacterToMap(ulong characterID)
        {
            var config = Service.Configuration;

            config.CharacterSettingsMap.Add(characterID, new CharacterSettings());

            UpdatePlayerName(characterID);

            config.Save();
        }

        private static void UpdatePlayerName(ulong characterID)
        {
            var config = Service.Configuration;

            if (config.CharacterSettingsMap[_currentCharacter].CharacterName == "NameNotSet")
            {
                var localPlayer = Service.ClientState.LocalPlayer;
                if (localPlayer != null)
                {
                    config.CharacterSettingsMap[characterID].CharacterName = localPlayer.Name.ToString();
                }
                else
                {
                    PluginLog.Information($"Unable to Update Player Name: {characterID}");
                }
            }
        }

        /// <summary>
        /// Is called when Local Content ID is not Zero, and was not logged in
        /// </summary>
        public static void Login()
        {
            Service.LoggedIn = true;
            _currentCharacter = Service.ClientState.LocalContentId;
        }

        private static bool CharacterExists(ulong characterID)
        {
            var config = Service.Configuration;

            return config.CharacterSettingsMap.ContainsKey(characterID);
        }

        public static void Logout()
        {
            Service.LoggedIn = false;
            _currentCharacter = 0;
        }
    }
}

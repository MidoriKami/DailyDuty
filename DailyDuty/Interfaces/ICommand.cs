using DailyDuty.Localization;

namespace DailyDuty.Interfaces
{
    internal interface ICommand
    {
        protected void Execute(string? primaryCommand, string? secondaryCommand);

        public void ProcessCommand(string command, string arguments)
        {
            var primaryCommand = GetPrimaryCommand(arguments)?.ToLower();
            var secondaryCommand = GetSecondaryCommand(arguments)?.ToLower();

            Execute(primaryCommand, secondaryCommand);
        }

        private static string? GetSecondaryCommand(string arguments)
        {
            var stringArray = arguments.Split(' ');

            if (stringArray.Length == 1)
            {
                return null;
            }

            return stringArray[1];
        }

        private static string? GetPrimaryCommand(string arguments)
        {
            var stringArray = arguments.Split(' ');

            if (stringArray[0] == string.Empty)
            {
                return null;
            }

            return stringArray[0];
        }

        public static bool OpenCommand(string? command)
        {
            if (command == Strings.Command.On || command == "on")
                return true;

            if (command == Strings.Command.Enable || command == "enable")
                return true;

            if (command == Strings.Command.Show || command == "show")
                return true;

            if (command == Strings.Command.Open || command == "open")
                return true;

            return false;
        }

        public static bool CloseCommand(string? command)
        {
            if (command == Strings.Command.Off || command == "off")
                return true;

            if (command == Strings.Command.Disable || command == "disable")
                return true;

            if (command == Strings.Command.Hide || command == "hide")
                return true;

            if (command == Strings.Command.Close || command == "close")
                return true;

            return false;
        }

        public static bool ToggleCommand(string? command)
        {
            if (command == Strings.Command.Toggle || command == "toggle")
                return true;

            return false;
        }

        public static bool HelpCommand(string? command)
        {
            if (command == Strings.Command.Help || command is null or "help")
                return true;

            return false;
        }
    }
}
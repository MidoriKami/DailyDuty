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
            if (command == Strings.Command.On)
                return true;

            if (command == Strings.Command.Enable)
                return true;

            if (command == Strings.Command.Show)
                return true;

            if (command == Strings.Command.Open)
                return true;

            return false;
        }

        public static bool CloseCommand(string? command)
        {
            if (command == Strings.Command.Off)
                return true;

            if (command == Strings.Command.Disable)
                return true;

            if (command == Strings.Command.Hide)
                return true;

            if (command == Strings.Command.Close)
                return true;

            return false;
        }

        public static bool ToggleCommand(string? command)
        {
            if (command == Strings.Command.Toggle)
                return true;

            return false;
        }

        public static bool HelpCommand(string? command)
        {
            if (command == Strings.Command.Help || command == null)
                return true;

            return false;
        }
    }
}
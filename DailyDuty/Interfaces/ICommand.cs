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
    }
}
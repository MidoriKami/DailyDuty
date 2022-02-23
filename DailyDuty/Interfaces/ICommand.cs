namespace DailyDuty.Interfaces
{
    internal interface ICommand
    {
        public void ProcessCommand(string command, string arguments);

        protected static string? GetSecondaryCommand(string arguments)
        {
            var stringArray = arguments.Split(' ');

            if (stringArray.Length == 1)
            {
                return null;
            }

            return stringArray[1];
        }

        protected static string? GetPrimaryCommand(string arguments)
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
namespace DailyDuty.CommandSystem.CommandProcessors
{
    internal class TodoWindowCommandProcessor : CommandProcessor
    {
        public TodoWindowCommandProcessor()
        {
            PrimaryCommandFilters = new()
            {
                "todo",
            };
        }

        protected override void ProcessOnCommand()
        {
            Service.Configuration.ToDoWindowSettings.Open = true;

            Service.Configuration.Save();
        }

        protected override void ProcessOffCommand()
        {
            Service.Configuration.ToDoWindowSettings.Open = false;

            Service.Configuration.Save();
        }

        protected override void ProcessNullSecondaryCommand()
        {
            Service.Configuration.ToDoWindowSettings.Open = !Service.Configuration.ToDoWindowSettings.Open;

            Service.Configuration.Save();
        }
    }
}

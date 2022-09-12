using DailyDuty.UserInterface.Components.InfoBox;

namespace DailyDuty.Interfaces;

public interface IInfoBoxTableConfigurationRow
{
    void GetConfigurationRow(InfoBoxTable owner);
}

public interface IInfoBoxTableDataRow
{
    void GetDataRow(InfoBoxTable owner);
}

public interface IInfoBoxListConfigurationRow
{
    void GetConfigurationRow(InfoBoxList owner);
}
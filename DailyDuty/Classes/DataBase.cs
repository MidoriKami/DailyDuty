using System;
using DailyDuty.Interfaces;

namespace DailyDuty.Classes;

public class DataBase : Savable {
    public DateTime NextReset;

    protected override string FileExtension => ".data.json";
}

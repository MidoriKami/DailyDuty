using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DailyDuty.System.Helpers;

public static class AttributeHelper
{
    public static List<(FieldInfo, T)> GetFieldAttributes<T>(object obj) => obj
        .GetType().GetFields()
        .Where(field => field.GetCustomAttribute(typeof(T)) is not null)
        .Select(field => (field, (T)(object) field.GetCustomAttribute(typeof(T))!))
        .ToList();
}
using System.Reflection;

namespace DailyDuty.Extensions;

public static class MemberInfoExtensions {
    extension(MemberInfo memberInfo) {
        public T? GetValue<T>(object forObject) {
            return memberInfo.MemberType switch {
                MemberTypes.Field => (T?)((FieldInfo)memberInfo).GetValue(forObject),
                MemberTypes.Property => (T?)((PropertyInfo)memberInfo).GetValue(forObject),
                _ => default,
            };
        }

        public void SetValue<T>(object forObject, T value) {
            switch (memberInfo.MemberType) {
                case MemberTypes.Field:
                    ((FieldInfo)memberInfo).SetValue(forObject, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo)memberInfo).SetValue(forObject, value);
                    break;
            }
        }
    }
}

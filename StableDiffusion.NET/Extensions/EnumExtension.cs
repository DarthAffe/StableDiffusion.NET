using System;
using System.Reflection;

namespace StableDiffusion.NET;

internal static class EnumExtension
{
    #region Methods

    public static string? GetNativeName(this Enum value)
    {
        FieldInfo? fieldInfo = value.GetType().GetField(value.ToString());
        NativeName? nativeName = fieldInfo?.GetCustomAttribute<NativeName>();
        return nativeName?.Name;
    }

    #endregion
}
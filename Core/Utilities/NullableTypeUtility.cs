using System;

namespace Remotion.Utilities
{
  public class NullableTypeUtility
  {
    /// <summary>
    /// Determines whether a type is nullable, ie. whether variables of it can be assigned <see langword="null"/>.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>
    /// true if <paramref name="type"/> is nullable; otherwise, false.
    /// </returns>
    /// <remarks>
    /// A type is nullable if it is a reference type or a nullable value type. This method returns false only for non-nullable value types.
    /// </remarks>
    public static bool IsNullableType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return IsNullableType_NoArgumentCheck (type);
    }

    internal static bool IsNullableType_NoArgumentCheck (Type expectedType)
    {
      return !expectedType.IsValueType || Nullable.GetUnderlyingType (expectedType) != null;
    }

    public static Type GetNullableType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (IsNullableType (type))
        return type;
      else
        return typeof (Nullable<>).MakeGenericType (type);
    }

    public static Type GetBasicType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return Nullable.GetUnderlyingType (type) ?? type;
    }
  }
}
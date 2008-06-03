/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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

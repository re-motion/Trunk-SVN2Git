// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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

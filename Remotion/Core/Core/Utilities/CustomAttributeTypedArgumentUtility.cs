// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections.Generic;
using System.Reflection;

namespace Remotion.Utilities
{
  /// <summary>
  /// Utility for recursively unwrapping <see cref="CustomAttributeTypedArgument"/> instances.
  /// </summary>
  public static class CustomAttributeTypedArgumentUtility
  {
    public static object Unwrap (CustomAttributeTypedArgument typedArgument)
    {
      ArgumentUtility.CheckNotNull ("typedArgument", typedArgument);

      if (typedArgument.ArgumentType.IsArray)
      {
        var wrappedValues = (IList<CustomAttributeTypedArgument>) typedArgument.Value;
        var array = Array.CreateInstance (typedArgument.ArgumentType.GetElementType(), wrappedValues.Count);
        for (int i = 0; i < wrappedValues.Count; i++)
          array.SetValue (Unwrap (wrappedValues[i]), i);

        return array;
      }

      if (typedArgument.ArgumentType.IsEnum)
        return Enum.ToObject (typedArgument.ArgumentType, typedArgument.Value);

      return typedArgument.Value;
    }
  }
}
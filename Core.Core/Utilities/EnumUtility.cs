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
  /// <summary>
  /// This utility class provides methods for dealing with enumeration values.
  /// </summary>
  public static class EnumUtility
  {
	  /// <summary>
	  /// Checks whether the specified value is one of the values that the enumeration type defines.
	  /// </summary>
	  public static bool IsValidEnumValue (object enumValue)
	  {
		  if (enumValue == null)
			  throw new ArgumentNullException ("enumValue");

		  string stringRepresentation = enumValue.ToString();
		  if (stringRepresentation == null || stringRepresentation.Length == 0)
			  return false;
		  char firstChar = stringRepresentation[0];
		  return ! (firstChar == '-' || char.IsDigit (firstChar));

      // the following method is twice as fast, but does not consider flag combinations defined, except 
      // if there is an explicit enum field for this combination
      // return Enum.IsDefined (enumValue.GetType(), enumValue);
	  }
  }
}

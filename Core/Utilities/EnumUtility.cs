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

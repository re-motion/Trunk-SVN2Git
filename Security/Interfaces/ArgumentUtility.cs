using System;
using Remotion.Utilities;

namespace Remotion.Security
{
  internal class ArgumentUtility
  {
    public static T CheckNotNull<T> (string argumentName, T actualValue)
    {
      if (actualValue == null)
        throw new ArgumentNullException (argumentName);

      return actualValue;
    }

    public static string CheckNotNullOrEmpty (string argumentName, string actualValue)
    {
      CheckNotNull (argumentName, actualValue);
      if (actualValue.Length == 0)
        throw new ArgumentEmptyException (argumentName);

      return actualValue;
    }
  }
}
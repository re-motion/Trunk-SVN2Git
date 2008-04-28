using System;
using System.Runtime.Serialization;

namespace Remotion.Utilities
{
  /// <summary>
  /// This exception is thrown if a list argument contains a null reference.
  /// </summary>
  [Serializable]
  public class ArgumentItemNullException : ArgumentException
  {
    public ArgumentItemNullException (string argumentName, int index)
        : base (FormatMessage (argumentName, index))
    {
    }

    public ArgumentItemNullException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }

    private static string FormatMessage (string argumentName, int index)
    {
      return string.Format ("Item {0} of argument {1} is null.", index, argumentName);
    }
  }
}
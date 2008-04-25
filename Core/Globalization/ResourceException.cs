using System;
using System.Runtime.Serialization;

namespace Remotion.Globalization
{

/// <summary>
///   Represents errors that occur during resource handling.
/// </summary>
[Serializable]
public class ResourceException: Exception
{
  /// <summary>
  ///   Initializes a new instance of the <see cref="ResourceException"/>  class with a specified 
  ///   error message.
  /// </summary>
  /// <param name="message"> The error message string. </param>
  public ResourceException (string message)
    : base (message)
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="ResourceException"/> class with a specified 
  ///   error message and a reference to the inner exception that is the cause of this exception.
  /// </summary>
  /// <param name="message"> The error message string. </param>
  /// <param name="innerException"> The inner exception reference. </param>
  public ResourceException (string message, Exception innerException)
    : base (message, innerException)
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="ResourceException"/> class with serialized 
  ///   data.
  /// </summary>
  /// <param name="info"> The info parameter is a <see langword="null"/>.</param>
  /// <param name="context">
  /// The class name is a <see langword="null"/> or <see cref="Exception.HResult"/> is zero (0).
  /// </param>
  public ResourceException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

}
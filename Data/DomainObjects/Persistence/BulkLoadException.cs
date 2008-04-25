using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Data.DomainObjects.Persistence
{
  /// <summary>
  /// Thrown when the process of loading multiple objects at the same time fails.
  /// </summary>
  public class BulkLoadException : Exception
  {
    /// <summary>
    /// The exceptions that occurred while the objects were loaded.
    /// </summary>
    public readonly List<Exception> Exceptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="BulkLoadException"/> class.
    /// </summary>
    /// <param name="exceptions">The exceptions thrown while the objects were loaded.</param>
    public BulkLoadException (IEnumerable<Exception> exceptions)
        : base (CreateMessage (exceptions))
    {
      Exceptions = new List<Exception> (exceptions);
    }

    private static string CreateMessage (IEnumerable<Exception> exceptions)
    {
      StringBuilder message = new StringBuilder("There were errors when loading a bulk of DomainObjects:");
      message.AppendLine();
      foreach (Exception exception in exceptions)
        message.AppendLine (exception.Message);
      return message.ToString();
    }
  }
}
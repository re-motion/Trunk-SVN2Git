using System;
using Remotion.Implementation;

namespace Remotion.Logging.BridgeInterfaces
{
  [ConcreteImplementation ("Remotion.Logging.BridgeImplementations.LogManagerImplementation, Remotion, Version = <version>")]
  public interface ILogManagerImplementation
  {
    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="name">The name of the logger to retrieve.</param>
    /// <returns>A logger for the <paramref name="name"/> specified.</returns>
    ILog GetLogger (string name);

    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="type">The full name of <paramref name="type"/> will be used as the name of the logger to retrieve.</param>
    /// <returns>A logger for the fully qualified name of the <paramref name="type"/> specified.</returns>
    ILog GetLogger (Type type);

    /// <summary>
    /// Initializes the current logging framework.
    /// </summary>
    void Initialize ();

    /// <summary>
    /// Initializes the current logging framework to log to the console.
    /// </summary>
    void InitializeConsole ();
  }
}
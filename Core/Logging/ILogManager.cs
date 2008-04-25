using System;

namespace Remotion.Logging
{
  // TODO FS: Move to Remotion.Interfaces
  /// <summary>
  /// The <see cref="ILogManager"/> interface declares the methods available for retrieving a logger that implements
  /// <see cref="ILog"/> and initializing the respective logging framework.
  /// </summary>
  public interface ILogManager
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
    /// Initializes the logging framework abstracted through the <see cref="ILogManager"/> interface.
    /// </summary>
    void Initialize ();

    /// <summary>
    /// Initializes the logging framework to log to the console.
    /// </summary>
    void InitializeConsole ();
  }
}
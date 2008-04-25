using System;

namespace Remotion.Logging
{
  // TODO FS: Move to Remotion.Interfaces, split in implementation/interface
  /// <summary>
  /// Use this class to create a logger implementing <see cref="ILog"/> from the current <see cref="ILogManager"/>.
  /// </summary>
  /// <remarks>
  /// Currently only <b>log4net</b> is supported as logging infrastructure.
  /// </remarks>
  public static class LogManager
  {
    // TODO: Get from config section
    private static ILogManager s_current = new Log4NetLogManager ();

    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="name">The name of the logger to retrieve.</param>
    /// <returns>A logger for the <paramref name="name"/> specified.</returns>
    public static ILog GetLogger (string name)
    {
      return s_current.GetLogger (name);
    }

    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="type">The full name of <paramref name="type"/> will be used as the name of the logger to retrieve.</param>
    /// <returns>A logger for the fully qualified name of the <paramref name="type"/> specified.</returns>
    public static ILog GetLogger (Type type)
    {
      return s_current.GetLogger (type);
    }


    /// <summary>
    /// Initializes the current logging framework.
    /// </summary>
    public static void Initialize ()
    {
      //TODO: Test once current ILogManager can be set through configuration.
      s_current.Initialize ();
    }

    /// <summary>
    /// Initializes the current logging framework to log to the console.
    /// </summary>
    public static void InitializeConsole ()
    {
      s_current.InitializeConsole ();
    }
  }
}
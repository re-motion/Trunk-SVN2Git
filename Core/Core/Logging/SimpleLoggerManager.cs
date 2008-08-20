using System;
using System.IO;
using Remotion.Utilities;

namespace Remotion.Logging
{
  public class SimpleLoggerManager
  {
    public static ISimpleLogger GetLogger (bool enableConsole)
    {
      if (enableConsole)
      {
        return new SimpleLogger(enableConsole);
      }
      else
      {
        return new SimpleLoggerNull ();
      }
    }

    public static ISimpleLogger GetLogger (string fileName, bool enable)
    {
      if (enable)
      {
        return new SimpleLogger (fileName);
      }
      else
      {
        return new SimpleLoggerNull ();
      }
   }

  }
}
using System;
using System.Threading;

namespace Remotion.Development.UnitTesting
{
  public static class ThreadRunner
  {
    public static void Run (ThreadStart threadStart)
    {
      Exception lastException = null;
      UnhandledExceptionEventHandler unhandledExceptionEventHandler = delegate (object sender, UnhandledExceptionEventArgs e)
          {
            lastException = (Exception) e.ExceptionObject;
          };
      AppDomain.CurrentDomain.UnhandledException += unhandledExceptionEventHandler;

      Thread otherThread = new Thread (threadStart);

      try
      {
        otherThread.Start ();
        otherThread.Join ();
      }
      catch
      {
      }

      AppDomain.CurrentDomain.UnhandledException -= unhandledExceptionEventHandler;
      if (lastException != null)
        throw lastException;
    }
  }
}
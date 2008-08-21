/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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


    public static bool RunTimesOutAfterMilliseconds (ThreadStart threadStart, int timeoutMilliseconds)
    {
      return RunTimesOut(threadStart, new TimeSpan((long) (10000*timeoutMilliseconds)));
    }

    public static bool RunTimesOutAfterSeconds (ThreadStart threadStart, double timeoutSeconds)
    {
      return RunTimesOut(threadStart, new TimeSpan((long) (10000000*timeoutSeconds)));
    }

    public static bool RunTimesOut (ThreadStart threadStart, TimeSpan timeoutTimeSpan)
    {
      // Preserve exceptions thrown in thread, to be able to rethrow them.
      // TODO: To preserve also the callstack, rethrow new exception with thread exception as inner exception.
      Exception lastException = null;
      UnhandledExceptionEventHandler unhandledExceptionEventHandler = delegate (object sender, UnhandledExceptionEventArgs e)
      {
        lastException = (Exception) e.ExceptionObject;
      };

      AppDomain.CurrentDomain.UnhandledException += unhandledExceptionEventHandler;


      //Thread otherThread = new Thread ((ThreadStart) delegate { ThreadStartThreadAbortExceptionWrapper (threadStart); });
      Thread otherThread = new Thread ( () => ThreadStartThreadAbortExceptionWrapper (threadStart) );

      otherThread.Start ();
      bool timedOut = !otherThread.Join (timeoutTimeSpan);
      if (timedOut)
      {
        otherThread.Abort ();
      }

      AppDomain.CurrentDomain.UnhandledException -= unhandledExceptionEventHandler;
      if (lastException != null)
        throw lastException;
        
      return timedOut;
    }

    public static void ThreadStartThreadAbortExceptionWrapper (ThreadStart threadStart)
    {
      try
      {
        threadStart();
      }
      catch (System.Threading.ThreadAbortException e)
      {
        // Explicitely reset the ThreadAbortException
        //Console.WriteLine (">>> ThreadStartThreadAbortExceptionWrapper <<<");
        Thread.ResetAbort (); 
      }
    }

  }
}

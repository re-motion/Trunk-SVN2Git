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

      // Use anonymous delegate to catch and store exceptions from the thread in the current scope.
      Thread otherThread = 
        new Thread ((ThreadStart)
          delegate
          {
            try
            {
              threadStart();
            }
            catch (Exception e)
            {
              lastException = e;
            }
          }
         );


      try
      {
        otherThread.Start ();
        otherThread.Join ();
      }
      catch
      {
      }

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
      // TODO: To preserve the callstack, rethrow new exception with thread exception as inner exception.
      Exception lastException = null;

      Thread otherThread =
        new Thread ((ThreadStart)
          delegate
          {
            try
            {
              threadStart ();
            }
            catch (System.Threading.ThreadAbortException)
            {
              // Explicitely reset the ThreadAbortException
              Thread.ResetAbort ();
              // Do not report exception in lastException, since aborting is expected behavior.
            }
            catch (Exception e)
            {
              lastException = e;
            }
          }
         );


      otherThread.Start ();
      bool timedOut = !otherThread.Join (timeoutTimeSpan);
      if (timedOut)
      {
        otherThread.Abort ();
      }

      if (lastException != null)
        throw lastException;
        
      return timedOut;
    }


  }
}

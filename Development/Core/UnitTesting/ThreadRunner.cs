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
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting
{
  public class ThreadRunner
  {
    public static void Run (ThreadStart threadStart)
    {
      ArgumentUtility.CheckNotNull ("threadStart", threadStart);
      new ThreadRunner (threadStart).Run();
    }

    public static ThreadRunner WithMillisecondsTimeout (ThreadStart threadStart, double timeoutMilliseconds)
    {
      return new ThreadRunner (threadStart, TimeSpan.FromMilliseconds (timeoutMilliseconds));
    }

    public static ThreadRunner WithSecondsTimeout (ThreadStart threadStart, double timeoutSeconds)
    {
      return new ThreadRunner (threadStart, TimeSpan.FromSeconds (timeoutSeconds));
    }

    public static ThreadRunner WithTimeout (ThreadStart threadStart, TimeSpan timeout)
    {
      return new ThreadRunner (threadStart, timeout);
    }
    
    private readonly ThreadStart _threadStart;
    private readonly TimeSpan _timeoutTimeSpan;

    public ThreadRunner (ThreadStart threadStart)
      : this (threadStart, TimeSpan.FromMilliseconds (System.Threading.Timeout.Infinite))
    {
    }

    public ThreadRunner (ThreadStart threadStart, TimeSpan timeoutTimeSpan)
    {
      ArgumentUtility.CheckNotNull ("threadStart", threadStart);
      _threadStart = threadStart;
      _timeoutTimeSpan = timeoutTimeSpan;
    }

    public TimeSpan Timeout
    {
      get { return _timeoutTimeSpan; }
    }

    public bool Run ()
    {
      Exception lastException = null;

      // Use anonymous delegate to catch and store exceptions from the thread in the current scope.
      Thread otherThread = 
        new Thread ((ThreadStart)
          delegate
          {
            try
            {
              _threadStart();
            }
            catch (ThreadAbortException)
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
      bool timedOut = !JoinThread(otherThread);
      if (timedOut)
        AbortThread(otherThread);

      if (lastException != null)
        throw lastException; // TODO: wrap exception to preserve stack trace
      
      return timedOut;
    }

    protected virtual bool JoinThread (Thread otherThread)
    {
      return otherThread.Join (_timeoutTimeSpan);
    }

    protected virtual void AbortThread (Thread otherThread)
    {
      otherThread.Abort ();
    }
  }
}

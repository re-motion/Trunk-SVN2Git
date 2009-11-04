// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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

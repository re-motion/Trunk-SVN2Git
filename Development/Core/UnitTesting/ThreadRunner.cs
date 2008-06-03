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
  }
}

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
extern alias castle;

using System;
using System.Threading;
using castle::Castle.Core.Interceptor;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Assertion=Remotion.Utilities.Assertion;

namespace Remotion.UnitTests.Collections
{
  public class MonitorCheckingInterceptor : IInterceptor
  {
    private object _monitor;
    private object[] _expectedArguments;
    private object _returnValue;
    private bool _executed;

    public MonitorCheckingInterceptor ()
    {
    }

    public object[] ExpectedArguments
    {
      get { return _expectedArguments; }
      set { _expectedArguments = value; }
    }

    public object ReturnValue
    {
      get { return _returnValue; }
      set { _returnValue = value; }
    }

    public bool Executed
    {
      get { return _executed; }
    }

    public object Monitor
    {
      get { return _monitor; }
      set { _monitor = value; }
    }

    public void Intercept (IInvocation invocation)
    {
      EnsureMonitorIsHeld();
      Assert.That (invocation.Arguments, Is.EqualTo (_expectedArguments), "Arguments are not as expected");
      invocation.ReturnValue = _returnValue;
      _executed = true;
    }

    private void EnsureMonitorIsHeld ()
    {
      Assertion.IsNotNull(Monitor, "Monitor was not set");
      bool acquiredMonitor = false;
      Thread otherThread = new Thread ((ThreadStart) delegate
      {
        acquiredMonitor = System.Threading.Monitor.TryEnter (_monitor);
      });
      otherThread.Start();
      otherThread.Join();
      Thread.MemoryBarrier();
      Assert.That (acquiredMonitor, Is.False, "Lock was not taken");
    }
  }
}

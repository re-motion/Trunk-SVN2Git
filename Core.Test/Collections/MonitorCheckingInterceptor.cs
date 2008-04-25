using System;
using Castle.Core.Interceptor;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using System.Threading;
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
// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class OnExecutionPause : ScopedTransactionStrategyTestBase
  {
    private ScopedTransactionStrategyBase _strategy;

    public override void SetUp ()
    {
      base.SetUp();
      _strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
    }

    [Test]
    public void Test ()
    {
      InvokeOnExecutionPlay (_strategy);

      using (MockRepository.Ordered ())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionPause (Context, ExecutionListenerStub));
        ScopeMock.Expect (mock => mock.Leave ());
      }

      MockRepository.ReplayAll ();

      _strategy.OnExecutionPause (Context, ExecutionListenerStub);

      MockRepository.VerifyAll ();
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "OnExecutionPause may not be invoked unless OnExecutionPlay was called first.")]
    public void Test_WithNullScope ()
    {
      Assert.That (_strategy.Scope, Is.Null);
      _strategy.OnExecutionPause (Context, ExecutionListenerStub);
    }

    [Test]
    public void Test_ChildStrategyThrows ()
    {
      var innerException = new ApplicationException ("InnerListener Exception");

      InvokeOnExecutionPlay (_strategy);
      using (MockRepository.Ordered())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionPause (Context, ExecutionListenerStub)).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave ());
      }

      MockRepository.ReplayAll();

      try
      {
        _strategy.OnExecutionPause (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_ChildStrategyThrowsFatalException ()
    {
      var innerException = new WxeFatalExecutionException (new Exception ("ChildStrategy Exception"), null);

      InvokeOnExecutionPlay (_strategy);
      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionPause (Context, ExecutionListenerStub)).Throw (innerException);

      MockRepository.ReplayAll ();

      try
      {
        _strategy.OnExecutionPause (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (_strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_LeaveThrows ()
    {
      var innerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (_strategy);
      using (MockRepository.Ordered())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionPause (Context, ExecutionListenerStub));
        ScopeMock.Expect (mock => mock.Leave ()).Throw (innerException);
      }

      MockRepository.ReplayAll();

      try
      {
        _strategy.OnExecutionPause (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_ChildStrategyThrows_And_LeaveThrows ()
    {
      var innerException = new Exception ("InnerListener Exception");
      var outerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (_strategy);
      using (MockRepository.Ordered())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionPause (Context, ExecutionListenerStub)).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave ()).Throw (outerException);
      }

      MockRepository.ReplayAll();

      try
      {
        _strategy.OnExecutionPause (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
        Assert.That (actualException.OuterException, Is.SameAs (outerException));
      }

      MockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.Not.Null);
    }
  }
}

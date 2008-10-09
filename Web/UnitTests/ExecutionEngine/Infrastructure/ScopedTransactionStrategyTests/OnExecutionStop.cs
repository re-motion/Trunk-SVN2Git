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
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class OnExecutionStop : ScopedTransactionStrategyTestBase
  {
    [Test]
    public void Test_WithoutAutoCommit ()
    {
      var strategy = CreateScopedTransactionStrategy (false, NullTransactionStrategy.Null);

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
        ExecutionContextMock.Expect (mock => mock.GetOutParameters ()).Return (new object[0]);
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release());
      }

      MockRepository.ReplayAll();

      strategy.OnExecutionStop (Context, ExecutionListenerStub);

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithAutoCommit ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
        TransactionMock.Expect (mock => mock.Commit ());
        ExecutionContextMock.Expect (mock => mock.GetOutParameters()).Return (new object[0]);
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release());
      }

      MockRepository.ReplayAll();

      strategy.OnExecutionStop (Context, ExecutionListenerStub);

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithParentTransactionStrategy ()
    {
      var strategy = CreateScopedTransactionStrategy (true, OuterTransactionStrategyMock);
      var expectedObjects = new[] { new object() };

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
        TransactionMock.Expect (mock => mock.Commit ());

        ExecutionContextMock.Expect (mock => mock.GetOutParameters()).Return (expectedObjects);
        OuterTransactionStrategyMock.Expect (mock => mock.RegisterObjects (expectedObjects));

        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release());
      }

      MockRepository.ReplayAll();

      strategy.OnExecutionStop (Context, ExecutionListenerStub);

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithCommitTransactionOverride ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
        strategy.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "CommitTransaction"));
        ExecutionContextMock.Expect (mock => mock.GetOutParameters ()).Return (new object[0]);
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ());
      }

      MockRepository.ReplayAll ();

      strategy.OnExecutionStop (Context, ExecutionListenerStub);

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithReleaseTransactionOverride ()
    {
      var strategy = CreateScopedTransactionStrategy (false, NullTransactionStrategy.Null);

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
        ExecutionContextMock.Expect (mock => mock.GetOutParameters ()).Return (new object[0]);
        ScopeMock.Expect (mock => mock.Leave ());
        strategy.Expect (mock => PrivateInvoke.InvokeNonPublicMethod ( mock, "ReleaseTransaction"));
      }

      MockRepository.ReplayAll ();

      strategy.OnExecutionStop (Context, ExecutionListenerStub);

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "OnExecutionStop may not be invoked unless OnExecutionPlay was called first.")]
    public void Test_WithNullScope ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);

      Assert.That (strategy.Scope, Is.Null);

      strategy.OnExecutionStop (Context, ExecutionListenerStub);
    }

    [Test]
    public void Test_ChildStrategyThrows ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
      var innerException = new ApplicationException ("InnerListener Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub)).Throw (innerException);

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_ChildStrategyThrowsFatalException ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
      var innerException = new WxeFatalExecutionException (new Exception ("ChildStrategy Exception"), null);

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub)).Throw (innerException);

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_CommitThrows ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
      var innerException = new ApplicationException ("Commit Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
      TransactionMock.Expect (mock => mock.Commit ()).Throw (innerException);

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_GetOutParameterThrows ()
    {
      var strategy = CreateScopedTransactionStrategy (false, OuterTransactionStrategyMock);
      var innerException = new ApplicationException ("GetOutParameters Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
      ExecutionContextMock.Expect (mock => mock.GetOutParameters()).Throw (innerException);

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_RegisterObjectsThrows ()
    {
      var strategy = CreateScopedTransactionStrategy (false, OuterTransactionStrategyMock);
      var innerException = new ApplicationException ("GetOutParameters Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));

      ExecutionContextMock.Expect (mock => mock.GetOutParameters()).Return (new object[0]);
      OuterTransactionStrategyMock.Expect (mock => mock.RegisterObjects (Arg<IEnumerable>.Is.Anything)).Throw (innerException);

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_LeaveThrows ()
    {
      var strategy = CreateScopedTransactionStrategy (false, NullTransactionStrategy.Null);
      var innerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
      ExecutionContextMock.Expect (mock => mock.GetOutParameters ()).Return (new object[0]);
      ScopeMock.Expect (mock => mock.Leave()).Throw (innerException);

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_ReleaseThrows ()
    {
      var strategy = CreateScopedTransactionStrategy (false, NullTransactionStrategy.Null);
      var innerException = new Exception ("Release Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
      ExecutionContextMock.Expect (mock => mock.GetOutParameters ()).Return (new object[0]);
      ScopeMock.Expect (mock => mock.Leave());
      TransactionMock.Expect (mock => mock.Release()).Throw (innerException);

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }
  }
}
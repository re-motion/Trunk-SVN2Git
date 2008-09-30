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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.ExecutionEngine;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure.RootTransactionStrategyTests
{
  [TestFixture]
  public class OnExecutionStop:RootTransactionStrategyTestBase
  {


    [Test]
    public void Test_WithoutAutoCommit ()
    {
      var strategy = CreateRootTransactionStrategy (false);

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ());
      }

      MockRepository.ReplayAll ();

      strategy.OnExecutionStop (Context);

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithAutoCommit ()
    {
      var strategy = CreateRootTransactionStrategy (true);

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        TransactionMock.Expect (mock => mock.Commit ());
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ());
      }

      MockRepository.ReplayAll ();

      strategy.OnExecutionStop (Context);

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "OnExecutionStop may not be invoked unless OnExecutionPlay was called first.")]
    public void Test_WithNullScope ()
    {
      var strategy = CreateRootTransactionStrategy (true);
      
      Assert.That (strategy.Scope, Is.Null);
      
      strategy.OnExecutionStop (Context);
    }

    [Test]
    public void Test_InnerListenerThrows ()
    {
      var strategy = CreateRootTransactionStrategy (true);
      var innerException = new ApplicationException ("InnerListener Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context)).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ());
      }

      MockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (Context);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_CommitListenerThrows ()
    {
      var strategy = CreateRootTransactionStrategy (true);
      var innerException = new ApplicationException ("Commit Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        TransactionMock.Expect (mock => mock.Commit ()).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ());
      }

      MockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (Context);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_LeaveThrows ()
    {
      var strategy = CreateRootTransactionStrategy (false);
      var innerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        ScopeMock.Expect (mock => mock.Leave ()).Throw (innerException);
      }

      MockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (Context);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_ReleaseThrows ()
    {
      var strategy = CreateRootTransactionStrategy (false);
      var innerException = new Exception ("Release Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ()).Throw (innerException);
      }

      MockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (Context);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Not.Null);
    }

  }
}
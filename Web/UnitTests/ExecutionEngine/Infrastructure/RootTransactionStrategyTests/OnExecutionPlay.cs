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
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure.RootTransactionStrategyTests
{
  [TestFixture]
  public class OnExecutionPlay:RootTransactionStrategyTestBase
  {
    private RootTransactionStrategy _strategy;

    public override void SetUp ()
    {
      base.SetUp ();

      _strategy = CreateRootTransactionStrategy (true, NullTransactionStrategy.Null);
    }

    [Test]
    public void Test ()
    {
      using (MockRepository.Ordered ())
      {
        TransactionMock.Expect (mock => mock.EnterScope ()).Return (ScopeMock);
        ExecutionListenerMock.Expect (mock => mock.OnExecutionPlay (Context));
      }

      MockRepository.ReplayAll ();

      _strategy.OnExecutionPlay (Context, ExecutionListenerMock);

      MockRepository.VerifyAll ();
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_WithChildStrategy ()
    {
      _strategy.SetChild (ChildTransactionStrategyMock);

      using (MockRepository.Ordered ())
      {
        TransactionMock.Expect (mock => mock.EnterScope ()).Return (ScopeMock);
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionPlay (Context, ExecutionListenerMock));
      }

      MockRepository.ReplayAll ();

      _strategy.OnExecutionPlay (Context, ExecutionListenerMock);

      MockRepository.VerifyAll ();
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_AfterPause ()
    {
      InvokeOnExecutionPlay (_strategy);
      InvokeOnExecutionPause (_strategy);
      InvokeOnExecutionPlay (_strategy);
    }

    [Test]
    public void Test_EnterScopeThrows ()
    {
      var innerException = new Exception ("Enter Scope Exception");
      TransactionMock.Expect (mock => mock.EnterScope ()).Throw (innerException);

      MockRepository.ReplayAll ();

      try
      {
        _strategy.OnExecutionPlay (Context, ExecutionListenerMock);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
            "OnExecutionPlay may not be invoked twice without calling OnExecutionStop, OnExecutionPause, or OnExecutionFail in-between.")]
    public void Test_WithScope ()
    {
      InvokeOnExecutionPlay (_strategy);
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
      _strategy.OnExecutionPlay (Context, ExecutionListenerMock);
    }

    [Test]
    public void Test_InnerListenerThrows ()
    {
      var innerException = new ApplicationException ("InnerListener Exception");
      
      using (MockRepository.Ordered ())
      {
        TransactionMock.Expect (mock => mock.EnterScope ()).Return (ScopeMock);
        ExecutionListenerMock.Expect (mock => mock.OnExecutionPlay (Context)).Throw (innerException);
      }

      MockRepository.ReplayAll ();

      try
      {
        _strategy.OnExecutionPlay (Context, ExecutionListenerMock);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (_strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_ChildStrategyThrowsFatalException ()
    {
      var innerException = new WxeFatalExecutionException (new Exception ("InnerListener Exception"), null);

      using (MockRepository.Ordered())
      {
        TransactionMock.Expect (mock => mock.EnterScope()).Return (ScopeMock);
        ExecutionListenerMock.Expect (mock => mock.OnExecutionPlay (Context)).Throw (innerException);
      }

      MockRepository.ReplayAll();

      try
      {
        _strategy.OnExecutionPlay (Context, ExecutionListenerMock);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.Not.Null);
    }
  }
}
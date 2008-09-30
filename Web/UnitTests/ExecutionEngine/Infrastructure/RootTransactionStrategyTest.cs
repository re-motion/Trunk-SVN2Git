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
using Remotion.Data;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class RootTransactionStrategyTest
  {
    private IWxeFunctionExecutionListener _executionListenerMock;
    private RootTransactionStrategy _strategy;
    private ITransactionScopeManager _scopeManagerMock;
    private ITransactionScope _scopeMock;
    private ITransaction _transactionMock;
    private WxeContext _context;
    private MockRepository _mockRepository;

    [SetUp]
    public void SetUp ()
    {
      WxeContextFactory wxeContextFactory = new WxeContextFactory();
      _context = wxeContextFactory.CreateContext (new TestFunction());

      _mockRepository = new MockRepository();
      _executionListenerMock = _mockRepository.StrictMock<IWxeFunctionExecutionListener>();
      _transactionMock = _mockRepository.StrictMock<ITransaction>();

      _scopeMock = _mockRepository.StrictMock<ITransactionScope>();

      _scopeManagerMock = _mockRepository.StrictMock<ITransactionScopeManager>();
      _strategy = CreateRootTransactionStrategy (true);
    }

    [Test]
    public void GetInnerListener ()
    {
      Assert.That (_strategy.InnerListener, Is.SameAs (_executionListenerMock));
    }

    [Test]
    public void GetAutoCommit ()
    {
      Assert.That (_strategy.AutoCommit, Is.True);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_strategy.IsNull, Is.False);
    }

    [Test]
    public void GetScopeManager ()
    {
      Assert.That (_strategy.ScopeManager, Is.SameAs (_scopeManagerMock));
    }

    [Test]
    public void GetTransaction ()
    {
      Assert.That (_strategy.Transaction, Is.SameAs (_transactionMock));
    }


    [Test]
    public void OnExecutionPlay ()
    {
      using (_mockRepository.Ordered())
      {
        _transactionMock.Expect (mock => mock.EnterScope()).Return (_scopeMock);
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
      }

      _mockRepository.ReplayAll();

      _strategy.OnExecutionPlay (_context);

      _mockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.SameAs (_scopeMock));
    }

    [Test]
    public void OnExecutionPlay_AfterPause ()
    {
      InvokeOnExecutionPlay (_strategy);
      InvokeOnExecutionPause (_strategy);
      InvokeOnExecutionPlay (_strategy);
    }

    [Test]
    public void OnExecutionPlay_EnterScopeThrows ()
    {
      var innerException = new Exception ("Enter Scope Exception");
      _transactionMock.Expect (mock => mock.EnterScope()).Throw (innerException);

      _mockRepository.ReplayAll();

      try
      {
        _strategy.OnExecutionPlay (_context);
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
    public void OnExecutionPlay_WithScope ()
    {
      InvokeOnExecutionPlay (_strategy);
      Assert.That (_strategy.Scope, Is.SameAs (_scopeMock));
      _strategy.OnExecutionPlay (_context);
    }

    [Test]
    [Ignore]
    public void OnExecutionPlay_InnerListenerThrows ()
    {
    }


    [Test]
    public void OnExecutionStop_WithoutAutoCommit ()
    {
      var strategy = CreateRootTransactionStrategy (false);

      InvokeOnExecutionPlay (strategy);
      using (_mockRepository.Ordered())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionStop (_context));
        _scopeMock.Expect (mock => mock.Leave());
        _transactionMock.Expect (mock => mock.Release());
      }

      _mockRepository.ReplayAll();

      strategy.OnExecutionStop (_context);

      _mockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void OnExecutionStop_WithAutoCommit ()
    {
      var strategy = CreateRootTransactionStrategy (true);

      InvokeOnExecutionPlay (strategy);
      using (_mockRepository.Ordered())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionStop (_context));
        _transactionMock.Expect (mock => mock.Commit());
        _scopeMock.Expect (mock => mock.Leave());
        _transactionMock.Expect (mock => mock.Release());
      }

      _mockRepository.ReplayAll();

      strategy.OnExecutionStop (_context);

      _mockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "OnExecutionStop may not be invoked unless OnExecutionPlay was called first.")]
    public void OnExecutionStop_WithNullScope ()
    {
      Assert.That (_strategy.Scope, Is.Null);
      _strategy.OnExecutionStop (_context);
    }

    [Test]
    public void OnExecutionStop_InnerListenerThrows ()
    {
      var strategy = CreateRootTransactionStrategy (true);
      var innerException = new ApplicationException ("InnerListener Exception");

      InvokeOnExecutionPlay (strategy);
      using (_mockRepository.Ordered ())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionStop (_context)).Throw (innerException);
        _scopeMock.Expect (mock => mock.Leave ());
        _transactionMock.Expect (mock => mock.Release ());
      }

      _mockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (_context);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      _mockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void OnExecutionStop_CommitListenerThrows ()
    {
      var strategy = CreateRootTransactionStrategy (true);
      var innerException = new ApplicationException ("Commit Exception");

      InvokeOnExecutionPlay (strategy);
      using (_mockRepository.Ordered ())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionStop (_context));
        _transactionMock.Expect (mock => mock.Commit()).Throw (innerException);
        _scopeMock.Expect (mock => mock.Leave ());
        _transactionMock.Expect (mock => mock.Release ());
      }

      _mockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (_context);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      _mockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void OnExecutionStop_LeaveThrows ()
    {
      var strategy = CreateRootTransactionStrategy (false);
      var innerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (strategy);
      using (_mockRepository.Ordered ())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionStop (_context));
        _scopeMock.Expect (mock => mock.Leave ()).Throw (innerException);
      }

      _mockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (_context);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      _mockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void OnExecutionStop_ReleaseThrows ()
    {
      var strategy = CreateRootTransactionStrategy (false);
      var innerException = new Exception ("Release Exception");

      InvokeOnExecutionPlay (strategy);
      using (_mockRepository.Ordered ())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionStop (_context));
        _scopeMock.Expect (mock => mock.Leave ());
        _transactionMock.Expect (mock => mock.Release()).Throw (innerException);
      }

      _mockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (_context);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      _mockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void OnExecutionPause ()
    {
      InvokeOnExecutionPlay (_strategy);
      using (_mockRepository.Ordered())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPause (_context));
        _scopeMock.Expect (mock => mock.Leave());
      }

      _mockRepository.ReplayAll();

      _strategy.OnExecutionPause (_context);

      _mockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "OnExecutionPause may not be invoked unless OnExecutionPlay was called first.")]
    public void OnExecutionPause_WithNullScope ()
    {
      Assert.That (_strategy.Scope, Is.Null);
      _strategy.OnExecutionPause (_context);
    }

    [Test]
    public void OnExecutionPause_InnerListenerThrows ()
    {
      var innerException = new ApplicationException ("InnerListener Exception");

      InvokeOnExecutionPlay (_strategy);
      using (_mockRepository.Ordered())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPause (_context)).Throw (innerException);
        _scopeMock.Expect (mock => mock.Leave());
      }

      _mockRepository.ReplayAll();

      try
      {
        _strategy.OnExecutionPause (_context);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      _mockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    public void OnExecutionPause_LeaveThrows ()
    {
      var innerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (_strategy);
      using (_mockRepository.Ordered())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPause (_context));
        _scopeMock.Expect (mock => mock.Leave()).Throw (innerException);
      }

      _mockRepository.ReplayAll();

      try
      {
        _strategy.OnExecutionPause (_context);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      _mockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void OnExecutionPause_InnerListenerThrows_And_LeaveThrows ()
    {
      var innerException = new Exception ("InnerListener Exception");
      var outerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (_strategy);
      using (_mockRepository.Ordered())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPause (_context)).Throw (innerException);
        _scopeMock.Expect (mock => mock.Leave()).Throw (outerException);
      }

      _mockRepository.ReplayAll();

      try
      {
        _strategy.OnExecutionPause (_context);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
        Assert.That (actualException.OuterException, Is.SameAs (outerException));
      }

      _mockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.Not.Null);
    }


    //[Test]
    //public void OnExecutionFail ()
    //{
    //  var exception = new Exception ();
    //  ((IWxeFunctionExecutionListener) _strategy).OnExecutionFail (_context, exception);
    //  _executionListenerMock.AssertWasCalled (mock => mock.OnExecutionFail (_context, exception));
    //}


    private RootTransactionStrategy CreateRootTransactionStrategy (bool autoCommit)
    {
      _scopeManagerMock.BackToRecord();
      _scopeManagerMock.Expect (stub => stub.CreateRootTransaction()).Return (_transactionMock).Repeat.Once();
      _scopeManagerMock.Replay();

      return new RootTransactionStrategy (autoCommit, _executionListenerMock, _scopeManagerMock);
    }

    private void InvokeOnExecutionPlay (RootTransactionStrategy strategy)
    {
      _transactionMock.Stub (stub => stub.EnterScope()).Return (_scopeMock);
      _executionListenerMock.Stub (stub => stub.OnExecutionPlay (_context));
      _mockRepository.Replay (_transactionMock);
      _mockRepository.Replay (_executionListenerMock);

      strategy.OnExecutionPlay (_context);

      _mockRepository.BackToRecord (_transactionMock);
      _mockRepository.BackToRecord (_executionListenerMock);
    }

    private void InvokeOnExecutionPause (RootTransactionStrategy strategy)
    {
      _executionListenerMock.Stub (stub => stub.OnExecutionPause (_context));
      _scopeMock.Stub (stub => stub.Leave());

      _mockRepository.Replay (_executionListenerMock);
      _mockRepository.Replay (_scopeMock);

      strategy.OnExecutionPause (_context);

      _mockRepository.BackToRecord (_executionListenerMock);
      _mockRepository.BackToRecord (_scopeMock);
    }
  }
}
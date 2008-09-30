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
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure.RootTransactionStrategyTests
{
  [TestFixture]
  public class Reset : RootTransactionStrategyTestBase
  {
    private RootTransactionStrategy _strategy;

    public override void SetUp ()
    {
      base.SetUp();
      _strategy = CreateRootTransactionStrategy (true);
    }

    [Test]
    public void Test_WithoutScope ()
    {
      ScopeManagerMock.BackToRecord();
      ITransaction newTransactionMock = MockRepository.StrictMock<ITransaction>();
      using (MockRepository.Ordered())
      {
        TransactionMock.Expect (mock => mock.Release());
        ScopeManagerMock.Expect (mock => mock.CreateRootTransaction()).Return (newTransactionMock);
      }
      Assert.That (_strategy.Scope, Is.Null);
      MockRepository.ReplayAll();

      _strategy.Reset();

      MockRepository.VerifyAll();
      Assert.That (_strategy.Transaction, Is.SameAs (newTransactionMock));
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithoutScope_And_ReleaseThrows ()
    {
      ScopeManagerMock.BackToRecord();
      var exception = new ApplicationException ("Release Exception");
      using (MockRepository.Ordered())
      {
        TransactionMock.Expect (mock => mock.Release()).Throw (exception);
      }
      Assert.That (_strategy.Scope, Is.Null);
      MockRepository.ReplayAll();

      try
      {
        _strategy.Reset();
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (exception));
      }
      MockRepository.VerifyAll();
      Assert.That (_strategy.Transaction, Is.SameAs (TransactionMock));
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithoutScope_And_CreateRootTransactionThrows ()
    {
      ScopeManagerMock.BackToRecord ();
      var exception = new ApplicationException ("CreateRootTransaction Exception");
      using (MockRepository.Ordered ())
      {
        TransactionMock.Expect (mock => mock.Release ());
        ScopeManagerMock.Expect (mock => mock.CreateRootTransaction()).Throw (exception);
      }
      Assert.That (_strategy.Scope, Is.Null);
      MockRepository.ReplayAll ();

      try
      {
        _strategy.Reset ();
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (exception));
      }
      MockRepository.VerifyAll ();
      Assert.That (_strategy.Transaction, Is.SameAs (TransactionMock));
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithScope ()
    {
      InvokeOnExecutionPlay (_strategy);
      ScopeManagerMock.BackToRecord ();
      var newTransactionMock = MockRepository.StrictMock<ITransaction> ();
      var newScopeMock = MockRepository.StrictMock<ITransactionScope> ();
      using (MockRepository.Ordered ())
      {
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release ());
        ScopeManagerMock.Expect (mock => mock.CreateRootTransaction ()).Return (newTransactionMock);
        newTransactionMock.Expect (mock => mock.EnterScope ()).Return (newScopeMock);
      }
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
      MockRepository.ReplayAll ();

      _strategy.Reset ();

      MockRepository.VerifyAll ();
      Assert.That (_strategy.Transaction, Is.SameAs (newTransactionMock));
      Assert.That (_strategy.Scope, Is.SameAs (newScopeMock));
    }

    [Test]
    public void Test_WithScope_And_ReleaseThrows ()
    {
      InvokeOnExecutionPlay (_strategy);
      ScopeManagerMock.BackToRecord ();
      var exception = new ApplicationException ("Release Exception");
      using (MockRepository.Ordered ())
      {
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ()).Throw (exception);
      }
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
      MockRepository.ReplayAll ();

      try
      {
        _strategy.Reset ();
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (exception));
      }
      MockRepository.VerifyAll ();
      Assert.That (_strategy.Transaction, Is.SameAs (TransactionMock));
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_WithScope_And_LeaveThrows ()
    {
      InvokeOnExecutionPlay (_strategy);
      ScopeManagerMock.BackToRecord ();
      var exception = new ApplicationException ("Leave Exception");
      using (MockRepository.Ordered ())
      {
        ScopeMock.Expect (mock => mock.Leave ()).Throw (exception);
      }
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
      MockRepository.ReplayAll ();

      try
      {
        _strategy.Reset ();
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (exception));
      }
      MockRepository.VerifyAll ();
      Assert.That (_strategy.Transaction, Is.SameAs (TransactionMock));
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_WithScope_And_CreateRootTransactionThrows ()
    {
      InvokeOnExecutionPlay (_strategy);
      ScopeManagerMock.BackToRecord ();
      var exception = new ApplicationException ("CreateRootTransaction Exception");
      using (MockRepository.Ordered ())
      {
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ());
        ScopeManagerMock.Expect (mock => mock.CreateRootTransaction()).Throw (exception);
      }
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
      MockRepository.ReplayAll ();

      try
      {
        _strategy.Reset ();
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (exception));
      }
      MockRepository.VerifyAll ();
      Assert.That (_strategy.Transaction, Is.SameAs (TransactionMock));
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
    }
  }
}
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

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class Reset : ScopedTransactionStrategyTestBase
  {
    private ScopedTransactionStrategyBase _strategy;

    public override void SetUp ()
    {
      base.SetUp();
      _strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
    }

    [Test]
    public void Test_WithoutScope ()
    {
      TransactionMock.BackToRecord ();
      using (MockRepository.Ordered())
      {
        TransactionMock.Expect (mock => mock.Reset ());
      }
      Assert.That (_strategy.Scope, Is.Null);
      MockRepository.ReplayAll();

      _strategy.Reset();

      MockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithoutScope_And_ResetThrows ()
    {
      var exception = new ApplicationException ("Reset Exception");
      TransactionMock.Expect (mock => mock.Reset ()).Throw (exception);
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
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithScope ()
    {
      InvokeOnExecutionPlay (_strategy);
      TransactionMock.BackToRecord ();
      var newScopeMock = MockRepository.StrictMock<ITransactionScope>();

      using (MockRepository.Ordered ())
      {
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Reset ());
        TransactionMock.Expect (mock => mock.EnterScope ()).Return (newScopeMock);
      }
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
      MockRepository.ReplayAll();

      _strategy.Reset();

      MockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.SameAs (newScopeMock));
    }

    [Test]
    public void Test_WithScope_And_LeaveThrows ()
    {
      InvokeOnExecutionPlay (_strategy);
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
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_WithScope_And_ResetThrows ()
    {
      InvokeOnExecutionPlay (_strategy);
      var exception = new ApplicationException ("Reset Exception");
      using (MockRepository.Ordered())
      {
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Reset ()).Throw (exception);
      }
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
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
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
    }
  }
}
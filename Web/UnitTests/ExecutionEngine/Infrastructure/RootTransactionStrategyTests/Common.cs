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
using Remotion.Data;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure.RootTransactionStrategyTests
{
  [TestFixture]
  public class Common : RootTransactionStrategyTestBase
  {
    private RootTransactionStrategy _strategy;

    public override void SetUp ()
    {
      base.SetUp();
      _strategy = CreateRootTransactionStrategy (true, NullTransactionStrategy.Null);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_strategy.ExecutionContext, Is.SameAs (ExecutionContextMock));
      Assert.That (_strategy.AutoCommit, Is.True);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_strategy.IsNull, Is.False);
    }

    [Test]
    public void GetTransaction ()
    {
      TransactionMock.Expect (mock => mock.To<ITransaction>()).Return (TransactionMock);
      TransactionMock.Replay();
      Assert.That (_strategy.GetNativeTransaction<ITransaction>(), Is.SameAs (TransactionMock));
    }

    [Test]
    public void Commit ()
    {
      TransactionMock.Expect (mock => mock.Commit());
      MockRepository.ReplayAll();

      _strategy.Commit();

      MockRepository.VerifyAll();
    }

    [Test]
    public void Rollback ()
    {
      TransactionMock.Expect (mock => mock.Rollback());
      MockRepository.ReplayAll();

      _strategy.Rollback();

      MockRepository.VerifyAll();
    }

    [Test]
    public void CreateExecutionListener ()
    {
      IWxeFunctionExecutionListener executionListener = _strategy.CreateExecutionListener (ExecutionListenerMock);

      Assert.That (executionListener, Is.InstanceOfType (typeof (TransactionExecutionListener)));
      var transactionExecutionListener = (TransactionExecutionListener) executionListener;
      Assert.That (transactionExecutionListener.InnerListener, Is.SameAs (ExecutionListenerMock));
      Assert.That (transactionExecutionListener.TransactionStrategy, Is.SameAs (_strategy));
    }

    [Test]
    public void SetChild ()
    {
      _strategy.SetChild (ChildTransactionStrategyMock);

      Assert.That (_strategy.Child, Is.SameAs (ChildTransactionStrategyMock));
    }
  }
}
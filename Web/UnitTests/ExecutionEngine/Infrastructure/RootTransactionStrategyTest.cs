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

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class RootTransactionStrategyTest
  {
    private RootTransactionStrategy _strategy;
    private ITransaction _transactionMock;
    private TransactionStrategyBase _outerTransactionStrategyStub;
    private IWxeFunctionExecutionContext _executionContextStub;

    [SetUp]
    public void SetUp ()
    {
      _transactionMock = MockRepository.GenerateMock<ITransaction>();
      _outerTransactionStrategyStub = MockRepository.GenerateStub<TransactionStrategyBase>();
      _executionContextStub = MockRepository.GenerateStub<IWxeFunctionExecutionContext>();
      _executionContextStub.Stub (stub => stub.GetInParameters()).Return (new object[0]);

      _strategy = new RootTransactionStrategy (true, _transactionMock, _outerTransactionStrategyStub, _executionContextStub);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_strategy.Transaction, Is.SameAs (_transactionMock));
      Assert.That (_strategy.OuterTransactionStrategy, Is.SameAs (_outerTransactionStrategyStub));
      Assert.That (_strategy.ExecutionContext, Is.SameAs (_executionContextStub));
      Assert.That (_strategy.AutoCommit, Is.True);
      Assert.That (_strategy.IsNull, Is.False);
    }

    [Test]
    public void CreateExecutionListener ()
    {
      var innerExecutionListenerStub = MockRepository.GenerateStub<IWxeFunctionExecutionListener>();
      IWxeFunctionExecutionListener executionListener = _strategy.CreateExecutionListener (innerExecutionListenerStub);

      Assert.That (executionListener, Is.InstanceOfType (typeof (TransactionExecutionListener)));
      var transactionExecutionListener = (TransactionExecutionListener) executionListener;
      Assert.That (transactionExecutionListener.InnerListener, Is.SameAs (innerExecutionListenerStub));
      Assert.That (transactionExecutionListener.TransactionStrategy, Is.SameAs (_strategy));
    }

    [Test]
    public void CreateChildTransactionStrategy ()
    {
      var childTransaction = MockRepository.GenerateStub<ITransaction>();
      _transactionMock.Expect (mock => mock.CreateChild()).Return (childTransaction);

      var childExecutionContextStub = MockRepository.GenerateStub<IWxeFunctionExecutionContext>();
      childExecutionContextStub.Stub (stub => stub.GetInParameters ()).Return (new object[0]);

      ChildTransactionStrategy childTransactionStrategy = _strategy.CreateChildTransactionStrategy (true, childExecutionContextStub);

      Assert.That (childTransactionStrategy, Is.Not.Null);
      Assert.That (childTransactionStrategy.AutoCommit, Is.True);
      Assert.That (childTransactionStrategy.Transaction, Is.SameAs (childTransaction));
      Assert.That (childTransactionStrategy.OuterTransactionStrategy, Is.SameAs (_strategy));
      Assert.That (childTransactionStrategy.ExecutionContext, Is.SameAs (childExecutionContextStub));
      Assert.That (_strategy.Child, Is.SameAs (childTransactionStrategy));
    }
  }
}
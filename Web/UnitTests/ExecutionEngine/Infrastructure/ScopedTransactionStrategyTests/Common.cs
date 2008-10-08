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

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class Common : ScopedTransactionStrategyTestBase
  {
    private ScopedTransactionStrategyBase _strategy;

    public override void SetUp ()
    {
      base.SetUp();
      ExecutionContextMock.BackToRecord();
      ExecutionContextMock.Stub (stub => stub.GetInParameters()).Return (new object[0]).Repeat.Any();
      ExecutionContextMock.Replay();

      TransactionMock.BackToRecord();
      TransactionMock.Stub (stub => stub.RegisterObjects (Arg<IEnumerable>.Is.NotNull));
      TransactionMock.Replay();

      _strategy = MockRepository.PartialMock<ScopedTransactionStrategyBase> (
          true, TransactionMock, OuterTransactionStrategyMock, ExecutionContextMock);
      _strategy.Replay();

      ExecutionContextMock.BackToRecord();
      TransactionMock.BackToRecord();
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_strategy.ExecutionContext, Is.SameAs (ExecutionContextMock));
      Assert.That (_strategy.AutoCommit, Is.True);
      Assert.That (_strategy.IsNull, Is.False);
      Assert.That (_strategy.OuterTransactionStrategy, Is.SameAs (OuterTransactionStrategyMock));
      Assert.That (_strategy.Child, Is.SameAs (NullTransactionStrategy.Null));
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
    public void GetChild ()
    {
      SetChild (_strategy, ChildTransactionStrategyMock);

      Assert.That (_strategy.Child, Is.SameAs (ChildTransactionStrategyMock));
    }

    [Test]
    public void CreateChildTransactionStrategy ()
    {
      var childTransaction = MockRepository.GenerateStub<ITransaction>();
      TransactionMock.Expect (mock => mock.CreateChild()).Return (childTransaction);

      var childExecutionContextStub = MockRepository.GenerateStub<IWxeFunctionExecutionContext>();
      childExecutionContextStub.Stub (stub => stub.GetInParameters()).Return (new object[0]);

      MockRepository.ReplayAll();

      ScopedTransactionStrategyBase childTransactionStrategy = _strategy.CreateChildTransactionStrategy (true, childExecutionContextStub);

      MockRepository.VerifyAll();
      Assert.That (childTransactionStrategy, Is.InstanceOfType (typeof (ChildTransactionStrategy)));
      Assert.That (childTransactionStrategy.AutoCommit, Is.True);
      Assert.That (childTransactionStrategy.Transaction, Is.SameAs (childTransaction));
      Assert.That (childTransactionStrategy.OuterTransactionStrategy, Is.SameAs (_strategy));
      Assert.That (childTransactionStrategy.ExecutionContext, Is.SameAs (childExecutionContextStub));
      Assert.That (_strategy.Child, Is.SameAs (childTransactionStrategy));
    }

    [Test]
    public void UnregisterChildTransactionStrategy ()
    {
      SetChild (_strategy, ChildTransactionStrategyMock);
      Assert.That (_strategy.Child, Is.SameAs (ChildTransactionStrategyMock));

      _strategy.UnregisterChildTransactionStrategy (ChildTransactionStrategyMock);

      Assert.That (_strategy.Child, Is.SameAs (NullTransactionStrategy.Null));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The child transaction strategy passed for de-registration is not the same that is presently associated with the transaction strategy.")]
    public void UnregisterChildTransactionStrategy_Throws ()
    {
      SetChild (_strategy, ChildTransactionStrategyMock);
      Assert.That (_strategy.Child, Is.SameAs (ChildTransactionStrategyMock));

      try
      {
        _strategy.UnregisterChildTransactionStrategy (NullTransactionStrategy.Null);
      }
      finally
      {
        Assert.That (_strategy.Child, Is.SameAs (ChildTransactionStrategyMock));
      }
    }
  }
}
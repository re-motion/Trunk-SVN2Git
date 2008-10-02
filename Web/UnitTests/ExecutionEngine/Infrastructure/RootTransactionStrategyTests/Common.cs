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
      _strategy = CreateRootTransactionStrategy (true);
    }

    [Test]
    public void GetInnerListener ()
    {
      Assert.That (_strategy.InnerListener, Is.SameAs (ExecutionListenerMock));
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
    public void GetTransaction ()
    {
      TransactionManagerMock.Stub (stub => stub.Transaction).Return (TransactionMock);
      Assert.That (_strategy.Transaction, Is.SameAs (TransactionMock));
    }

    [Test]
    public void Commit ()
    {
      TransactionManagerMock.Stub (stub => stub.Transaction).Return (TransactionMock);
      TransactionMock.Expect (mock => mock.Commit ());
      MockRepository.ReplayAll();

      _strategy.Commit();

      MockRepository.VerifyAll();
    }

    [Test]
    public void Rollback ()
    {
      TransactionManagerMock.Stub (stub => stub.Transaction).Return (TransactionMock);
      TransactionMock.Expect (mock => mock.Rollback ());
      MockRepository.ReplayAll();

      _strategy.Rollback();

      MockRepository.VerifyAll();
    }

    [Test]
    public void Initialize_WithInParameters ()
    {
      var expectedInParamters = new object[0];

      ExecutionContextMock.BackToRecord();
      TransactionManagerMock.BackToRecord();

      using (MockRepository.Ordered())
      {
        TransactionManagerMock.Expect (mock => mock.InitializeTransaction());
        ExecutionContextMock.Expect (mock => mock.GetInParameters()).Return (expectedInParamters);
        TransactionManagerMock.Expect (mock => mock.RegisterObjects (expectedInParamters));
      }

      TransactionManagerMock.Replay();
      ExecutionContextMock.Replay();


      new RootTransactionStrategy (false, ExecutionListenerMock, TransactionManagerMock, ExecutionContextMock);

      ExecutionContextMock.VerifyAllExpectations();
      TransactionManagerMock.VerifyAllExpectations();
    }
  }
}
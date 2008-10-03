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
      Assert.That (_strategy.Transaction, Is.SameAs (TransactionMock));
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
    public void RegisterObjects ()
    {
      var expectedObject1 = new object();
      var expectedObject2 = new object();

      SetupForInitializationWithRegisterObjects (
          new[] { expectedObject1, expectedObject2 },
          new[] { expectedObject1, expectedObject2 });
      MockRepository.ReplayAll();

      new RootTransactionStrategy (false, TransactionMock, ExecutionContextMock, ExecutionListenerMock);

      MockRepository.VerifyAll();
    }

    [Test]
    public void RegisterObjects_WithNullValue ()
    {
      var expectedObject1 = new object();
      var expectedObject2 = new object();

      SetupForInitializationWithRegisterObjects (
          new[] { expectedObject1, null, expectedObject2 },
          new[] { expectedObject1, expectedObject2 });
      MockRepository.ReplayAll();

      new RootTransactionStrategy (false, TransactionMock, ExecutionContextMock, ExecutionListenerMock);

      MockRepository.VerifyAll();
    }

    [Test]
    public void RegisterObjects_Recursively ()
    {
      var expectedObject1 = new object();
      var expectedObject2 = new object();
      var expectedObject3 = new object();

      SetupForInitializationWithRegisterObjects (
          new[] { expectedObject1, new[] { expectedObject2, expectedObject3 } },
          new[] { expectedObject1, expectedObject2, expectedObject3 });
      MockRepository.ReplayAll();

      new RootTransactionStrategy (false, TransactionMock, ExecutionContextMock, ExecutionListenerMock);

      MockRepository.VerifyAll();
    }

    [Test]
    public void RegisterObjects_RecursivelyWithNullValue ()
    {
      var expectedObject1 = new object();
      var expectedObject2 = new object();
      var expectedObject3 = new object();

      SetupForInitializationWithRegisterObjects (
          new[] { expectedObject1, new[] { expectedObject2, null, expectedObject3 } },
          new[] { expectedObject1, expectedObject2, expectedObject3 });
      MockRepository.ReplayAll();

      new RootTransactionStrategy (false, TransactionMock, ExecutionContextMock, ExecutionListenerMock);

      MockRepository.VerifyAll();
    }

    private void SetupForInitializationWithRegisterObjects (object[] actualInParamters, object[] expectedInParamters)
    {
      using (MockRepository.Ordered())
      {
        ExecutionContextMock.Expect (mock => mock.GetInParameters()).Return (actualInParamters);
        TransactionMock.Expect (mock => mock.RegisterObjects (Arg<IEnumerable>.List.ContainsAll (expectedInParamters)));
      }
    }
  }
}
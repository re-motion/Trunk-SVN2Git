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

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class TransactionManagerTest
  {
    private ITransaction _transactionMock;
    private ITransactionManager _transactionManager;
    private ITransactionScopeManager _scopeManagerMock;

    [SetUp]
    public void SetUp ()
    {
      _transactionMock = MockRepository.GenerateMock<ITransaction>();
      _scopeManagerMock = MockRepository.GenerateStub<ITransactionScopeManager>();
      
      _scopeManagerMock.Stub (stub => stub.CreateRootTransaction()).Return (_transactionMock);
      _transactionManager = new TransactionManager (_scopeManagerMock);
      _transactionManager.InitializeTransaction ();
      _scopeManagerMock.BackToRecord ();
    }

    [Test]
    public void InitializeTransaction ()
    {
      Assert.That (_transactionManager.Transaction, Is.SameAs (_transactionMock));

      ITransaction transactionStub = MockRepository.GenerateStub<ITransaction>();
      _scopeManagerMock.Stub (stub => stub.CreateRootTransaction ()).Return (transactionStub);
      _scopeManagerMock.Replay();

      _transactionManager.InitializeTransaction();

      Assert.That (_transactionManager.Transaction, Is.SameAs (transactionStub));
    }

    [Test]
    public void EnterScope ()
    {
      var scopeStub = MockRepository.GenerateStub<ITransactionScope>();
      _transactionMock.Expect (mock => mock.EnterScope ()).Return (scopeStub);

      var actualScope = _transactionManager.EnterScope ();
      Assert.That (actualScope, Is.SameAs (scopeStub));
    }

    [Test]
    public void ReleaseTransaction ()
    {
      _transactionManager.ReleaseTransaction();
      _transactionMock.AssertWasCalled (mock => mock.Release());
    }

    [Test]
    public void ResetTransaction ()
    {
      _transactionManager.ResetTransaction ();
      _transactionMock.AssertWasCalled (mock => mock.Reset ());
    }

    [Test]
    public void RegisterObjects ()
    {
      var expectedObject1 = new object ();
      var expectedObject2 = new object ();
      IEnumerable objects = new[] { expectedObject1, expectedObject2 };

      _transactionManager.RegisterObjects (objects);
      _transactionMock.AssertWasCalled (mock => mock.RegisterObjects (Arg<IEnumerable>.List.ContainsAll (new[] { expectedObject1, expectedObject2 })));
    }

    [Test]
    public void RegisterObjects_WithNullValue ()
    {
      var expectedObject1 = new object ();
      var expectedObject2 = new object ();
      IEnumerable objects = new[] { expectedObject1, null, expectedObject2 };

      _transactionManager.RegisterObjects (objects);
      _transactionMock.AssertWasCalled (mock => mock.RegisterObjects (Arg<IEnumerable>.List.ContainsAll (new[] { expectedObject1, expectedObject2 })));
    }

    [Test]
    public void RegisterObjects_Recursively ()
    {
      var expectedObject1 = new object ();
      var expectedObject2 = new object ();
      var expectedObject3 = new object ();
      IEnumerable objects = new[] { expectedObject1, new[] { expectedObject2, expectedObject3 } };

      _transactionManager.RegisterObjects (objects);
      _transactionMock.AssertWasCalled (mock => mock.RegisterObjects (Arg<IEnumerable>.List.ContainsAll (new[] { expectedObject1, expectedObject2, expectedObject3 })));
    }

    [Test]
    public void RegisterObjects_RecursivelyWithNullValue ()
    {
      var expectedObject1 = new object ();
      var expectedObject2 = new object ();
      var expectedObject3 = new object ();
      IEnumerable objects = new[] { expectedObject1, new[] { expectedObject2, null, expectedObject3 } };

      _transactionManager.RegisterObjects (objects);
      _transactionMock.AssertWasCalled (mock => mock.RegisterObjects (Arg<IEnumerable>.List.ContainsAll (new[] { expectedObject1, expectedObject2, expectedObject3 })));
    }
  }
}
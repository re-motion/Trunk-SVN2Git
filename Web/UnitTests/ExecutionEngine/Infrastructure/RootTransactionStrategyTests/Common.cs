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
    public void GetExecutionContext ()
    {
      Assert.That (_strategy.ExecutionContext, Is.SameAs (ExecutionContextMock));
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
    public void RegisterObjects ()
    {
      var object1 = new object();
      var object2 = new object();

      using (MockRepository.Ordered())
      {
        ExecutionContextMock.Expect (mock => mock.GetInParameters()).Return (new[] { object1, object2 });
        TransactionMock.Expect (mock => mock.RegisterObjects (Arg<IEnumerable>.List.ContainsAll (new[] { object1, object2 })));
      }
      MockRepository.ReplayAll();

      new RootTransactionStrategy (false, TransactionMock, NullTransactionStrategy.Null, ExecutionContextMock);

      MockRepository.VerifyAll();
    }

    [Test]
    public void RegisterObjects_WithNullValue ()
    {
      var object1 = new object();
      var object2 = new object();

      using (MockRepository.Ordered())
      {
        ExecutionContextMock.Expect (mock => mock.GetInParameters()).Return (new[] { object1, null, object2 });
        TransactionMock.Expect (mock => mock.RegisterObjects (Arg<IEnumerable>.List.ContainsAll (new[] { object1, object2 })));
      }
      MockRepository.ReplayAll();

      new RootTransactionStrategy (false, TransactionMock, NullTransactionStrategy.Null, ExecutionContextMock);

      MockRepository.VerifyAll();
    }

    [Test]
    public void RegisterObjects_Recursively ()
    {
      var object1 = new object();
      var object2 = new object();
      var object3 = new object();

      using (MockRepository.Ordered())
      {
        ExecutionContextMock.Expect (mock => mock.GetInParameters()).Return (new[] { object1, new[] { object2, object3 } });
        TransactionMock.Expect (mock => mock.RegisterObjects (Arg<IEnumerable>.List.ContainsAll (new[] { object1, object2, object3 })));
      }
      MockRepository.ReplayAll();

      new RootTransactionStrategy (false, TransactionMock, NullTransactionStrategy.Null, ExecutionContextMock);

      MockRepository.VerifyAll();
    }

    [Test]
    public void RegisterObjects_RecursivelyWithNullValue ()
    {
      var object1 = new object();
      var object2 = new object();
      var object3 = new object();

      using (MockRepository.Ordered())
      {
        ExecutionContextMock.Expect (mock => mock.GetInParameters()).Return (new[] { object1, new[] { object2, null, object3 } });
        TransactionMock.Expect (mock => mock.RegisterObjects (Arg<IEnumerable>.List.ContainsAll (new[] { object1, object2, object3 })));
      }
      MockRepository.ReplayAll();

      new RootTransactionStrategy (false, TransactionMock, NullTransactionStrategy.Null, ExecutionContextMock);

      MockRepository.VerifyAll();
    }
  }
}
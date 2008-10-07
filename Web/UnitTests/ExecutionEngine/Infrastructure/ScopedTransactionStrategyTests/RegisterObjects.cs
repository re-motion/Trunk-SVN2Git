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
  public class RegisterObjects : ScopedTransactionStrategyTestBase
  {
    private ScopedTransactionStrategyBase _strategy;

    public override void SetUp ()
    {
      base.SetUp();
      _strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
    }

    [Test]
    public void Test ()
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
    public void Test_WithNullValue ()
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
    public void Test_Recursively ()
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
    public void Test_RecursivelyWithNullValue ()
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
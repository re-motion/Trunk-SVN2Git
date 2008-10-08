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
  public class ChildTransactionStrategyTest
  {
    private ChildTransactionStrategy _strategy;
    private ITransaction _childTransactionStub;
    private IWxeFunctionExecutionContext _executionContextStub;
    private TransactionStrategyBase _outerTransactionStrategyStub;

    [SetUp]
    public void SetUp ()
    {
      _outerTransactionStrategyStub = MockRepository.GenerateStub<TransactionStrategyBase> ();
      _childTransactionStub = MockRepository.GenerateStub<ITransaction> ();
      _executionContextStub = MockRepository.GenerateStub<IWxeFunctionExecutionContext> ();

      _executionContextStub.Stub (stub => stub.GetInParameters ()).Return (new object[0]);

      _strategy = new ChildTransactionStrategy (true, _childTransactionStub, _outerTransactionStrategyStub, _executionContextStub);  
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_strategy.Transaction, Is.SameAs (_childTransactionStub));
      Assert.That (_strategy.OuterTransactionStrategy, Is.SameAs (_outerTransactionStrategyStub));
      Assert.That (_strategy.ExecutionContext, Is.SameAs (_executionContextStub));
      Assert.That (_strategy.AutoCommit, Is.True);
      Assert.That (_strategy.IsNull, Is.False);
    }

    [Test]
    public void CreateExecutionListener ()
    {
      IWxeFunctionExecutionListener innerExecutionListenerStub = MockRepository.GenerateStub<IWxeFunctionExecutionListener> ();
      IWxeFunctionExecutionListener executionListener = _strategy.CreateExecutionListener (innerExecutionListenerStub);

      Assert.That (executionListener, Is.SameAs (innerExecutionListenerStub));
    }
  }
}
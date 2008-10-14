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
using Remotion.Development.Web.UnitTesting.ExecutionEngine.TestFunctions;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class ChildTransactionExecutionListenerTest
  {
    private WxeContext _wxeContext;
    private ChildTransactionStrategy _transactionStrategyMock;
    private IWxeFunctionExecutionListener _innerListenerStub;
    private IWxeFunctionExecutionListener _transactionListener;
    private ITransaction _childTransactionMock;

    [SetUp]
    public void SetUp ()
    {
      WxeContextFactory wxeContextFactory = new WxeContextFactory();
      _wxeContext = wxeContextFactory.CreateContext (new TestFunction());
      ITransaction transactionMock = MockRepository.GenerateMock<ITransaction>();
      TransactionStrategyBase outerTransactionStrategyStub = MockRepository.GenerateStub<TransactionStrategyBase>();
      IWxeFunctionExecutionContext executionContextStub = MockRepository.GenerateStub<IWxeFunctionExecutionContext>();
      executionContextStub.Stub (stub => stub.GetInParameters()).Return (new object[0]);

      _childTransactionMock = transactionMock;
      _transactionStrategyMock = MockRepository.GenerateMock<ChildTransactionStrategy> (
          false, _childTransactionMock, outerTransactionStrategyStub, executionContextStub);

      _innerListenerStub = MockRepository.GenerateStub<IWxeFunctionExecutionListener>();
      _transactionListener = new ChildTransactionExecutionListener (_transactionStrategyMock, _innerListenerStub);
    }

    [Test]
    public void OnExecutionPlay ()
    {
      _transactionListener.OnExecutionPlay (_wxeContext);

      _transactionStrategyMock.AssertWasNotCalled(mock => mock.OnExecutionPlay (_wxeContext, _innerListenerStub));
    }

    [Test]
    public void OnExecutionStop ()
    {
      _transactionListener.OnExecutionStop (_wxeContext);

      _transactionStrategyMock.AssertWasCalled (mock => mock.OnExecutionStop (_wxeContext, _innerListenerStub));
    }

    [Test]
    public void OnExecutionPause ()
    {
      _childTransactionMock.Stub (stub => stub.EnterScope()).Return (MockRepository.GenerateStub<ITransactionScope> ());
      _transactionStrategyMock.Stub(mock => mock.OnExecutionPlay(_wxeContext, _innerListenerStub)).CallOriginalMethod (OriginalCallOptions.NoExpectation);
      _transactionStrategyMock.AssertWasNotCalled (mock => mock.OnExecutionPause (_wxeContext, _innerListenerStub));

      _transactionListener.OnExecutionPause (_wxeContext);
    }

    [Test]
    public void OnExecutionFail ()
    {
      Exception exception = new Exception();

      _transactionListener.OnExecutionFail (_wxeContext, exception);

      _transactionStrategyMock.AssertWasCalled (mock => mock.OnExecutionFail (_wxeContext, _innerListenerStub, exception));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_transactionListener.IsNull, Is.False);
    }
  }
}
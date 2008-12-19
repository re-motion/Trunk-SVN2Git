// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
    private IWxeFunctionExecutionListener _innerListenerMock;
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

      _innerListenerMock = MockRepository.GenerateMock<IWxeFunctionExecutionListener>();
      _transactionListener = new ChildTransactionExecutionListener (_transactionStrategyMock, _innerListenerMock);
    }

    [Test]
    public void OnExecutionPlay ()
    {
      InvokeTransactionStrategyPlay();

      _transactionListener.OnExecutionPlay (_wxeContext);

      _transactionStrategyMock.AssertWasNotCalled (mock => mock.OnExecutionPlay (_wxeContext, _innerListenerMock));
      _innerListenerMock.AssertWasCalled (mock => mock.OnExecutionPlay (_wxeContext));
    }

    [Test]
    public void OnExecutionStop ()
    {
      InvokeTransactionStrategyPlay ();

      _transactionListener.OnExecutionStop (_wxeContext);

      _transactionStrategyMock.AssertWasCalled (mock => mock.OnExecutionStop (_wxeContext, _innerListenerMock));
      _innerListenerMock.AssertWasNotCalled (mock => mock.OnExecutionStop (_wxeContext));
    }

    [Test]
    public void OnExecutionPause ()
    {
      InvokeTransactionStrategyPlay();

      _transactionListener.OnExecutionPause (_wxeContext);
      
      _transactionStrategyMock.AssertWasNotCalled (mock => mock.OnExecutionPause (_wxeContext, _innerListenerMock));
      _innerListenerMock.AssertWasCalled (mock => mock.OnExecutionPause (_wxeContext));
    }

    [Test]
    public void OnExecutionFail ()
    {
      InvokeTransactionStrategyPlay ();

      Exception exception = new Exception ();

      _transactionStrategyMock.OnExecutionFail (_wxeContext, _innerListenerMock, exception);

      _transactionListener.OnExecutionStop (_wxeContext);
      _transactionStrategyMock.AssertWasCalled (mock => mock.OnExecutionFail (_wxeContext, _innerListenerMock, exception));
      _innerListenerMock.AssertWasNotCalled (mock => mock.OnExecutionFail (_wxeContext, exception));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_transactionListener.IsNull, Is.False);
    }

    private void InvokeTransactionStrategyPlay ()
    {
      _childTransactionMock.Stub (stub => stub.EnterScope ()).Return (MockRepository.GenerateStub<ITransactionScope> ());
      _childTransactionMock.Replay ();

      _transactionStrategyMock.Stub (stub => stub.OnExecutionPlay (Arg<WxeContext>.Is.NotNull, Arg<IWxeFunctionExecutionListener>.Is.NotNull))
          .CallOriginalMethod (OriginalCallOptions.NoExpectation);
      _transactionStrategyMock.Replay ();
      _transactionStrategyMock.OnExecutionPlay (_wxeContext, MockRepository.GenerateStub<IWxeFunctionExecutionListener> ());
      _transactionStrategyMock.BackToRecord ();
      _transactionStrategyMock.Replay ();
    }
  }
}

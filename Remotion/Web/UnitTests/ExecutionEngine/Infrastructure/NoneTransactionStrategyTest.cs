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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class NoneTransactionStrategyTest
  {
    private IWxeFunctionExecutionListener _executionListenerMock;
    private NoneTransactionStrategy _strategy;
    private WxeContext _context;
    private IWxeFunctionExecutionContext _executionContextMock;
    private TransactionStrategyBase _outerTransactionStrategyMock;

    [SetUp]
    public void SetUp ()
    {
      WxeContextFactory wxeContextFactory = new WxeContextFactory();
      _context = wxeContextFactory.CreateContext (new TestFunction());

      _executionListenerMock = MockRepository.GenerateMock<IWxeFunctionExecutionListener>();
      _executionContextMock = MockRepository.GenerateMock<IWxeFunctionExecutionContext>();
      _outerTransactionStrategyMock = MockRepository.GenerateMock<TransactionStrategyBase>();
      _strategy = new NoneTransactionStrategy (_outerTransactionStrategyMock);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Commit ()
    {
      _strategy.Commit();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Rollback ()
    {
      _strategy.Rollback();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Reset ()
    {
      _strategy.Reset();
    }

    [Test]
    public void GetTransaction ()
    {
      Assert.That (_strategy.GetNativeTransaction<object> (), Is.Null);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject) _strategy).IsNull, Is.True);
    }

    [Test]
    public void CreateExecutionListener ()
    {
      Assert.That (_strategy.CreateExecutionListener (_executionListenerMock), Is.SameAs (_executionListenerMock));
    }

    [Test]
    public void CreateChildTransactionStrategy ()
    {
      var grandParentTransactionStrategyMock = MockRepository.GenerateMock<TransactionStrategyBase> ();

      var noneTransactionStrategy = new NoneTransactionStrategy (grandParentTransactionStrategyMock);

      var childExecutionContextStub = MockRepository.GenerateStub<IWxeFunctionExecutionContext>();
      childExecutionContextStub.Stub (stub => stub.GetInParameters()).Return (new object[0]);

      ChildTransactionStrategy expected = new ChildTransactionStrategy (
          false, MockRepository.GenerateStub<ITransaction>(), grandParentTransactionStrategyMock, childExecutionContextStub);

      grandParentTransactionStrategyMock.Expect (mock => mock.CreateChildTransactionStrategy (true, childExecutionContextStub, _context)).Return (expected);
      
      TransactionStrategyBase actual = noneTransactionStrategy.CreateChildTransactionStrategy (true, childExecutionContextStub, _context);
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void UnregisterChildTransactionStrategy ()
    {
      Assert.That (_strategy.OuterTransactionStrategy, Is.SameAs (_outerTransactionStrategyMock));
      var childTransactionStrategyStub = MockRepository.GenerateStub<TransactionStrategyBase> ();

      _strategy.UnregisterChildTransactionStrategy (childTransactionStrategyStub);

      _outerTransactionStrategyMock.AssertWasCalled (mock => mock.UnregisterChildTransactionStrategy (childTransactionStrategyStub));
    }

    [Test]
    public void RegisterObjects ()
    {
      var expectedObjects = new[] { new object() };

      _outerTransactionStrategyMock.Expect (mock => mock.RegisterObjects (expectedObjects));

      _strategy.RegisterObjects (expectedObjects);

      _executionContextMock.VerifyAllExpectations();
      _outerTransactionStrategyMock.VerifyAllExpectations();
    }

    [Test]
    public void OnExecutionPlay ()
    {
      _strategy.OnExecutionPlay (_context, _executionListenerMock);
      _executionListenerMock.AssertWasCalled (mock => mock.OnExecutionPlay (_context));
    }

    [Test]
    public void OnExecutionStop ()
    {
      _strategy.OnExecutionStop (_context, _executionListenerMock);
      _executionListenerMock.AssertWasCalled (mock => mock.OnExecutionStop (_context));
    }

    [Test]
    public void OnExecutionPause ()
    {
      _strategy.OnExecutionPause (_context, _executionListenerMock);
      _executionListenerMock.AssertWasCalled (mock => mock.OnExecutionPause (_context));
    }

    [Test]
    public void OnExecutionFail ()
    {
      var exception = new Exception();
      _strategy.OnExecutionFail (_context, _executionListenerMock, exception);
      _executionListenerMock.AssertWasCalled (mock => mock.OnExecutionFail (_context, exception));
    }
  }
}

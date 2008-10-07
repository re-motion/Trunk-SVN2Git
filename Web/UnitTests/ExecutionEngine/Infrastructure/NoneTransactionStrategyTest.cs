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
    private ITransactionStrategy _outerTransactionStrategyMock;
    private TransactionStrategyBase _childTransactionStrategyMock;

    [SetUp]
    public void SetUp ()
    {
      WxeContextFactory wxeContextFactory = new WxeContextFactory();
      _context = wxeContextFactory.CreateContext (new TestFunction());

      _executionListenerMock = MockRepository.GenerateMock<IWxeFunctionExecutionListener> ();
      _executionContextMock = MockRepository.GenerateMock<IWxeFunctionExecutionContext> ();
      _outerTransactionStrategyMock = MockRepository.GenerateMock<ITransactionStrategy> ();
      _childTransactionStrategyMock = MockRepository.GenerateMock<TransactionStrategyBase> (
        false, NullTransactionStrategy.Null, MockRepository.GenerateStub<IWxeFunctionExecutionContext> ());
      _strategy = new NoneTransactionStrategy (NullTransactionStrategy.Null, _executionContextMock);
    }

    [Test]
    public void GetAutoCommit ()
    {
      Assert.That (_strategy.AutoCommit, Is.False);
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
    public void CreateExecutionListener ()
    {
      Assert.That (_strategy.CreateExecutionListener (_executionListenerMock), Is.SameAs (_executionListenerMock));
    }
    [Test]
    public void RegisterObjects ()
    {
      var strategy = new NoneTransactionStrategy (_outerTransactionStrategyMock, _executionContextMock);
      var expectedObjects = new[] { new object () };

      _outerTransactionStrategyMock.Expect (mock => mock.RegisterObjects (expectedObjects));

      strategy.RegisterObjects (expectedObjects);

      _executionContextMock.VerifyAllExpectations ();
      _outerTransactionStrategyMock.VerifyAllExpectations ();
    }

    [Test]
    public void OnExecutionPlay ()
    {
      _strategy.OnExecutionPlay (_context, _executionListenerMock);
      _executionListenerMock.AssertWasCalled (mock => mock.OnExecutionPlay (_context));
    }

    [Test]
    public void OnExecutionPlay_WithChildStrategy ()
    {
      _strategy.SetChild (_childTransactionStrategyMock);

      _strategy.OnExecutionPlay (_context, _executionListenerMock);
      
      _childTransactionStrategyMock.AssertWasCalled (mock => mock.OnExecutionPlay (_context, _executionListenerMock));
    }

    [Test]
    public void OnExecutionStop ()
    {
      _strategy.OnExecutionStop (_context, _executionListenerMock);
      _executionListenerMock.AssertWasCalled (mock => mock.OnExecutionStop (_context));
    }

    [Test]
    public void OnExecutionStop_WithChildStrategy ()
    {
      _strategy.SetChild (_childTransactionStrategyMock);

      _strategy.OnExecutionStop (_context, _executionListenerMock);

      _childTransactionStrategyMock.AssertWasCalled (mock => mock.OnExecutionStop (_context, _executionListenerMock));
    }

    [Test]
    public void OnExecutionPause ()
    {
      _strategy.OnExecutionPause (_context, _executionListenerMock);
      _executionListenerMock.AssertWasCalled (mock => mock.OnExecutionPause (_context));
    }

    [Test]
    public void OnExecutionPause_WithChildStrategey ()
    {
      _strategy.SetChild (_childTransactionStrategyMock);

      _strategy.OnExecutionPause (_context, _executionListenerMock);

      _childTransactionStrategyMock.AssertWasCalled (mock => mock.OnExecutionPause (_context, _executionListenerMock));
    }
    
    [Test]
    public void OnExecutionFail ()
    {
      var exception = new Exception();
      _strategy.OnExecutionFail (_context, _executionListenerMock, exception);
      _executionListenerMock.AssertWasCalled (mock => mock.OnExecutionFail (_context, exception));
    }

    [Test]
    public void OnExecutionFail_WithChild ()
    {
      _strategy.SetChild (_childTransactionStrategyMock);
      var exception = new Exception ();

      _strategy.OnExecutionFail (_context, _executionListenerMock, exception);

      _childTransactionStrategyMock.AssertWasCalled (mock => mock.OnExecutionFail (_context, _executionListenerMock, exception));
    }

    [Test]
    public void GetTransaction ()
    {
      Assert.That (_strategy.GetNativeTransaction<object> (), Is.Null);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject)_strategy).IsNull, Is.True);
    }
  }
}
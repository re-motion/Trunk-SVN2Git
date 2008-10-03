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
    private ITransactionStrategy _parentTransactionStrategyMock;

    [SetUp]
    public void SetUp ()
    {
      WxeContextFactory wxeContextFactory = new WxeContextFactory();
      _context = wxeContextFactory.CreateContext (new TestFunction());

      _executionListenerMock = MockRepository.GenerateMock<IWxeFunctionExecutionListener> ();
      _executionContextMock = MockRepository.GenerateMock<IWxeFunctionExecutionContext> ();
      _parentTransactionStrategyMock = MockRepository.GenerateMock<ITransactionStrategy> ();
      _strategy = new NoneTransactionStrategy (_executionListenerMock, _executionContextMock);
    }

    [Test]
    public void GetInnerListener ()
    {
      Assert.That (_strategy.InnerListener, Is.SameAs (_executionListenerMock));
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
    public void GetTransaction ()
    {
      Assert.That (_strategy.Transaction, Is.Null);
    }

    [Test]
    public void OnExecutionPlay ()
    {
      ((IWxeFunctionExecutionListener) _strategy).OnExecutionPlay (_context);
      _executionListenerMock.AssertWasCalled (mock => mock.OnExecutionPlay (_context));
    }

    [Test]
    public void OnExecutionStop ()
    {
      ((IWxeFunctionExecutionListener) _strategy).OnExecutionStop (_context);
      _executionListenerMock.AssertWasCalled (mock => mock.OnExecutionStop (_context));
    }

    [Test]
    public void OnExecutionPause ()
    {
      ((IWxeFunctionExecutionListener) _strategy).OnExecutionPause (_context);
      _executionListenerMock.AssertWasCalled (mock => mock.OnExecutionPause (_context));
    }
    
    [Test]
    public void OnExecutionFail ()
    {
      var exception = new Exception();
      ((IWxeFunctionExecutionListener) _strategy).OnExecutionFail (_context, exception);
      _executionListenerMock.AssertWasCalled (mock => mock.OnExecutionFail (_context, exception));
    }

    [Test]
    public void RegisterObjects ()
    {
      var expectedObjects = new[] { new object () };
      
      _executionContextMock.Expect (mock => mock.ParentTransactionStrategy).Return (_parentTransactionStrategyMock);
      _parentTransactionStrategyMock.Expect (mock => mock.RegisterObjects (expectedObjects));

      _strategy.RegisterObjects (expectedObjects);

      _executionContextMock.VerifyAllExpectations();
      _parentTransactionStrategyMock.VerifyAllExpectations();
    }

    [Test]
    public void RegisterObjects_WithoutParentTransactionStrategy ()
    {
      var expectedObjects = new[] { new object () };

      _executionContextMock.Expect (mock => mock.ParentTransactionStrategy).Return (null);

      _strategy.RegisterObjects (expectedObjects);

      _executionContextMock.VerifyAllExpectations ();
      _parentTransactionStrategyMock.VerifyAllExpectations ();
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject)_strategy).IsNull, Is.True);
    }
  }
}
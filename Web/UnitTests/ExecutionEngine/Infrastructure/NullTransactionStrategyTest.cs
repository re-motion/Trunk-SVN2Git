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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class NullTransactionStrategyTest
  {
    private TransactionStrategyBase _strategy;
    private WxeContext _context;
    private IWxeFunctionExecutionListener _executionListenerMock;

    [SetUp]
    public void SetUp ()
    {
      WxeContextFactory wxeContextFactory = new WxeContextFactory ();
      _context = wxeContextFactory.CreateContext (new TestFunction ());

      _executionListenerMock = MockRepository.GenerateMock<IWxeFunctionExecutionListener> ();

      _strategy = NullTransactionStrategy.Null;
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_strategy.IsNull, Is.True);
    }

    [Test]
    public void GetOuterTransactionStrategy ()
    {
      Assert.That (_strategy.OuterTransactionStrategy, Is.Null);
    }

    [Test]
    public void GetNativeTransaction ()
    {
      Assert.That (_strategy.GetNativeTransaction<ITransaction>(), Is.Null);
    }

    [Test]
    public void CreateChildTransactionStrategy ()
    {
      Assert.That (_strategy.CreateChildTransactionStrategy (true, MockRepository.GenerateStub<IWxeFunctionExecutionContext> ()), Is.Null);
    }

    [Test]
    public void RegisterObjects ()
    {
      _strategy.RegisterObjects (null);
    }

    [Test]
    public void Commit()
    {
      _strategy.Commit();
    }

    [Test]
    public void Rollback ()
    {
      _strategy.Rollback ();
    }

    [Test]
    public void Reset ()
    {
      _strategy.Reset ();
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
      var exception = new Exception ();
      _strategy.OnExecutionFail (_context, _executionListenerMock, exception);
      _executionListenerMock.AssertWasCalled (mock => mock.OnExecutionFail (_context, exception));
    }
  }
}
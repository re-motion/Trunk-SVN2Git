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
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class ChildTransactionStrategyTest
  {
    private MockRepository _mockRepository;
    private ChildTransactionStrategy _strategy;
    private ITransaction _childTransactionMock;
    private IWxeFunctionExecutionContext _executionContextStub;
    private TransactionStrategyBase _outerTransactionStrategyMock;
    private IWxeFunctionExecutionListener _executionListenerStub;
    private WxeContext _context;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      WxeContextFactory wxeContextFactory = new WxeContextFactory ();
      _context = wxeContextFactory.CreateContext (new TestFunction ());
      _outerTransactionStrategyMock = _mockRepository.StrictMock<TransactionStrategyBase> ();
      _childTransactionMock = _mockRepository.StrictMock<ITransaction> ();
      _executionContextStub = _mockRepository.Stub<IWxeFunctionExecutionContext> ();
      _executionListenerStub = _mockRepository.Stub<IWxeFunctionExecutionListener> ();
     
      _executionContextStub.Stub (stub => stub.GetInParameters ()).Return (new object[0]);
      _childTransactionMock.Stub (stub => stub.RegisterObjects (Arg<IEnumerable>.Is.NotNull));
      _mockRepository.ReplayAll ();

      _strategy = new ChildTransactionStrategy (true, _childTransactionMock, _outerTransactionStrategyMock, _executionContextStub);
      
      _mockRepository.BackToRecordAll();
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_strategy.Transaction, Is.SameAs (_childTransactionMock));
      Assert.That (_strategy.OuterTransactionStrategy, Is.SameAs (_outerTransactionStrategyMock));
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

    [Test]
    public void ReleaseTransaction ()
    {
      using (_mockRepository.Ordered ())
      {
        _childTransactionMock.Expect (mock => mock.Release ());
        _outerTransactionStrategyMock.Expect (mock => mock.UnregisterChildTransactionStrategy (_strategy));
      }
      _mockRepository.ReplayAll();

      PrivateInvoke.InvokeNonPublicMethod (_strategy, "ReleaseTransaction");

      _mockRepository.VerifyAll();
    }

    [Test]
    public void OnExecutionStop ()
    {
      _childTransactionMock.Stub (stub => stub.EnterScope ()).Return (MockRepository.GenerateStub<ITransactionScope> ());
      _mockRepository.ReplayAll();
      _strategy.OnExecutionPlay (_context, _executionListenerStub);
      _childTransactionMock.BackToRecord ();

      using (_mockRepository.Ordered ())
      {
        _childTransactionMock.Stub (stub => stub.Commit ());
        _executionContextStub.Stub (stub => stub.GetOutParameters ()).Return (new object[0]);
        _childTransactionMock.Stub (stub => stub.RegisterObjects (Arg<IEnumerable>.Is.NotNull));
        _outerTransactionStrategyMock.Stub (mock => mock.RegisterObjects (Arg<IEnumerable>.Is.NotNull));
        _childTransactionMock.Expect (mock => mock.Release ());
        _outerTransactionStrategyMock.Expect (mock => mock.UnregisterChildTransactionStrategy (_strategy));
      }

      _mockRepository.ReplayAll ();

      _strategy.OnExecutionStop (_context, _executionListenerStub);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void OnExecutionFail ()
    {
      _childTransactionMock.Stub (stub => stub.EnterScope ()).Return (MockRepository.GenerateStub<ITransactionScope> ());
      _mockRepository.ReplayAll ();
      _strategy.OnExecutionPlay (_context, _executionListenerStub);
      _childTransactionMock.BackToRecord ();

      using (_mockRepository.Ordered ())
      {
        _childTransactionMock.Expect (mock => mock.Release ());
        _outerTransactionStrategyMock.Expect (mock => mock.UnregisterChildTransactionStrategy (_strategy));
      }

      _mockRepository.ReplayAll ();

      _strategy.OnExecutionFail(_context, _executionListenerStub, new Exception ("Inner Exception"));

      _mockRepository.VerifyAll ();
    }
  }
}
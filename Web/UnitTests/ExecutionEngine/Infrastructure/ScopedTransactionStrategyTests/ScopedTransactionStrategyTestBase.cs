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
using Remotion.Data;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  public class ScopedTransactionStrategyTestBase
  {
    private IWxeFunctionExecutionListener _executionListenerStub;
    private ITransactionScope _scopeMock;
    private ITransaction _transactionMock;
    private TransactionStrategyBase _outerTransactionStrategyMock;
    private WxeContext _context;
    private MockRepository _mockRepository;
    private IWxeFunctionExecutionContext _executionContextMock;
    private TransactionStrategyBase _childTransactionStrategyMock;

    [SetUp]
    public virtual void SetUp ()
    {
      WxeContextFactory wxeContextFactory = new WxeContextFactory();
      _context = wxeContextFactory.CreateContext (new TestFunction());

      _mockRepository = new MockRepository();
      _executionListenerStub = MockRepository.Stub<IWxeFunctionExecutionListener>();
      _transactionMock = MockRepository.StrictMock<ITransaction>();
      _scopeMock = MockRepository.StrictMock<ITransactionScope>();
      _executionContextMock = MockRepository.StrictMock<IWxeFunctionExecutionContext>();
      _outerTransactionStrategyMock = MockRepository.StrictMock<TransactionStrategyBase>();
      _childTransactionStrategyMock = MockRepository.StrictMock<TransactionStrategyBase> ();
    }

    public MockRepository MockRepository
    {
      get { return _mockRepository; }
    }

    public WxeContext Context
    {
      get { return _context; }
    }

    public TransactionStrategyBase OuterTransactionStrategyMock
    {
      get { return _outerTransactionStrategyMock; }
    }

    public TransactionStrategyBase ChildTransactionStrategyMock
    {
      get { return _childTransactionStrategyMock; }
    }

    public ITransaction TransactionMock
    {
      get { return _transactionMock; }
    }

    public ITransactionScope ScopeMock
    {
      get { return _scopeMock; }
    }

    public IWxeFunctionExecutionListener ExecutionListenerStub
    {
      get { return _executionListenerStub; }
    }

    public IWxeFunctionExecutionContext ExecutionContextMock
    {
      get { return _executionContextMock; }
    }

    protected void SetChild (ScopedTransactionStrategyBase strategy, TransactionStrategyBase childStrategy)
    {
      PrivateInvoke.InvokeNonPublicMethod (strategy, "SetChild", childStrategy);
    }

    protected ScopedTransactionStrategyBase CreateScopedTransactionStrategy (bool autoCommit, TransactionStrategyBase parentTransactionStrategy)
    {
      _executionContextMock.BackToRecord();
      _executionContextMock.Stub (stub => stub.GetInParameters()).Return (new object[0]).Repeat.Any();
      _executionContextMock.Replay();

      _transactionMock.BackToRecord();
      _transactionMock.Stub (stub => stub.RegisterObjects (Arg<IEnumerable>.Is.NotNull));
      _transactionMock.Replay();

      var strategy = MockRepository.PartialMock<ScopedTransactionStrategyBase> (
          autoCommit, TransactionMock, parentTransactionStrategy, _executionContextMock);
      strategy.Replay();

      SetChild (strategy, ChildTransactionStrategyMock);

      _executionContextMock.BackToRecord();
      _transactionMock.BackToRecord();

      return strategy;
    }

    protected void InvokeOnExecutionPlay (ScopedTransactionStrategyBase strategy)
    {
      _childTransactionStrategyMock.BackToRecord();
      _childTransactionStrategyMock.Stub (stub => stub.OnExecutionPlay (_context, _executionListenerStub));
      _childTransactionStrategyMock.Replay ();

      _transactionMock.BackToRecord();
      _transactionMock.Stub (stub => stub.EnterScope()).Return (ScopeMock);
      _transactionMock.Replay();

      strategy.OnExecutionPlay (Context, ExecutionListenerStub);

      _transactionMock.BackToRecord();
      _childTransactionStrategyMock.BackToRecord ();
    }

    protected void InvokeOnExecutionPause (ScopedTransactionStrategyBase strategy)
    {
      _childTransactionStrategyMock.BackToRecord ();
      _childTransactionStrategyMock.Stub (stub => stub.OnExecutionPause (_context, _executionListenerStub));
      _childTransactionStrategyMock.Replay ();

      _scopeMock.BackToRecord();
      _scopeMock.Stub (stub => stub.Leave());
      _scopeMock.Replay();

      strategy.OnExecutionPause (Context, _executionListenerStub);

      _childTransactionStrategyMock.BackToRecord ();
      _scopeMock.BackToRecord();
    }
  }
}
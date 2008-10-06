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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure.RootTransactionStrategyTests
{
  public class RootTransactionStrategyTestBase
  {
    private IWxeFunctionExecutionListener _executionListenerMock;
    private ITransactionScope _scopeMock;
    private ITransaction _transactionMock;
    private ITransactionStrategy _parentTransactionStrategyMock;
    private WxeContext _context;
    private MockRepository _mockRepository;
    private IWxeFunctionExecutionContext _executionContextMock;

    [SetUp]
    public virtual void SetUp ()
    {
      WxeContextFactory wxeContextFactory = new WxeContextFactory();
      _context = wxeContextFactory.CreateContext (new TestFunction());

      _mockRepository = new MockRepository();
      _executionListenerMock = MockRepository.StrictMock<IWxeFunctionExecutionListener> ();
      _transactionMock = MockRepository.StrictMock<ITransaction>();
      _scopeMock = MockRepository.StrictMock<ITransactionScope>();
      _executionContextMock = MockRepository.StrictMock<IWxeFunctionExecutionContext>();
      _parentTransactionStrategyMock = MockRepository.StrictMock<ITransactionStrategy> ();
    }

    public MockRepository MockRepository
    {
      get { return _mockRepository; }
    }

    public WxeContext Context
    {
      get { return _context; }
    }

    public ITransactionStrategy ParentTransactionStrategyMock
    {
      get { return _parentTransactionStrategyMock; }
    }

    public ITransaction TransactionMock
    {
      get { return _transactionMock; }
    }

    public ITransactionScope ScopeMock
    {
      get { return _scopeMock; }
    }

    public IWxeFunctionExecutionListener ExecutionListenerMock
    {
      get { return _executionListenerMock; }
    }

    public IWxeFunctionExecutionContext ExecutionContextMock
    {
      get { return _executionContextMock; }
    }

    protected RootTransactionStrategy CreateRootTransactionStrategy (bool autoCommit, ITransactionStrategy parentTransactionStrategy)
    {
      _executionContextMock.BackToRecord();
      _executionContextMock.Stub (stub => stub.GetInParameters()).Return (new object[0]).Repeat.Any();
      _executionContextMock.Replay();

      _transactionMock.BackToRecord();
      _transactionMock.Stub (stub => stub.RegisterObjects (Arg<IEnumerable>.Is.NotNull));
      _transactionMock.Replay();

      var strategy = new RootTransactionStrategy (
          autoCommit, TransactionMock, parentTransactionStrategy, _executionContextMock, _executionListenerMock);

      _executionContextMock.BackToRecord();
      _transactionMock.BackToRecord();

      return strategy;
    }

    protected void InvokeOnExecutionPlay (RootTransactionStrategy strategy)
    {
      _executionListenerMock.BackToRecord();
      _executionListenerMock.Stub (stub => stub.OnExecutionPlay (Context));
      _executionListenerMock.Replay();

      _transactionMock.BackToRecord();
      _transactionMock.Stub (stub => stub.EnterScope()).Return (ScopeMock);
      _transactionMock.Replay();

      strategy.OnExecutionPlay (Context);

      _transactionMock.BackToRecord();
      _executionListenerMock.BackToRecord();
    }

    protected void InvokeOnExecutionPause (RootTransactionStrategy strategy)
    {
      _executionListenerMock.BackToRecord();
      _executionListenerMock.Stub (stub => stub.OnExecutionPause (Context));
      _executionListenerMock.Replay();

      _scopeMock.BackToRecord();
      _scopeMock.Stub (stub => stub.Leave());
      _scopeMock.Replay();

      strategy.OnExecutionPause (Context);

      _executionListenerMock.BackToRecord();
      _scopeMock.BackToRecord();
    }
  }
}
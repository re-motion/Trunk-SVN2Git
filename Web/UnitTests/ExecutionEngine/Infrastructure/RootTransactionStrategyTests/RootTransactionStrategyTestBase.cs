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
using System.Collections;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure.RootTransactionStrategyTests
{
  public class RootTransactionStrategyTestBase
  {
    private IWxeFunctionExecutionListener _executionListenerMock;
    private ITransactionScope _scopeMock;
    private ITransaction _transactionMock;
    private WxeContext _context;
    private MockRepository _mockRepository;
    private ITransactionManager _transactionManagerMock;
    private IWxeFunctionExecutionContext _executionContextMock;

    [SetUp]
    public virtual void SetUp ()
    {
      WxeContextFactory wxeContextFactory = new WxeContextFactory();
      _context = wxeContextFactory.CreateContext (new TestFunction());

      _mockRepository = new MockRepository();
      _executionListenerMock = MockRepository.StrictMock<IWxeFunctionExecutionListener>();
      _transactionMock = MockRepository.StrictMock<ITransaction>();
      _scopeMock = MockRepository.StrictMock<ITransactionScope>();
      _transactionManagerMock = MockRepository.StrictMock<ITransactionManager>();

      _executionContextMock = MockRepository.StrictMock<IWxeFunctionExecutionContext>();
      _executionContextMock.Stub (stub => stub.GetInParameters ()).Return (new object[0]).Repeat.Any();
    }

    public MockRepository MockRepository
    {
      get { return _mockRepository; }
    }

    public WxeContext Context
    {
      get { return _context; }
    }

    public ITransaction TransactionMock
    {
      get { return _transactionMock; }
    }

    public ITransactionScope ScopeMock
    {
      get { return _scopeMock; }
    }

    public ITransactionManager TransactionManagerMock
    {
      get { return _transactionManagerMock; }
    }

    public IWxeFunctionExecutionListener ExecutionListenerMock
    {
      get { return _executionListenerMock; }
    }

    public IWxeFunctionExecutionContext ExecutionContextMock
    {
      get { return _executionContextMock; }
    }

    protected RootTransactionStrategy CreateRootTransactionStrategy (bool autoCommit)
    {
      _transactionManagerMock.BackToRecord();
      _transactionManagerMock.Expect (mock => mock.InitializeTransaction ());
      _transactionManagerMock.Stub (stub => stub.RegisterObjects (Arg<IEnumerable>.Is.Anything));
      _transactionManagerMock.Replay ();

      _executionContextMock.Replay();

      return new RootTransactionStrategy (autoCommit, _executionListenerMock, _transactionManagerMock, _executionContextMock);
    }

    protected void InvokeOnExecutionPlay (RootTransactionStrategy strategy)
    {
      _executionListenerMock.BackToRecord ();
      _executionListenerMock.Stub (stub => stub.OnExecutionPlay (Context));
      _executionListenerMock.Replay ();

      _transactionManagerMock.BackToRecord ();
      _transactionManagerMock.Stub (stub => stub.EnterScope ()).Return (ScopeMock);
      _transactionManagerMock.Replay ();

      strategy.OnExecutionPlay (Context);

      _transactionManagerMock.BackToRecord ();
      _executionListenerMock.BackToRecord ();
    }

    protected void InvokeOnExecutionPause (RootTransactionStrategy strategy)
    {
      ExecutionListenerMock.Stub (stub => stub.OnExecutionPause (Context));
      ScopeMock.Stub (stub => stub.Leave());

      MockRepository.Replay (ExecutionListenerMock);
      MockRepository.Replay (ScopeMock);

      strategy.OnExecutionPause (Context);

      MockRepository.BackToRecord (ExecutionListenerMock);
      MockRepository.BackToRecord (ScopeMock);
    }
  }
}
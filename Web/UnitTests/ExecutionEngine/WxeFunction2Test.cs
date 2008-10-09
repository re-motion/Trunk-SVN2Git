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
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeFunction2Test
  {
    private MockRepository _mockRepository;
    private WxeContext _context;
    private IWxeFunctionExecutionListener _executionListenerMock;

    [SetUp]
    public void SetUp ()
    {
      TestFunction rootFunction = new TestFunction();
      WxeContextFactory contextFactory = new WxeContextFactory();
      _context = contextFactory.CreateContext (rootFunction);
      _mockRepository = new MockRepository();

      _executionListenerMock = _mockRepository.StrictMock<IWxeFunctionExecutionListener>();
    }

    [Test]
    public void Execute_NoException ()
    {
      TestFunction2 function = new TestFunction2 ();
      function.ExecutionListener = _executionListenerMock;

      using (_mockRepository.Ordered ())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionStop (_context));
      }

      _mockRepository.ReplayAll();

      function.Execute (_context);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Execute_WithTransactionStrategy ()
    {
      ITransactionMode transactionModeMock = _mockRepository.StrictMock<ITransactionMode>();
      TestFunction2 function = new TestFunction2 (transactionModeMock);
      TransactionStrategyBase transactionStrategyMock = MockRepository.GenerateMock<TransactionStrategyBase>();
      transactionModeMock.Expect (mock => mock.CreateTransactionStrategy (function, _context)).Return (transactionStrategyMock);
      transactionStrategyMock.Expect (mock => mock.CreateExecutionListener (function.ExecutionListener)).Return (_executionListenerMock);

      using (_mockRepository.Ordered ())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionStop (_context));
      }

      _mockRepository.ReplayAll ();

      function.Execute (_context);

      _mockRepository.VerifyAll ();
      Assert.That (function.ExecutionListener, Is.SameAs (_executionListenerMock));
    }

    [Test]
    public void Execute_ReEntryAfterThreadAbort ()
    {
      TestFunction2 function = new TestFunction2 ();
      function.ExecutionListener = _executionListenerMock;
      
      WxeStep step1 = MockRepository.GenerateMock<WxeStep> ();
      step1.Expect (mock => mock.Execute (_context)).Do (invocation => Thread.CurrentThread.Abort());
      function.Add (step1);

      WxeStep step2 = MockRepository.GenerateMock<WxeStep>();
      step2.Expect (mock => mock.Execute (_context));
      function.Add (step2);

      using (_mockRepository.Ordered())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionPause (_context));
      }
      _mockRepository.ReplayAll();

      try
      {
        function.Execute (_context);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      _mockRepository.VerifyAll();
      _mockRepository.BackToRecordAll();

      using (_mockRepository.Ordered())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionStop (_context));
      }
      _mockRepository.ReplayAll();

      function.Execute (_context);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Execute_ThreadAbort_WithFatalException ()
    {
      TestFunction2 function = new TestFunction2 ();
      function.ExecutionListener = _executionListenerMock;

      WxeStep step1 = MockRepository.GenerateMock<WxeStep> ();
      step1.Expect (mock => mock.Execute (_context)).Do (invocation => Thread.CurrentThread.Abort ());
      function.Add (step1);

      var fatalExecutionException = new WxeFatalExecutionException (new Exception ("Pause exception"), null);

      using (_mockRepository.Ordered ())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionPause (_context)).Throw (fatalExecutionException);
      }
      _mockRepository.ReplayAll ();

      try
      {
        function.Execute (_context);
        Assert.Fail ();
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException, Is.SameAs (fatalExecutionException));
        Thread.ResetAbort ();
      }
    } 

    [Test]
    public void Execute_FailAfterException ()
    {
      TestFunction2 function = new TestFunction2 ();
      function.ExecutionListener = _executionListenerMock;
      
      WxeStep step1 = MockRepository.GenerateMock<WxeStep> ();
      Exception stepException = new Exception ("StepException");
      step1.Expect (mock => mock.Execute (_context)).Throw (stepException);
      function.Add (step1);

      using (_mockRepository.Ordered())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionFail (_context, stepException));
      }
      _mockRepository.ReplayAll();

      try
      {
        function.Execute (_context);
        Assert.Fail();
      }
      catch (Exception actualException)
      {
        Assert.That (actualException, Is.SameAs (stepException));
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Execute_FailAfterExceptionAndFailInListener ()
    {
      TestFunction2 function = new TestFunction2 ();
      function.ExecutionListener = _executionListenerMock;
      
      WxeStep step1 = MockRepository.GenerateMock<WxeStep> ();
      Exception stepException = new Exception ("StepException");
      step1.Expect (mock => mock.Execute (_context)).Throw (stepException);
      function.Add (step1);

      Exception listenerException = new Exception ("ListenerException");

      using (_mockRepository.Ordered())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionFail (_context, stepException)).Throw (listenerException);
      }
      _mockRepository.ReplayAll();

      try
      {
        function.Execute (_context);
        Assert.Fail();
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (stepException));
        Assert.That (actualException.OuterException, Is.SameAs (listenerException));
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Execute_UseNullListener ()
    {
      TestFunction2 function = new TestFunction2();
      function.Execute (_context);
    }

    [Test]
    public void GetExecutionListener ()
    {
      TestFunction2 function = new TestFunction2 ();
      Assert.That (function.ExecutionListener, Is.InstanceOfType (typeof (NullExecutionListener)));
    }

    [Test]
    public void SetExecutionListener ()
    {
      TestFunction2 function = new TestFunction2 ();
      function.ExecutionListener = _executionListenerMock;
      Assert.That (function.ExecutionListener, Is.SameAs (_executionListenerMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void SetExecutionListenerNull ()
    {
      TestFunction2 function = new TestFunction2 ();
      function.ExecutionListener = null;
    }
  }
}
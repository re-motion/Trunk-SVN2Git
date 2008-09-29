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
using System.Collections.Specialized;
using System.Threading;
using System.Web.SessionState;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeFunction2Test
  {
    private MockRepository _mockRepository;
    private WxeContext _context;
    private TestFunction2 _function;
    private IWxeFunctionExecutionListener _executionListenerMock;

    [SetUp]
    public void SetUp ()
    {
      TestFunction rootFunction = new TestFunction();
      IHttpContext httpContext = MockRepository.GenerateStub<IHttpContext>();
      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (MockRepository.GenerateStub<IHttpSessionState>());
      WxeFunctionState functionState = new WxeFunctionState (rootFunction, false);
      NameValueCollection queryString = new NameValueCollection();
      _context = new WxeContext (httpContext, functionStateManager, functionState, queryString);
      _mockRepository = new MockRepository();

      _function = new TestFunction2();
      _executionListenerMock = _mockRepository.StrictMock<IWxeFunctionExecutionListener>();
      _function.ExecutionListener = _executionListenerMock;
    }

    [Test]
    public void Execute_NoException ()
    {
      using (_mockRepository.Ordered())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionStop (_context));
      }

      _mockRepository.ReplayAll();

      _function.Execute (_context);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Execute_ReEntryAfterThreadAbort ()
    {
      WxeStep step1 = MockRepository.GenerateMock<WxeStep>();
      step1.Expect (mock => mock.Execute (_context)).Do (invocation => Thread.CurrentThread.Abort());
      _function.Add (step1);

      WxeStep step2 = MockRepository.GenerateMock<WxeStep> ();
      step2.Expect (mock => mock.Execute (_context));
      _function.Add (step2);

      using (_mockRepository.Ordered())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionPause (_context));
      }
      _mockRepository.ReplayAll();

      try
      {
        _function.Execute (_context);
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
      _mockRepository.ReplayAll ();

      _function.Execute (_context);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Execute_FailAfterException ()
    {
      WxeStep step1 = MockRepository.GenerateMock<WxeStep> ();
      Exception stepException = new Exception ("StepException");
      step1.Expect (mock => mock.Execute (_context)).Throw (stepException);
      _function.Add (step1);

      using (_mockRepository.Ordered ())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionFail (_context, stepException));
      }
      _mockRepository.ReplayAll ();

      try
      {
        _function.Execute (_context);
        Assert.Fail ();
      }
      catch (Exception actualException)
      {
        Assert.That (actualException, Is.SameAs (stepException));
      }

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Execute_FailAfterExceptionAndFailInListener ()
    {
      WxeStep step1 = MockRepository.GenerateMock<WxeStep> ();
      Exception stepException = new Exception ("StepException");
      step1.Expect (mock => mock.Execute (_context)).Throw (stepException);
      _function.Add (step1);

      Exception listenerException = new Exception ("ListenerException");

      using (_mockRepository.Ordered ())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionFail (_context, stepException)).Throw (listenerException);
      }
      _mockRepository.ReplayAll ();

      try
      {
        _function.Execute (_context);
        Assert.Fail ();
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (stepException));
        Assert.That (actualException.OuterException, Is.SameAs (listenerException));
      }

      _mockRepository.VerifyAll ();
    }
  }
}
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
using Remotion.Security;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class SecurityExecutionListenerTest
  {
    private MockRepository _mockRepository;
    private IWxeSecurityAdapter _securityAdapterMock;
    private IWxeFunctionExecutionListener _securityListener;
    private IWxeFunctionExecutionListener _innerListenerMock;
    private TestFunction _function;
    private WxeContext _wxeContext;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      WxeContextFactory wxeContextFactory = new WxeContextFactory();
      _wxeContext = wxeContextFactory.CreateContext (new TestFunction());

      _securityAdapterMock = _mockRepository.StrictMock<IWxeSecurityAdapter>();
      _innerListenerMock = _mockRepository.StrictMock<IWxeFunctionExecutionListener>();

      _function = new TestFunction();
      _securityListener = new SecurityExecutionListener (_function, _innerListenerMock);

      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), _securityAdapterMock);
    }

    [TearDown]
    public void TearDown ()
    {
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), null);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (((SecurityExecutionListener) _securityListener).Function, Is.SameAs (_function));
      Assert.That (((SecurityExecutionListener) _securityListener).InnerListener, Is.SameAs (_innerListenerMock));
    }

    [Test]
    public void ExecutionPlay_WithAccessGranted ()
    {
      using (_mockRepository.Ordered())
      {
        _securityAdapterMock.Expect (mock => mock.CheckAccess (_function));
        _innerListenerMock.Expect (mock => mock.OnExecutionPlay (_wxeContext));
      }
      _mockRepository.ReplayAll();

      _securityListener.OnExecutionPlay (_wxeContext);

      _mockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void ExecutionPlay_WithAccessDenied ()
    {
      _securityAdapterMock.Expect (mock => mock.CheckAccess (_function)).Throw (new PermissionDeniedException());
      _mockRepository.ReplayAll();

      _securityListener.OnExecutionPlay (_wxeContext);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ExecutionPlay_WithoutWxeSecurityProvider ()
    {
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), null);
      _innerListenerMock.Expect (mock => mock.OnExecutionPlay (_wxeContext));
      _mockRepository.ReplayAll ();

      _securityListener.OnExecutionPlay (_wxeContext);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExecutionPlay_AfterExecutionStarted ()
    {
      _function.Execute (_wxeContext);
      _mockRepository.BackToRecordAll();
      _innerListenerMock.Expect (mock => mock.OnExecutionPlay (_wxeContext));
      _mockRepository.ReplayAll ();

      _securityListener.OnExecutionPlay (_wxeContext);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void OnExecutionStop ()
    {
      _innerListenerMock.Expect (mock => mock.OnExecutionStop (_wxeContext));
      _mockRepository.ReplayAll ();
      
      _securityListener.OnExecutionStop (_wxeContext);
      
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void OnExecutionPause ()
    {
      _innerListenerMock.Expect (mock => mock.OnExecutionPause (_wxeContext));
      _mockRepository.ReplayAll ();
      
      _securityListener.OnExecutionPause (_wxeContext);
      
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void OnExecutionFail ()
    {
      Exception exception = new Exception ();
      _innerListenerMock.Expect (mock => mock.OnExecutionFail (_wxeContext, exception));
      _mockRepository.ReplayAll ();
      
      _securityListener.OnExecutionFail (_wxeContext, exception);
      
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_securityListener.IsNull, Is.False);
    }
  }
}
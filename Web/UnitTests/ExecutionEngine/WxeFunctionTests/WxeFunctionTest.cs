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
using System.Web;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxeFunctionTests
{
  [TestFixture]
  public class WxeFunctionTest
  {
    private MockRepository _mockRepository;
    private WxeContext _context;
    private IWxeFunctionExecutionListener _executionListenerMock;

    [SetUp]
    public void SetUp ()
    {
      TestFunction rootFunction = new TestFunction ();
      WxeContextFactory contextFactory = new WxeContextFactory ();
      _context = contextFactory.CreateContext (rootFunction);
      _mockRepository = new MockRepository ();

      _executionListenerMock = _mockRepository.StrictMock<IWxeFunctionExecutionListener> ();
    }

    [Test]
    public void GetFunctionToken_AsRootFunction ()
    {
      TestFunction rootFunction = new TestFunction();
      PrivateInvoke.InvokeNonPublicMethod (rootFunction, "SetFunctionToken", "RootFunction");

      Assert.That (rootFunction.FunctionToken, Is.EqualTo ("RootFunction"));
    }

    [Test]
    public void GetFunctionToken_AsSubFunction ()
    {
      TestFunction rootFunction = new TestFunction();
      TestFunction subFunction = new TestFunction ();
      rootFunction.Add (subFunction);
      PrivateInvoke.InvokeNonPublicMethod (rootFunction, "SetFunctionToken", "RootFunction");

      Assert.That (subFunction.FunctionToken, Is.EqualTo ("RootFunction"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), 
        ExpectedMessage = "The WxeFunction does not have a RootFunction, i.e. the top-most WxeFunction does not have a FunctionToken.")]
    public void GetFunctionToken_MissingFunctionToken ()
    {
      TestFunction rootFunction = new TestFunction ();

      Dev.Null = rootFunction.FunctionToken;
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
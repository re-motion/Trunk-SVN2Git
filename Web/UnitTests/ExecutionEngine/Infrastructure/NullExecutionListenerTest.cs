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
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class NullExecutionListenerTest
  {
    private IWxeFunctionExecutionListener _executionListener;
    private WxeContext _context;

    [SetUp]
    public void SetUp ()
    {
      WxeContextFactory contextFactory = new WxeContextFactory();
      _context = contextFactory.CreateContext (new TestFunction());

      _executionListener = new NullExecutionListener();
    }

    [Test]
    public void OnExecutionPlay ()
    {
      _executionListener.OnExecutionPlay (_context);
    }

    [Test]
    public void OnExecutionStop ()
    {
      _executionListener.OnExecutionStop (_context);
    }

    [Test]
    public void OnExecutionPause ()
    {
      _executionListener.OnExecutionPause (_context);
    }

    [Test]
    public void OnExecutionFail ()
    {
      _executionListener.OnExecutionFail (_context, new Exception());
    }

    [Test]
    public void IsNull ()
    {
      INullObject nullListener = _executionListener;
      Assert.That (nullListener.IsNull);
    }

    [Test]
    public void IsSerializeable ()
    {
      Assert.That (Serializer.SerializeAndDeserialize (_executionListener), Is.Not.Null);
    }
  }
}
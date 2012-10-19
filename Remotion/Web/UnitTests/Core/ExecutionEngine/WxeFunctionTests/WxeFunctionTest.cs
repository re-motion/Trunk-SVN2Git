// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.WxeFunctionTests
{
  [TestFixture]
  public class WxeFunctionTest
  {
    private MockRepository _mockRepository;
    private IWxeFunctionExecutionListener _executionListenerMock;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _executionListenerMock = _mockRepository.StrictMock<IWxeFunctionExecutionListener>();
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
      TestFunction subFunction = new TestFunction();
      rootFunction.Add (subFunction);
      PrivateInvoke.InvokeNonPublicMethod (rootFunction, "SetFunctionToken", "RootFunction");

      Assert.That (subFunction.FunctionToken, Is.EqualTo ("RootFunction"));
    }

    [Test]
    public void GetFunctionToken_MissingFunctionToken ()
    {
      TestFunction rootFunction = new TestFunction();

      Assert.That (
          () => Dev.Null = rootFunction.FunctionToken,
          Throws.InvalidOperationException
              .With.Message.EqualTo ("The WxeFunction does not have a RootFunction, i.e. the top-most WxeFunction does not have a FunctionToken."));
    }

    [Test]
    public void GetExecutionListener ()
    {
      TestFunction2 function = new TestFunction2();
      Assert.That (function.ExecutionListener, Is.InstanceOf (typeof (NullExecutionListener)));
    }

    [Test]
    public void SetExecutionListener ()
    {
      TestFunction2 function = new TestFunction2();
      function.ExecutionListener = _executionListenerMock;
      Assert.That (function.ExecutionListener, Is.SameAs (_executionListenerMock));
    }

    [Test]
    public void SetExecutionListenerNull ()
    {
      TestFunction2 function = new TestFunction2();
      Assert.That (() => function.ExecutionListener = null, Throws.TypeOf<ArgumentNullException>());
    }
  }
}
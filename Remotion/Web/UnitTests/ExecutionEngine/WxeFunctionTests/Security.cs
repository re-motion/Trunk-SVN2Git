// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;
using Remotion.Security;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxeFunctionTests
{
  [TestFixture]
  public class Security
  {
    private MockRepository _mockRepository;
    private IWxeSecurityAdapter _mockWxeSecurityAdapter;
    private WxeContext _wxeContext;

    [SetUp]
    public void SetUp ()
    {
      TestFunction rootFunction = new TestFunction ();
      WxeContextFactory contextFactory = new WxeContextFactory ();
      _wxeContext = contextFactory.CreateContext (rootFunction);
      _mockRepository = new MockRepository ();
      _mockWxeSecurityAdapter = _mockRepository.StrictMock<IWxeSecurityAdapter> ();

      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), _mockWxeSecurityAdapter);
    }

    [Test]
    public void ExecuteFunctionWithAccessGranted ()
    {
      TestFunction function = new TestFunction ();
      _mockWxeSecurityAdapter.CheckAccess (function);
      _mockRepository.ReplayAll ();

      function.Execute (_wxeContext);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void HasStatelessAccessGranted ()
    {
      Expect.Call (_mockWxeSecurityAdapter.HasStatelessAccess (typeof (TestFunction))).Return (true);
      _mockRepository.ReplayAll ();

      bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

      _mockRepository.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasStatelessAccessDenied ()
    {
      Expect.Call (_mockWxeSecurityAdapter.HasStatelessAccess (typeof (TestFunction))).Return (false);
      _mockRepository.ReplayAll ();

      bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

      _mockRepository.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasStatelessAccessGrantedWithoutWxeSecurityProvider ()
    {
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), null);
      _mockRepository.ReplayAll ();

      bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

      _mockRepository.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }
  }
}

// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Security.Principal;
using NUnit.Framework;
using Remotion.Security;
using Remotion.Web.UnitTests.Security.Configuration;
using Remotion.Web.UnitTests.Security.Domain;
using Rhino.Mocks;
using Remotion.Security.Configuration;
using Remotion.Security.Metadata;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.Security.ExecutionEngine
{
  [TestFixture]
  public class WxeSecurityAdapterTestWithPermissionsFromInstanceMethod
  {
    // types

    // static members

    // member fields

    private IWxeSecurityAdapter _securityAdapter;
    private MockRepository _mocks;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private ISecurityProvider _mockSecurityProvider;
    private IUserProvider _userProvider;
    private IPrincipal _user;

    // construction and disposing

    public WxeSecurityAdapterTestWithPermissionsFromInstanceMethod ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _securityAdapter = new WxeSecurityAdapter ();

      _mocks = new MockRepository ();

      _mockSecurityProvider = _mocks.StrictMock<ISecurityProvider> ();
      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _userProvider = _mocks.StrictMock<IUserProvider> ();
      SetupResult.For (_userProvider.GetUser ()).Return (_user);

      _mockObjectSecurityStrategy = _mocks.StrictMock<IObjectSecurityStrategy> ();
      _mockFunctionalSecurityStrategy = _mocks.StrictMock<IFunctionalSecurityStrategy> ();

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
      SecurityConfiguration.Current.SecurityProvider = _mockSecurityProvider;
      SecurityConfiguration.Current.UserProvider = _userProvider;
      SecurityConfiguration.Current.PermissionProvider = new PermissionReflector ();
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _mockFunctionalSecurityStrategy;
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
    }

    [Test]
    public void CheckAccess_AccessGranted ()
    {
      ExpectObjectSecurityStrategyHasAccessForSecurableObject (GeneralAccessTypes.Read, true);
      SecurableObject thisObject = new SecurableObject (_mockObjectSecurityStrategy);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      _mocks.ReplayAll ();

      _securityAdapter.CheckAccess (function);

      _mocks.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void CheckAccess_AccessDenied ()
    {
      ExpectObjectSecurityStrategyHasAccessForSecurableObject (GeneralAccessTypes.Read, false);
      SecurableObject thisObject = new SecurableObject (_mockObjectSecurityStrategy);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      _mocks.ReplayAll ();

      _securityAdapter.CheckAccess (function);
    }

    [Test]
    public void CheckAccess_WithinSecurityFreeSection_AccessGranted ()
    {
      SecurableObject thisObject = new SecurableObject (_mockObjectSecurityStrategy);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      _mocks.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityAdapter.CheckAccess (function);
      }

      _mocks.VerifyAll ();
    }

    [Test]
    public void HasAccess_AccessGranted ()
    {
      ExpectObjectSecurityStrategyHasAccessForSecurableObject(GeneralAccessTypes.Read, true);
      SecurableObject thisObject = new SecurableObject (_mockObjectSecurityStrategy);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (function);

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccess_AccessDenied ()
    {
      ExpectObjectSecurityStrategyHasAccessForSecurableObject (GeneralAccessTypes.Read, false);
      SecurableObject thisObject = new SecurableObject (_mockObjectSecurityStrategy);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (function);

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasAccess_WithinSecurityFreeSection_AccessGranted ()
    {
      SecurableObject thisObject = new SecurableObject (_mockObjectSecurityStrategy);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      _mocks.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityAdapter.HasAccess (function);
      }

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasStatelessAccess_AccessGranted ()
    {
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject (GeneralAccessTypes.Read, true);
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasStatelessAccess (typeof (TestFunctionWithPermissionsFromInstanceMethod));

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasStatelessAccess_AccessDenied ()
    {
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject (GeneralAccessTypes.Read, false);
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasStatelessAccess (typeof (TestFunctionWithPermissionsFromInstanceMethod));

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasStatelessAccess_WithinSecurityFreeSection_AccessGranted ()
    {
      _mocks.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityAdapter.HasStatelessAccess (typeof (TestFunctionWithPermissionsFromInstanceMethod));
      }

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    private void ExpectObjectSecurityStrategyHasAccessForSecurableObject (Enum accessTypeEnum, bool returnValue)
    {
      Expect
          .Call (_mockObjectSecurityStrategy.HasAccess (_mockSecurityProvider, _user, AccessType.Get (accessTypeEnum)))
          .Return (returnValue);
    }

    private void ExpectFunctionalSecurityStrategyHasAccessForSecurableObject (Enum accessTypeEnum, bool returnValue)
    {
      Expect
          .Call (_mockFunctionalSecurityStrategy.HasAccess (typeof (SecurableObject), _mockSecurityProvider, _user, AccessType.Get (accessTypeEnum)))
          .Return (returnValue);
    }
  }
}

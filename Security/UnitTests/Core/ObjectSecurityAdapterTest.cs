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
using System.Security.Principal;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Security.Configuration;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.Core.Configuration;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class ObjectSecurityAdapterTest
  {
    // types

    // static members

    // member fields

    private IObjectSecurityAdapter _securityAdapter;
    private MockRepository _mocks;
    private SecurableObject _securableObject;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private ISecurityProvider _mockSecurityProvider;
    private IUserProvider _mockUserProvider;
    private IPrincipal _user;
    private IPermissionProvider _mockPermissionProvider;

    // construction and disposing

    public ObjectSecurityAdapterTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _securityAdapter = new ObjectSecurityAdapter ();

      _mocks = new MockRepository ();

      _mockSecurityProvider = _mocks.CreateMock<ISecurityProvider> ();
      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
      _mockUserProvider = _mocks.CreateMock<IUserProvider> ();
      _mockPermissionProvider = _mocks.CreateMock<IPermissionProvider> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      SetupResult.For (_mockUserProvider.GetUser ()).Return (_user);

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
      SecurityConfiguration.Current.SecurityProvider = _mockSecurityProvider;
      SecurityConfiguration.Current.UserProvider = _mockUserProvider;
      SecurityConfiguration.Current.PermissionProvider = _mockPermissionProvider;

      _mockObjectSecurityStrategy = _mocks.CreateMock<IObjectSecurityStrategy> ();
      _securableObject = new SecurableObject (_mockObjectSecurityStrategy);
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
    }

    [Test]
    public void HasAccessOnGetAccessor_AccessGranted ()
    {
      ExpectGetRequiredPropertyReadPermissions ("Name");
      ExpectExpectObjectSecurityStrategyHasAccess (true);
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccessOnGetAccessor (_securableObject, "Name");

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnGetAccessor_AccessDenied ()
    {
      ExpectGetRequiredPropertyReadPermissions ("Name");
      ExpectExpectObjectSecurityStrategyHasAccess (false);
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccessOnGetAccessor (_securableObject, "Name");

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasAccessOnGetAccessor_WithinSecurityFreeSeciton_AccessGranted ()
    {
      _mocks.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityAdapter.HasAccessOnGetAccessor (_securableObject, "Name");
      }

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnSetAccessor_AccessGranted ()
    {
      ExpectGetRequiredPropertyWritePermissions ("Name");
      ExpectExpectObjectSecurityStrategyHasAccess (true);
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccessOnSetAccessor (_securableObject, "Name");

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnSetAccessor_AccessDenied ()
    {
      ExpectGetRequiredPropertyWritePermissions ("Name");
      ExpectExpectObjectSecurityStrategyHasAccess (false);
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccessOnSetAccessor (_securableObject, "Name");

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasAccessOnSetAccessor_WithinSecurityFreeSeciton_AccessGranted ()
    {
      _mocks.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityAdapter.HasAccessOnSetAccessor (_securableObject, "Name");
      }

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    private void ExpectExpectObjectSecurityStrategyHasAccess (bool accessAllowed)
    {
      AccessType[] accessTypes = new AccessType[] { AccessType.Get (TestAccessTypes.First) };
      Expect.Call (_mockObjectSecurityStrategy.HasAccess (_mockSecurityProvider, _user, accessTypes)).Return (accessAllowed);
    }

    private void ExpectGetRequiredPropertyReadPermissions (string propertyName)
    {
      Expect.Call (_mockPermissionProvider.GetRequiredPropertyReadPermissions (typeof (SecurableObject), propertyName)).Return (new Enum[] { TestAccessTypes.First });
    }

    private void ExpectGetRequiredPropertyWritePermissions (string propertyName)
    {
      Expect.Call (_mockPermissionProvider.GetRequiredPropertyWritePermissions (typeof (SecurableObject), propertyName)).Return (new Enum[] { TestAccessTypes.First });
    }
  }
}
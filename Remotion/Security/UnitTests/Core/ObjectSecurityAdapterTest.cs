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
using Remotion.Reflection;
using Remotion.Security.Configuration;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.Core.Configuration;
using Remotion.Security.UnitTests.Core.SampleDomain;
using Rhino.Mocks;

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
    private IPrincipalProvider _mockPrincipalProvider;
    private ISecurityPrincipal _userStub;
    private IPermissionProvider _mockPermissionProvider;
    private IMemberResolver _mockMemberResolver;
    private IPropertyInformation _mockPropertyInformation;
    private IMethodInformation _mockMethodInformation;

    // construction and disposing

    public ObjectSecurityAdapterTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _securityAdapter = new ObjectSecurityAdapter();

      _mocks = new MockRepository();

      _mockSecurityProvider = _mocks.StrictMock<ISecurityProvider>();
      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
      _mockPrincipalProvider = _mocks.StrictMock<IPrincipalProvider>();
      _mockPermissionProvider = _mocks.StrictMock<IPermissionProvider>();
      _mockMemberResolver = _mocks.StrictMock<IMemberResolver>();
      _mockPropertyInformation = _mocks.StrictMock<IPropertyInformation>();
      _mockMethodInformation = _mocks.StrictMock<IMethodInformation>();

      _userStub = _mocks.Stub<ISecurityPrincipal>();
      SetupResult.For (_userStub.User).Return ("user");
      SetupResult.For (_mockPrincipalProvider.GetPrincipal()).Return (_userStub);

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration());
      SecurityConfiguration.Current.SecurityProvider = _mockSecurityProvider;
      SecurityConfiguration.Current.PrincipalProvider = _mockPrincipalProvider;
      SecurityConfiguration.Current.PermissionProvider = _mockPermissionProvider;
      SecurityConfiguration.Current.MemberResolver = _mockMemberResolver;

      _mockObjectSecurityStrategy = _mocks.StrictMock<IObjectSecurityStrategy>();
      _securableObject = new SecurableObject (_mockObjectSecurityStrategy);
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration());
    }

    [Test]
    public void HasAccessOnGetAccessor_AccessGranted ()
    {
      var mockMethodInformation = MockRepository.GenerateMock<IMethodInformation>();
      _mockPropertyInformation.Expect (mock => mock.GetGetMethod (true)).Return (mockMethodInformation);
      ExpectGetRequiredMethodPermissions (mockMethodInformation);

      ExpectExpectObjectSecurityStrategyHasAccess (true);
      _mocks.ReplayAll();

      bool hasAccess = _securityAdapter.HasAccessOnGetAccessor (_securableObject, _mockPropertyInformation);

      _mocks.VerifyAll();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnGetAccessor_AccessDenied ()
    {
      var mockMethodInformation = MockRepository.GenerateMock<IMethodInformation>();
      _mockPropertyInformation.Expect (mock => mock.GetGetMethod (true)).Return (mockMethodInformation);
      ExpectGetRequiredMethodPermissions (mockMethodInformation);
      ExpectExpectObjectSecurityStrategyHasAccess (false);
      _mocks.ReplayAll();

      bool hasAccess = _securityAdapter.HasAccessOnGetAccessor (_securableObject, _mockPropertyInformation);

      _mocks.VerifyAll();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasAccessOnGetAccessor_WithinSecurityFreeSeciton_AccessGranted ()
    {
      _mocks.ReplayAll();

      bool hasAccess;
      using (new SecurityFreeSection())
      {
        hasAccess = _securityAdapter.HasAccessOnGetAccessor (_securableObject, _mockPropertyInformation);
      }

      _mocks.VerifyAll();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnGetAccesor_GetGetMethodReturnsNull_NullMethodInformationInstanceIsUsed ()
    {
      _mockPropertyInformation.Expect (mock => mock.GetGetMethod (true)).Return (null);
      ExpectGetRequiredMethodPermissions (new NullMethodInformation());
      ExpectExpectObjectSecurityStrategyHasAccess (true);
      _mocks.ReplayAll();

      bool hasAccess = _securityAdapter.HasAccessOnGetAccessor (_securableObject, _mockPropertyInformation);

      _mocks.VerifyAll();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnSetAccessor_AccessGranted ()
    {
      var mockMethodInformation = MockRepository.GenerateMock<IMethodInformation>();
      _mockPropertyInformation.Expect (mock => mock.GetSetMethod (true)).Return (mockMethodInformation);
      ExpectGetRequiredMethodPermissions (mockMethodInformation);
      ExpectExpectObjectSecurityStrategyHasAccess (true);
      _mocks.ReplayAll();

      bool hasAccess = _securityAdapter.HasAccessOnSetAccessor (_securableObject, _mockPropertyInformation);

      _mocks.VerifyAll();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnSetAccessor_AccessDenied ()
    {
      var mockMethodInformation = MockRepository.GenerateMock<IMethodInformation>();
      _mockPropertyInformation.Expect (mock => mock.GetSetMethod (true)).Return (mockMethodInformation);
      ExpectGetRequiredMethodPermissions (mockMethodInformation);
      ExpectExpectObjectSecurityStrategyHasAccess (false);
      _mocks.ReplayAll();

      bool hasAccess = _securityAdapter.HasAccessOnSetAccessor (_securableObject, _mockPropertyInformation);

      _mocks.VerifyAll();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasAccessOnSetAccessor_WithinSecurityFreeSeciton_AccessGranted ()
    {
      _mocks.ReplayAll();

      bool hasAccess;
      using (new SecurityFreeSection())
      {
        hasAccess = _securityAdapter.HasAccessOnSetAccessor (_securableObject, _mockPropertyInformation);
      }

      _mocks.VerifyAll();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnSetAccesor_GetSetMethodReturnsNull_NullMethodInformationInstanceIsUsed ()
    {
      _mockPropertyInformation.Expect (mock => mock.GetSetMethod (true)).Return (null);
      ExpectGetRequiredMethodPermissions (new NullMethodInformation());
      ExpectExpectObjectSecurityStrategyHasAccess (true);
      _mocks.ReplayAll();

      bool hasAccess = _securityAdapter.HasAccessOnSetAccessor (_securableObject, _mockPropertyInformation);

      _mocks.VerifyAll();
      Assert.IsTrue (hasAccess);
    }

    private void ExpectExpectObjectSecurityStrategyHasAccess (bool accessAllowed)
    {
      AccessType[] accessTypes = new[] { AccessType.Get (TestAccessTypes.First) };
      Expect.Call (_mockObjectSecurityStrategy.HasAccess (_mockSecurityProvider, _userStub, accessTypes)).Return (accessAllowed);
    }

    private void ExpectGetRequiredMethodPermissions (IMethodInformation methodInformation)
    {
      Expect.Call (_mockPermissionProvider.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation))
          .Return (new Enum[] { TestAccessTypes.First });
    }
  }
}
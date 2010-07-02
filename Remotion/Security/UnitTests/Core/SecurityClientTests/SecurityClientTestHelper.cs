// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using Remotion.Reflection;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.Core.SampleDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.Core.SecurityClientTests
{
  public class SecurityClientTestHelper
  {
    public static SecurityClientTestHelper CreateForStatelessSecurity ()
    {
      return new SecurityClientTestHelper (SecurityContext.CreateStateless (typeof (SecurableObject)));
    }

    public static SecurityClientTestHelper CreateForStatefulSecurity ()
    {
      SecurityContext context = SecurityContext.Create (
          typeof (SecurableObject), "owner", "group", "tenant", new Dictionary<string, Enum>(), new Enum[0]);
      return new SecurityClientTestHelper (context);
    }

    private readonly MockRepository _mocks;
    private readonly ISecurityPrincipal _userStub;
    private readonly SecurityContext _context;
    private readonly ISecurityProvider _mockSecurityProvider;
    private readonly IPermissionProvider _mockPermissionReflector;
    private readonly IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private readonly IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private readonly IMemberResolver _mockMemberResolver;
    private readonly IPrincipalProvider _stubPrincipalProvider;
    private readonly SecurableObject _securableObject;

    private SecurityClientTestHelper (SecurityContext context)
    {
      _context = context;

      _mocks = new MockRepository();
      _mockSecurityProvider = _mocks.StrictMock<ISecurityProvider>();
      _mockPermissionReflector = _mocks.StrictMock<IPermissionProvider>();
      _mockObjectSecurityStrategy = _mocks.StrictMock<IObjectSecurityStrategy>();
      _mockFunctionalSecurityStrategy = _mocks.StrictMock<IFunctionalSecurityStrategy>();
      _mockMemberResolver = _mocks.StrictMock<IMemberResolver>();
      _userStub = _mocks.Stub<ISecurityPrincipal>();
      SetupResult.For (_userStub.User).Return ("user");
      _stubPrincipalProvider = _mocks.Stub<IPrincipalProvider>();
      SetupResult.For (_stubPrincipalProvider.GetPrincipal()).Return (_userStub);

      _securableObject = new SecurableObject (_mockObjectSecurityStrategy);
    }

    public SecurityClient CreateSecurityClient ()
    {
      return new SecurityClient (_mockSecurityProvider, _mockPermissionReflector, _stubPrincipalProvider, _mockFunctionalSecurityStrategy, _mockMemberResolver);
    }

    public SecurableObject SecurableObject
    {
      get { return _securableObject; }
    }

    public void ExpectMemberResolverGetMethodInformation (string methodName, MemberAffiliation memberAffiliation, IMethodInformation returnValue)
    {
      Expect.Call (_mockMemberResolver.GetMethodInformation (typeof (SecurableObject), methodName, memberAffiliation)).Return (returnValue);
    }

    public void ExpectMemberResolverGetPropertyInformation (string propertyName, IPropertyInformation returnValue)
    {
      Expect.Call (_mockMemberResolver.GetPropertyInformation (typeof (SecurableObject), propertyName)).Return (returnValue);
    }

    public void ExpectPermissionReflectorGetRequiredMethodPermissions (string methodName, params Enum[] returnValue)
    {
      Expect.Call (_mockPermissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodName)).Return (returnValue);
    }

    public void ExpectPermissionReflectorGetRequiredMethodPermissions (IMethodInformation methodInformation, params Enum[] returnValue)
    {
      Expect.Call (_mockPermissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation)).Return (returnValue);
    }

    public void ExpectPermissionReflectorGetRequiredStaticMethodPermissions (string methodName, params Enum[] returnValue)
    {
      Expect.Call (_mockPermissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), methodName)).Return (returnValue);
    }

    public void ExpectPermissionReflectorGetRequiredStaticMethodPermissions (IMethodInformation methodInformation, params Enum[] returnValue)
    {
      Expect.Call (_mockPermissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), methodInformation)).Return (returnValue);
    }

    public void ExpectPermissionReflectorGetRequiredPropertyWritePermissions (string propertyName, params Enum[] accessTypes)
    {
      Expect.Call (_mockPermissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), propertyName)).Return (accessTypes);
    }

    public void ExpectPermissionReflectorGetRequiredPropertyWritePermissions (IPropertyInformation propertyInformation, params Enum[] accessTypes)
    {
      Expect.Call (_mockPermissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), propertyInformation)).Return (accessTypes);
    }

    public void ExpectPermissionReflectorGetRequiredPropertyReadPermissions (string propertyName, params Enum[] accessTypes)
    {
      Expect.Call (_mockPermissionReflector.GetRequiredPropertyReadPermissions (typeof (SecurableObject), propertyName)).Return (accessTypes);
    }

    public void ExpectPermissionReflectorGetRequiredPropertyReadPermissions (IPropertyInformation propertyInformation, params Enum[] returnValue)
    {
      Expect.Call (_mockPermissionReflector.GetRequiredPropertyReadPermissions (typeof (SecurableObject), propertyInformation)).Return (returnValue);
    }

    public void ExpectObjectSecurityStrategyHasAccess (Enum requiredAccessType, bool returnValue)
    {
      ExpectObjectSecurityStrategyHasAccess (new[] { requiredAccessType }, returnValue);
    }

    public void ExpectObjectSecurityStrategyHasAccess (Enum[] requiredAccessTypes, bool returnValue)
    {
      Expect
          .Call (_mockObjectSecurityStrategy.HasAccess (_mockSecurityProvider, _userStub, ConvertAccessTypeEnums (requiredAccessTypes)))
          .Return (returnValue);
    }

    public void ExpectFunctionalSecurityStrategyHasAccess (Enum requiredAccessType, bool returnValue)
    {
      ExpectFunctionalSecurityStrategyHasAccess (new[] { requiredAccessType }, returnValue);
    }

    public void ExpectFunctionalSecurityStrategyHasAccess (Enum[] requiredAccessTypes, bool returnValue)
    {
      Expect
          .Call (
          _mockFunctionalSecurityStrategy.HasAccess (
              typeof (SecurableObject), _mockSecurityProvider, _userStub, ConvertAccessTypeEnums (requiredAccessTypes)))
          .Return (returnValue);
    }

    public void ExpectSecurityProviderGetAccess (params Enum[] returnValue)
    {
      AccessType[] accessTypes = Array.ConvertAll (returnValue, new Converter<Enum, AccessType> (AccessType.Get));
      Expect.Call (_mockSecurityProvider.GetAccess (_context, _userStub)).Return (accessTypes);
    }

    public void ReplayAll ()
    {
      _mocks.ReplayAll();
    }

    public void VerifyAll ()
    {
      _mocks.VerifyAll();
    }

    private AccessType[] ConvertAccessTypeEnums (Enum[] accessTypeEnums)
    {
      return Array.ConvertAll (accessTypeEnums, new Converter<Enum, AccessType> (AccessType.Get));
    }
  }
}

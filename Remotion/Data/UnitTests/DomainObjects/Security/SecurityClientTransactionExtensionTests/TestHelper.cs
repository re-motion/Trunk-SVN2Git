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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Security;
using Remotion.Data.UnitTests.DomainObjects.Security.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.Security.Metadata;
using Remotion.Utilities;
using Rhino.Mocks;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Data.UnitTests.DomainObjects.Security.SecurityClientTransactionExtensionTests
{
  public delegate bool HasAccessDelegate (ISecurityProvider securityProvider, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes);
  public delegate bool HasStatelessAccessDelegate (Type type, ISecurityProvider securityProvider, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes);

  public class TestHelper
  {
    // types

    // static members

    // member fields

    private readonly MockRepository _mocks;
    private readonly ISecurityPrincipal _stubUser;
    private readonly ISecurityProvider _mockSecurityProvider;
    private readonly IPrincipalProvider _stubPrincipalProvider;
    private readonly IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private readonly IPermissionProvider _mockPermissionReflector;
    private readonly IMemberResolver _mockMemberResolver;
    private readonly ClientTransaction _transaction;

    // construction and disposing

    public TestHelper ()
    {
      _mocks = new MockRepository ();
      _mockSecurityProvider = _mocks.StrictMock<ISecurityProvider> ();
      _stubUser = _mocks.Stub<ISecurityPrincipal> ();
      SetupResult.For (_stubUser.User).Return ("user");
      _stubPrincipalProvider = _mocks.StrictMock<IPrincipalProvider> ();
      SetupResult.For (_stubPrincipalProvider.GetPrincipal ()).Return (_stubUser);
      _mockFunctionalSecurityStrategy = _mocks.StrictMock<IFunctionalSecurityStrategy> ();
      _mockPermissionReflector = _mocks.StrictMock<IPermissionProvider> ();
      _mockMemberResolver = _mocks.StrictMock<IMemberResolver>();
      _transaction = ClientTransaction.CreateRootTransaction();

      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
    }

    // methods and properties

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public SecurableObject CreateSecurableObject ()
    {
      return SecurableObject.NewObject (_transaction, CreateObjectSecurityStrategy ());
    }

    public NonSecurableObject CreateNonSecurableObject ()
    {
      return NonSecurableObject.NewObject (_transaction);
    }

    public IObjectSecurityStrategy CreateObjectSecurityStrategy ()
    {
      return _mocks.StrictMock<IObjectSecurityStrategy> ();
    }

    public void SetupSecurityConfiguration ()
    {
      PrivateInvoke.InvokeNonPublicStaticMethod (typeof (SecurityConfiguration), "SetCurrent", new SecurityConfiguration ());
      SecurityConfiguration.Current.SecurityProvider = _mockSecurityProvider;
      SecurityConfiguration.Current.PrincipalProvider = _stubPrincipalProvider;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _mockFunctionalSecurityStrategy;
      SecurityConfiguration.Current.PermissionProvider = _mockPermissionReflector;
      SecurityConfiguration.Current.MemberResolver = _mockMemberResolver;
    }

    public void TearDownSecurityConfiguration ()
    {
      PrivateInvoke.InvokeNonPublicStaticMethod (typeof (SecurityConfiguration), "SetCurrent", new SecurityConfiguration ());
    }

    public void ReplayAll ()
    {
      _mocks.ReplayAll ();
    }

    public void VerifyAll ()
    {
      _mocks.VerifyAll ();
    }

    public IDisposable Ordered ()
    {
      return _mocks.Ordered ();
    }

    public void AddExtension (IClientTransactionExtension extension)
    {
      ArgumentUtility.CheckNotNullAndType<SecurityClientTransactionExtension> ("extension", extension);

      _transaction.Extensions.Add (typeof (SecurityClientTransactionExtension).FullName, extension);
    }

    public void ExpectObjectSecurityStrategyHasAccess (SecurableObject securableObject, Enum accessTypeEnum, HasAccessDelegate doDelegate)
    {
      IObjectSecurityStrategy objectSecurityStrategy = securableObject.GetSecurityStrategy ();
      Expect
          .Call (objectSecurityStrategy.HasAccess (_mockSecurityProvider, _stubUser, AccessType.Get (accessTypeEnum)))
          .WhenCalled (mi => CheckCurrentTransaction())
          .Do (doDelegate);
    }

    public void ExpectObjectSecurityStrategyHasAccess (SecurableObject securableObject, Enum accessTypeEnum, bool returnValue)
    {
      IObjectSecurityStrategy objectSecurityStrategy = securableObject.GetSecurityStrategy ();
      Expect
          .Call (objectSecurityStrategy.HasAccess (_mockSecurityProvider, _stubUser, AccessType.Get (accessTypeEnum)))
          .WhenCalled (mi => CheckCurrentTransaction ())
          .Return (returnValue);
    }

    public void ExpectFunctionalSecurityStrategyHasAccess (Type securableObjectType, Enum accessTypeEnum, HasStatelessAccessDelegate doDelegate)
    {
      Expect
          .Call (_mockFunctionalSecurityStrategy.HasAccess (securableObjectType, _mockSecurityProvider, _stubUser, AccessType.Get (accessTypeEnum)))
          .WhenCalled (mi => CheckCurrentTransaction ())
          .Do (doDelegate);
    }

    public void ExpectFunctionalSecurityStrategyHasAccess (Type securableObjectType, Enum accessTypeEnum, bool returnValue)
    {
      Expect
          .Call (_mockFunctionalSecurityStrategy.HasAccess (securableObjectType, _mockSecurityProvider, _stubUser, AccessType.Get (accessTypeEnum)))
          .WhenCalled (mi => CheckCurrentTransaction ())
          .Return (returnValue);
    }

    public void ExpectPermissionReflectorGetRequiredPropertyWritePermissions (IPropertyInformation propertyInformation, params Enum[] returnedAccessTypes)
    {
      Expect
          .Call (_mockPermissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), propertyInformation))
          .WhenCalled (mi => CheckCurrentTransaction ())
          .Return (returnedAccessTypes);
    }

    public void ExpectPermissionReflectorGetRequiredPropertyWritePermissions (PropertyInfo propertyInfo, params Enum[] returnedAccessTypes)
    {
      Expect
          .Call (_mockPermissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), new PropertyInfoAdapter(propertyInfo)))
          .WhenCalled (mi => CheckCurrentTransaction ())
          .Return (returnedAccessTypes);
    }

    public void ExpectPermissionReflectorGetRequiredMethodPermissions (IMethodInformation methodInformation, params Enum[] returnedAccessTypes)
    {
      Expect.Call (
          _mockPermissionReflector.GetRequiredMethodPermissions (
          Arg<Type>.Matches (n => n == typeof (SecurableObject)),
          Arg<IMethodInformation>.Is.Anything))
          .Return (returnedAccessTypes);
    }

    public void ExpectMemberResolverGetPropertyInformation (string propertyName, IPropertyInformation returnValue)
    {
      Expect
        .Call (_mockMemberResolver.GetPropertyInformation (typeof (SecurableObject), propertyName))
        .Return (returnValue);
    }

    public void ExpectMemberResolverGetPropertyInformation (PropertyInfo propertyInfo, IPropertyInformation returnValue)
    {
      Expect
        .Call (_mockMemberResolver.GetPropertyInformation (typeof (SecurableObject), propertyInfo))
        .Return (returnValue);
    }

    public void ExpectSecurityProviderGetAccess (SecurityContext context, params Enum[] returnedAccessTypes)
    {
      Expect
          .Call (_mockSecurityProvider.GetAccess (context, _stubUser))
          .WhenCalled (mi => CheckCurrentTransaction ())
          .Return (Array.ConvertAll<Enum, AccessType> (returnedAccessTypes, AccessType.Get));
    }

    private void CheckCurrentTransaction ()
    {
      Assert.That (ClientTransaction.Current, Is.SameAs (_transaction));
    }
  }
}

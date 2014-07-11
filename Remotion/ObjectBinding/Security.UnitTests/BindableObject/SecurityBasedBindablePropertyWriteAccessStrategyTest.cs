﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.Security.BindableObject;
using Remotion.ObjectBinding.Security.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.Security;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Security.UnitTests.BindableObject
{
  [TestFixture]
  public class SecurityBasedBindablePropertyWriteAccessStrategyTest
  {
    private BindableObjectClass _bindableClass;
    private SecurableClassWithReferenceType<string> _securableObject;
    private ServiceLocatorScope _serviceLocatorScope;
    private ISecurityProvider _securityProviderStub;
    private ISecurityPrincipal _principalStub;
    private IObjectSecurityStrategy _objectSecurityStrategyMock;
    private SecurityBasedBindablePropertyWriteAccessStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _bindableClass = new BindableObjectClass (
          typeof (ClassWithReferenceType<string>),
          CreateBindableObjectProviderWithStubBusinessObjectServiceFactory(),
          new PropertyBase[0]);

      _objectSecurityStrategyMock = MockRepository.GenerateStrictMock<IObjectSecurityStrategy>();

      _securableObject = new SecurableClassWithReferenceType<string> (_objectSecurityStrategyMock);

      _securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();

      _principalStub = MockRepository.GenerateStub<ISecurityPrincipal>();
      var principalProviderStub = MockRepository.GenerateStub<IPrincipalProvider>();
      principalProviderStub.Stub (_ => _.GetPrincipal()).Return (_principalStub);

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle (() => _securityProviderStub);
      serviceLocator.RegisterSingle (() => principalProviderStub);
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);

      _strategy = new SecurityBasedBindablePropertyWriteAccessStrategy();
    }

    [TearDown]
    public void TearDown ()
    {
      _serviceLocatorScope.Dispose();
    }

    [Test]
    public void CanWrite_WithBusinessObjectIsNull_ReturnsTrue ()
    {
      var bindableProperty = CreateBindableProperty ((() => ((ClassWithReferenceType<string>) null).Scalar));

      var result = _strategy.CanWrite (_bindableClass, bindableProperty, null);

      Assert.That (result, Is.True);
    }

    [Test]
    public void CanWrite_WithNonSecurableObject_ReturnsTrue ()
    {
      var bindableProperty = CreateBindableProperty ((() => ((ClassWithReferenceType<string>) null).Scalar));

      var result = _strategy.CanWrite (_bindableClass, bindableProperty, new ClassWithReferenceType<string>());

      Assert.That (result, Is.True);
    }

    [Test]
    public void CanWrite_WithSecurableObject_EvaluatesObjectSecurityStratey_ReturnsResult ()
    {
      var expectedResult = BooleanObjectMother.GetRandomBoolean();
      ExpectHasAccessOnObjectSecurityStrategy (expectedResult, CustomAccessTypes.CustomEdit);

      var bindableProperty = CreateBindableProperty (() => ((SecurableClassWithReferenceType<string>) null).CustomPermissisons);

      var actualResult = _strategy.CanWrite (_bindableClass, bindableProperty, _securableObject);

      Assert.That (actualResult, Is.EqualTo (expectedResult));
      _objectSecurityStrategyMock.VerifyAllExpectations();
    }

    [Test]
    public void CanWrite_WithSecurableObject_WithoutSetter_UsesNullMethodInfo_ReturnsResult ()
    {
      var expectedResult = BooleanObjectMother.GetRandomBoolean();
      ExpectHasAccessOnObjectSecurityStrategy (expectedResult, GeneralAccessTypes.Edit);

      var bindableProperty = CreateBindableProperty ((() => ((ClassWithReferenceType<string>) null).PropertyWithNoSetter));

      var actualResult = _strategy.CanWrite (_bindableClass, bindableProperty, _securableObject);

      Assert.That (actualResult, Is.EqualTo (expectedResult));
      _objectSecurityStrategyMock.VerifyAllExpectations();
    }

    private PropertyBase CreateBindableProperty<TPropertyType> (Expression<Func<TPropertyType>> propertyExpression)
    {
      return new StubPropertyBase (
          GetPropertyParameters (PropertyInfoAdapter.Create (MemberInfoFromExpressionUtility.GetProperty (propertyExpression))));
    }

    private PropertyBase.Parameters GetPropertyParameters (IPropertyInformation propertyInformation)
    {
      return new PropertyBase.Parameters (
          CreateBindableObjectProviderWithStubBusinessObjectServiceFactory(),
          propertyInformation,
          typeof (IBusinessObject),
          typeof (IBusinessObject),
          null,
          false,
          false,
          MockRepository.GenerateStub<IDefaultValueStrategy>(),
          MockRepository.GenerateStub<IObjectSecurityAdapter>());
    }

    private void ExpectHasAccessOnObjectSecurityStrategy (bool expectedResult, Enum accessType)
    {
      _objectSecurityStrategyMock.Expect (
          _ => _.HasAccess (
              Arg.Is (_securityProviderStub),
              Arg.Is (_principalStub),
              Arg<IReadOnlyList<AccessType>>.List.Equal (new[] { AccessType.Get (accessType) })))
          .Return (expectedResult);
    }

    protected BindableObjectProvider CreateBindableObjectProviderWithStubBusinessObjectServiceFactory ()
    {
      return new BindableObjectProvider (BindableObjectMetadataFactory.Create(), MockRepository.GenerateStub<IBusinessObjectServiceFactory>());
    }
  }
}
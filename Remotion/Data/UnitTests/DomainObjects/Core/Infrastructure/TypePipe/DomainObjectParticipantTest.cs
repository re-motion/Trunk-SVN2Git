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
using System.Linq;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Implementation;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.TypePipe
{
  [TestFixture]
  public class DomainObjectParticipantTest
  {
    private ITypeDefinitionProvider _typeDefinitionProviderMock;

    private DomainObjectParticipant _participant;
    private IInterceptedPropertyFinder _interceptedPropertyFinderMock;

    private TypeContext _typeContext;
    private ProxyType _proxyType;

    [SetUp]
    public void SetUp ()
    {
      _typeDefinitionProviderMock = MockRepository.GenerateStrictMock<ITypeDefinitionProvider>();
      _interceptedPropertyFinderMock = MockRepository.GenerateStrictMock<IInterceptedPropertyFinder>();

      _participant = new DomainObjectParticipant (_typeDefinitionProviderMock, _interceptedPropertyFinderMock);

      _typeContext = new TypeContext (new MutableTypeFactory(), typeof (Order));
      _proxyType = _typeContext.ProxyType;
    }

    [Test]
    public void PartialCacheKeyProvider ()
    {
      var cacheKeyProvider = _participant.PartialCacheKeyProvider;
      // Retrieving the property does not cause any calls to the mock objects.

      var requestedType = ReflectionObjectMother.GetSomeType();
      var fakePublicDomainType = ReflectionObjectMother.GetSomeOtherType();
      var fakeClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      _typeDefinitionProviderMock.Expect (mock => mock.GetPublicDomainObjectType (requestedType)).Return (fakePublicDomainType);
      _typeDefinitionProviderMock.Expect (mock => mock.GetTypeDefinition (fakePublicDomainType)).Return (fakeClassDefinition);

      var result = cacheKeyProvider.GetCacheKey (requestedType);

      _typeDefinitionProviderMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeClassDefinition));
    }

    [Test]
    public void ModifyType_RetrievesDomainObjectType_AndUsesItToGetInterceptedProperties ()
    {
      var fakeDomainObjectType = ReflectionObjectMother.GetSomeType();
      var fakeClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      var fakeInterceptors = new IAccessorInterceptor[0];
      _typeDefinitionProviderMock.Expect (mock => mock.GetPublicDomainObjectType (_proxyType.BaseType)).Return (fakeDomainObjectType);
      _typeDefinitionProviderMock.Expect (mock => mock.GetTypeDefinition (fakeDomainObjectType)).Return (fakeClassDefinition);
      _interceptedPropertyFinderMock.Expect (mock => mock.GetPropertyInterceptors (fakeClassDefinition, _proxyType.BaseType)).Return (fakeInterceptors);

      _participant.Modify (_typeContext);

      _typeDefinitionProviderMock.VerifyAllExpectations();
      _interceptedPropertyFinderMock.VerifyAllExpectations();
    }

    [Test]
    public void ModifyType_AddsMarkerInterface_And_OverridesHooks ()
    {
      var fakeDomainObjectType = ReflectionObjectMother.GetSomeType();
      StubGetPropertyInterceptors (fakeDomainObjectType);

      _participant.Modify (_typeContext);

      Assert.That (_proxyType.AddedInterfaces, Is.EqualTo (new[] { typeof (IInterceptedDomainObject) }));
      Assert.That (_proxyType.AddedMethods, Has.Count.EqualTo (2));
      
      var performConstructorCheck = _proxyType.AddedMethods.Single (m => m.Name == "PerformConstructorCheck");
      Assert.That (performConstructorCheck.Body, Is.TypeOf<DefaultExpression>().And.Property ("Type").SameAs (typeof (void)));

      var getPublicDomainObjectTypeImplementation = _proxyType.AddedMethods.Single (m => m.Name == "GetPublicDomainObjectTypeImplementation");
      Assert.That (getPublicDomainObjectTypeImplementation.Body, Is.TypeOf<ConstantExpression> ().And.Property ("Value").SameAs (fakeDomainObjectType));
    }

    [Test]
    public void ModifyType_ExecutesAccessorInterceptors ()
    {
      var accessorInterceptor = MockRepository.GenerateStrictMock<IAccessorInterceptor>();
      accessorInterceptor.Expect (mock => mock.Intercept (_proxyType));
      StubGetPropertyInterceptors (accessorInterceptors: new[] { accessorInterceptor });

      _participant.Modify (_typeContext);

      accessorInterceptor.VerifyAllExpectations();
    }

    private void StubGetPropertyInterceptors (Type publicDomainObjectType = null, params IAccessorInterceptor[] accessorInterceptors)
    {
      publicDomainObjectType = publicDomainObjectType ?? ReflectionObjectMother.GetSomeType();
      var fakeClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      _typeDefinitionProviderMock.Stub (stub => stub.GetPublicDomainObjectType (Arg<Type>.Is.Anything)).Return (publicDomainObjectType);
      _typeDefinitionProviderMock.Stub (stub => stub.GetTypeDefinition (Arg<Type>.Is.Anything)).Return (fakeClassDefinition);
      _interceptedPropertyFinderMock.Stub (stub => stub.GetPropertyInterceptors (null, null)).IgnoreArguments().Return (accessorInterceptors);
    }
  }
}
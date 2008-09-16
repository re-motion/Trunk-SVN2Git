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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class SupportsSearchAvailableObjects : TestBase
  {
    private MockRepository _mockRepository;
    private BindableObjectProvider _bindableObjectProvider;
    private BindableObjectProvider _bindableObjectWithIdentityProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();
      _bindableObjectProvider = new BindableObjectProvider ();
      _bindableObjectWithIdentityProvider = new BindableObjectProvider ();

      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute> (_bindableObjectProvider);
      BusinessObjectProvider.SetProvider<BindableObjectWithIdentityProviderAttribute> (_bindableObjectWithIdentityProvider);
    }

    [Test]
    public void SearchServiceFromType ()
    {
      ISearchServiceOnProperty mockService = _mockRepository.StrictMock<ISearchServiceOnProperty>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromProperty");

      Expect.Call (mockService.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectWithIdentityProvider.AddService (mockService);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void SearchServiceFromPropertyType ()
    {
      ISearchServiceOnProperty mockService = _mockRepository.StrictMock<ISearchServiceOnProperty>();
      ISearchServiceOnType stubSearchServiceOnType = _mockRepository.StrictMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromProperty");

      Expect.Call (mockService.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectWithIdentityProvider.AddService (stubSearchServiceOnType);
      _bindableObjectWithIdentityProvider.AddService (mockService);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void UnknownSearchService ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromType");

      Assert.That (property.SupportsSearchAvailableObjects, Is.False);
    }

    [Test]
    public void WithoutSearchServiceAttribute_AndDefaultSearchService ()
    {
      ISearchAvailableObjectsService mockAvailableObjectsService = _mockRepository.StrictMock<ISearchAvailableObjectsService>();
      IBusinessObjectClassService mockBusinessObjectClassService = _mockRepository.StrictMock<IBusinessObjectClassService>();
      IBusinessObjectProvider mockBusinessObjectProvider = _mockRepository.StrictMock<IBusinessObjectProvider>();
      IBusinessObjectClassWithIdentity mockBusinessObjectClassWithIdentity = _mockRepository.StrictMock<IBusinessObjectClassWithIdentity>();
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService");

      Expect.Call (mockBusinessObjectClassWithIdentity.BusinessObjectProvider).Return (mockBusinessObjectProvider).Repeat.Any ();
      Expect.Call (mockBusinessObjectProvider.GetService (typeof (ISearchAvailableObjectsService))).Return (mockAvailableObjectsService);
      Expect.Call (mockBusinessObjectClassService.GetBusinessObjectClass (typeof (ClassFromOtherBusinessObjectImplementation)))
          .Return (mockBusinessObjectClassWithIdentity);
      Expect.Call (mockAvailableObjectsService.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectProvider.AddService (mockBusinessObjectClassService);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void WithoutSearchServiceAttribute_AndNoDefaultSearchService ()
    {
      IBusinessObjectClassService mockBusinessObjectClassService = _mockRepository.StrictMock<IBusinessObjectClassService> ();
      IBusinessObjectProvider mockBusinessObjectProvider = _mockRepository.StrictMock<IBusinessObjectProvider> ();
      IBusinessObjectClassWithIdentity mockBusinessObjectClassWithIdentity = _mockRepository.StrictMock<IBusinessObjectClassWithIdentity> ();
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService");

      Expect.Call (mockBusinessObjectClassWithIdentity.BusinessObjectProvider).Return (mockBusinessObjectProvider).Repeat.Any ();
      Expect.Call (mockBusinessObjectProvider.GetService (typeof (ISearchAvailableObjectsService))).Return (null);
      Expect.Call (mockBusinessObjectClassService.GetBusinessObjectClass (typeof (ClassFromOtherBusinessObjectImplementation)))
          .Return (mockBusinessObjectClassWithIdentity);
      _mockRepository.ReplayAll ();
      
      _bindableObjectProvider.AddService (mockBusinessObjectClassService);

      Assert.That (property.SupportsSearchAvailableObjects, Is.False);
    }

    private ReferenceProperty CreateProperty (string propertyName)
    {
      PropertyBase.Parameters propertyParameters = GetPropertyParameters (propertyName);
      return new ReferenceProperty (propertyParameters, TypeFactory.GetConcreteType (propertyParameters.UnderlyingType));
    }

    private ReferenceProperty CreatePropertyWithoutMixing (string propertyName)
    {
      PropertyBase.Parameters propertyParameters = GetPropertyParameters (propertyName);
      return new ReferenceProperty (propertyParameters, propertyParameters.UnderlyingType);
    }

    private PropertyBase.Parameters GetPropertyParameters (string propertyName)
    {
      return GetPropertyParameters (GetPropertyInfo (typeof (ClassWithBusinessObjectProperties), propertyName), _bindableObjectProvider);
    }
  }
}

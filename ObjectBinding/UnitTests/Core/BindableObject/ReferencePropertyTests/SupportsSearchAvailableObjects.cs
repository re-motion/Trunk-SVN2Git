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
    private BindableObjectProvider _businessObjectProvider;
    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = new BindableObjectProvider();
      _mockRepository = new MockRepository();
    }

    [Test]
    public void SearchServiceFromType_AndRequiresIdentity ()
    {
      ISearchServiceOnProperty mockService = _mockRepository.StrictMock<ISearchServiceOnProperty>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyWithIdentity");

      Expect.Call (mockService.SupportsIdentity (property)).Return (true);
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (mockService);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void SearchServiceFromType_AndNotRequiresIdentity ()
    {
      ISearchServiceOnType mockService = _mockRepository.StrictMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromType");

      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (mockService);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void SearchServiceFromPropertyType ()
    {
      ISearchServiceOnProperty mockService = _mockRepository.StrictMock<ISearchServiceOnProperty>();
      ISearchServiceOnType stubSearchServiceOnType = _mockRepository.StrictMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyWithIdentity");

      Expect.Call (mockService.SupportsIdentity (property)).Return (true);
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (stubSearchServiceOnType);
      _businessObjectProvider.AddService (mockService);
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
      IBusinessObjectClassWithIdentity mockBusinessObjectClassWithIdentity = _mockRepository.StrictMock<IBusinessObjectClassWithIdentity>();
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchServiceWithIdentity");

      Expect.Call (mockBusinessObjectClassService.GetBusinessObjectClass (typeof (ClassWithIdentityFromOtherBusinessObjectImplementation)))
        .Return (mockBusinessObjectClassWithIdentity);
      Expect.Call (mockAvailableObjectsService.SupportsIdentity (property)).Return (true);
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (mockAvailableObjectsService);
      _businessObjectProvider.AddService (mockBusinessObjectClassService);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void WithoutSearchServiceAttribute_AndNoDefaultSearchService ()
    {
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService");

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
      return GetPropertyParameters (GetPropertyInfo (typeof (ClassWithBusinessObjectProperties), propertyName), _businessObjectProvider);
    }
  }
}

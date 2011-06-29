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
using NUnit.Framework;
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
      _bindableObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory ();
      _bindableObjectWithIdentityProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory ();

      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute> (_bindableObjectProvider);
      BusinessObjectProvider.SetProvider<BindableObjectWithIdentityProviderAttribute> (_bindableObjectWithIdentityProvider);
    }

    [Test]
    public void SearchServiceFromPropertyType ()
    {
      var serviceMock = _mockRepository.StrictMock<ISearchServiceOnType> ();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyType");

      Expect.Call (serviceMock.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectWithIdentityProvider.AddService (serviceMock);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void SearchServiceFromPropertyDeclaration ()
    {
      var serviceMock = _mockRepository.StrictMock<ISearchServiceOnProperty>();
      var stubSearchServiceOnType = _mockRepository.StrictMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyDeclaration");

      Expect.Call (serviceMock.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectWithIdentityProvider.AddService (stubSearchServiceOnType);
      _bindableObjectProvider.AddService (serviceMock);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void UnknownSearchService ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyType");

      Assert.That (property.SupportsSearchAvailableObjects, Is.False);
    }

    [Test]
    public void WithoutSearchServiceAttribute_AndDefaultSearchService_FromPropertyDeclaration ()
    {
      var searchAvailableObjectsServiceMock = _mockRepository.StrictMock<ISearchAvailableObjectsService>();
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService");

      Expect.Call (searchAvailableObjectsServiceMock.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectProvider.AddService (searchAvailableObjectsServiceMock);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    [Ignore ("TODO RM-4105: Extend fallback behavior to include property type.")]
    public void WithoutSearchServiceAttribute_AndDefaultSearchService_FromPropertyType ()
    {
      var searchAvailableObjectsServiceMock = _mockRepository.StrictMock<ISearchAvailableObjectsService> ();
      var businessObjectClassServiceMock = _mockRepository.StrictMock<IBusinessObjectClassService> ();
      var businessObjectProviderMock = _mockRepository.StrictMock<IBusinessObjectProvider> ();
      var businessObjectClassWithIdentityMock = _mockRepository.StrictMock<IBusinessObjectClassWithIdentity> ();
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService");

      Expect.Call (businessObjectClassWithIdentityMock.BusinessObjectProvider).Return (businessObjectProviderMock).Repeat.Any ();
      Expect.Call (businessObjectProviderMock.GetService (typeof (ISearchAvailableObjectsService))).Return (searchAvailableObjectsServiceMock);
      Expect.Call (businessObjectClassServiceMock.GetBusinessObjectClass (typeof (ClassFromOtherBusinessObjectImplementation)))
          .Return (businessObjectClassWithIdentityMock);
      Expect.Call (searchAvailableObjectsServiceMock.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll ();

      _bindableObjectProvider.AddService (businessObjectClassServiceMock);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void WithoutSearchServiceAttribute_AndNoDefaultSearchService_FromPropertyDeclaration ()
    {
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService");

      _mockRepository.ReplayAll ();

      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.False);
    }

    [Test]
    [Ignore ("TODO RM-4105: Extend fallback behavior to include property type.")]
    public void WithoutSearchServiceAttribute_AndNoDefaultSearchService_FromPropertyType ()
    {
      IBusinessObjectClassService businessObjectClassServiceMock = _mockRepository.StrictMock<IBusinessObjectClassService> ();
      IBusinessObjectProvider businessObjectProviderMock = _mockRepository.StrictMock<IBusinessObjectProvider> ();
      IBusinessObjectClassWithIdentity businessObjectClassWithIdentityMock = _mockRepository.StrictMock<IBusinessObjectClassWithIdentity> ();
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService");

      Expect.Call (businessObjectClassWithIdentityMock.BusinessObjectProvider).Return (businessObjectProviderMock).Repeat.Any ();
      Expect.Call (businessObjectProviderMock.GetService (typeof (ISearchAvailableObjectsService))).Return (null);
      Expect.Call (businessObjectClassServiceMock.GetBusinessObjectClass (typeof (ClassFromOtherBusinessObjectImplementation)))
          .Return (businessObjectClassWithIdentityMock);
      _mockRepository.ReplayAll ();
      
      _bindableObjectProvider.AddService (businessObjectClassServiceMock);

      _mockRepository.VerifyAll ();
      Assert.That (property.SupportsSearchAvailableObjects, Is.False);
    }

    private ReferenceProperty CreateProperty (string propertyName)
    {
      PropertyBase.Parameters propertyParameters = GetPropertyParameters (propertyName);
      return new ReferenceProperty (propertyParameters);
    }

    private ReferenceProperty CreatePropertyWithoutMixing (string propertyName)
    {
      PropertyBase.Parameters propertyParameters;
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        propertyParameters = GetPropertyParameters (propertyName);
      }
      return new ReferenceProperty (propertyParameters);
    }

    private PropertyBase.Parameters GetPropertyParameters (string propertyName)
    {
      return GetPropertyParameters (GetPropertyInfo (typeof (ClassWithBusinessObjectProperties), propertyName), _bindableObjectProvider);
    }
  }
}

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
    public void SearchServiceFromPropertyType ()
    {
      ISearchServiceOnType mockService = _mockRepository.StrictMock<ISearchServiceOnType> ();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyType");

      Expect.Call (mockService.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectWithIdentityProvider.AddService (mockService);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void SearchServiceFromPropertyDeclaration ()
    {
      ISearchServiceOnProperty mockService = _mockRepository.StrictMock<ISearchServiceOnProperty>();
      ISearchServiceOnType stubSearchServiceOnType = _mockRepository.StrictMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyDeclaration");

      Expect.Call (mockService.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectWithIdentityProvider.AddService (stubSearchServiceOnType);
      _bindableObjectProvider.AddService (mockService);
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
      ISearchAvailableObjectsService mockSearchAvailableObjectsService = _mockRepository.StrictMock<ISearchAvailableObjectsService>();
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService");

      Expect.Call (mockSearchAvailableObjectsService.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectProvider.AddService (mockSearchAvailableObjectsService);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    [Ignore ("Extend fallback behavior to include property type.")]
    public void WithoutSearchServiceAttribute_AndDefaultSearchService_FromPropertyType ()
    {
      ISearchAvailableObjectsService mockAvailableObjectsService = _mockRepository.StrictMock<ISearchAvailableObjectsService> ();
      IBusinessObjectClassService mockBusinessObjectClassService = _mockRepository.StrictMock<IBusinessObjectClassService> ();
      IBusinessObjectProvider mockBusinessObjectProvider = _mockRepository.StrictMock<IBusinessObjectProvider> ();
      IBusinessObjectClassWithIdentity mockBusinessObjectClassWithIdentity = _mockRepository.StrictMock<IBusinessObjectClassWithIdentity> ();
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService");

      Expect.Call (mockBusinessObjectClassWithIdentity.BusinessObjectProvider).Return (mockBusinessObjectProvider).Repeat.Any ();
      Expect.Call (mockBusinessObjectProvider.GetService (typeof (ISearchAvailableObjectsService))).Return (mockAvailableObjectsService);
      Expect.Call (mockBusinessObjectClassService.GetBusinessObjectClass (typeof (ClassFromOtherBusinessObjectImplementation)))
          .Return (mockBusinessObjectClassWithIdentity);
      Expect.Call (mockAvailableObjectsService.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll ();

      _bindableObjectProvider.AddService (mockBusinessObjectClassService);
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
    [Ignore ("Extend fallback behavior to include property type.")]
    public void WithoutSearchServiceAttribute_AndNoDefaultSearchService_FromPropertyType ()
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

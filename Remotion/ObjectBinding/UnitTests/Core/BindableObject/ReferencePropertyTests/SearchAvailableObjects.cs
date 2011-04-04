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
using Remotion.Reflection;
using Rhino.Mocks;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class SearchAvailableObjects : TestBase
  {
    private MockRepository _mockRepository;
    private BindableObjectProvider _bindableObjectProvider;
    private BindableObjectProvider _bindableObjectWithIdentityProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();
      _bindableObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
      _bindableObjectWithIdentityProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();

      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute> (_bindableObjectProvider);
      BusinessObjectProvider.SetProvider<BindableObjectWithIdentityProviderAttribute> (_bindableObjectWithIdentityProvider);
    }

    [Test]
    public void Search_WithSearchSupported ()
    {
      IBusinessObject stubBusinessObject = _mockRepository.Stub<IBusinessObject>();
      ISearchServiceOnProperty mockService = _mockRepository.StrictMock<ISearchServiceOnProperty> ();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyDeclaration");
      IBusinessObject[] expected = new IBusinessObject[0];
      ISearchAvailableObjectsArguments _searchArgumentsStub = _mockRepository.Stub<ISearchAvailableObjectsArguments>();

      using (_mockRepository.Ordered())
      {
        Expect.Call (mockService.SupportsProperty (property)).Return (true);
        Expect.Call (mockService.Search (stubBusinessObject, property, _searchArgumentsStub)).Return (expected);
      }
      _mockRepository.ReplayAll();

      _bindableObjectProvider.AddService (mockService);
      IBusinessObject[] actual = property.SearchAvailableObjects (stubBusinessObject, _searchArgumentsStub);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void Search_WithSearchSupportedAndReferencingObjectNull ()
    {
      ISearchServiceOnType mockService = _mockRepository.StrictMock<ISearchServiceOnType> ();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyType");
      IBusinessObject[] expected = new IBusinessObject[0];
      ISearchAvailableObjectsArguments _searchArgumentsStub = _mockRepository.Stub<ISearchAvailableObjectsArguments> ();

      using (_mockRepository.Ordered ())
      {
        Expect.Call (mockService.SupportsProperty (property)).Return (true);
        Expect.Call (mockService.Search (null, property, _searchArgumentsStub)).Return (expected);
      }
      _mockRepository.ReplayAll ();

      _bindableObjectWithIdentityProvider.AddService (mockService);
      IBusinessObject[] actual = property.SearchAvailableObjects (null, _searchArgumentsStub);

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException),
        ExpectedMessage =
        "Searching is not supported for reference property 'SearchServiceFromPropertyDeclaration' of business object class "
        + "'Remotion.ObjectBinding.UnitTests.Core.TestDomain.ClassWithBusinessObjectProperties, Remotion.ObjectBinding.UnitTests'.")]
    public void Search_WithSearchNotSupported ()
    {
      IBusinessObject businessObject = (IBusinessObject) ObjectFactory.Create<ClassWithBusinessObjectProperties> (ParamList.Empty);
      ISearchServiceOnProperty mockService = _mockRepository.StrictMock<ISearchServiceOnProperty> ();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyDeclaration");
      ISearchAvailableObjectsArguments _searchArgumentsStub = _mockRepository.Stub<ISearchAvailableObjectsArguments> ();

      Expect.Call (mockService.SupportsProperty (property)).Return (false);
      _mockRepository.ReplayAll();

      _bindableObjectProvider.AddService (mockService);
      try
      {
        property.SearchAvailableObjects (businessObject, _searchArgumentsStub);
      }
      finally
      {
        _mockRepository.VerifyAll();
      }
    }

    private ReferenceProperty CreateProperty (string propertyName)
    {
      PropertyBase.Parameters propertyParameters = 
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithBusinessObjectProperties), propertyName), _bindableObjectProvider);
      ReferenceProperty property = new ReferenceProperty (propertyParameters);
      property.SetReflectedClass (BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (ClassWithBusinessObjectProperties)));

      return property;
    }
  }
}

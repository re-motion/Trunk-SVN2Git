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
      base.SetUp ();

      _mockRepository = new MockRepository ();
      _bindableObjectProvider = new BindableObjectProvider ();
      _bindableObjectWithIdentityProvider = new BindableObjectProvider ();

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
      IBusinessObject businessObject = (IBusinessObject) ObjectFactory.Create<ClassWithBusinessObjectProperties> ().With ();
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
      ReferenceProperty property = new ReferenceProperty (propertyParameters,  TypeFactory.GetConcreteType (propertyParameters.UnderlyingType));
      property.SetReflectedClass (BindableObjectProvider.GetBindableObjectClass (typeof (ClassWithBusinessObjectProperties)));

      return property;
    }
  }
}

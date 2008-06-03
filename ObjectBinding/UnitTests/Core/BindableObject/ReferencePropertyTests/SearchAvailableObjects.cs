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
    private BindableObjectProvider _businessObjectProvider;
    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp ();

      _businessObjectProvider = new BindableObjectProvider ();
      _mockRepository = new MockRepository();
    }

    [Test]
    public void Search_WithSearchSupported ()
    {
      IBusinessObject stubBusinessObject = _mockRepository.Stub<IBusinessObject>();
      ISearchServiceOnProperty mockService = _mockRepository.CreateMock<ISearchServiceOnProperty> ();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyWithIdentity");
      IBusinessObject[] expected = new IBusinessObject[0];

      using (_mockRepository.Ordered())
      {
        Expect.Call (mockService.SupportsIdentity (property)).Return (true);
        Expect.Call (mockService.Search (stubBusinessObject, property, "*")).Return (expected);
      }
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (mockService);
      IBusinessObject[] actual = property.SearchAvailableObjects (stubBusinessObject, "*");

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void Search_WithSearchSupportedAndReferencingObjectNull ()
    {
      ISearchServiceOnProperty mockService = _mockRepository.CreateMock<ISearchServiceOnProperty> ();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyWithIdentity");
      IBusinessObject[] expected = new IBusinessObject[0];

      using (_mockRepository.Ordered ())
      {
        Expect.Call (mockService.SupportsIdentity (property)).Return (true);
        Expect.Call (mockService.Search (null, property, "*")).Return (expected);
      }
      _mockRepository.ReplayAll ();

      _businessObjectProvider.AddService (mockService);
      IBusinessObject[] actual = property.SearchAvailableObjects (null, "*");

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException),
        ExpectedMessage =
        "Searching is not supported for reference property 'SearchServiceFromPropertyWithIdentity' of business object class "
        + "'Remotion.ObjectBinding.UnitTests.Core.TestDomain.ClassWithBusinessObjectProperties, Remotion.ObjectBinding.UnitTests'.")]
    public void Search_WithSearchNotSupported ()
    {
      IBusinessObject businessObject = (IBusinessObject) ObjectFactory.Create<ClassWithBusinessObjectProperties> ().With ();
      ISearchServiceOnProperty mockService = _mockRepository.CreateMock<ISearchServiceOnProperty> ();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyWithIdentity");

      Expect.Call (mockService.SupportsIdentity (property)).Return (false);
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (mockService);
      try
      {
        property.SearchAvailableObjects (businessObject, "*");
      }
      finally
      {
        _mockRepository.VerifyAll();
      }
    }

    private ReferenceProperty CreateProperty (string propertyName)
    {
      PropertyBase.Parameters propertyParameters = 
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithBusinessObjectProperties), propertyName), _businessObjectProvider);
      ReferenceProperty property = new ReferenceProperty (propertyParameters,  TypeFactory.GetConcreteType (propertyParameters.UnderlyingType));
      property.SetReflectedClass (_businessObjectProvider.GetBindableObjectClass (typeof (ClassWithBusinessObjectProperties)));

      return property;
    }
  }
}

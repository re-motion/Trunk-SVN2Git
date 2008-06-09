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
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class GetReferenceClass : TestBase
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
    public void UseBindableObjectProvider ()
    {
      IBusinessObjectReferenceProperty property = new ReferenceProperty (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleBusinessObjectClass>), "Scalar"),
          typeof (SimpleBusinessObjectClass), null, false, false),
          TypeFactory.GetConcreteType (typeof (SimpleBusinessObjectClass)));

      Assert.That (property.ReferenceClass, Is.SameAs (_businessObjectProvider.GetBindableObjectClass (typeof (SimpleBusinessObjectClass))));
    }

    [Test]
    public void UseBusinessObjectClassService ()
    {
      IBusinessObjectClassService mockService = _mockRepository.CreateMock<IBusinessObjectClassService>();
      IBusinessObjectClass expectedClass = _mockRepository.Stub<IBusinessObjectClass>();
      IBusinessObject businessObjectFromOtherBusinessObjectProvider = _mockRepository.Stub<IBusinessObject>();
      Type typeFromOtherBusinessObjectProvider = businessObjectFromOtherBusinessObjectProvider.GetType();
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar", typeFromOtherBusinessObjectProvider);

      Expect.Call (mockService.GetBusinessObjectClass (typeFromOtherBusinessObjectProvider)).Return (expectedClass);
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (typeof (IBusinessObjectClassService), mockService);
      IBusinessObjectClass actualClass = property.ReferenceClass;

      _mockRepository.VerifyAll();
      Assert.That (actualClass, Is.SameAs (expectedClass));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
        "The 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.ClassFromOtherBusinessObjectImplementation' type does not use the "
        + "'Remotion.ObjectBinding.BindableObject' implementation of 'Remotion.ObjectBinding.IBusinessObject' and there is no "
        + "'Remotion.ObjectBinding.IBusinessObjectClassService' registered with the 'Remotion.ObjectBinding.BusinessObjectProvider' associated with this type.")]
    public void UseBusinessObjectClassService_WithoutService ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar", typeof (ClassFromOtherBusinessObjectImplementation));

      Dev.Null = property.ReferenceClass;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
        "The GetBusinessObjectClass method of 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.StubBusinessObjectClassService', registered "
        + "with the 'Remotion.ObjectBinding.BindableObject.BindableObjectProvider', failed to return an 'Remotion.ObjectBinding.IBusinessObjectClass' "
        + "for type 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.ClassFromOtherBusinessObjectImplementation'.")]
    public void UseBusinessObjectClassService_WithServiceReturningNull ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar", typeof (ClassFromOtherBusinessObjectImplementation));

      _businessObjectProvider.AddService (typeof (IBusinessObjectClassService), new StubBusinessObjectClassService());
      Dev.Null = property.ReferenceClass;
    }

    private ReferenceProperty CreateProperty (string propertyName, Type propertyType)
    {
      return new ReferenceProperty (
        GetPropertyParameters (GetPropertyInfo (typeof (ClassWithReferenceType<>).MakeGenericType (propertyType), propertyName), _businessObjectProvider),
        propertyType);
    }
  }
}

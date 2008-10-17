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
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectWithIdentityBaseTest
  {
    private ClassDerivedFromBindableObjectWithIdentityBase _instance;
    private IBindableObjectBaseImplementation _implementationMock;
    private IBusinessObjectProperty _propertyFake;
    private IBusinessObjectClass _businessObjectClassFake;

    [SetUp]
    public void SetUp()
    {
      _implementationMock = MockRepository.GenerateMock<IBindableObjectBaseImplementation> ();
      _instance = new ClassDerivedFromBindableObjectWithIdentityBase (_implementationMock);

      _propertyFake = MockRepository.GenerateMock<IBusinessObjectProperty> ();
      _businessObjectClassFake = MockRepository.GenerateMock<IBusinessObjectClass> ();
    }

    [Test]
    public void BindableObjectProviderAttribute()
    {
      Assert.That (typeof (BindableObjectWithIdentityBase).IsDefined (typeof (BindableObjectWithIdentityProviderAttribute), false), Is.True);
    }

    [Test]
    public void BindableObjectBaseClassAttribute ()
    {
      Assert.That (typeof (BindableObjectWithIdentityBase).IsDefined (typeof (BindableObjectBaseClassAttribute), false), Is.True);
    }

    [Test]
    public void CreateImplementation ()
    {
      var instance = new ClassDerivedFromBindableObjectWithIdentityBase ();
      Assert.That (PrivateInvoke.GetNonPublicField (instance, "_implementation"), Is.InstanceOfType (typeof (BindableObjectWithIdentityBaseImplementation)));
    }

    [Test]
    public void Implementation_IsInitialized ()
    {
      var instance = new ClassDerivedFromBindableObjectWithIdentityBase();
      var mixin = (BindableObjectWithIdentityBaseImplementation) PrivateInvoke.GetNonPublicField (instance, "_implementation");
      Assert.That (mixin.BusinessObjectClass, Is.Not.Null);
    }

    [Test]
    public void Serialization ()
    {
      var instance = new ClassDerivedFromBindableObjectWithIdentityBase ();
      instance = Serializer.SerializeAndDeserialize (instance);
      var mixin = (BindableObjectWithIdentityBaseImplementation) PrivateInvoke.GetNonPublicField (instance, "_implementation");
      Assert.That (mixin.BusinessObjectClass, Is.Not.Null);
    }

    [Test]
    public void GetProperty()
    {
      _implementationMock.Expect (mock => mock.GetProperty (_propertyFake)).Return (12);
      _implementationMock.Replay ();

      Assert.That (_instance.GetProperty (_propertyFake), Is.EqualTo (12));
      _implementationMock.VerifyAllExpectations ();
    }

    [Test]
    public void SetProperty ()
    {
      _implementationMock.Expect (mock => mock.SetProperty (_propertyFake, 174));
      _implementationMock.Replay ();

      _instance.SetProperty (_propertyFake, 174);
      _implementationMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetPropertyString()
    {
      _implementationMock.Expect (mock => mock.GetPropertyString (_propertyFake, "gj")).Return ("yay");
      _implementationMock.Replay ();

      Assert.That (_instance.GetPropertyString (_propertyFake, "gj"), Is.EqualTo ("yay"));
      _implementationMock.VerifyAllExpectations (); 
    }

    [Test]
    public void DisplayName()
    {
      _implementationMock.Expect (mock => mock.BaseDisplayName).Return ("Philips");
      _implementationMock.Replay ();

      Assert.That (_instance.DisplayName, Is.EqualTo ("Philips"));
      _implementationMock.VerifyAllExpectations (); 
    }

    [Test]
    public void DisplayNameSafe ()
    {
      _implementationMock.Expect (mock => mock.DisplayNameSafe).Return ("Megatron");
      _implementationMock.Replay ();

      Assert.That (_instance.DisplayNameSafe, Is.EqualTo ("Megatron"));
      _implementationMock.VerifyAllExpectations ();
    }

    [Test]
    public void BusinessObjectClass ()
    {
      _implementationMock.Expect (mock => mock.BusinessObjectClass).Return (_businessObjectClassFake);
      _implementationMock.Replay ();

      Assert.That (_instance.BusinessObjectClass, Is.SameAs (_businessObjectClassFake));
      _implementationMock.VerifyAllExpectations ();
    }

    [Test]
    public void UniqueIdentifier ()
    {
      _instance.SetUniqueIdentifier("uniqueID");
      Assert.That (_instance.UniqueIdentifier, Is.EqualTo ("uniqueID"));
    }
  }
}
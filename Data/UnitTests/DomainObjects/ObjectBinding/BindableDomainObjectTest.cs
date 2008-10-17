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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding
{
  [TestFixture]
  public class BindableDomainObjectTest : ClientTransactionBaseTest
  {
    private SampleBindableDomainObject _instance;
    private IBindableDomainObjectImplementation _implementationMock;
    private IBusinessObjectProperty _propertyFake;
    private IBusinessObjectClass _businessObjectClassFake;

    [SetUp]
    public override void SetUp()
    {
      base.SetUp ();

      _implementationMock = MockRepository.GenerateMock<IBindableDomainObjectImplementation> ();
      _instance = SampleBindableDomainObject.NewObject (_implementationMock);

      _propertyFake = MockRepository.GenerateMock<IBusinessObjectProperty> ();
      _businessObjectClassFake = MockRepository.GenerateMock<IBusinessObjectClass> ();
    }

    [Test]
    public void BindableObjectProviderAttribute()
    {
      Assert.That (typeof (BindableDomainObject).IsDefined (typeof (BindableDomainObjectProviderAttribute), false), Is.True);
    }

    [Test]
    public void BindableObjectBaseClassAttribute ()
    {
      Assert.That (typeof (BindableDomainObject).IsDefined (typeof (BindableObjectBaseClassAttribute), false), Is.True);
    }

    [Test]
    public void CreateImplementation ()
    {
      var instance = SampleBindableDomainObject.NewObject ();
      Assert.That (PrivateInvoke.GetNonPublicField (instance, "_implementation"), Is.InstanceOfType (typeof (BindableDomainObjectImplementation)));
    }

    [Test]
    public void Implementation_IsInitialized ()
    {
      var instance = SampleBindableDomainObject.NewObject ();
      var implementation = (BindableDomainObjectImplementation) PrivateInvoke.GetNonPublicField (instance, "_implementation");
      Assert.That (implementation.BusinessObjectClass, Is.Not.Null);
    }

    [Test]
    public void Serialization ()
    {
      var instance = SampleBindableDomainObject.NewObject ();
      instance = Serializer.SerializeAndDeserialize (instance);
      var implementation = (BindableDomainObjectImplementation) PrivateInvoke.GetNonPublicField (instance, "_implementation");
      Assert.That (implementation.BusinessObjectClass, Is.Not.Null);
    }

    [Test]
    public void Loading ()
    {
      var newInstance = SampleBindableDomainObject.NewObject ();
      SetDatabaseModifyable ();
      ClientTransaction.Current.Commit ();
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var instance = SampleBindableDomainObject.GetObject (newInstance.ID);
        var implementation = (BindableDomainObjectImplementation) PrivateInvoke.GetNonPublicField (instance, "_implementation");
        Assert.That (implementation, Is.Not.Null);
        Assert.That (implementation.BusinessObjectClass, Is.Not.Null);
      }
    }

    [Test]
    public void Reloading ()
    {
      var instance1 = SampleBindableDomainObject.NewObject ();
      var implementation1 = (BindableDomainObjectImplementation) PrivateInvoke.GetNonPublicField (instance1, "_implementation");
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var instance2 = SampleBindableDomainObject.GetObject (instance1.ID);
        var implementation2 = (BindableDomainObjectImplementation) PrivateInvoke.GetNonPublicField (instance2, "_implementation");
        Assert.That (implementation2, Is.SameAs (implementation1));
      }
    }

    [Test]
    public void GetProperty()
    {
      _implementationMock.Expect (mock => mock.GetProperty (_propertyFake)).Return (12);
      _implementationMock.Replay ();

      Assert.That (((IBusinessObject)_instance).GetProperty (_propertyFake), Is.EqualTo (12));
      _implementationMock.VerifyAllExpectations ();
    }

    [Test]
    public void SetProperty ()
    {
      _implementationMock.Expect (mock => mock.SetProperty (_propertyFake, 174));
      _implementationMock.Replay ();

      ((IBusinessObject) _instance).SetProperty (_propertyFake, 174);
      _implementationMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetPropertyString()
    {
      _implementationMock.Expect (mock => mock.GetPropertyString (_propertyFake, "gj")).Return ("yay");
      _implementationMock.Replay ();

      Assert.That (((IBusinessObject) _instance).GetPropertyString (_propertyFake, "gj"), Is.EqualTo ("yay"));
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

      Assert.That (((IBusinessObject) _instance).DisplayNameSafe, Is.EqualTo ("Megatron"));
      _implementationMock.VerifyAllExpectations ();
    }

    [Test]
    public void BusinessObjectClass ()
    {
      _implementationMock.Expect (mock => mock.BusinessObjectClass).Return (_businessObjectClassFake);
      _implementationMock.Replay ();

      Assert.That (((IBusinessObject) _instance).BusinessObjectClass, Is.SameAs (_businessObjectClassFake));
      _implementationMock.VerifyAllExpectations ();
    }

    [Test]
    public void UniqueIdentifier ()
    {
      _implementationMock.Expect (mock => mock.BaseUniqueIdentifier).Return ("123");
      _implementationMock.Replay ();

      Assert.That (((IBusinessObjectWithIdentity) _instance).UniqueIdentifier, Is.EqualTo ("123"));
      _implementationMock.VerifyAllExpectations ();
    }

    [Test]
    public void Properties()
    {
      Assert.That (((BindableDomainObjectMixin.IDomainObject) _instance).Properties, Is.EqualTo (_instance.PublicProperties));
      _implementationMock.VerifyAllExpectations ();
    }
  }
}
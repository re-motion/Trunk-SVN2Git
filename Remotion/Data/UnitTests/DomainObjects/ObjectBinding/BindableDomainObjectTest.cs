// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
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
      Assert.That (PrivateInvoke.GetNonPublicField (instance, "_implementation"), Is.InstanceOf (typeof (BindableDomainObjectImplementation)));
    }

    [Test]
    public void Implementation_IsInitialized ()
    {
      var instance = SampleBindableDomainObject.NewObject ();
      var implementation = (BindableDomainObjectImplementation) PrivateInvoke.GetNonPublicField (instance, "_implementation");
      Assert.That (implementation.BusinessObjectClass, Is.Not.Null);
    }

    [Test]
    public void Implementation_IsInitialized_BeforeDerivedCtorRuns ()
    {
      var instance = SampleBindableDomainObject_AccessingImplementationFromCtor.NewObject ();
      Assert.That (instance.DisplayNameFromCtor, Is.Not.Null);
      Assert.That (instance.DisplayNameFromCtor, Is.EqualTo (instance.DisplayName));
    }

    [Test]
    [UseLegacyCodeGeneration]
    public void Serialization ()
    {
      var instance = SampleBindableDomainObject.NewObject ();
      instance = Serializer.SerializeAndDeserialize (instance);
      var implementation = (BindableDomainObjectImplementation) PrivateInvoke.GetNonPublicField (instance, "_implementation");
      Assert.That (implementation, Is.Not.Null);
      Assert.That (implementation.BusinessObjectClass, Is.Not.Null);
      Assert.That (implementation.BusinessObjectClass.TargetType, Is.SameAs (typeof (SampleBindableDomainObject)));
    }

    [Test]
    [UseLegacyCodeGeneration]
    public void Serialization_ViaISerializable ()
    {
      var instance = SampleBindableDomainObject_ImplementingISerializable.NewObject ();
      instance = Serializer.SerializeAndDeserialize (instance);
      
      var implementation = (BindableDomainObjectImplementation) PrivateInvoke.GetNonPublicField (instance, "_implementation");
      Assert.That (implementation, Is.Not.Null);
      Assert.That (implementation.BusinessObjectClass, Is.Not.Null);
      Assert.That (implementation.BusinessObjectClass.TargetType, Is.SameAs (typeof (SampleBindableDomainObject_ImplementingISerializable)));
    }

    [Test]
    public void Loading ()
    {
      var newInstance = SampleBindableDomainObject.NewObject ();
      SetDatabaseModifyable ();
      ClientTransaction.Current.Commit ();
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var instance = newInstance.ID.GetObject<SampleBindableDomainObject> ();
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
        var instance2 = instance1.ID.GetObject<SampleBindableDomainObject> ();
        var implementation2 = (BindableDomainObjectImplementation) PrivateInvoke.GetNonPublicField (instance2, "_implementation");
        Assert.That (implementation2, Is.SameAs (implementation1));
      }
    }

    [Test]
    public void ObjectReference ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (SampleBindableDomainObject));
      var instance = classDefinition.InstanceCreator.CreateObjectReference (new ObjectID(classDefinition, Guid.NewGuid()), TestableClientTransaction);
      
      var implementation = (BindableDomainObjectImplementation) PrivateInvoke.GetNonPublicField (instance, "_implementation");
      Assert.That (implementation, Is.Not.Null);
      Assert.That (implementation.BusinessObjectClass, Is.Not.Null);
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

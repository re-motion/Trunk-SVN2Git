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
using System.Runtime.Serialization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding
{
  [TestFixture]
  public class BindableDomainObjectTest : ObjectBindingBaseTest
  {
    [Test]
    public void BindableDomainObjectIsDomainObject ()
    {
      Assert.IsTrue (typeof (DomainObject).IsAssignableFrom (typeof (SampleBindableDomainObject)));
    }

    [Test]
    public void BindableDomainObjectAddsMixin ()
    {
      Assert.IsTrue (MixinTypeUtility.HasMixin (typeof (SampleBindableDomainObject), typeof (BindableDomainObjectMixin)));
    }

    [Test]
    public void DefaultDisplayName ()
    {
      IBusinessObject businessObject = (IBusinessObject) RepositoryAccessor.NewObject (typeof (SampleBindableDomainObject)).With();
      Assert.AreEqual (Utilities.TypeUtility.GetPartialAssemblyQualifiedName (typeof (SampleBindableDomainObject)), businessObject.DisplayName);
    }

    [Test]
    public void OverriddenDisplayName ()
    {
      IBusinessObject businessObject = (IBusinessObject) RepositoryAccessor.NewObject (typeof (SampleBindableDomainObjectWithOverriddenDisplayName)).With();
      Assert.AreNotEqual (
          Utilities.TypeUtility.GetPartialAssemblyQualifiedName (typeof (SampleBindableDomainObjectWithOverriddenDisplayName)),
          businessObject.DisplayName);
      Assert.AreEqual ("TheDisplayName", businessObject.DisplayName);
    }

    [Test]
    public void VerifyInterfaceImplementation ()
    {
      IBusinessObjectWithIdentity businessObject =
          (SampleBindableDomainObjectWithOverriddenDisplayName) RepositoryAccessor.NewObject (typeof (SampleBindableDomainObjectWithOverriddenDisplayName)).With();
      IBusinessObjectWithIdentity businessObjectMixin = Mixin.Get<BindableDomainObjectMixin> (businessObject);

      Assert.AreSame (businessObjectMixin.BusinessObjectClass, businessObject.BusinessObjectClass);
      Assert.AreEqual (businessObjectMixin.DisplayName, businessObject.DisplayName);
      Assert.AreEqual (businessObjectMixin.DisplayNameSafe, businessObject.DisplayNameSafe);
      businessObject.SetProperty ("Int32", 1);
      Assert.AreEqual (1, businessObject.GetProperty ("Int32"));
      Assert.AreEqual (1, businessObject.GetProperty (businessObjectMixin.BusinessObjectClass.GetPropertyDefinition ("Int32")));
      Assert.AreEqual ("001", businessObject.GetPropertyString (businessObjectMixin.BusinessObjectClass.GetPropertyDefinition ("Int32"), "000"));
      Assert.AreEqual ("1", businessObject.GetPropertyString ("Int32"));
      Assert.AreEqual (businessObjectMixin.UniqueIdentifier, businessObject.UniqueIdentifier);
      businessObject.SetProperty (businessObjectMixin.BusinessObjectClass.GetPropertyDefinition ("Int32"), 2);
      Assert.AreEqual (2, businessObject.GetProperty ("Int32"));
    }

    [Test]
    public void SerializeAndDeserialize ()
    {
      SampleBindableDomainObjectWithOverriddenDisplayName domainObject =
          (SampleBindableDomainObjectWithOverriddenDisplayName) RepositoryAccessor.NewObject (typeof (SampleBindableDomainObjectWithOverriddenDisplayName)).With();

      Serializer.SerializeAndDeserialize (domainObject);
    }

    [Test]
    public void GetProviderForBindableObjectType ()
    {
      BindableObjectProvider provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (BindableDomainObject));

      Assert.That (provider, Is.Not.Null);
      Assert.That (provider, Is.InstanceOfType (typeof (BindableDomainObjectProvider)));
      Assert.That (provider, Is.SameAs (BusinessObjectProvider.GetProvider (typeof (BindableDomainObjectProviderAttribute))));
      Assert.That (provider, Is.Not.SameAs (BusinessObjectProvider.GetProvider (typeof (BindableObjectProviderAttribute))));
    }

    [Test]
    public void DeserializationConstructor_CallsBase ()
    {
      var serializable = SampleBindableDomainObject_ImplementingISerializable.NewObject ().With ();

      var info = new SerializationInfo (typeof (SampleBindableDomainObject_ImplementingISerializable), new FormatterConverter ());
      var context = new StreamingContext ();

      serializable.GetObjectData (info, context);
      Assert.That (info.MemberCount, Is.GreaterThan (0));

      var deserialized =
          (SampleBindableDomainObject_ImplementingISerializable) Activator.CreateInstance (((object) serializable).GetType (), info, context);
      Assert.That (deserialized.ID, Is.EqualTo (serializable.ID));
    }
  }
}

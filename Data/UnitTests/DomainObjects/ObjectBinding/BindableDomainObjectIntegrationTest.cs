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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding
{
  [TestFixture]
  public class BindableDomainObjectIntegrationTest : ObjectBindingBaseTest
  {
    private IBusinessObjectWithIdentity _instance;
    private IBusinessObjectWithIdentity _instanceOverridingDisplayName;

    public override void SetUp ()
    {
      base.SetUp ();
      _instance = SampleBindableDomainObject.NewObject ();
      _instanceOverridingDisplayName = SampleBindableDomainObjectWithOverriddenDisplayName.NewObject ();
    }

    [Test]
    public void BindableDomainObjectIsDomainObject ()
    {
      Assert.That (typeof (DomainObject).IsAssignableFrom (typeof (SampleBindableDomainObject)), Is.True);
    }

    [Test]
    public void DisplayName_Default ()
    {
      Assert.That (_instance.DisplayName, Is.EqualTo (TypeUtility.GetPartialAssemblyQualifiedName (typeof (SampleBindableDomainObject))));
    }

    [Test]
    public void DisplayName_Overridden ()
    {
      Assert.That (_instanceOverridingDisplayName.DisplayName, Is.EqualTo ("TheDisplayName"));
    }

    [Test]
    public void DisplayNameSafe_Default ()
    {
      Assert.That (_instance.DisplayNameSafe, Is.EqualTo (TypeUtility.GetPartialAssemblyQualifiedName (typeof (SampleBindableDomainObject))));
    }

    [Test]
    public void DisplayNameSafe_Overridden ()
    {
      Assert.That (_instanceOverridingDisplayName.DisplayNameSafe, Is.EqualTo ("TheDisplayName"));
    }

    [Test]
    public void UniqueIdentifier ()
    { 
      Assert.That (_instance.UniqueIdentifier, Is.EqualTo (((SampleBindableDomainObject) _instance).ID.ToString()));
    }

    /// <summary>
    /// Verifies the interface implementation.
    /// </summary>
    [Test]
    public void VerifyInterfaceImplementation ()
    {
      IBusinessObjectWithIdentity businessObject =
          (SampleBindableDomainObjectWithOverriddenDisplayName) RepositoryAccessor.NewObject (typeof (SampleBindableDomainObjectWithOverriddenDisplayName)).With();
      var implementation = (BindableDomainObjectImplementation) PrivateInvoke.GetNonPublicField (businessObject, typeof (BindableDomainObject), "_implementation");

      Assert.That (businessObject.BusinessObjectClass, Is.SameAs (implementation.BusinessObjectClass));
      Assert.That (businessObject.DisplayName, Is.EqualTo (implementation.DisplayName));
      Assert.That (businessObject.DisplayName, Is.EqualTo ("TheDisplayName"));
      Assert.That (businessObject.DisplayNameSafe, Is.EqualTo (implementation.DisplayNameSafe));
      businessObject.SetProperty ("Int32", 1);
      Assert.That (businessObject.GetProperty ("Int32"), Is.EqualTo (1));
      Assert.That (businessObject.GetProperty (implementation.BusinessObjectClass.GetPropertyDefinition ("Int32")), Is.EqualTo (1));
      Assert.That (businessObject.GetPropertyString (implementation.BusinessObjectClass.GetPropertyDefinition ("Int32"), "000"), Is.EqualTo ("001"));
      Assert.That (businessObject.GetPropertyString ("Int32"), Is.EqualTo ("1"));
      Assert.That (businessObject.UniqueIdentifier, Is.EqualTo (implementation.UniqueIdentifier));
      businessObject.SetProperty (implementation.BusinessObjectClass.GetPropertyDefinition ("Int32"), 2);
      Assert.That (businessObject.GetProperty ("Int32"), Is.EqualTo (2));
    }

    [Test]
    public void SerializeAndDeserialize ()
    {
      Serializer.SerializeAndDeserialize (_instanceOverridingDisplayName);
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

    [Test]
    public void GetProvider ()
    {
      Assert.That (
          BindableDomainObjectProvider.GetProviderForBindableObjectType (typeof (SampleBindableDomainObject)),
          Is.SameAs (BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute> ()));
      Assert.That (
          BindableDomainObjectProvider.GetProviderForBindableObjectType (typeof (SampleBindableDomainObject)),
          Is.Not.SameAs (BusinessObjectProvider.GetProvider<BindableObjectProviderAttribute> ()));
      Assert.That (
          BindableDomainObjectProvider.GetProviderForBindableObjectType (typeof (SampleBindableDomainObject)),
          Is.Not.SameAs (BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute> ()));
    }

    [Test]
    public void NoPropertyFromDomainObject ()
    {
      var properties = (PropertyBase[]) _instance.BusinessObjectClass.GetPropertyDefinitions ();

      foreach (PropertyBase property in properties)
        Assert.That (property.PropertyInfo.DeclaringType, Is.Not.EqualTo (typeof (DomainObject)));
    }

    [Test]
    public void NoPropertyFromBindableDomainObject ()
    {
      var properties = (PropertyBase[]) (_instance).BusinessObjectClass.GetPropertyDefinitions ();

      foreach (PropertyBase property in properties)
        Assert.That (property.PropertyInfo.DeclaringType, Is.Not.EqualTo (typeof (BindableDomainObject)));
    }
  }
}

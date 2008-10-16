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
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class ObjectBindingToMappingTest : ObjectBindingBaseTest
  {
    private BindableObjectClass _businessObjectClassWithProperties;
    private BindableDomainObjectWithProperties _classWithPropertiesInstance;
    private BindableDomainObjectMixin _classWithPropertiesMixin;

    private BindableObjectClass _businessObjectClassWithMixedProperties;
    private BindableDomainObjectWithMixedPersistentProperties _classWithMixedPropertiesInstance;
    private BindableDomainObjectMixin _classWithMixedPropertiesImplementation;

    public override void SetUp ()
    {
      base.SetUp ();
      _businessObjectClassWithProperties = BindableObjectProvider.GetBindableObjectClass (typeof (BindableDomainObjectWithProperties));
      _classWithPropertiesInstance = BindableDomainObjectWithProperties.NewObject();
      _classWithPropertiesMixin = Mixin.Get<BindableDomainObjectMixin>  (_classWithPropertiesInstance);

      _businessObjectClassWithMixedProperties = BindableObjectProvider.GetBindableObjectClass (typeof (BindableDomainObjectWithMixedPersistentProperties));
      _classWithMixedPropertiesInstance = BindableDomainObjectWithMixedPersistentProperties.NewObject ();
      _classWithMixedPropertiesImplementation = (BindableDomainObjectImplementation) PrivateInvoke.GetNonPublicField (_classWithMixedPropertiesInstance, typeof (BindableDomainObject), "_implementation");
    }

    [Test]
    public void GetMappingPropertyIdentifier_Standard ()
    {
      PropertyBase property = (PropertyBase) _businessObjectClassWithProperties.GetPropertyDefinition ("RequiredStringProperty");
      string identifier = _classWithPropertiesMixin.GetMappingPropertyIdentifier (property);
      Assert.That (
          identifier,
          Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain.BindableDomainObjectWithProperties.RequiredStringProperty"));
      Assert.That (_classWithPropertiesInstance.ID.ClassDefinition.GetPropertyDefinition (identifier), Is.Not.Null);
    }

    [Test]
    public void GetMappingPropertyIdentifier_Interface ()
    {
      PropertyBase property = (PropertyBase) _businessObjectClassWithProperties.GetPropertyDefinition ("RequiredStringPropertyInInterface");
      string identifier = _classWithPropertiesMixin.GetMappingPropertyIdentifier (property);
      Assert.That (
          identifier,
          Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain.BindableDomainObjectWithProperties.RequiredStringPropertyInInterface"));
      Assert.That (_classWithPropertiesInstance.ID.ClassDefinition.GetPropertyDefinition (identifier), Is.Not.Null);
    }

    [Test]
    public void GetMappingPropertyIdentifier_ExplicitInterface ()
    {
      PropertyBase property = (PropertyBase) _businessObjectClassWithProperties.GetPropertyDefinition ("RequiredStringPropertyExplicitInInterface");
      string identifier = _classWithPropertiesMixin.GetMappingPropertyIdentifier (property);
      Assert.That (
          identifier,
          Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain.BindableDomainObjectWithProperties."
          + "Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain.IBindableDomainObjectWithProperties."
          + "RequiredStringPropertyExplicitInInterface"));
      Assert.That (_classWithPropertiesInstance.ID.ClassDefinition.GetPropertyDefinition (identifier), Is.Not.Null);
    }

    [Test]
    public void GetMappingPropertyIdentifier_MixedPrivate ()
    {
      PropertyBase property = (PropertyBase) _businessObjectClassWithMixedProperties.GetPropertyDefinition ("PrivateMixedProperty");
      string identifier = _classWithMixedPropertiesImplementation.GetMappingPropertyIdentifier (property);
      Assert.That (
          identifier,
          Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain.MixinAddingPersistentProperties.PrivateMixedProperty"));
      Assert.That (_classWithMixedPropertiesInstance.ID.ClassDefinition.GetPropertyDefinition (identifier), Is.Not.Null);
    }

    [Test]
    public void GetMappingPropertyIdentifier_MixedPublic ()
    {
      PropertyBase property = (PropertyBase) _businessObjectClassWithMixedProperties.GetPropertyDefinition ("PublicMixedProperty");
      string identifier = _classWithMixedPropertiesImplementation.GetMappingPropertyIdentifier (property);
      Assert.That (
          identifier,
          Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain.MixinAddingPersistentProperties.PublicMixedProperty"));
      Assert.That (_classWithMixedPropertiesInstance.ID.ClassDefinition.GetPropertyDefinition (identifier), Is.Not.Null);
    }

    [Test]
    public void GetMappingPropertyIdentifier_Mixed_ExplicitOnMixin ()
    {
      PropertyBase property = (PropertyBase) _businessObjectClassWithMixedProperties.GetPropertyDefinition ("ExplicitMixedProperty");
      string identifier = _classWithMixedPropertiesImplementation.GetMappingPropertyIdentifier (property);
      Assert.That (
          identifier,
          Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain.MixinAddingPersistentProperties."
          + "Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain.IMixinAddingPersistentProperties."
          + "ExplicitMixedProperty"));
      Assert.That (_classWithMixedPropertiesInstance.ID.ClassDefinition.GetPropertyDefinition (identifier), Is.Not.Null);
    }
  }
}

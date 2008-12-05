// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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

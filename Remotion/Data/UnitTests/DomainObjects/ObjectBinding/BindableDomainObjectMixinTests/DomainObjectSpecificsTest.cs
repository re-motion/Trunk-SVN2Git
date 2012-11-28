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
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class DomainObjectSpecificsTest : ObjectBindingBaseTest
  {
    private BindableObjectClass _businessObjectClassWithProperties;
    private BindableObjectClass _businessObjectSampleClass;

    public override void SetUp ()
    {
      base.SetUp ();
      
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (BindableDomainObjectWithProperties));
      _businessObjectClassWithProperties = provider.GetBindableObjectClass (typeof (BindableDomainObjectWithProperties));
      _businessObjectSampleClass = provider.GetBindableObjectClass (typeof (SampleBindableMixinDomainObject));
    }

    [Test]
    public void OrdinaryProperty ()
    {
      Assert.IsNotNull (_businessObjectSampleClass.GetPropertyDefinition ("Name"));
    }

    [Test]
    public void UsesBindableDomainObjectMetadataFactory ()
    {
      Assert.That (
        BindableObjectProvider.GetProviderForBindableObjectType(typeof (SampleBindableMixinDomainObject)).MetadataFactory,
        Is.InstanceOf (typeof (BindableDomainObjectMetadataFactory)));
    }

    [Test]
    public void NoIDProperty ()
    {
      Assert.IsNull (_businessObjectSampleClass.GetPropertyDefinition ("ID"));
    }

    [Test]
    public void NoPropertyFromDomainObject ()
    {
      PropertyBase[] properties = (PropertyBase[]) _businessObjectSampleClass.GetPropertyDefinitions ();

      foreach (PropertyBase property in properties)
        Assert.AreNotEqual (typeof (DomainObject), property.PropertyInfo.DeclaringType);
    }

    [Test]
    public void PropertyNotInMapping ()
    {
      Assert.IsNotNull (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredPropertyNotInMapping"));
    }

    [Test]
    public void PropertyInMapping ()
    {
      Assert.IsNotNull (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredStringProperty"));
    }

    [Test]
    public void ProtectedPropertyInMapping ()
    {
      Assert.IsNull (_businessObjectClassWithProperties.GetPropertyDefinition ("ProtectedStringProperty"));
    }

    [Test]
    public void Requiredness ()
    {
      Assert.IsTrue (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredPropertyNotInMapping").IsRequired);
      Assert.IsTrue (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredStringProperty").IsRequired);
      Assert.IsTrue (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredValueProperty").IsRequired);
      Assert.IsTrue (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredEnumProperty").IsRequired);
      Assert.IsTrue (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredRelatedObjectProperty").IsRequired);
      Assert.IsTrue (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredBidirectionalRelatedObjectProperty").IsRequired);
      Assert.IsTrue (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredBidirectionalRelatedObjectsProperty").IsRequired);

      Assert.IsFalse (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredPropertyNotInMapping").IsRequired);
      Assert.IsFalse (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredStringProperty").IsRequired);
      Assert.IsFalse (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredValueProperty").IsRequired);
      Assert.IsFalse (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredEnumProperty").IsRequired);
      Assert.IsFalse (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredUndefinedEnumProperty").IsRequired);
      Assert.IsFalse (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredRelatedObjectProperty").IsRequired);
      Assert.IsFalse (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredBidirectionalRelatedObjectProperty").IsRequired);
      Assert.IsFalse (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredBidirectionalRelatedObjectsProperty").IsRequired);
    }

    [Test]
    public void MaxLength ()
    {
      Assert.AreEqual (7, ((IBusinessObjectStringProperty)
          _businessObjectClassWithProperties.GetPropertyDefinition ("MaxLength7StringProperty")).MaxLength);

      Assert.IsNull (((IBusinessObjectStringProperty)
          _businessObjectClassWithProperties.GetPropertyDefinition ("NoMaxLengthStringPropertyNotInMapping")).MaxLength);
      Assert.IsNull (((IBusinessObjectStringProperty)
          _businessObjectClassWithProperties.GetPropertyDefinition ("NoMaxLengthStringProperty")).MaxLength);
    }

    [Test]
    public void InheritanceAndOverriding ()
    {
      Assert.IsNotNull (_businessObjectClassWithProperties.GetPropertyDefinition ("BasePropertyWithMaxLength3"));
      Assert.IsNotNull (_businessObjectClassWithProperties.GetPropertyDefinition ("BasePropertyWithMaxLength4"));

      Assert.AreEqual (33, ((IBusinessObjectStringProperty)
          _businessObjectClassWithProperties.GetPropertyDefinition ("BasePropertyWithMaxLength3")).MaxLength);
      Assert.AreEqual (4, ((IBusinessObjectStringProperty)
          _businessObjectClassWithProperties.GetPropertyDefinition ("BasePropertyWithMaxLength4")).MaxLength);
    }

    [Test]
    public void NullabilityResolvedFromAboveInheritanceRoot ()
    {
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (BindableDomainObjectAboveInheritanceRoot));
      var businessObjectClass = provider.GetBindableObjectClass (typeof (BindableDomainObjectAboveInheritanceRoot));

      var notNullableBooleanProperty = businessObjectClass.GetPropertyDefinition ("NotNullableBooleanProperty");
      Assert.That (notNullableBooleanProperty.IsRequired, Is.True);

      var notNullableStringProperty = businessObjectClass.GetPropertyDefinition ("NotNullableStringPropertyWithLengthConstraint");
      Assert.That (notNullableStringProperty.IsRequired, Is.True);

      var notNullableRelationProperty = businessObjectClass.GetPropertyDefinition ("MandatoryUnidirectionalRelation");
      Assert.That (notNullableRelationProperty.IsRequired, Is.True);

      var nullableBooleanProperty = businessObjectClass.GetPropertyDefinition ("NullableBooleanProperty");
      Assert.That (nullableBooleanProperty.IsRequired, Is.False);

      var nullableStringProperty = businessObjectClass.GetPropertyDefinition ("NullableStringPropertyWithoutLengthConstraint");
      Assert.That (nullableStringProperty.IsRequired, Is.False);

      var nullableRelationProperty = businessObjectClass.GetPropertyDefinition ("NotMandatoryUnidirectionalRelation");
      Assert.That (nullableRelationProperty.IsRequired, Is.False);
    }

    [Test]
    public void LengthConstraintResolvedFromAboveInheritanceRoot ()
    {
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (BindableDomainObjectAboveInheritanceRoot));
      var businessObjectClass = provider.GetBindableObjectClass (typeof (BindableDomainObjectAboveInheritanceRoot));

      var stringPropertyWithLengthConstraint =
          (IBusinessObjectStringProperty) businessObjectClass.GetPropertyDefinition ("NotNullableStringPropertyWithLengthConstraint");
      Assert.That (stringPropertyWithLengthConstraint.MaxLength, Is.EqualTo (100));

      var stringPropertyWithoutLengthConstraint =
          (IBusinessObjectStringProperty) businessObjectClass.GetPropertyDefinition ("NullableStringPropertyWithoutLengthConstraint");
      Assert.That (stringPropertyWithoutLengthConstraint.MaxLength, Is.Null);
    }
  }
}

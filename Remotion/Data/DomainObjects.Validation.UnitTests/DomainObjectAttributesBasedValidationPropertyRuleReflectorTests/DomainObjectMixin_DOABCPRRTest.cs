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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation.Validators;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain;
using Remotion.Validation.MetaValidation.Rules.Custom;

namespace Remotion.Data.DomainObjects.Validation.UnitTests.DomainObjectAttributesBasedValidationPropertyRuleReflectorTests
{
  [TestFixture]
// ReSharper disable InconsistentNaming
    public class DomainObjectMixin_DomainObjectAttributesBasedValidationPropertyRuleReflectorTest
// ReSharper enable InconsistentNaming
  {
    private PropertyInfo _mixinPropertyWithoutAttribute;
    private PropertyInfo _mixinPropertyWithMandatoryAttribute;
    private PropertyInfo _mixinPropertyWithNullableStringPropertyAttribute;
    private PropertyInfo _mixinPropertyWithMandatoryStringPropertyAttribute;
    private PropertyInfo _interfacePropertyWithoutAttribute;
    private PropertyInfo _interfacePropertyWithMandatoryAttribute;
    private PropertyInfo _interfacePropertyWithNullableStringPropertyAttribute;
    private PropertyInfo _interfacePropertyWithMandatoryStringPropertyAttribute;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _propertyWithoutAttributeReflector;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _propertyWithNullableStringPropertyAttributeReflector;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _propertyWithMandatoryStringPropertyAttributeReflector;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _propertyWithMandatoryAttributeReflector;

    [SetUp]
    public void SetUp ()
    {
      _mixinPropertyWithoutAttribute =
          typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("PropertyWithoutAttribute");
      _interfacePropertyWithoutAttribute =
          typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("PropertyWithoutAttribute");

      _mixinPropertyWithMandatoryAttribute =
          typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("PropertyWithMandatoryAttribute");
      _interfacePropertyWithMandatoryAttribute =
          typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("PropertyWithMandatoryAttribute");

      _mixinPropertyWithNullableStringPropertyAttribute =
          typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("PropertyWithNullableStringPropertyAttribute");
      _interfacePropertyWithNullableStringPropertyAttribute =
          typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("PropertyWithNullableStringPropertyAttribute");

      _mixinPropertyWithMandatoryStringPropertyAttribute =
          typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("PropertyWithMandatoryStringPropertyAttribute");
      _interfacePropertyWithMandatoryStringPropertyAttribute =
          typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty (
              "PropertyWithMandatoryStringPropertyAttribute");

      _propertyWithoutAttributeReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector (
          _interfacePropertyWithoutAttribute,
          _mixinPropertyWithoutAttribute);
      _propertyWithMandatoryAttributeReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector (
          _interfacePropertyWithMandatoryAttribute,
          _mixinPropertyWithMandatoryAttribute
          );
      _propertyWithNullableStringPropertyAttributeReflector =
          new DomainObjectAttributesBasedValidationPropertyRuleReflector (
              _interfacePropertyWithNullableStringPropertyAttribute,
              _mixinPropertyWithNullableStringPropertyAttribute
              );
      _propertyWithMandatoryStringPropertyAttributeReflector =
          new DomainObjectAttributesBasedValidationPropertyRuleReflector (
              _interfacePropertyWithMandatoryStringPropertyAttribute,
              _mixinPropertyWithMandatoryStringPropertyAttribute
              );
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_propertyWithoutAttributeReflector.PropertyType, Is.EqualTo (_interfacePropertyWithoutAttribute.PropertyType));
      Assert.That (_propertyWithMandatoryAttributeReflector.PropertyType, Is.EqualTo (_interfacePropertyWithMandatoryAttribute.PropertyType));
      Assert.That (
          _propertyWithNullableStringPropertyAttributeReflector.PropertyType,
          Is.EqualTo (_interfacePropertyWithNullableStringPropertyAttribute.PropertyType));
      Assert.That (
          _propertyWithMandatoryStringPropertyAttributeReflector.PropertyType,
          Is.EqualTo (_interfacePropertyWithMandatoryStringPropertyAttribute.PropertyType));
    }

    [Test]
    public void GetPropertyAccessExpression ()
    {
      var result = (LambdaExpression) _propertyWithMandatoryAttributeReflector
          .GetPropertyAccessExpression (typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

      Assert.That (result.Body, Is.InstanceOf (typeof (UnaryExpression)));
      Assert.That (((UnaryExpression) result.Body).Operand, Is.InstanceOf (typeof (MemberExpression)));
      Assert.That (
          ((MemberExpression) ((UnaryExpression) result.Body).Operand).Member.DeclaringType,
          Is.EqualTo (typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface)));
    }

    [Test]
    public void NoAttributes ()
    {
      Assert.That (_propertyWithoutAttributeReflector.GetAddingPropertyValidators().Any(), Is.False);
      Assert.That (_propertyWithoutAttributeReflector.GetHardConstraintPropertyValidators().Any(), Is.False);
      Assert.That (_propertyWithoutAttributeReflector.GetRemovingPropertyRegistrations().Any(), Is.False);
      Assert.That (_propertyWithoutAttributeReflector.GetMetaValidationRules().Any(), Is.False);
    }

    [Test]
    public void GetHardConstraintPropertyValidators_MandatoryAttribute ()
    {
      var result = _propertyWithMandatoryAttributeReflector.GetHardConstraintPropertyValidators().ToArray();

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (NotNullValidator)));
    }

    [Test]
    public void GettAddingPropertyValidators_NullableStringPropertyAttribute ()
    {
      var result = _propertyWithNullableStringPropertyAttributeReflector.GetAddingPropertyValidators().ToArray();

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (LengthValidator)));
      Assert.That (((LengthValidator) result[0]).Max, Is.EqualTo (10));
    }

    [Test]
    public void GettAddingPropertyValidators_MandatoryStringPropertyAttribute ()
    {
      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetAddingPropertyValidators().ToArray();

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (LengthValidator)));
      Assert.That (((LengthValidator) result[0]).Max, Is.EqualTo (20));
    }

    [Test]
    public void GetHardConstraintPropertyValidators_NullableStringPropertyAttribute ()
    {
      var result = _propertyWithNullableStringPropertyAttributeReflector.GetHardConstraintPropertyValidators().ToArray();

      Assert.That (result.Any(), Is.False);
    }

    [Test]
    public void GetHardConstraintPropertyValidators_MandatoryStringPropertyAttribute ()
    {
      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetHardConstraintPropertyValidators().ToArray();

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (NotNullValidator)));
    }

    [Test]
    public void GetRemovingPropertyRegistrations ()
    {
      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetRemovingPropertyRegistrations().ToArray();

      Assert.That (result.Any(), Is.False);
    }

    [Test]
    public void GetMetaValidationRules_MandatoryAttribute ()
    {
      var result = _propertyWithMandatoryAttributeReflector.GetMetaValidationRules().ToArray();

      Assert.That (result.Any(), Is.False);
    }

    [Test]
    public void GetMetaValidationRules_NullableStringPropertyAttribute ()
    {
      var result = _propertyWithNullableStringPropertyAttributeReflector.GetMetaValidationRules().ToArray();

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (RemotionMaxLengthMetaValidationRule)));
      Assert.That (((RemotionMaxLengthMetaValidationRule) result[0]).MaxLength, Is.EqualTo (10));
    }

    [Test]
    public void Initialize_WithMixinPropertyAsInterfaceProperty_ThrowsArgumentException ()
    {
      Assert.That (
          () => new DomainObjectAttributesBasedValidationPropertyRuleReflector (
              _mixinPropertyWithMandatoryAttribute,
              _mixinPropertyWithMandatoryAttribute),
          Throws.ArgumentException.And.Message.EqualTo (
              "The property 'PropertyWithMandatoryAttribute' was declared on type 'MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface' "
              + "but only interface declarations are supported when using mixin properties.\r\nParameter name: interfaceProperty"));
    }
  }
}
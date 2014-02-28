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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation.Validators;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Mixins;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.MetaValidation.Rules.Custom;
using Remotion.Validation.Rules;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Create <see cref="IPropertyValidator"/>s based on the <see cref="ILengthConstrainedPropertyAttribute"/> and the <see cref="INullablePropertyAttribute"/>.
  /// </summary>
  public class DomainObjectAttributesBasedValidationPropertyRuleReflector : IAttributesBasedValidationPropertyRuleReflector
  {
    private readonly PropertyInfo _interfaceProperty;
    private readonly PropertyInfo _implementationProperty;

    public DomainObjectAttributesBasedValidationPropertyRuleReflector (PropertyInfo interfaceProperty, PropertyInfo implementationProperty)
    {
      ArgumentUtility.CheckNotNull ("interfaceProperty", interfaceProperty);
      ArgumentUtility.CheckNotNull ("implementationProperty", implementationProperty);
      if (Utilities.ReflectionUtility.CanAscribe (implementationProperty.DeclaringType, typeof (Mixin<>))
          && !interfaceProperty.DeclaringType.IsInterface)
      {
        throw new ArgumentException (
            string.Format (
                "The property '{0}' was declared on type '{1}' but only interface declarations are supported when using mixin properties.",
                interfaceProperty.Name,
                interfaceProperty.DeclaringType.Name),
            "interfaceProperty");
      }

      _interfaceProperty = interfaceProperty;
      _implementationProperty = implementationProperty;
    }

    public Type PropertyType
    {
      get { return _interfaceProperty.PropertyType; }
    }

    public Expression<Func<TValidatedType, TProperty>> GetPropertyAccessExpression<TValidatedType, TProperty> ()
    {
      var parameterExpression = Expression.Parameter (typeof(TValidatedType), "t");
      var propertyExpression = Expression.Property (parameterExpression, _interfaceProperty);
      return
          (Expression<Func<TValidatedType, TProperty>>)
              Expression.Lambda (typeof (Func<TValidatedType, TProperty>), propertyExpression, parameterExpression);
    }

    public IEnumerable<IPropertyValidator> GetAddingPropertyValidators ()
    {
      var lengthConstraintAttribute = AttributeUtility.GetCustomAttribute<ILengthConstrainedPropertyAttribute> (_implementationProperty, false);
      if (lengthConstraintAttribute != null && lengthConstraintAttribute.MaximumLength.HasValue)
        yield return new LengthValidator (0, lengthConstraintAttribute.MaximumLength.Value);
    }

    public IEnumerable<IPropertyValidator> GetHardConstraintPropertyValidators ()
    {
      var nullableAttribute = AttributeUtility.GetCustomAttribute<INullablePropertyAttribute> (_implementationProperty, false);
      if (nullableAttribute != null && !nullableAttribute.IsNullable)
        yield return new NotNullValidator();
    }

    public IEnumerable<ValidatorRegistration> GetRemovingPropertyRegistrations ()
    {
      return Enumerable.Empty<ValidatorRegistration>();
    }

    public IEnumerable<IMetaValidationRule> GetMetaValidationRules ()
    {
      var lengthConstraintAttribute = AttributeUtility.GetCustomAttribute<ILengthConstrainedPropertyAttribute> (_implementationProperty, false);
      if (lengthConstraintAttribute != null && lengthConstraintAttribute.MaximumLength.HasValue)
        yield return new RemotionMaxLengthMetaValidationRule (_implementationProperty, lengthConstraintAttribute.MaximumLength.Value);
    }
  }
}
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
using Remotion.Utilities;
using Remotion.Validation.Attributes.MetaValidation;
using Remotion.Validation.Attributes.Validation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Rules;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Creates <see cref="IPropertyValidator"/>s based on attributes derived from <see cref="AddingValidationAttributeBase"/>.
  /// </summary>
  public class ValidationAttributesBasedPropertyRuleReflector : IAttributesBasedValidationPropertyRuleReflector
  {
    private readonly PropertyInfo _property;

    public ValidationAttributesBasedPropertyRuleReflector (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      _property = property;
    }

    public Type PropertyType
    {
      get { return _property.PropertyType; }
    }

    public Expression<Func<TValidatedType, TProperty>> GetPropertyAccessExpression<TValidatedType, TProperty> ()
    {
      var parameterExpression = Expression.Parameter (typeof (TValidatedType), "t");
      var propertyExpression = Expression.Property (parameterExpression, _property);
      return (Expression<Func<TValidatedType, TProperty>>)
          Expression.Lambda (typeof (Func<TValidatedType, TProperty>), propertyExpression, parameterExpression);
    }

    public IEnumerable<IPropertyValidator> GetAddingPropertyValidators ()
    {
      var addingValidationAttributes =
          _property.GetCustomAttributes (typeof (AddingValidationAttributeBase), false)
              .Cast<AddingValidationAttributeBase>()
              .Where (a => !a.IsHardConstraint);
      return addingValidationAttributes.SelectMany (a => a.GetPropertyValidators (_property));
    }

    public IEnumerable<IPropertyValidator> GetHardConstraintPropertyValidators ()
    {
      var addingValidationAttributes =
          _property.GetCustomAttributes (typeof (AddingValidationAttributeBase), false)
              .Cast<AddingValidationAttributeBase>()
              .Where (a => a.IsHardConstraint);
      return addingValidationAttributes.SelectMany (a => a.GetPropertyValidators (_property));
    }

    public IEnumerable<ValidatorRegistration> GetRemovingPropertyRegistrations ()
    {
      var removingValidationAttributes = _property.GetCustomAttributes (typeof (RemoveValidatorAttribute), false).Cast<RemoveValidatorAttribute>();
      return removingValidationAttributes.Select (a => new ValidatorRegistration (a.ValidatorType, a.CollectorTypeToRemoveFrom));
    }

    public IEnumerable<IMetaValidationRule> GetMetaValidationRules ()
    {
      var metaValidationAttributes =
          _property.GetCustomAttributes (typeof (AddingMetaValidationRuleAttributeBase), false).Cast<AddingMetaValidationRuleAttributeBase>();
      return metaValidationAttributes.Select (mvr => mvr.GetMetaValidationRule (_property));
    }
  }
}
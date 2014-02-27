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
using Remotion.FunctionalProgramming;
using Remotion.Utilities;
using Remotion.Utilities.ReSharperAnnotations;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.RuleBuilders;

namespace Remotion.Validation.Providers
{
  /// <summary>
  /// Base class for <see cref="IValidationCollectorProvider"/> implementations which use property annotations to define the constraints. 
  /// </summary>
  public abstract class AttributeBasedValidationCollectorProviderBase : IValidationCollectorProvider
  {
    private class AttributeValidationCollector<T> : ComponentValidationCollector<T>
    {
      public AttributeValidationCollector ()
      {
      }
    }

    private static readonly MethodInfo s_SetValidationRulesForPropertyMethod =
        typeof (AttributeBasedValidationCollectorProviderBase).GetMethod (
            "SetValidationRulesForProperty",
            BindingFlags.Instance | BindingFlags.NonPublic);

    public const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    protected AttributeBasedValidationCollectorProviderBase ()
    {
    }

    protected abstract ILookup<Type, IAttributesBasedValidationPropertyRuleReflector> CreatePropertyRuleReflectors (IEnumerable<Type> types);

    public IEnumerable<IEnumerable<ValidationCollectorInfo>> GetValidationCollectors (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      var reflectorLookUp = CreatePropertyRuleReflectors (types);
      return reflectorLookUp.Select (g => GetValidationCollector (g.Key, g))
          .Select (collector => EnumerableUtility.Singleton (new ValidationCollectorInfo (collector, GetType())));
    }

    private IComponentValidationCollector GetValidationCollector (
        Type validatedType,
        IEnumerable<IAttributesBasedValidationPropertyRuleReflector> propertyRuleReflectors)
    {
      var collectorType = typeof (AttributeValidationCollector<>).MakeGenericType (validatedType);
      var collectorInstance = (IComponentValidationCollector) Activator.CreateInstance (collectorType);

      foreach (var propertyRuleReflector in propertyRuleReflectors)
      {
        s_SetValidationRulesForPropertyMethod.MakeGenericMethod (validatedType, propertyRuleReflector.PropertyType)
            .Invoke (this, new object[] { propertyRuleReflector, collectorInstance });
      }
      return collectorInstance;
    }

    [ReflectionAPI]
    private void SetValidationRulesForProperty<TValidatedType, TProperty> (
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector,
        AttributeValidationCollector<TValidatedType> collectorInstance)
    {
      var propertyAccessExpression = propertyRuleReflector.GetPropertyAccessExpression<TValidatedType, TProperty>();
      AddValidationRules (collectorInstance, propertyRuleReflector, propertyAccessExpression);
      RemoveValidationRules (collectorInstance, propertyRuleReflector, propertyAccessExpression);
    }

    private void AddValidationRules<TValidatedType, TProperty> (
        IComponentValidationCollector<TValidatedType> collectorInstance,
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector,
        Expression<Func<TValidatedType, TProperty>> propertyAccessExpression)
    {
      var addingValidators = propertyRuleReflector.GetAddingPropertyValidators().ToArray();
      if (addingValidators.Any())
        SetValidators (collectorInstance.AddRule (propertyAccessExpression), addingValidators, false);

      var hardConstraintValidators = propertyRuleReflector.GetHardConstraintPropertyValidators().ToArray();
      if (hardConstraintValidators.Any())
        SetValidators (collectorInstance.AddRule (propertyAccessExpression), hardConstraintValidators, true);

      var metaValidationRules = propertyRuleReflector.GetMetaValidationRules().ToArray();
      if (metaValidationRules.Any())
        SetMetaValidationRules (collectorInstance.AddRule (propertyAccessExpression), metaValidationRules);
    }

    private void RemoveValidationRules<TValidatedType, TProperty> (
        IComponentValidationCollector<TValidatedType> collectorInstance,
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector,
        Expression<Func<TValidatedType, TProperty>> propertyAccessExpression)
    {
      var removingValidators = propertyRuleReflector.GetRemovingPropertyRegistrations().ToArray();
      if (removingValidators.Any())
      {
        var removingComponentRuleBuilder = collectorInstance.RemoveRule (propertyAccessExpression);
        foreach (var validatorRegistration in removingValidators)
          removingComponentRuleBuilder.Validator (validatorRegistration.ValidatorType, validatorRegistration.CollectorTypeToRemoveFrom);
      }
    }

    private void SetValidators<TValidatedType, TProperty> (
        IAddingComponentRuleBuilderOptions<TValidatedType, TProperty> addingComponentRuleBuilder,
        IPropertyValidator[] addingValidators,
        bool isHardConstraint)
    {
      if (isHardConstraint)
        addingComponentRuleBuilder.NotRemovable();
      foreach (var propertyValidator in addingValidators)
        addingComponentRuleBuilder.SetValidator (propertyValidator);
    }

    private void SetMetaValidationRules<TValidatedType, TProperty> (
        IAddingComponentRuleBuilderOptions<TValidatedType, TProperty> addingComponentRuleBuilder,
        IMetaValidationRule[] metaValidationRules)
    {
      foreach (var metaValidationRule in metaValidationRules)
        addingComponentRuleBuilder.AddMetaValidationRule (metaValidationRule);
    }
  }
}
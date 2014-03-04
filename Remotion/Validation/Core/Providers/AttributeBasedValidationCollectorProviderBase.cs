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
using FluentValidation.Internal;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;
using Remotion.Utilities.ReSharperAnnotations;
using Remotion.Validation.Implementation;
using Remotion.Validation.Rules;

namespace Remotion.Validation.Providers
{
  /// <summary>
  /// Base class for <see cref="IValidationCollectorProvider"/> implementations which use property annotations to define the constraints. 
  /// </summary>
  public abstract class AttributeBasedValidationCollectorProviderBase : IValidationCollectorProvider
  {
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
      var validationRules =
          propertyRuleReflectors.Select (
              r =>
                  (
                      Tuple
                          <AddingComponentPropertyRule, AddingComponentPropertyRule, AddingComponentPropertyMetaValidationRule,
                              RemovingComponentPropertyRule>)
                      s_SetValidationRulesForPropertyMethod.MakeGenericMethod (validatedType, r.PropertyType)
                          .Invoke (this, new object[] { r })).ToList();

      return new AttributeBasedComponentValidationCollector (
          validatedType,
          validationRules.Select (vr => vr.Item1).Concat (validationRules.Select (vr => vr.Item2)),
          validationRules.Select (vr => vr.Item3),
          validationRules.Select (vr => vr.Item4));
    }

    [ReflectionAPI]
    private Tuple<AddingComponentPropertyRule, AddingComponentPropertyRule, AddingComponentPropertyMetaValidationRule, RemovingComponentPropertyRule>
        SetValidationRulesForProperty<TValidatedType, TProperty> (
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector)
    {
      var propertyAccessExpression = propertyRuleReflector.GetPropertyAccessExpression<TValidatedType, TProperty>();
      var propertyInfo = propertyAccessExpression.GetMember() as PropertyInfo;
      var propertyFunc = propertyAccessExpression.Compile().CoerceToNonGeneric();
      var collectorType = typeof (AttributeBasedComponentValidationCollector);

      var addingPropertyRule = GetAddingPropertyRule (propertyRuleReflector, propertyInfo, collectorType, propertyFunc, propertyAccessExpression);
      var addingHardConstraintPropertyRule = GetAddingHardConstraintPropertyRule (
          propertyRuleReflector,
          propertyInfo,
          collectorType,
          propertyFunc,
          propertyAccessExpression);
      var addingMetaValidationPropertyRule = GetAddingComponentPropertyMetaValidationRule (propertyRuleReflector, propertyInfo, collectorType);
      var removingPropertyRule = GetRemovingComponentPropertyRule<TValidatedType, TProperty> (propertyRuleReflector, propertyInfo, collectorType);

      return Tuple.Create (
          addingPropertyRule,
          addingHardConstraintPropertyRule,
          addingMetaValidationPropertyRule,
          removingPropertyRule);
    }

    private RemovingComponentPropertyRule GetRemovingComponentPropertyRule<TValidatedType, TProperty> (
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector,
        PropertyInfo propertyInfo,
        Type collectorType)
    {
      var removingPropertyRule = new RemovingComponentPropertyRule (propertyInfo, collectorType);
      foreach (var validatorRegistration in propertyRuleReflector.GetRemovingPropertyRegistrations())
        removingPropertyRule.RegisterValidator (validatorRegistration.ValidatorType, validatorRegistration.CollectorTypeToRemoveFrom);
      return removingPropertyRule;
    }

    private AddingComponentPropertyMetaValidationRule GetAddingComponentPropertyMetaValidationRule (
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector,
        PropertyInfo propertyInfo,
        Type collectorType)
    {
      var addingMetaValidationPropertyRule = new AddingComponentPropertyMetaValidationRule (
          propertyInfo,
          collectorType);
      foreach (var metaValidationRule in propertyRuleReflector.GetMetaValidationRules())
        addingMetaValidationPropertyRule.RegisterMetaValidationRule (metaValidationRule);
      return addingMetaValidationPropertyRule;
    }

    private AddingComponentPropertyRule GetAddingHardConstraintPropertyRule<TValidatedType, TProperty> (
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector,
        PropertyInfo propertyInfo,
        Type collectorType,
        Func<object, object> propertyFunc,
        Expression<Func<TValidatedType, TProperty>> propertyAccessExpression)
    {
      var addingHardConstraintPropertyRule = new AddingComponentPropertyRule (
          propertyInfo,
          collectorType,
          propertyFunc,
          typeof (TValidatedType),
          propertyAccessExpression);
      addingHardConstraintPropertyRule.SetHardConstraint();
      foreach (var validator in propertyRuleReflector.GetHardConstraintPropertyValidators())
        addingHardConstraintPropertyRule.RegisterValidator (validator);
      return addingHardConstraintPropertyRule;
    }

    private AddingComponentPropertyRule GetAddingPropertyRule<TValidatedType, TProperty> (
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector,
        PropertyInfo propertyInfo,
        Type collectorType,
        Func<object, object> propertyFunc,
        Expression<Func<TValidatedType, TProperty>> propertyAccessExpression)
    {
      var addingPropertyRule = new AddingComponentPropertyRule (
          propertyInfo,
          collectorType,
          propertyFunc,
          typeof (TValidatedType),
          propertyAccessExpression);
      foreach (var validator in propertyRuleReflector.GetAddingPropertyValidators())
        addingPropertyRule.RegisterValidator (validator);
      return addingPropertyRule;
    }
  }
}
﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Merging
{
  /// <summary>
  /// Default implementation of the <see cref="IPropertyValidatorExtractor"/> interface.
  /// </summary>
  public class PropertyValidatorExtractor : IPropertyValidatorExtractor
  {
    private readonly ILookup<Type, PropertyValidatorRegistrationWithContext> _validatorTypesToRemove;
    private readonly ILogContext _logContext;

    public PropertyValidatorExtractor (IEnumerable<PropertyValidatorRegistrationWithContext> removedPropertyRuleRegistrations, ILogContext logContext)
    {
      ArgumentUtility.CheckNotNull ("removedPropertyRuleRegistrations", removedPropertyRuleRegistrations);
      ArgumentUtility.CheckNotNull ("logContext", logContext);

      _validatorTypesToRemove = removedPropertyRuleRegistrations.ToLookup (r => r.ValidatorRegistration.ValidatorType);
      _logContext = logContext;
    }

    public IEnumerable<IPropertyValidator> ExtractPropertyValidatorsToRemove (IAddingPropertyValidationRuleCollector addingPropertyValidationRuleCollector)
    {
      ArgumentUtility.CheckNotNull ("addingPropertyValidationRuleCollector", addingPropertyValidationRuleCollector);

      foreach (var existingValidator in addingPropertyValidationRuleCollector.Validators)
      {
        var removingValidatorRegistrationsWithContext = GetRemovingPropertyRegistrations (existingValidator, addingPropertyValidationRuleCollector).ToArray();
        if (removingValidatorRegistrationsWithContext.Any())
        {
          _logContext.ValidatorRemoved (existingValidator, removingValidatorRegistrationsWithContext, addingPropertyValidationRuleCollector);
          yield return existingValidator;
        }
      }
    }

    //TODO RM-5906: add integration test for redefined (new) property in derived class for that a validator should be removed
    private IEnumerable<PropertyValidatorRegistrationWithContext> GetRemovingPropertyRegistrations (
        IPropertyValidator validator, IAddingPropertyValidationRuleCollector addingPropertyValidationRuleCollector)
    {
      return
          _validatorTypesToRemove[validator.GetType()].Where (
              rwc =>
              rwc.RemovingPropertyValidationRuleCollector.Property.Name== addingPropertyValidationRuleCollector.Property.Name
// ReSharper disable PossibleNullReferenceException
              && addingPropertyValidationRuleCollector.Property.DeclaringType.IsAssignableFrom (rwc.RemovingPropertyValidationRuleCollector.Property.DeclaringType)
// ReSharper restore PossibleNullReferenceException
              && (rwc.ValidatorRegistration.CollectorTypeToRemoveFrom == null
                  || rwc.ValidatorRegistration.CollectorTypeToRemoveFrom == addingPropertyValidationRuleCollector.CollectorType));
    }
  }
}
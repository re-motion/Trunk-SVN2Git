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
using Remotion.Utilities;
using Remotion.Validation.RuleCollectors;

namespace Remotion.Validation.RuleBuilders
{
  /// <summary>
  /// Default implementation of the <see cref="IRemovingPropertyValidationRuleBuilder{TValidatedType,TProperty}"/>.
  /// </summary>
  public class RemovingPropertyValidationRuleBuilder<TValidatedType, TProperty> : IRemovingPropertyValidationRuleBuilder<TValidatedType, TProperty>
  {
    private readonly IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollector;
    
    public RemovingPropertyValidationRuleBuilder (IRemovingPropertyValidationRuleCollector removingPropertyValidationRuleCollector)
    {
      ArgumentUtility.CheckNotNull ("removingPropertyValidationRuleCollector", removingPropertyValidationRuleCollector);

      _removingPropertyValidationRuleCollector = removingPropertyValidationRuleCollector;
    }

    public IRemovingPropertyValidationRuleCollector RemovingPropertyValidationRuleCollector
    {
      get { return _removingPropertyValidationRuleCollector; }
    }

    public IRemovingPropertyValidationRuleBuilder<TValidatedType, TProperty> Validator<TValidatorType> ()
    {
      return Validator (typeof (TValidatorType), null);
    }

    public IRemovingPropertyValidationRuleBuilder<TValidatedType, TProperty> Validator<TValidatorType, TCollectorTypeToRemoveFrom> ()
    {
      return Validator (typeof (TValidatorType), typeof(TCollectorTypeToRemoveFrom));
    }

    public IRemovingPropertyValidationRuleBuilder<TValidatedType, TProperty> Validator (Type validatorType)
    {
      ArgumentUtility.CheckNotNull ("validatorType", validatorType);

      return Validator (validatorType, null);
    }

    public IRemovingPropertyValidationRuleBuilder<TValidatedType, TProperty> Validator (Type validatorType, Type collectorTypeToRemoveFrom)
    {
      ArgumentUtility.CheckNotNull ("validatorType", validatorType);
      
      _removingPropertyValidationRuleCollector.RegisterValidator (validatorType, collectorTypeToRemoveFrom);
      return this;
    }
  }
}
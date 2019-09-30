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
using System.Text;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.Rules;
using Remotion.Validation.Validators;

namespace Remotion.Validation.RuleBuilders
{
  /// <summary>
  /// Default implementation of the <see cref="IAddingComponentPropertyRule"/> interface.
  /// </summary>
  public sealed class AddingComponentPropertyRule
      : IAddingComponentPropertyRule
  {
    public static AddingComponentPropertyRule Create<TValidatedType, TProperty> (Expression<Func<TValidatedType, TProperty>> expression, Type collectorType)
    {
      var member = expression.GetMember() as PropertyInfo;
      if (member == null)
        throw new ArgumentException ($"An '{typeof (AddingComponentPropertyRule).Name}' can only created for property members.", "expression");

      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("collectorType", collectorType, typeof (IComponentValidationCollector));

      return new AddingComponentPropertyRule (
          typeof (TValidatedType),
          PropertyInfoAdapter.Create (member),
          collectorType);
    }

    public Type CollectorType { get; }
    public Type ValidatedType { get; }
    public IPropertyInformation Property { get; }
    public bool IsHardConstraint { get; private set; }
    private readonly List<IPropertyValidator> _validators = new List<IPropertyValidator>();

    public AddingComponentPropertyRule (
        Type validatedType,
        IPropertyInformation property,
        Type collectorType)
    {
      ArgumentUtility.CheckNotNull ("validatedType", validatedType);
      ArgumentUtility.CheckNotNull ("property", property);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("collectorType", collectorType, typeof (IComponentValidationCollector));

      CollectorType = collectorType;
      ValidatedType = validatedType;
      Property = property;
      IsHardConstraint = false;
    }

    IValidationRule IAddingComponentPropertyRule.CreateValidationRule ()
    {
      return new PropertyValidationRule (Property, _validators);
    }

    public IEnumerable<IPropertyValidator> Validators
    {
      get { return _validators.ToArray(); }
    }

    public void SetHardConstraint ()
    {
      IsHardConstraint = true;
    }

    public void RegisterValidator (IPropertyValidator validator)
    {
      ArgumentUtility.CheckNotNull ("validator", validator);

      //TODO RM-5906: replace with AddValidator
      AddValidator (validator);
    }

    public void ApplyRemoveValidatorRegistrations (IPropertyValidatorExtractor propertyValidatorExtractor)
    {
      ArgumentUtility.CheckNotNull ("propertyValidatorExtractor", propertyValidatorExtractor);

      var validatorsToRemove = propertyValidatorExtractor.ExtractPropertyValidatorsToRemove (this).ToArray();
      CheckForHardConstraintViolation (validatorsToRemove);
      foreach (var validator in validatorsToRemove)
        RemoveValidator (validator);
    }

    private void CheckForHardConstraintViolation (IPropertyValidator[] validatorsToRemove)
    {
      if (IsHardConstraint && validatorsToRemove.Any())
      {
        throw new ValidationConfigurationException (
            string.Format ("Hard constraint validator(s) '{0}' on property '{1}.{2}' cannot be removed.",
                string.Join (", ", validatorsToRemove.Select (v => v.GetType().Name).ToArray()),
                Property.DeclaringType.FullName,
                Property.Name));
      }
    }

    public void ApplyCondition (Func<object, bool> predicate)
    {
      // TODO RM-5906
      foreach (IPropertyValidator propertyValidator in _validators.ToList())
      {
        var delegatingValidator = new DelegatingValidator (predicate, propertyValidator);
        ReplaceValidator (propertyValidator, delegatingValidator);
      }
    }

    private void AddValidator (IPropertyValidator validator)
    {
      // TODO RM-5906: unique validators
      _validators.Add (validator);
    }

    private void RemoveValidator (IPropertyValidator validator)
    {
      // TODO RM-5906: unique validators
      _validators.Remove (validator);
    }

    private void ReplaceValidator (IPropertyValidator originalValidator, DelegatingValidator delegatingValidator)
    {
      // TODO RM-5906
      var index = _validators.IndexOf (originalValidator);
      if (index <= -1)
        return;

      _validators[index] = delegatingValidator;
    }

    public override string ToString ()
    {
      var sb = new StringBuilder (GetType().Name);
      if (IsHardConstraint)
        sb.Append (" (HARD CONSTRAINT)");

      sb.Append (": ");
      sb.Append (Property.DeclaringType != null ? Property.DeclaringType.FullName + "#" : string.Empty);
      sb.Append (Property.Name);

      return sb.ToString();
    }
  }
}
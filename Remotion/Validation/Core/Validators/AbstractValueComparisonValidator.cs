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
using System.Collections;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Validators
{
  public abstract class AbstractValueComparisonValidator : PropertyValidator, IValueComparisonValidator
  {
    public IComparable ValueToCompare { get; }
    public Comparison Comparison { get; }
    private IComparer Comparer { get; }

    protected AbstractValueComparisonValidator (
        [NotNull] IComparable value,
        [CanBeNull] IComparer comparer,
        Comparison comparison,
        [NotNull] string errorMessage,
        [NotNull] IValidationMessage validationMessage)
        : base (errorMessage, validationMessage)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("errorMessage", errorMessage);
      ArgumentUtility.CheckNotNull ("validationMessage", validationMessage);

      Comparer = comparer;
      ValueToCompare = value;
      Comparison = comparison;
    }

    object IValueComparisonValidator.ValueToCompare => ValueToCompare;

    protected sealed override bool IsValid (PropertyValidatorContext context)
    {
      var propertyValue = context.PropertyValue;

      if (propertyValue == null)
        return true;

      if (Comparer != null)
      {
        var comparerComparisonResult = Comparer.Compare (propertyValue, ValueToCompare);
        if (IsComparisonResultValid (comparerComparisonResult))
          return true;
      }
      else
      {
        if (propertyValue.GetType() != ValueToCompare.GetType())
          return true;

        var comparisonResult = ((IComparable) propertyValue).CompareTo (ValueToCompare);

        if (IsComparisonResultValid (comparisonResult))
          return true;
      }

      context.MessageFormatter.AppendArgument ("ComparisonValue", ValueToCompare);
      return false;
    }

    private bool IsComparisonResultValid (int comparisonResult)
    {
      switch (Comparison)
      {
        case Comparison.GreaterThanOrEqual:
          return comparisonResult >= 0;
        case Comparison.GreaterThan:
          return comparisonResult > 0;
        case Comparison.Equal:
          return comparisonResult == 0;
        case Comparison.LessThan:
          return comparisonResult < 0;
        case Comparison.LessThanOrEqual:
          return comparisonResult <= 0;
        default:
          throw new InvalidOperationException ($"Unknown comparison type '{Comparison}'.");
      }
    }
  }
}
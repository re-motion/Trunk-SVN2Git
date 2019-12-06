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
  public class NotEqualValidator : PropertyValidator, IValueComparisonValidator
  {
    public object ValueToCompare { get; }
    private IEqualityComparer Comparer { get; }

    public NotEqualValidator ([NotNull] object comparisonValue, [CanBeNull] IEqualityComparer comparer = null, [CanBeNull] IValidationMessage validationMessage = null)
        : base (Constants.NotEqualError, validationMessage ?? new InvariantValidationMessage (Constants.NotEqualError))
    {
      ArgumentUtility.CheckNotNull ("comparisonValue", comparisonValue);

      ValueToCompare = comparisonValue;
      Comparer = comparer;
    }

    protected override bool IsValid (PropertyValidatorContext context)
    {
      if (!Compare (ValueToCompare, context.PropertyValue))
        return true;

      context.MessageFormatter.AppendArgument ("ComparisonValue", ValueToCompare);

      return false;
    }

    private bool Compare (object comparisonValue, object propertyValue)
    {
      if (propertyValue == null)
        return false;

      if (Comparer != null)
        return Comparer.Equals (comparisonValue, propertyValue);

      if (comparisonValue.GetType() != propertyValue.GetType())
        return false;

      return comparisonValue.Equals (propertyValue);
    }
  }
}
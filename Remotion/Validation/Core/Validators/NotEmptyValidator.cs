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
using System.Linq;
using JetBrains.Annotations;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Validators
{
  public class NotEmptyValidator : PropertyValidator, INotEmptyValidator
  {
    public NotEmptyValidator ([CanBeNull] IValidationMessage validationMessage = null)
        : base (Constants.NotEmptyError, validationMessage ?? new InvariantValidationMessage (Constants.NotEmptyError))
    {
    }

    protected override bool IsValid (PropertyValidatorContext context)
    {
      var propertyValue = context.PropertyValue;

      if (propertyValue == null)
        return true;

      return !IsEmptyString (propertyValue) && !IsEmptyCollection (propertyValue);
    }

    private bool IsEmptyCollection (object propertyValue)
    {
      if (propertyValue is ICollection collectionValue)
        return !collectionValue.Cast<object>().Any();

      return false;
    }

    private bool IsEmptyString (object value)
    {
      if (value is string stringValue)
        return stringValue.Length == 0;

      return false;
    }
  }
}
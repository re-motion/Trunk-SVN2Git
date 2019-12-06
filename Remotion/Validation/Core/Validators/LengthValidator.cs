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
using JetBrains.Annotations;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Validators
{
  public class LengthValidator : PropertyValidator, ILengthValidator
  {
    public int Min { get; }

    public int? Max { get; }

    public LengthValidator (int min, int max, [CanBeNull] IValidationMessage validationMessage = null)
        : this (min, max, Constants.LengthError, validationMessage ?? new InvariantValidationMessage (Constants.LengthError))
    {
    }

    protected LengthValidator (int min, int? max, [NotNull] string errorMessage, [NotNull] IValidationMessage validationMessage)
        : base (errorMessage, validationMessage)
    {
      Max = max;
      Min = min;

      if (max != null && max < min)
        throw new ArgumentOutOfRangeException ("max", "Max should be larger than min.");
    }

    protected override bool IsValid (PropertyValidatorContext context)
    {
      if (context.PropertyValue == null)
        return true;

      if (!(context.PropertyValue is string stringValue))
        return true;

      if (stringValue.Length < Min || Max != null && stringValue.Length > Max)
      {
        context.MessageFormatter.AppendArgument ("MinLength", Min).AppendArgument ("MaxLength", Max).AppendArgument ("TotalLength", stringValue.Length);
        return false;
      }

      return true;
    }
  }
}
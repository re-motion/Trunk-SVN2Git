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
  public class ExclusiveBetweenValidator : PropertyValidator, IBetweenValidator
  {
    public IComparable From { get; }
    public IComparable To { get; }

    [CanBeNull]
    public IComparer Comparer { get; }

    public ExclusiveBetweenValidator ([NotNull] IComparable from, [NotNull] IComparable to, IComparer comparer = null, [CanBeNull] IValidationMessage validationMessage = null)
        : base (Constants.ExclusiveBetweenError, validationMessage ?? new InvariantValidationMessage (Constants.ExclusiveBetweenError))
    {
      ArgumentUtility.CheckNotNull ("from", from);
      ArgumentUtility.CheckNotNull ("to", to);

      if (from.GetType() != to.GetType())
        throw new ArgumentException ("'from' must have the same type as 'to'.", "to");

      if (to.CompareTo (from) < 0)
        throw new ArgumentOutOfRangeException ("to", "'to' should be larger than 'from'.");

      To = to;
      From = from;
      Comparer = comparer;
    }

    protected override bool IsValid (PropertyValidatorContext context)
    {
      var propertyValue = (IComparable) context.PropertyValue;

      if (propertyValue == null)
        return true;

      if (Comparer != null)
      {
        if (Comparer.Compare (propertyValue, From) > 0 && Comparer.Compare (propertyValue, To) < 0)
          return true;
      }
      else
      {
        if (propertyValue.GetType() != From.GetType())
          return true;

        if (propertyValue.CompareTo (From) > 0 && propertyValue.CompareTo (To) < 0)
          return true;
      }

      context.MessageFormatter.AppendArgument ("From", From).AppendArgument ("To", To).AppendArgument ("Value", context.PropertyValue);
      return false;
    }
  }
}
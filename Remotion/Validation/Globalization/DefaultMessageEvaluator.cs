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
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Globalization
{
  [ImplementationFor (typeof(IDefaultMessageEvaluator), Lifetime = LifetimeKind.Singleton)]
  public class DefaultMessageEvaluator : IDefaultMessageEvaluator
  {
    public bool HasDefaultMessageAssigned (IPropertyValidator validator)
    {
      ArgumentUtility.CheckNotNull ("validator", validator);

      if (validator is NotNullValidator)
        return IsDefault (validator, Constants.NotNullError);

      if (validator is NotEmptyValidator)
        return IsDefault (validator, Constants.NotEmptyError);

      if (validator is NotEqualValidator)
        return IsDefault (validator, Constants.NotEqualError);

      if (validator is EqualValidator)
        return IsDefault (validator, Constants.EqualError);

      if (validator is ExactLengthValidator)
        return IsDefault (validator, Constants.ExactLengthError);

      if (validator is LengthValidator)
        return IsDefault (validator, Constants.LengthError);

      if (validator is ExclusiveBetweenValidator)
        return IsDefault (validator, Constants.ExclusiveBetweenError);

      if (validator is InclusiveBetweenValidator)
        return IsDefault (validator, Constants.InclusiveBetweenError);

      if (validator is LessThanValidator)
        return IsDefault (validator, Constants.LessThanError);

      if (validator is LessThanOrEqualValidator)
        return IsDefault (validator, Constants.LessThanOrEqualError);

      if (validator is GreaterThanValidator)
        return IsDefault (validator, Constants.GreaterThanError);

      if (validator is GreaterThanOrEqualValidator)
        return IsDefault (validator, Constants.GreaterThanOrEqualError);

      if (validator is PredicateValidator)
        return IsDefault (validator, Constants.PredicateError);

      if (validator is RegularExpressionValidator)
        return IsDefault (validator, Constants.RegularExpressionError);

      if (validator is ScalePrecisionValidator)
        return IsDefault (validator, Constants.ScalePrecisionError);

      return false;
    }

    private bool IsDefault (IPropertyValidator validator, string errorMessage)
    {
      return validator.ErrorMessageSource is StaticStringSource && validator.ErrorMessageSource.GetString() == errorMessage;
    }
  }
}
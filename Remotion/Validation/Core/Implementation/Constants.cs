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

namespace Remotion.Validation.Implementation
{
  public static class Constants
  {
    public const string EqualError = "'{PropertyName}' should be equal to '{ComparisonValue}'.";
    public const string ExactLengthError = "'{PropertyName}' must be {MaxLength} characters in length. You entered {TotalLength} characters.";
    public const string ExclusiveBetweenError = "'{PropertyName}' must be between {From} and {To} (exclusive). You entered {Value}.";
    public const string GreaterThanOrEqualError = "'{PropertyName}' must be greater than or equal to '{ComparisonValue}'.";
    public const string GreaterThanError = "'{PropertyName}' must be greater than '{ComparisonValue}'.";
    public const string InclusiveBetweenError = "'{PropertyName}' must be between {From} and {To}. You entered {Value}.";
    public const string LengthError = "'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.";
    public const string LessThanOrEqualError = "'{PropertyName}' must be less than or equal to '{ComparisonValue}'.";
    public const string LessThanError = "'{PropertyName}' must be less than '{ComparisonValue}'.";
    public const string NotEmptyError = "'{PropertyName}' should not be empty.";
    public const string NotEqualError = "'{PropertyName}' should not be equal to '{ComparisonValue}'.";
    public const string NotNullError = "'{PropertyName}' must not be empty.";
    public const string PredicateError = "The specified condition was not met for '{PropertyName}'.";
    public const string RegularExpressionError = "'{PropertyName}' is not in the correct format.";
    public const string ScalePrecisionError = "'{PropertyName}' may not be more than {expectedPrecision} digits in total, with allowance for {expectedScale} decimals. {digits} digits and {actualScale} decimals were found.";
  }
}
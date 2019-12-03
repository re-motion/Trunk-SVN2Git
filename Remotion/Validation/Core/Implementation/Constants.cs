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
    public const string EqualError = "Enter a value equal to '{ComparisonValue}'.";
    public const string ExactLengthError = "Enter exactly {MaxLength} characters.";
    public const string ExclusiveBetweenError = "Enter a value between {From} and {To} (exclusive).";
    public const string GreaterThanOrEqualError = "Enter a value that is greater than or equal to '{ComparisonValue}'.";
    public const string GreaterThanError = "Enter a value that is greater than '{ComparisonValue}'.";
    public const string InclusiveBetweenError = "Enter a value between {From} and {To}.";
    public const string LengthError = "Enter between {MinLength} and {MaxLength} characters.";
    public const string LessThanOrEqualError = "Enter a value that is less than or equal to '{ComparisonValue}'.";
    public const string LessThanError = "Enter a value that is be less than '{ComparisonValue}'.";
    public const string NotEmptyError = "Enter a value.";
    public const string NotEqualError = "Enter a value not equal to '{ComparisonValue}'.";
    public const string NotNullError = "Enter a value.";
    public const string PredicateError = "Enter a value that meets the specified condition.";
    public const string RegularExpressionError = "Enter a value in the correct format.";
    public const string ScalePrecisionError = "!!! The value must not be more than {ExpectedPrecision} digits in total, with allowance for {ExpectedScale} decimals. {Digits} digits and {ActualScale} decimals were found.";
  }
}
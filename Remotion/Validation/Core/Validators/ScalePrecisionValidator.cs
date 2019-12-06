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
using System.Globalization;
using JetBrains.Annotations;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Validators
{
  /// <summary>
  /// Allows a decimal to be validated for scale and precision.
  /// Scale would be the number of digits to the right of the decimal point.
  /// Precision would be the number of digits.
  /// 
  /// It can be configured to use the effective scale and precision
  /// (i.e. ignore trailing zeros) if required.
  /// 
  /// 123.4500 has an scale of 4 and a precision of 7, but an effective scale
  /// and precision of 2 and 5 respectively.
  /// </summary>
  public class ScalePrecisionValidator : PropertyValidator
  {
    public ScalePrecisionValidator (int scale, int precision, [CanBeNull] IValidationMessage validationMessage = null)
        : base (Constants.ScalePrecisionError, validationMessage ?? new InvariantValidationMessage (Constants.ScalePrecisionError))
    {
      if (scale < 0)
        throw new ArgumentOutOfRangeException ("scale", $"Scale must be a positive integer. [value:{scale}].");

      if (precision < 0)
        throw new ArgumentOutOfRangeException ("precision", $"Precision must be a positive integer. [value:{precision}].");

      if (precision < scale)
        throw new ArgumentOutOfRangeException ("scale", $"Scale must be greater than precision. [scale:{scale}, precision:{precision}].");

      Scale = scale;
      Precision = precision;
    }

    public int Scale { get; }

    public int Precision { get; }

    protected override bool IsValid (PropertyValidatorContext context)
    {
      if (!(context.PropertyValue is decimal propertyValue))
        return true;

      var decimalAsString = propertyValue.ToString (CultureInfo.InvariantCulture);
      var splitDecimalString = decimalAsString.Split ('.');

      var characteristic = splitDecimalString[0];
      var mantissa = splitDecimalString.Length == 1 ? "" : splitDecimalString[1];

      var scale = mantissa.Length;
      var precision = characteristic.Length + scale;

      if (scale <= Scale && precision <= Precision)
        return true;

      context.MessageFormatter
          .AppendArgument ("ExpectedPrecision", Precision)
          .AppendArgument ("ExpectedScale", Scale)
          .AppendArgument ("Digits", precision - scale)
          .AppendArgument ("ActualScale", scale);

      return false;
    }
  }
}
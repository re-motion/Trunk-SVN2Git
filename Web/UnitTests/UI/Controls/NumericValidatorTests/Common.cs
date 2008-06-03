/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Globalization;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.UI.Controls.NumericValidatorTests
{
  [TestFixture]
  public class Common : TestBase
  {
    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The combination of the flags in the 'NumberStyle' property is invalid.")]
    public void Validate_WithInvalidNumberStyle ()
    {
      Validator.DataType = NumericValidationDataType.Double;
      Validator.NumberStyle = NumberStyles.HexNumber;
      TextBox.Text = "1";
      Validator.Validate();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The value '-1' of the 'DataType' property is not a valid value.")]
    public void Validate_WithInvalidDataType ()
    {
      PrivateInvoke.SetNonPublicField (Validator, "_dataType", -1);
      TextBox.Text = "a";
      Validator.Validate ();
    }
  }
}

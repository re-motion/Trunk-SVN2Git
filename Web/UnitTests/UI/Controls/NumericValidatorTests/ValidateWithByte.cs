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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.UI.Controls.NumericValidatorTests
{
  [TestFixture]
  public class ValidateWithByte : TestBase
  {
    public override void SetUp ()
    {
      base.SetUp();
      Validator.DataType = NumericValidationDataType.Byte;
    }

    [Test]
    public void ValidValue ()
    {
      Assert.That (Validator.NumberStyle, Is.EqualTo (NumberStyles.None));
      TextBox.Text = "1";
      Validator.Validate();
      Assert.That (Validator.IsValid, Is.True);
    }

    [Test]
    public void ValidValue_WithNumberStyle ()
    {
      Validator.NumberStyle = NumberStyles.Integer | NumberStyles.AllowLeadingWhite;
      TextBox.Text = " 1";
      Validator.Validate();
      Assert.That (Validator.IsValid, Is.True);
    }

    [Test]
    public void EmptyValue ()
    {
      TextBox.Text = string.Empty;
      Validator.Validate();
      Assert.That (Validator.IsValid, Is.True);
    }

    [Test]
    public void InvalidValue ()
    {
      TextBox.Text = "a";
      Validator.Validate();
      Assert.That (Validator.IsValid, Is.False);
    }

    [Test]
    public void InvalidValue_WithNegative ()
    {
      Validator.AllowNegative = false;
      TextBox.Text = "-1";
      Validator.Validate ();
      Assert.That (Validator.IsValid, Is.False);
    }

    [Test]
    public void ValidValue_WithNegativeAndAllowNegative ()
    {
      Assert.That (Validator.AllowNegative, Is.True);
      TextBox.Text = "-1";
      Validator.Validate ();
      Assert.That (Validator.IsValid, Is.False);
    }

    [Test]
    public void InvalidValue_WithNumberStyle ()
    {
      Validator.NumberStyle = NumberStyles.Integer;
      TextBox.Text = "a";
      Validator.Validate ();
      Assert.That (Validator.IsValid, Is.False);
    }

    [Test]
    public void InvalidValue_WithOverflow ()
    {
      TextBox.Text = "256";
      Validator.Validate ();
      Assert.That (Validator.IsValid, Is.False);
    }
  }
}

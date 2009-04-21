// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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

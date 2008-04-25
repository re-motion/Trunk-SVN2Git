using System;
using System.Globalization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.UI.Controls.NumericValidatorTests
{
  [TestFixture]
  public class ValidateWithSingle : TestBase
  {
    public override void SetUp ()
    {
      base.SetUp();
      Validator.DataType = NumericValidationDataType.Single;
    }

    [Test]
    public void ValidValue ()
    {
      Assert.That (Validator.NumberStyle, Is.EqualTo (NumberStyles.None));
      TextBox.Text = "1.1";
      Validator.Validate();
      Assert.That (Validator.IsValid, Is.True);
    }

    [Test]
    public void ValidValue_WithNegative ()
    {
      Assert.That (Validator.AllowNegative, Is.True);
      TextBox.Text = "-1.1";
      Validator.Validate ();
      Assert.That (Validator.IsValid, Is.True);
    }

    [Test]
    public void ValidValue_WithNumberStyle ()
    {
      Validator.NumberStyle = NumberStyles.Float | NumberStyles.AllowLeadingWhite;
      TextBox.Text = " 1.1";
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
      TextBox.Text = "-1.1";
      Validator.Validate ();
      Assert.That (Validator.IsValid, Is.False);
    }

    [Test]
    public void InvalidValue_WithNumberStyle ()
    {
      Validator.NumberStyle = NumberStyles.Float;
      TextBox.Text = "a";
      Validator.Validate ();
      Assert.That (Validator.IsValid, Is.False);
    }
  }
}
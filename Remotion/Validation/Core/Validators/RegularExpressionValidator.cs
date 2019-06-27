// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.RegularExpressionValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation.Resources;
using System;
using System.Text.RegularExpressions;

namespace Remotion.Validation.Validators
{
  public class RegularExpressionValidator : PropertyValidator, IRegularExpressionValidator, IPropertyValidator
  {
    private readonly string expression;
    private readonly Regex regex;

    public RegularExpressionValidator(string expression)
        : base((System.Linq.Expressions.Expression<Func<string>>) (() => Messages.regex_error))
    {
      this.expression = expression;
      this.regex = new Regex(expression);
    }

    public RegularExpressionValidator(Regex regex)
        : base((System.Linq.Expressions.Expression<Func<string>>) (() => Messages.regex_error))
    {
      this.expression = regex.ToString();
      this.regex = regex;
    }

    public RegularExpressionValidator(string expression, RegexOptions options)
        : base((System.Linq.Expressions.Expression<Func<string>>) (() => Messages.regex_error))
    {
      this.expression = expression;
      this.regex = new Regex(expression, options);
    }

    protected override bool IsValid(PropertyValidatorContext context)
    {
      return context.PropertyValue == null || this.regex.IsMatch((string) context.PropertyValue);
    }

    public string Expression
    {
      get
      {
        return this.expression;
      }
    }
  }
}
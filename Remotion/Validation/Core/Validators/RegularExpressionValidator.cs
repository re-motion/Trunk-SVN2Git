// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.RegularExpressionValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Text.RegularExpressions;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Validators
{
  public class RegularExpressionValidator : PropertyValidator, IRegularExpressionValidator
  {
    private readonly string _expression;
    private readonly Regex _regex;

    public RegularExpressionValidator (string expression)
        : base (() => Constants.RegularExpressionError)
    {
      _expression = expression;
      _regex = new Regex (expression);
    }

    protected override bool IsValid (PropertyValidatorContext context)
    {
      return context.PropertyValue == null || _regex.IsMatch ((string) context.PropertyValue);
    }

    public string Expression
    {
      get { return _expression; }
    }
  }
}
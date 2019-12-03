// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.NotEmptyValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Collections;
using System.Linq;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Validators
{
  public class NotEmptyValidator : PropertyValidator, INotEmptyValidator
  {
    private readonly object _defaultValueForType;

    public NotEmptyValidator (object defaultValueForType, IValidationMessage validationMessage = null)
        : base (Constants.NotEmptyError, validationMessage ?? new InvariantValidationMessage (Constants.NotEmptyError))
    {
      _defaultValueForType = defaultValueForType;
    }

    protected override bool IsValid (PropertyValidatorContext context)
    {
      return context.PropertyValue != null
             && !IsInvalidString (context.PropertyValue)
             && !IsEmptyCollection (context.PropertyValue)
             && !Equals (context.PropertyValue, _defaultValueForType);
    }

    private bool IsEmptyCollection (object propertyValue)
    {
      if (propertyValue is IEnumerable source)
        return !source.Cast<object>().Any();

      return false;
    }

    private bool IsInvalidString (object value)
    {
      if (value is string valueString)
        return IsNullOrWhiteSpace (valueString);

      return false;
    }

    private bool IsNullOrWhiteSpace (string value)
    {
      if (value == null)
        return true;

      foreach (var t in value)
      {
        if (!char.IsWhiteSpace (t))
          return false;
      }

      return true;
    }
  }
}
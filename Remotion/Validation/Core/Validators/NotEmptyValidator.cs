// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.NotEmptyValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation.Resources;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace Remotion.Validation.Validators
{
  public class NotEmptyValidator : PropertyValidator, INotEmptyValidator, IPropertyValidator
  {
    private readonly object defaultValueForType;

    public NotEmptyValidator(object defaultValueForType)
        : base((Expression<Func<string>>) (() => Messages.notempty_error))
    {
      this.defaultValueForType = defaultValueForType;
    }

    protected override bool IsValid(PropertyValidatorContext context)
    {
      return context.PropertyValue != null && !this.IsInvalidString(context.PropertyValue) && (!this.IsEmptyCollection(context.PropertyValue) && !object.Equals(context.PropertyValue, this.defaultValueForType));
    }

    private bool IsEmptyCollection(object propertyValue)
    {
      IEnumerable source = propertyValue as IEnumerable;
      if (source != null)
        return !source.Cast<object>().Any<object>();
      return false;
    }

    private bool IsInvalidString(object value)
    {
      if (value is string)
        return this.IsNullOrWhiteSpace(value as string);
      return false;
    }

    private bool IsNullOrWhiteSpace(string value)
    {
      if (value != null)
      {
        for (int index = 0; index < value.Length; ++index)
        {
          if (!char.IsWhiteSpace(value[index]))
            return false;
        }
      }
      return true;
    }
  }
}
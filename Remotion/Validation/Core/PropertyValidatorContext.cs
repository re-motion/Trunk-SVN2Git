// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.PropertyValidatorContext
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation;
using Remotion.Validation.Rules;

namespace Remotion.Validation
{
  public class PropertyValidatorContext
  {
    private readonly FluentValidation.Internal.MessageFormatter messageFormatter = new FluentValidation.Internal.MessageFormatter();
    private bool propertyValueSet;
    private object propertyValue;

    public ValidationContext ParentContext { get; private set; }

    public PropertyRule Rule { get; private set; }

    public string PropertyName { get; private set; }

    public string PropertyDescription
    {
      get
      {
        return this.Rule.GetDisplayName();
      }
    }

    public object Instance
    {
      get
      {
        return this.ParentContext.InstanceToValidate;
      }
    }

    public FluentValidation.Internal.MessageFormatter MessageFormatter
    {
      get
      {
        return this.messageFormatter;
      }
    }

    public object PropertyValue
    {
      get
      {
        if (!this.propertyValueSet)
        {
          this.propertyValue = this.Rule.PropertyFunc(this.Instance);
          this.propertyValueSet = true;
        }
        return this.propertyValue;
      }
      set
      {
        this.propertyValue = value;
        this.propertyValueSet = true;
      }
    }

    public PropertyValidatorContext(
        ValidationContext parentContext,
        PropertyRule rule,
        string propertyName)
    {
      this.ParentContext = parentContext;
      this.Rule = rule;
      this.PropertyName = propertyName;
    }
  }
}
// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.PropertyValidatorContext
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using Remotion.Validation.Implementation;
using Remotion.Validation.Rules;

namespace Remotion.Validation
{
  public class PropertyValidatorContext
  {
    private readonly MessageFormatter _messageFormatter = new MessageFormatter();
    private bool _propertyValueSet;
    private object _propertyValue;

    public ValidationContext ParentContext { get; }

    public PropertyRule Rule { get; }

    public string PropertyName { get; }

    public string PropertyDescription
    {
      get
      {
        return Rule.GetDisplayName();
      }
    }

    public object Instance
    {
      get
      {
        return ParentContext.InstanceToValidate;
      }
    }

    public MessageFormatter MessageFormatter
    {
      get
      {
        return _messageFormatter;
      }
    }

    public object PropertyValue
    {
      get
      {
        if (_propertyValueSet) 
          return _propertyValue;

        _propertyValue = Rule.PropertyFunc(Instance);
        _propertyValueSet = true;
        return _propertyValue;
      }
    }

    public PropertyValidatorContext(ValidationContext parentContext, PropertyRule rule, string propertyName)
    {
      ParentContext = parentContext;
      Rule = rule;
      PropertyName = propertyName;
    }
  }
}
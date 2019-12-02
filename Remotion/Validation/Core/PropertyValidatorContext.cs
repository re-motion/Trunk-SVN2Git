// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.PropertyValidatorContext
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.Rules;

namespace Remotion.Validation
{
  public class PropertyValidatorContext
  {
    private bool _propertyValueSet;
    private object _propertyValue;

    public ValidationContext ParentContext { get; }

    public PropertyValidationRule Rule { get; }

    public MessageFormatter MessageFormatter { get; }

    public PropertyValidatorContext(ValidationContext parentContext, PropertyValidationRule rule)
    {
      ParentContext = parentContext;
      Rule = rule;
      MessageFormatter = new MessageFormatter();
    }

    public IPropertyInformation Property => Rule.Property;

    public object Instance => ParentContext.InstanceToValidate;

    public object PropertyValue
    {
      get
      {
        if (_propertyValueSet) 
          return _propertyValue;

        // TODO RM-5906
        _propertyValue = Rule.Property.GetValue (Instance, new object[0]);
        _propertyValueSet = true;
        return _propertyValue;
      }
    }
  }
}
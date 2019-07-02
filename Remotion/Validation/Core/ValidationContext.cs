// Decompiled with JetBrains decompiler
// Type: FluentValidation.ValidationContext
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation
{
  public class ValidationContext
  {
    public ValidationContext(object instanceToValidate)
        : this(instanceToValidate, new PropertyChain(), (IValidatorSelector) new DefaultValidatorSelector())
    {
    }

    public ValidationContext(
        object instanceToValidate,
        PropertyChain propertyChain,
        IValidatorSelector validatorSelector)
    {
      this.PropertyChain = new PropertyChain(propertyChain);
      this.InstanceToValidate = instanceToValidate;
      this.Selector = validatorSelector;
    }

    public PropertyChain PropertyChain { get; private set; }

    public object InstanceToValidate { get; private set; }

    public IValidatorSelector Selector { get; private set; }

    public bool IsChildContext { get; internal set; }

    public ValidationContext Clone(
        PropertyChain chain = null,
        object instanceToValidate = null,
        IValidatorSelector selector = null)
    {
      return new ValidationContext(instanceToValidate ?? this.InstanceToValidate, chain ?? this.PropertyChain, selector ?? this.Selector);
    }

    internal ValidationContext CloneForChildValidator(object instanceToValidate)
    {
      return new ValidationContext(instanceToValidate, this.PropertyChain, this.Selector)
             {
                 IsChildContext = true
             };
    }
  }
}
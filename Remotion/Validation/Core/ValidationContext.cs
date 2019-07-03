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
    public ValidationContext (object instanceToValidate)
        : this (instanceToValidate, new PropertyChain(), new DefaultValidatorSelector())
    {
    }

    public ValidationContext (object instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector)
    {
      PropertyChain = new PropertyChain (propertyChain);
      InstanceToValidate = instanceToValidate;
      Selector = validatorSelector;
    }

    public PropertyChain PropertyChain { get; }

    public object InstanceToValidate { get; }

    public IValidatorSelector Selector { get; }

    private bool IsChildContext { get; set; }

    internal ValidationContext CloneForChildValidator (object instanceToValidate)
    {
      return new ValidationContext (instanceToValidate, PropertyChain, Selector)
             {
                 IsChildContext = true
             };
    }
  }
}
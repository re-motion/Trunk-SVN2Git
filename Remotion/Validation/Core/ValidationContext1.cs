// Decompiled with JetBrains decompiler
// Type: FluentValidation.ValidationContext`1
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation
{
  public class ValidationContext<T> : ValidationContext
  {
    public ValidationContext(T instanceToValidate)
        : this(instanceToValidate, new PropertyChain(), (IValidatorSelector) new DefaultValidatorSelector())
    {
    }

    public ValidationContext(
        T instanceToValidate,
        PropertyChain propertyChain,
        IValidatorSelector validatorSelector)
        : base((object) instanceToValidate, propertyChain, validatorSelector)
    {
      this.InstanceToValidate = instanceToValidate;
    }

    public new T InstanceToValidate { get; private set; }
  }
}
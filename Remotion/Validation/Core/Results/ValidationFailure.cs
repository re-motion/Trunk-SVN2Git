// Decompiled with JetBrains decompiler
// Type: FluentValidation.Results.ValidationFailure
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;

namespace Remotion.Validation.Results
{
  [Serializable]
  public class ValidationFailure
  {
    private ValidationFailure ()
    {
    }

    /// <summary>Creates a new validation failure.</summary>
    public ValidationFailure (string propertyName, string error)
    {
      PropertyName = propertyName;
      ErrorMessage = error;
    }

    /// <summary>The name of the property.</summary>
    public string PropertyName { get; }

    /// <summary>The error message</summary>
    public string ErrorMessage { get; }

    /// <summary>Custom state associated with the failure.</summary>
    public object CustomState { get; set; }

    /// <summary>Creates a textual representation of the failure.</summary>
    public override string ToString ()
    {
      return ErrorMessage;
    }
  }
}
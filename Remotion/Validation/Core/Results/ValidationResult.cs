// Decompiled with JetBrains decompiler
// Type: FluentValidation.Results.ValidationResult
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Remotion.Validation.Results
{
  [Serializable]
  public class ValidationResult
  {
    private readonly List<ValidationFailure> errors = new List<ValidationFailure>();

    public virtual bool IsValid
    {
      get { return this.Errors.Count == 0; }
    }

    public IList<ValidationFailure> Errors
    {
      get { return (IList<ValidationFailure>) this.errors; }
    }

    public ValidationResult ()
    {
    }

    public ValidationResult (IEnumerable<ValidationFailure> failures)
    {
      this.errors.AddRange (failures.Where<ValidationFailure> ((Func<ValidationFailure, bool>) (failure => failure != null)));
    }
  }
}
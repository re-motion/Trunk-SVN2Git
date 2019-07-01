// Decompiled with JetBrains decompiler
// Type: FluentValidation.ValidationException
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Validation.Results;

namespace Remotion.Validation
{
  public class ValidationException : Exception
  {
    public IEnumerable<ValidationFailure> Errors { get; private set; }

    public ValidationException (IEnumerable<ValidationFailure> errors)
        : base (ValidationException.BuildErrorMesage (errors))
    {
      this.Errors = errors;
    }

    private static string BuildErrorMesage (IEnumerable<ValidationFailure> errors)
    {
      return "Validation failed: "
             + string.Join (
                 "",
                 errors.Select<ValidationFailure, string> ((Func<ValidationFailure, string>) (x => "\r\n -- " + x.ErrorMessage)).ToArray<string>());
    }
  }
}
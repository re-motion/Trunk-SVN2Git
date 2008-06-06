/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using Remotion.Mixins.Definitions;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  [Serializable]
  public struct ValidationResult
  {
    public readonly IVisitableDefinition Definition;

    public readonly List<ValidationResultItem> Successes;
    public readonly List<ValidationResultItem> Warnings;
    public readonly List<ValidationResultItem> Failures;
    public readonly List<ValidationExceptionResultItem> Exceptions;

    public ValidationResult (IVisitableDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      Definition = definition;
      Successes = new List<ValidationResultItem> ();
      Warnings = new List<ValidationResultItem> ();
      Failures = new List<ValidationResultItem> ();
      Exceptions = new List<ValidationExceptionResultItem> ();
    }

    public string GetParentDefinitionString()
    {
      SeparatedStringBuilder sb = new SeparatedStringBuilder(" -> ");
      IVisitableDefinition parent = Definition.Parent;
      while (parent != null)
      {
        sb.Append (parent.FullName);
        parent = parent.Parent;
      }
      return sb.ToString();
    }

    public int TotalRulesExecuted
    {
      get { return Successes.Count + Warnings.Count + Failures.Count + Exceptions.Count; }
    }
  }
}

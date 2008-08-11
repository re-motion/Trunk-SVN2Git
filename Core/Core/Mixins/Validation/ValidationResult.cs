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
    private readonly IVisitableDefinition _definition;

    private readonly List<ValidationResultItem> _successes;
    private readonly List<ValidationResultItem> _warnings;
    private readonly List<ValidationResultItem> _failures;
    private readonly List<ValidationExceptionResultItem> _exceptions;

    public ValidationResult (IVisitableDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      _definition = definition;
      _successes = new List<ValidationResultItem> ();
      _warnings = new List<ValidationResultItem> ();
      _failures = new List<ValidationResultItem> ();
      _exceptions = new List<ValidationExceptionResultItem> ();
    }

    public string GetParentDefinitionString()
    {
      var sb = new SeparatedStringBuilder(" -> ");
      IVisitableDefinition parent = _definition.Parent;
      while (parent != null)
      {
        sb.Append (parent.FullName);
        parent = parent.Parent;
      }
      return sb.ToString();
    }

    public int TotalRulesExecuted
    {
      get { return _successes.Count + _warnings.Count + _failures.Count + _exceptions.Count; }
    }

    public List<ValidationExceptionResultItem> Exceptions
    {
      get { return _exceptions; }
    }

    public List<ValidationResultItem> Failures
    {
      get { return _failures; }
    }

    public List<ValidationResultItem> Warnings
    {
      get { return _warnings; }
    }

    public List<ValidationResultItem> Successes
    {
      get { return _successes; }
    }

    public IVisitableDefinition Definition
    {
      get { return _definition; }
    }
  }
}

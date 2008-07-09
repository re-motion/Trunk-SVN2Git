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
using Remotion.Collections;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  [Serializable]
  public class DefaultValidationLog : IValidationLog
  {
    private readonly Stack<ValidationResult> _currentData = new Stack<ValidationResult> ();
    private readonly List<ValidationResult> _results = new List<ValidationResult> ();

    private int failures = 0;
    private int warnings = 0;
    private int exceptions = 0;
    private int successes = 0;
    private readonly SimpleDataStore<object, object> _contextStore = new SimpleDataStore<object, object> ();

    public IEnumerable<ValidationResult> GetResults()
    {
      return _results;
    }

    public int ResultCount
    {
      get { return _results.Count; }
    }

    public int GetNumberOfFailures()
    {
      return failures;
    }

    public int GetNumberOfWarnings ()
    {
      return warnings;
    }

    public int GetNumberOfSuccesses ()
    {
      return successes;
    }

    public int GetNumberOfUnexpectedExceptions ()
    {
      return exceptions;
    }

    public int GetNumberOfRulesExecuted ()
    {
      return successes + warnings + failures + exceptions;
    }

    public void ValidationStartsFor (IVisitableDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);
      _currentData.Push (new ValidationResult (definition));
    }

    public void ValidationEndsFor (IVisitableDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);
      if (_currentData.Count == 0)
      {
        string message = string.Format ("Validation of definition {0}/{1} cannot be ended, because it wasn't started.", definition.GetType ().Name,
            definition.FullName);
        throw new InvalidOperationException (message);
      }
      else
      {
        ValidationResult popped = _currentData.Pop ();
        if (!popped.Definition.Equals (definition))
        {
          string message = string.Format("Cannot end validation for {0} while {1} is validated.", definition.FullName, popped.Definition.FullName);
          throw new InvalidOperationException (message);
        }
        _results.Add (popped);
      }
    }

    private ValidationResult GetCurrentResult ()
    {
      if (_currentData.Count == 0)
      {
        throw new InvalidOperationException ("Validation has not been started.");
      }
      return _currentData.Peek ();
    }

    public void Succeed (IValidationRule rule)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      GetCurrentResult().Successes.Add (new ValidationResultItem(rule));
      ++successes;
    }

    public void Warn (IValidationRule rule)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      GetCurrentResult ().Warnings.Add (new ValidationResultItem(rule));
      ++warnings;
    }

    public void Fail (IValidationRule rule)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      GetCurrentResult ().Failures.Add (new ValidationResultItem (rule));
      ++failures;
    }

    public void UnexpectedException (IValidationRule rule, Exception ex)
    {
      GetCurrentResult ().Exceptions.Add (new ValidationExceptionResultItem (rule, ex));
      ++exceptions;
    }

    public IDataStore<object, object> ContextStore
    {
      get { return _contextStore; }
    }

    public void MergeIn (IValidationLog log)
    {
      foreach (ValidationResult mergedResult in log.GetResults ())
      {
        ValidationResult? activeResult = FindMatchingResult (mergedResult.Definition);
        if (activeResult == null)
        {
          activeResult = new ValidationResult (mergedResult.Definition);
          _results.Add (activeResult.Value);
        }

        foreach (ValidationResultItem resultItem in mergedResult.Successes)
        {
          activeResult.Value.Successes.Add (resultItem);
          ++successes;
        }
        foreach (ValidationResultItem resultItem in mergedResult.Failures)
        {
          activeResult.Value.Failures.Add (resultItem);
          ++failures;
        }
        foreach (ValidationResultItem resultItem in mergedResult.Warnings)
        {
          activeResult.Value.Warnings.Add (resultItem);
          ++warnings;
        }
        foreach (ValidationExceptionResultItem resultItem in mergedResult.Exceptions)
        {
          activeResult.Value.Exceptions.Add (resultItem);
          ++exceptions;
        }
      }
    }

    private ValidationResult? FindMatchingResult (IVisitableDefinition definition)
    {
      foreach (ValidationResult result in GetResults ())
      {
        if (result.Definition == definition)
          return result;
      }
      return null;
    }
  }
}

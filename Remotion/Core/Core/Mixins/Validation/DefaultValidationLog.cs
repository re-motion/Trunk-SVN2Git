// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  // TODO 4010: Remove attribute
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
      var validationResult = new ValidationResult (ValidatedDefinitionID.FromDefinition (definition));
      _currentData.Push (validationResult);
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
        ValidationResult currentResult = _currentData.Peek();
        // Only compare the full name rather than creating a new ID - it's more performant, and it's only a safety check anyway
        if (currentResult.ValidatedDefinitionID.FullName != definition.FullName)
        {
          string message = string.Format (
              "Cannot end validation for {0} while {1} is validated.", 
              definition.FullName, 
              currentResult.ValidatedDefinitionID.FullName);
          throw new InvalidOperationException (message);
        }

        _currentData.Pop();
        _results.Add (currentResult);
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
      GetCurrentResult().Successes.Add (new ValidationResultItem(rule.RuleName, rule.Message));
      ++successes;
    }

    public void Warn (IValidationRule rule)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      GetCurrentResult ().Warnings.Add (new ValidationResultItem(rule.RuleName, rule.Message));
      ++warnings;
    }

    public void Fail (IValidationRule rule)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      GetCurrentResult ().Failures.Add (new ValidationResultItem (rule.RuleName, rule.Message));
      ++failures;
    }

    public void UnexpectedException (IValidationRule rule, Exception ex)
    {
      GetCurrentResult ().Exceptions.Add (new ValidationExceptionResultItem (rule.RuleName, ex));
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
        ValidationResult? activeResult = FindMatchingResult (mergedResult.ValidatedDefinitionID);
        if (activeResult == null)
        {
          activeResult = new ValidationResult (mergedResult.ValidatedDefinitionID);
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

    private ValidationResult? FindMatchingResult (ValidatedDefinitionID validatedDefinitionID)
    {
      foreach (var result in GetResults ())
      {
        if (result.ValidatedDefinitionID == validatedDefinitionID)
          return result;
      }
      return null;
    }
  }
}

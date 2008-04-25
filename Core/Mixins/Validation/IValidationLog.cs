using System;
using System.Collections.Generic;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.Validation
{
  public interface IValidationLog
  {
    // methods for writing the log
    
    void ValidationStartsFor (IVisitableDefinition definition);
    void ValidationEndsFor (IVisitableDefinition definition);

    void Succeed (IValidationRule rule);
    void Warn (IValidationRule rule);
    void Fail (IValidationRule rule);
    void UnexpectedException (IValidationRule rule, Exception ex);

    // methods for reading the log

    IEnumerable<ValidationResult> GetResults ();
    int ResultCount { get; }

    int GetNumberOfFailures();
    int GetNumberOfWarnings ();
    int GetNumberOfSuccesses ();
    int GetNumberOfUnexpectedExceptions ();
    int GetNumberOfRulesExecuted ();
    void MergeIn (IValidationLog log);
  }
}

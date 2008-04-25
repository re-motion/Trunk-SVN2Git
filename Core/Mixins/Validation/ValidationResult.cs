using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;
using Remotion.Text;

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
using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.Mixins.Validation.Rules;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  public static class Validator
  {
    public static DefaultValidationLog Validate (IVisitableDefinition startingPoint, params IRuleSet[] customRuleSets)
    {
      ArgumentUtility.CheckNotNull ("startingPoint", startingPoint);
      ArgumentUtility.CheckNotNull ("customRuleSets", customRuleSets);

      DefaultValidationLog log = new DefaultValidationLog ();
      Validate (startingPoint, log, customRuleSets);
      return log;
    }

    public static DefaultValidationLog Validate (IEnumerable<IVisitableDefinition> startingPoints, params IRuleSet[] customRuleSets)
    {
      ArgumentUtility.CheckNotNull ("startingPoints", startingPoints);
      ArgumentUtility.CheckNotNull ("customRuleSets", customRuleSets);

      DefaultValidationLog log = new DefaultValidationLog ();
      foreach (IVisitableDefinition startingPoint in startingPoints)
        Validate (startingPoint, log, customRuleSets);
      return log;
    }

    public static void Validate (IVisitableDefinition startingPoint, IValidationLog log, params IRuleSet[] customRuleSets)
    {
      ArgumentUtility.CheckNotNull ("startingPoint", startingPoint);
      ArgumentUtility.CheckNotNull ("log", log);
      ArgumentUtility.CheckNotNull ("customRuleSets", customRuleSets);

      ValidatingVisitor visitor = new ValidatingVisitor (log);
      InstallDefaultRules (visitor);

      foreach (IRuleSet ruleSet in customRuleSets)
        ruleSet.Install (visitor);

      startingPoint.Accept (visitor);
    }

    private static void InstallDefaultRules (ValidatingVisitor visitor)
    {
      foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
      {
        if (!t.IsAbstract && typeof (IRuleSet).IsAssignableFrom (t) && t.Namespace == typeof (IRuleSet).Namespace)
        {
          IRuleSet ruleSet = (IRuleSet) Activator.CreateInstance (t);
          ruleSet.Install (visitor);
        }
      }
    }
  }
}

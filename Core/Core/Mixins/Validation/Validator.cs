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
using System.Reflection;
using Remotion.Mixins.Definitions;
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

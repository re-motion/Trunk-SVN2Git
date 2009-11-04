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

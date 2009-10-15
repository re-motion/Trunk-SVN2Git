// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Linq;
using System.Reflection;
using Remotion.Reflection;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building.RequiredMethodDefinitionBuilding
{
  public class DuckTypingRequiredMethodDefinitionCollector : IRequiredMethodDefinitionCollector
  {
    private static readonly MemberSignatureEqualityComparer s_signatureComparer = new MemberSignatureEqualityComparer ();

    private readonly TargetClassDefinition _targetClassDefinition;
    private readonly ILookup<string, MethodDefinition> _allTargetMethodsByName;

    public DuckTypingRequiredMethodDefinitionCollector (TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);
      _targetClassDefinition = targetClassDefinition;
      _allTargetMethodsByName = targetClassDefinition.GetAllMethods ().ToLookup (method => method.Name);
    }

    public IEnumerable<RequiredMethodDefinition> CreateRequiredMethodDefinitions (RequirementDefinitionBase requirement)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);

      Assertion.IsTrue (requirement.Type.IsInterface);

      foreach (MethodInfo interfaceMethod in requirement.Type.GetMethods ())
      {
        MethodDefinition implementingMethod = FindMethod (interfaceMethod, requirement);
        yield return new RequiredMethodDefinition (requirement, interfaceMethod, implementingMethod);
      }
    }

    private MethodDefinition FindMethod (MethodInfo interfaceMethod, RequirementDefinitionBase requirement)
    {
      var candidatesByInheritanceOffset = from candidate in _allTargetMethodsByName[interfaceMethod.Name]
                                          where s_signatureComparer.Equals (candidate.MethodInfo, interfaceMethod)
                                          let offset = GetInheritanceOffset (candidate.MethodInfo.DeclaringType)
                                          group candidate by offset;

      IGrouping<int, MethodDefinition> candidateGroup = ChooseCandidateGroup (candidatesByInheritanceOffset, interfaceMethod, requirement);

      Assertion.DebugAssert (candidateGroup.Count () == 1, "Unless the signature comparer is broken, each group can only hold one match.");
      return candidateGroup.Single ();
    }

    private IGrouping<int, MethodDefinition> ChooseCandidateGroup (
        IEnumerable<IGrouping<int, MethodDefinition>> candidatesByInheritanceOffset, 
        MethodInfo interfaceMethod, 
        RequirementDefinitionBase requirement)
    {
      try
      {
        return candidatesByInheritanceOffset.OrderBy (group => group.Key).First (); // take the group with the lowest distance from the target class
      }
      catch (InvalidOperationException)
      {
        string requiringMixinsString = SeparatedStringBuilder.Build (
            ", ",
            requirement.FindRequiringMixins (),
            m => "'" + m.FullName + "'");

        string message = string.Format (
            "The dependency '{0}' (required by mixin(s) {1} applied to class '{2}') is not fulfilled - public or protected method '{3}' could not be "
            + "found on the target class.",
            requirement.Type.Name,
            requiringMixinsString,
            requirement.TargetClass.FullName,
            interfaceMethod);
        throw new ConfigurationException (message);
      }
    }

    private int GetInheritanceOffset (Type type)
    {
      int index = 0;
      Type currentBaseType = _targetClassDefinition.Type;

      while (type != currentBaseType)
      {
        currentBaseType = currentBaseType.BaseType;
        Assertion.IsNotNull (currentBaseType, "Types declaring methods of a target class must be in the inheritance hierarchy of that class.");
        ++index;
      }

      return index;
    }
  }
}
// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;
using Remotion.Collections;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;
using ReflectionUtility=Remotion.Mixins.Utilities.ReflectionUtility;

namespace Remotion.Mixins.Definitions.Building
{
  public class MemberDefinitionBuilder
  {
    private readonly ClassDefinitionBase _classDefinition;
    private readonly Predicate<MethodInfo> _methodFilter;
    private readonly Set<MethodInfo> _specialMethods = new Set<MethodInfo> ();
    private readonly BindingFlags _bindingFlags;

    public MemberDefinitionBuilder (ClassDefinitionBase classDefinition, Predicate<MethodInfo> methodFilter, BindingFlags bindingFlags)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("methodFilter", methodFilter);

      _classDefinition = classDefinition;
      _methodFilter = methodFilter;
      _bindingFlags = bindingFlags;
    }

    public void Apply (Type type)
    {
      IEnumerable<MethodInfo> methods = ReflectionUtility.RecursiveGetAllMethods (type, _bindingFlags);
      IEnumerable<PropertyInfo> properties = ReflectionUtility.RecursiveGetAllProperties (type, _bindingFlags);
      IEnumerable<EventInfo> events = ReflectionUtility.RecursiveGetAllEvents (type, _bindingFlags);

      AnalyzeProperties (CleanupMembers(properties, new PropertyNameAndSignatureEqualityComparer()));
      AnalyzeEvents (CleanupMembers(events, new EventNameAndSignatureEqualityComparer()));
      AnalyzeMethods (CleanupMembers(methods, new MethodNameAndSignatureEqualityComparer()));
    }

    private IEnumerable<T> CleanupMembers<T> (IEnumerable<T> members, IEqualityComparer<T> comparer) where T : MemberInfo
    {
      var memberSet = new MultiSet<T>(comparer);
      memberSet.AddRange (members); // collect duplicates
      var result = new Set<T>();
      
      foreach (T member in members)
      {
        var chain = new MemberChain<T> (memberSet[member]);
        result.AddRange (chain.GetNonOverriddenMembers());
      }
      return result;
    }

    private void AnalyzeProperties (IEnumerable<PropertyInfo> properties)
    {
      foreach (PropertyInfo property in properties)
      {
        MethodInfo getMethod = property.GetGetMethod (true);
        MethodInfo setMethod = property.GetSetMethod (true);

        MethodDefinition getMethodDefinition = CreateSpecialMethodDefinition (getMethod);
        MethodDefinition setMethodDefinition = CreateSpecialMethodDefinition (setMethod);

        if (getMethodDefinition != null || setMethodDefinition != null)
        {
          var definition = new PropertyDefinition (property, _classDefinition, getMethodDefinition, setMethodDefinition);
          var attributeBuilder = new AttributeDefinitionBuilder (definition);
          attributeBuilder.Apply (property);
          _classDefinition.Properties.Add (definition);
        }
      }
    }

    private void AnalyzeEvents (IEnumerable<EventInfo> events)
    {
      foreach (EventInfo eventInfo in events)
      {
        MethodInfo addMethod = eventInfo.GetAddMethod (true);
        MethodInfo removeMethod = eventInfo.GetRemoveMethod (true);

        MethodDefinition addMethodDefinition = CreateSpecialMethodDefinition (addMethod);
        MethodDefinition removeMethodDefinition = CreateSpecialMethodDefinition (removeMethod);

        if (addMethodDefinition != null || removeMethodDefinition != null)
        {
          var definition = new EventDefinition (eventInfo, _classDefinition, addMethodDefinition, removeMethodDefinition);
          var attributeBuilder = new AttributeDefinitionBuilder (definition);
          attributeBuilder.Apply (eventInfo);
          _classDefinition.Events.Add (definition);
        }
      }
    }

    private void AnalyzeMethods (IEnumerable<MethodInfo> methods)
    {
      foreach (MethodInfo method in methods)
      {
        if (!_specialMethods.Contains (method) && _methodFilter (method))
        {
          var definition = new MethodDefinition (method, _classDefinition);
          var attributeBuilder = new AttributeDefinitionBuilder (definition);
          attributeBuilder.Apply (method);
          _classDefinition.Methods.Add (definition);
        }
      }
    }

    private MethodDefinition CreateSpecialMethodDefinition (MethodInfo methodInfo)
    {
      if (methodInfo != null && _methodFilter (methodInfo))
      {
        var methodDefinition = new MethodDefinition (methodInfo, _classDefinition);
        var attributeBuilder = new AttributeDefinitionBuilder (methodDefinition);
        attributeBuilder.Apply (methodInfo);
        _specialMethods.Add (methodInfo);
        return methodDefinition;
      }
      else
        return null;
    }
  }
}

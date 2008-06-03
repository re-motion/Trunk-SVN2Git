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
using System.Text;
using Remotion.Collections;
using Remotion.Mixins.Utilities;
using System.Reflection;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  public class RequiredMethodDefinitionBuilder
  {
    private struct SpecialMethods
    {
      public Dictionary<MethodInfo, PropertyInfo> PropertyGetters;
      public Dictionary<MethodInfo, PropertyInfo> PropertySetters;
      public Dictionary<MethodInfo, EventInfo> EventAdders;
      public Dictionary<MethodInfo, EventInfo> EventRemovers;

      public SpecialMethods (RequirementDefinitionBase declaringRequirement)
      {
        // since clients can only use public members of required types (via This and Base calls), we only need to consider those public members
        PropertyGetters = new Dictionary<MethodInfo, PropertyInfo> ();
        PropertySetters = new Dictionary<MethodInfo, PropertyInfo> ();
        foreach (PropertyInfo property in declaringRequirement.Type.GetProperties ())
        {
          MethodInfo getMethod = property.GetGetMethod ();
          if (getMethod != null)
            PropertyGetters.Add (getMethod, property);
          MethodInfo setMethod = property.GetSetMethod ();
          if (setMethod != null)
            PropertySetters.Add (setMethod, property);
        }

        EventAdders = new Dictionary<MethodInfo, EventInfo> ();
        EventRemovers = new Dictionary<MethodInfo, EventInfo> ();
        foreach (EventInfo eventInfo in declaringRequirement.Type.GetEvents ())
        {
          EventAdders.Add (eventInfo.GetAddMethod (), eventInfo);
          EventRemovers.Add (eventInfo.GetRemoveMethod (), eventInfo);
        }
      }

      public MethodDefinition FindMethodUsingSpecials (RequirementDefinitionBase requirement, MethodInfo interfaceMethod, SpecialMethods specialMethods)
      {
        if (specialMethods.PropertyGetters.ContainsKey (interfaceMethod))
          return FindProperty (requirement, specialMethods.PropertyGetters[interfaceMethod], requirement.TargetClass.Properties).GetMethod;
        else if (specialMethods.PropertySetters.ContainsKey (interfaceMethod))
          return FindProperty (requirement, specialMethods.PropertySetters[interfaceMethod], requirement.TargetClass.Properties).SetMethod;
        else if (specialMethods.EventAdders.ContainsKey (interfaceMethod))
          return FindEvent (requirement, specialMethods.EventAdders[interfaceMethod], requirement.TargetClass.Events).AddMethod;
        else if (specialMethods.EventRemovers.ContainsKey (interfaceMethod))
          return FindEvent (requirement, specialMethods.EventRemovers[interfaceMethod], requirement.TargetClass.Events).RemoveMethod;
        else
          return null;
      }
    }
    private readonly static MethodNameAndSignatureEqualityComparer s_methodComparer = new MethodNameAndSignatureEqualityComparer ();
    private readonly static PropertyNameAndSignatureEqualityComparer s_propertyComparer = new PropertyNameAndSignatureEqualityComparer ();
    private readonly static EventNameAndSignatureEqualityComparer s_eventComparer = new EventNameAndSignatureEqualityComparer ();

    private readonly TargetClassDefinition _targetClassDefinition;
    private readonly Dictionary<MethodInfo, MethodDefinition> _baseMethods;

    public RequiredMethodDefinitionBuilder (TargetClassDefinition targetClassDefinition)
    {
      _baseMethods = new Dictionary<MethodInfo, MethodDefinition> ();
      foreach (MethodDefinition methodDefinition in targetClassDefinition.GetAllMethods ())
        _baseMethods.Add (methodDefinition.MethodInfo, methodDefinition);

      _targetClassDefinition = targetClassDefinition;
    }

    public void Apply (RequirementDefinitionBase requirement)
    {
      if (requirement.IsEmptyInterface || !requirement.Type.IsInterface)
        return;

      if (requirement.TargetClass.ImplementedInterfaces.Contains (requirement.Type))
        ApplyForImplementedInterface (requirement);
      else if (requirement.TargetClass.IntroducedInterfaces.ContainsKey (requirement.Type))
        ApplyForIntroducedInterface (requirement);
      else
        ApplyWithDuckTyping (requirement);
    }

    private void ApplyForImplementedInterface (RequirementDefinitionBase requirement)
    {
      Assertion.IsTrue (requirement.Type.IsInterface);
      InterfaceMapping interfaceMapping = _targetClassDefinition.GetAdjustedInterfaceMap (requirement.Type);
      for (int i = 0; i < interfaceMapping.InterfaceMethods.Length; ++i)
      {
        MethodInfo interfaceMethod = interfaceMapping.InterfaceMethods[i];
        MethodDefinition implementingMethod = _baseMethods[interfaceMapping.TargetMethods[i]];

        AddRequiredMethod (requirement, interfaceMethod, implementingMethod);
      }
    }

    private void ApplyForIntroducedInterface (RequirementDefinitionBase requirement)
    {
      Assertion.IsTrue (requirement.Type.IsInterface);
      InterfaceIntroductionDefinition introduction = _targetClassDefinition.IntroducedInterfaces[requirement.Type];
      foreach (EventIntroductionDefinition eventIntroduction in introduction.IntroducedEvents)
      {
        AddRequiredMethod (requirement, eventIntroduction.InterfaceMember.GetAddMethod (), eventIntroduction.ImplementingMember.AddMethod);
        AddRequiredMethod (requirement, eventIntroduction.InterfaceMember.GetRemoveMethod (), eventIntroduction.ImplementingMember.RemoveMethod);
      }
      foreach (PropertyIntroductionDefinition propertyIntroduction in introduction.IntroducedProperties)
      {
        AddRequiredMethod (requirement, propertyIntroduction.InterfaceMember.GetGetMethod (), propertyIntroduction.ImplementingMember.GetMethod);
        AddRequiredMethod (requirement, propertyIntroduction.InterfaceMember.GetSetMethod (), propertyIntroduction.ImplementingMember.SetMethod);
      }
      foreach (MethodIntroductionDefinition methodIntroduction in introduction.IntroducedMethods)
        AddRequiredMethod (requirement, methodIntroduction.InterfaceMember, methodIntroduction.ImplementingMember);
    }

    private void ApplyWithDuckTyping (RequirementDefinitionBase requirement)
    {
      Assertion.IsTrue (requirement.Type.IsInterface);
      SpecialMethods specialMethods = new SpecialMethods (requirement);

      foreach (MethodInfo interfaceMethod in requirement.Type.GetMethods())
      {
        Assertion.IsFalse (interfaceMethod.IsStatic);
        MethodDefinition implementingMethod = specialMethods.FindMethodUsingSpecials (requirement, interfaceMethod, specialMethods);
        if (implementingMethod == null)
          implementingMethod = FindMethod (requirement, interfaceMethod, _targetClassDefinition.Methods);
        AddRequiredMethod (requirement, interfaceMethod, implementingMethod);
      }
    }

    private void AddRequiredMethod (RequirementDefinitionBase requirement, MethodInfo interfaceMethod, MethodDefinition implementingMethod)
    {
      if (interfaceMethod != null)
      {
        Assertion.IsNotNull (implementingMethod);
        requirement.Methods.Add (new RequiredMethodDefinition (requirement, interfaceMethod, implementingMethod));
      }
    }

    private static MethodDefinition FindMethod (RequirementDefinitionBase requirement, MethodInfo interfaceMethod, UniqueDefinitionCollection<MethodInfo, MethodDefinition> methods)
    {
      foreach (MethodDefinition method in methods)
      {
        if (s_methodComparer.Equals (method.MethodInfo, interfaceMethod))
          return method;
      }

      throw ConstructExceptionOnMemberNotFound (requirement, "method " + interfaceMethod.Name);
    }

    private static PropertyDefinition FindProperty (RequirementDefinitionBase requirement, PropertyInfo interfaceProperty, UniqueDefinitionCollection<PropertyInfo, PropertyDefinition> properties)
    {
      foreach (PropertyDefinition property in properties)
      {
        if (s_propertyComparer.Equals (property.PropertyInfo, interfaceProperty))
          return property;
      }

      throw ConstructExceptionOnMemberNotFound (requirement, "property " + interfaceProperty.Name);
    }

    private static EventDefinition FindEvent (RequirementDefinitionBase requirement, EventInfo interfaceEvent, UniqueDefinitionCollection<EventInfo, EventDefinition> events)
    {
      foreach (EventDefinition eventInfo in events)
      {
        if (s_eventComparer.Equals (eventInfo.EventInfo, interfaceEvent))
          return eventInfo;
      }

      throw ConstructExceptionOnMemberNotFound (requirement, "event " + interfaceEvent.Name);
    }

    private static Exception ConstructExceptionOnMemberNotFound (RequirementDefinitionBase requirement, string memberString)
    {
      string dependenciesString = SeparatedStringBuilder.Build (
          ", ",
          requirement.FindRequiringMixins (),
          delegate (MixinDefinition m) { return m.FullName; });
      string message = string.Format (
          "The dependency {0} (mixins {1} applied to class {2}) is not fulfilled - public or protected {3} could not be "
          + "found on the base class.",
          requirement.Type.Name,
          dependenciesString,
          requirement.TargetClass.FullName,
          memberString);
      throw new ConfigurationException (message);
    }
  }
}


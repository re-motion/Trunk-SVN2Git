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
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Collections;

namespace Remotion.Mixins.Definitions.Building
{
  public class InterfaceIntroductionDefinitionBuilder
  {
    private readonly MixinDefinition _mixin;
    private readonly Set<Type> _suppressedInterfaces;

    public InterfaceIntroductionDefinitionBuilder (MixinDefinition mixin)
    {
      _mixin = mixin;
      _suppressedInterfaces = new Set<Type> (typeof (ISerializable), typeof (IDeserializationCallback), typeof (IInitializableMixin));
      AnalyzeSuppressedInterfaces ();
    }

    private void AnalyzeSuppressedInterfaces ()
    {
      foreach (NonIntroducedAttribute notIntroducedAttribute in _mixin.Type.GetCustomAttributes (typeof (NonIntroducedAttribute), true))
        _suppressedInterfaces.Add (notIntroducedAttribute.SuppressedInterface);
    }

    public void Apply ()
    {
      foreach (Type implementedInterface in _mixin.ImplementedInterfaces)
      {
        if (_suppressedInterfaces.Contains (implementedInterface))
          ApplySuppressed (implementedInterface, true);
        else if (_mixin.TargetClass.ImplementedInterfaces.Contains (implementedInterface))
          ApplySuppressed (implementedInterface, false);
        else
          Apply (implementedInterface);
      }
    }

    public void Apply (Type implementedInterface)
    {
      if (_mixin.TargetClass.IntroducedInterfaces.ContainsKey (implementedInterface))
      {
        MixinDefinition otherIntroducer = _mixin.TargetClass.IntroducedInterfaces[implementedInterface].Implementer;
        string message = string.Format (
            "Two mixins introduce the same interface {0} to base class {1}: {2} and {3}.",
            implementedInterface.FullName,
            _mixin.TargetClass.FullName,
            otherIntroducer.FullName,
            _mixin.FullName);
        throw new ConfigurationException (message);
      }

      InterfaceIntroductionDefinition introducedInterface = new InterfaceIntroductionDefinition (implementedInterface, _mixin);
      _mixin.InterfaceIntroductions.Add (introducedInterface);
      _mixin.TargetClass.IntroducedInterfaces.Add (introducedInterface);

      AnalyzeIntroducedMembers (introducedInterface);
    }

    public void ApplySuppressed (Type implementedInterface, bool explicitSuppression)
    {
      SuppressedInterfaceIntroductionDefinition introducedInterface =
          new SuppressedInterfaceIntroductionDefinition (implementedInterface, _mixin, explicitSuppression);
      _mixin.SuppressedInterfaceIntroductions.Add (introducedInterface);
    }


    private void AnalyzeIntroducedMembers (InterfaceIntroductionDefinition introducedInterface)
    {
      MemberImplementationFinder memberFinder = new MemberImplementationFinder (introducedInterface.Type, _mixin);
      Set<MethodInfo> specialMethods = new Set<MethodInfo>();

      AnalyzeProperties (introducedInterface, memberFinder, specialMethods);
      AnalyzeEvents (introducedInterface, memberFinder, specialMethods);
      AnalyzeMethods (introducedInterface, memberFinder, specialMethods);
    }

    private void AnalyzeProperties (InterfaceIntroductionDefinition introducedInterface, MemberImplementationFinder memberFinder,
         Set<MethodInfo> specialMethods)
    {
      foreach (PropertyInfo interfaceProperty in introducedInterface.Type.GetProperties())
      {
        PropertyDefinition implementer = memberFinder.FindPropertyImplementation (interfaceProperty);
        CheckMemberImplementationFound (implementer, interfaceProperty);
        introducedInterface.IntroducedProperties.Add (new PropertyIntroductionDefinition (introducedInterface, interfaceProperty, implementer));

        MethodInfo getMethod = interfaceProperty.GetGetMethod();
        if (getMethod != null)
          specialMethods.Add (getMethod);

        MethodInfo setMethod = interfaceProperty.GetSetMethod();
        if (setMethod != null)
          specialMethods.Add (setMethod);
      }
    }

    private void AnalyzeEvents (InterfaceIntroductionDefinition introducedInterface, MemberImplementationFinder memberFinder,
        Set<MethodInfo> specialMethods)
    {
      foreach (EventInfo interfaceEvent in introducedInterface.Type.GetEvents())
      {
        EventDefinition implementer = memberFinder.FindEventImplementation (interfaceEvent);
        CheckMemberImplementationFound (implementer, interfaceEvent);
        introducedInterface.IntroducedEvents.Add (new EventIntroductionDefinition (introducedInterface, interfaceEvent, implementer));

        specialMethods.Add (interfaceEvent.GetAddMethod());
        specialMethods.Add (interfaceEvent.GetRemoveMethod());
      }
    }

    private void AnalyzeMethods (InterfaceIntroductionDefinition introducedInterface, MemberImplementationFinder memberFinder,
        Set<MethodInfo> specialMethods)
    {
      foreach (MethodInfo interfaceMethod in introducedInterface.Type.GetMethods())
      {
        if (!specialMethods.Contains (interfaceMethod))
        {
          MethodDefinition implementer = memberFinder.FindMethodImplementation (interfaceMethod);
          CheckMemberImplementationFound (implementer, interfaceMethod);
          introducedInterface.IntroducedMethods.Add (new MethodIntroductionDefinition (introducedInterface, interfaceMethod, implementer));
        }
      }
    }

    private void CheckMemberImplementationFound (object implementation, MemberInfo interfaceMember)
    {
      if (implementation == null)
      {
        string message = string.Format (
            "An implementation for interface member {0}.{1} could not be found in mixin {2}.",
            interfaceMember.DeclaringType.FullName,
            interfaceMember.Name,
            _mixin.FullName);
        throw new ConfigurationException (message);
      }
    }
  }
}

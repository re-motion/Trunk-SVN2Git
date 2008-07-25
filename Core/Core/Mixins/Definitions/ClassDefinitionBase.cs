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
using Remotion.Collections;
using Remotion.Utilities;
using ReflectionUtility=Remotion.Mixins.Utilities.ReflectionUtility;

namespace Remotion.Mixins.Definitions
{
  public abstract class ClassDefinitionBase : IAttributableDefinition, IVisitableDefinition
  {
    public readonly UniqueDefinitionCollection<MethodInfo, MethodDefinition> Methods =
        new UniqueDefinitionCollection<MethodInfo, MethodDefinition> (delegate (MethodDefinition m) { return m.MethodInfo; });
    public readonly UniqueDefinitionCollection<PropertyInfo, PropertyDefinition> Properties =
        new UniqueDefinitionCollection<PropertyInfo, PropertyDefinition> (delegate (PropertyDefinition p) { return p.PropertyInfo; });
    public readonly UniqueDefinitionCollection<EventInfo, EventDefinition> Events =
        new UniqueDefinitionCollection<EventInfo, EventDefinition> (delegate (EventDefinition p) { return p.EventInfo; });

    private readonly MultiDefinitionCollection<Type, AttributeDefinition> _customAttributes =
        new MultiDefinitionCollection<Type, AttributeDefinition> (delegate (AttributeDefinition a) { return a.AttributeType; });

    private readonly Type _type;
    private readonly Set<Type> _implementedInterfaces;

    public ClassDefinitionBase (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (type.ContainsGenericParameters)
        throw new ArgumentException (string.Format ("The type {0} contains generic parameters, which is not allowed.", type.FullName), "type");
      _type = type;
      _implementedInterfaces = new Set<Type> (_type.GetInterfaces());
    }

    public Type Type
    {
      get { return _type; }
    }

    public string Name
    {
      get { return Type.Name; }
    }

    public string FullName
    {
      get {
        if (Type.IsGenericType)
          return Type.GetGenericTypeDefinition ().FullName;
        else
          return Type.FullName;
      }
    }

    public InterfaceMapping GetAdjustedInterfaceMap(Type interfaceType)
    {
      const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

      InterfaceMapping mapping = Type.GetInterfaceMap (interfaceType);
      for (int i = 0; i < mapping.InterfaceMethods.Length; ++i)
      {
        MethodInfo targetMethod = mapping.TargetMethods[i];
        if (!targetMethod.DeclaringType.Equals (Type))
        {
          Type[] types = ReflectionUtility.GetMethodParameterTypes (targetMethod);
          mapping.TargetMethods[i] = targetMethod.DeclaringType.GetMethod (targetMethod.Name, bindingFlags, null, types, null);
        }
      }
      return mapping;
    }

    public abstract IVisitableDefinition Parent { get; }

    public Set<Type> ImplementedInterfaces
    {
      get { return _implementedInterfaces; }
    }

    public MultiDefinitionCollection<Type, AttributeDefinition> CustomAttributes
    {
      get { return _customAttributes; }
    }

    public ICustomAttributeProvider CustomAttributeProvider
    {
      get { return Type; }
    }

    public IEnumerable<MemberDefinition> GetAllMembers()
    {
      foreach (MethodDefinition method in Methods)
        yield return method;
      foreach (PropertyDefinition property in Properties)
        yield return property;
      foreach (EventDefinition eventDefinition in Events)
        yield return eventDefinition;
    }

    public IEnumerable<MethodDefinition> GetAllMethods ()
    {
      foreach (MethodDefinition method in Methods)
        yield return method;
      foreach (PropertyDefinition property in Properties)
      {
        if (property.GetMethod != null)
          yield return property.GetMethod;
        if (property.SetMethod != null)
          yield return property.SetMethod;
      }
      foreach (EventDefinition eventDef in Events)
      {
        yield return eventDef.AddMethod;
        yield return eventDef.RemoveMethod;
      }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      ChildSpecificAccept (visitor);

      Methods.Accept (visitor);
      Properties.Accept (visitor);
      Events.Accept (visitor);
      CustomAttributes.Accept (visitor);
    }

    protected abstract void ChildSpecificAccept (IDefinitionVisitor visitor);

    public bool HasOverriddenMembers ()
    {
      foreach (MemberDefinition member in GetAllMembers ())
      {
        if (member.Overrides.Count > 0)
          return true;
      }
      return false;
    }

    public bool HasProtectedOverriders ()
    {
      foreach (MethodDefinition method in GetAllMethods())
      {
        if (method.Base != null && (method.MethodInfo.IsFamily || method.MethodInfo.IsFamilyOrAssembly))
          return true;
      }
      return false;
    }

    public override string ToString ()
    {
      return FullName;
    }
  }
}

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
using System.Diagnostics;
using Remotion.Mixins.Utilities;
using Remotion.Collections;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{Type}, TargetClass = {TargetClass.Type}")]
  public abstract class RequirementDefinitionBase : IVisitableDefinition
  {
    public readonly UniqueDefinitionCollection<DependencyDefinitionBase, DependencyDefinitionBase> RequiringDependencies =
        new UniqueDefinitionCollection<DependencyDefinitionBase, DependencyDefinitionBase> (delegate (DependencyDefinitionBase d) { return d; });

    public readonly UniqueDefinitionCollection<MethodInfo, RequiredMethodDefinition> Methods =
        new UniqueDefinitionCollection<MethodInfo, RequiredMethodDefinition> (delegate (RequiredMethodDefinition m) { return m.InterfaceMethod; });

    private readonly TargetClassDefinition _targetClass;
    private readonly Type _type;

    public RequirementDefinitionBase(TargetClassDefinition targetClass, Type type)
    {
      ArgumentUtility.CheckNotNull ("targetClass", targetClass);
      ArgumentUtility.CheckNotNull ("type", type);

      _targetClass = targetClass;
      _type = type;
    }

    public TargetClassDefinition TargetClass
    {
      get { return _targetClass; }
    }

    public Type Type
    {
      get { return _type; }
    }

    public string FullName
    {
      get { return Type.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return TargetClass; }
    }

    public bool IsEmptyInterface
    {
      get { return Type.IsInterface && Type.GetMethods().Length == 0; }
    }

    public bool IsAggregatorInterface
    {
      get { return IsEmptyInterface && Type.GetInterfaces().Length != 0; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      ConcreteAccept (visitor);
      Methods.Accept (visitor);
    }

    protected abstract void ConcreteAccept (IDefinitionVisitor visitor);

    public IEnumerable<MixinDefinition> FindRequiringMixins()
    {
      Set<MixinDefinition> mixins = new Set<MixinDefinition>();
      foreach (DependencyDefinitionBase dependency in RequiringDependencies)
        mixins.Add (dependency.Depender);
      return mixins;
    }
  }
}

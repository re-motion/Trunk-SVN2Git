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
using System.Reflection;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{Type}, TargetClass = {TargetClass.Type}")]
  public abstract class RequirementDefinitionBase : IVisitableDefinition
  {
    private readonly UniqueDefinitionCollection<DependencyDefinitionBase, DependencyDefinitionBase> _requiringDependencies =
        new UniqueDefinitionCollection<DependencyDefinitionBase, DependencyDefinitionBase> (d => d);

    private readonly UniqueDefinitionCollection<MethodInfo, RequiredMethodDefinition> _methods =
        new UniqueDefinitionCollection<MethodInfo, RequiredMethodDefinition> (m => m.InterfaceMethod);

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

    public UniqueDefinitionCollection<DependencyDefinitionBase, DependencyDefinitionBase> RequiringDependencies
    {
      get { return _requiringDependencies; }
    }

    public UniqueDefinitionCollection<MethodInfo, RequiredMethodDefinition> Methods
    {
      get { return _methods; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      ConcreteAccept (visitor);
      _methods.Accept (visitor);
    }

    protected abstract void ConcreteAccept (IDefinitionVisitor visitor);

    public IEnumerable<MixinDefinition> FindRequiringMixins()
    {
      var mixins = new Set<MixinDefinition>();
      foreach (DependencyDefinitionBase dependency in _requiringDependencies)
        mixins.Add (dependency.Depender);
      return mixins;
    }
  }
}

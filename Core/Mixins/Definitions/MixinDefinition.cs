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
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{Type}, TargetClass = {TargetClass.Type}")]
  public class MixinDefinition : ClassDefinitionBase
  {
    public readonly UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> InterfaceIntroductions =
        new UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> (delegate (InterfaceIntroductionDefinition i) { return i.Type; });
    public readonly UniqueDefinitionCollection<Type, SuppressedInterfaceIntroductionDefinition> SuppressedInterfaceIntroductions =
        new UniqueDefinitionCollection<Type, SuppressedInterfaceIntroductionDefinition> (
            delegate (SuppressedInterfaceIntroductionDefinition i) { return i.Type; });
    public readonly UniqueDefinitionCollection<Type, ThisDependencyDefinition> ThisDependencies =
        new UniqueDefinitionCollection<Type, ThisDependencyDefinition> (delegate (ThisDependencyDefinition d) { return d.RequiredType.Type; });
    public readonly UniqueDefinitionCollection<Type, BaseDependencyDefinition> BaseDependencies =
        new UniqueDefinitionCollection<Type, BaseDependencyDefinition> (delegate (BaseDependencyDefinition d) { return d.RequiredType.Type; });
    public readonly UniqueDefinitionCollection<Type, MixinDependencyDefinition> MixinDependencies =
        new UniqueDefinitionCollection<Type, MixinDependencyDefinition> (delegate (MixinDependencyDefinition d) { return d.RequiredType.Type; });

    private readonly TargetClassDefinition _targetClass;
    private readonly bool _acceptsAlphabeticOrdering;

    private int _mixinIndex;
    private readonly MixinKind _mixinKind;

    public MixinDefinition (MixinKind mixinKind, Type type, TargetClassDefinition targetClass, bool acceptsAlphabeticOrdering)
        : base (type)
    {
      ArgumentUtility.CheckNotNull ("targetClass", targetClass);
      _mixinKind = mixinKind;
      _targetClass = targetClass;
      _acceptsAlphabeticOrdering = acceptsAlphabeticOrdering;
    }

    public TargetClassDefinition TargetClass
    {
      get { return _targetClass; }
    }

    public IEnumerable<MemberDefinition> GetAllOverrides()
    {
      foreach (MemberDefinition member in GetAllMembers())
      {
        if (member.BaseAsMember != null)
          yield return member;
      }
    }

    public override IVisitableDefinition Parent
    {
      get { return TargetClass; }
    }

    public int MixinIndex
    {
      get { return _mixinIndex; }
      internal set { _mixinIndex = value; }
    }

    public MixinKind MixinKind
    {
      get { return _mixinKind; }
    }

    public bool AcceptsAlphabeticOrdering
    {
      get { return _acceptsAlphabeticOrdering; }
    }

    protected override void ChildSpecificAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.Visit (this);

      InterfaceIntroductions.Accept (visitor);
      SuppressedInterfaceIntroductions.Accept (visitor);
      ThisDependencies.Accept (visitor);
      BaseDependencies.Accept (visitor);
      MixinDependencies.Accept (visitor);
    }

    internal IEnumerable<DependencyDefinitionBase> GetOrderRelevantDependencies ()
    {
      foreach (BaseDependencyDefinition dependency in BaseDependencies)
        yield return dependency;
      foreach (MixinDependencyDefinition dependency in MixinDependencies)
        yield return dependency;
    }
  }
}

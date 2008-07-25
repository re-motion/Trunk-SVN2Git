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
  public class MixinDefinition : ClassDefinitionBase, IAttributeIntroductionSource
  {
    public readonly UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> InterfaceIntroductions =
        new UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> (i => i.InterfaceType);
    public readonly UniqueDefinitionCollection<Type, NonInterfaceIntroductionDefinition> NonInterfaceIntroductions =
        new UniqueDefinitionCollection<Type, NonInterfaceIntroductionDefinition> (i => i.InterfaceType);

    public readonly UniqueDefinitionCollection<Type, ThisDependencyDefinition> ThisDependencies =
        new UniqueDefinitionCollection<Type, ThisDependencyDefinition> (d => d.RequiredType.Type);
    public readonly UniqueDefinitionCollection<Type, BaseDependencyDefinition> BaseDependencies =
        new UniqueDefinitionCollection<Type, BaseDependencyDefinition> (d => d.RequiredType.Type);
    public readonly UniqueDefinitionCollection<Type, MixinDependencyDefinition> MixinDependencies =
        new UniqueDefinitionCollection<Type, MixinDependencyDefinition> (d => d.RequiredType.Type);

    public MixinDefinition (MixinKind mixinKind, Type type, TargetClassDefinition targetClass, bool acceptsAlphabeticOrdering)
        : base (type)
    {
      ArgumentUtility.CheckNotNull ("targetClass", targetClass);

      SuppressedAttributeIntroductions = new MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> (a => a.AttributeType);
      NonAttributeIntroductions = new MultiDefinitionCollection<Type, NonAttributeIntroductionDefinition> (a => a.AttributeType);
      AttributeIntroductions = new MultiDefinitionCollection<Type, AttributeIntroductionDefinition> (a => a.AttributeType);

      MixinKind = mixinKind;
      TargetClass = targetClass;
      AcceptsAlphabeticOrdering = acceptsAlphabeticOrdering;
    }

    public MultiDefinitionCollection<Type, AttributeIntroductionDefinition> AttributeIntroductions { get; private set; }
    public MultiDefinitionCollection<Type, NonAttributeIntroductionDefinition> NonAttributeIntroductions { get; private set; }
    public MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> SuppressedAttributeIntroductions { get; private set; }

    public TargetClassDefinition TargetClass { get; private set; }
    public int MixinIndex { get; internal set; }
    public MixinKind MixinKind { get; private set; }
    public bool AcceptsAlphabeticOrdering { get; private set; }

    public override IVisitableDefinition Parent
    {
      get { return TargetClass; }
    }

    public IEnumerable<MemberDefinition> GetAllOverrides ()
    {
      foreach (MemberDefinition member in GetAllMembers ())
      {
        if (member.BaseAsMember != null)
          yield return member;
      }
    }

    protected override void ChildSpecificAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.Visit (this);

      InterfaceIntroductions.Accept (visitor);
      NonInterfaceIntroductions.Accept (visitor);
      
      AttributeIntroductions.Accept (visitor);
      NonAttributeIntroductions.Accept (visitor);
      SuppressedAttributeIntroductions.Accept (visitor);

      ThisDependencies.Accept (visitor);
      BaseDependencies.Accept (visitor);
      MixinDependencies.Accept (visitor);
    }

    internal IEnumerable<DependencyDefinitionBase> GetOrderRelevantDependencies ()
    {
      foreach (var dependency in BaseDependencies)
        yield return dependency;
      foreach (var dependency in MixinDependencies)
        yield return dependency;
    }
  }
}

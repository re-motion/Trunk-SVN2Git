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
using System.Diagnostics;
using Remotion.Collections;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{Type}, TargetClass = {TargetClass.Type}")]
  public class MixinDefinition : ClassDefinitionBase, IAttributeIntroductionSource
  {
    private readonly UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> _interfaceIntroductions =
        new UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> (i => i.InterfaceType);
    private readonly UniqueDefinitionCollection<Type, NonInterfaceIntroductionDefinition> _nonInterfaceIntroductions =
        new UniqueDefinitionCollection<Type, NonInterfaceIntroductionDefinition> (i => i.InterfaceType);

    private readonly UniqueDefinitionCollection<Type, ThisDependencyDefinition> _thisDependencies =
        new UniqueDefinitionCollection<Type, ThisDependencyDefinition> (d => d.RequiredType.Type);
    private readonly UniqueDefinitionCollection<Type, BaseDependencyDefinition> _baseDependencies =
        new UniqueDefinitionCollection<Type, BaseDependencyDefinition> (d => d.RequiredType.Type);
    private readonly UniqueDefinitionCollection<Type, MixinDependencyDefinition> _mixinDependencies =
        new UniqueDefinitionCollection<Type, MixinDependencyDefinition> (d => d.RequiredType.Type);

    private readonly MultiDefinitionCollection<Type, AttributeIntroductionDefinition> _attributeIntroductions = 
        new MultiDefinitionCollection<Type, AttributeIntroductionDefinition> (a => a.AttributeType);
    private readonly MultiDefinitionCollection<Type, NonAttributeIntroductionDefinition> _nonAttributeIntroductions =
        new MultiDefinitionCollection<Type, NonAttributeIntroductionDefinition> (a => a.AttributeType);
    private readonly MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> _suppressedAttributeIntroductions =
        new MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> (a => a.AttributeType);

    private readonly TargetClassDefinition _targetClass;
    private readonly MixinKind _mixinKind;
    private readonly bool _acceptsAlphabeticOrdering;

    private object _concreteTypeIdentifier;
    
    public MixinDefinition (MixinKind mixinKind, Type type, TargetClassDefinition targetClass, bool acceptsAlphabeticOrdering)
        : base (type)
    {
      ArgumentUtility.CheckNotNull ("targetClass", targetClass);

      _mixinKind = mixinKind;
      _targetClass = targetClass;
      _acceptsAlphabeticOrdering = acceptsAlphabeticOrdering;
    }

    public MultiDefinitionCollection<Type, AttributeIntroductionDefinition> AttributeIntroductions
    {
      get { return _attributeIntroductions; }
    }

    public MultiDefinitionCollection<Type, NonAttributeIntroductionDefinition> NonAttributeIntroductions
    {
      get { return _nonAttributeIntroductions; }
    }

    public MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> SuppressedAttributeIntroductions
    {
      get { return _suppressedAttributeIntroductions; }
    }

    public TargetClassDefinition TargetClass
    {
      get { return _targetClass; }
    }

    public MixinKind MixinKind
    {
      get { return _mixinKind; }
    }

    public bool AcceptsAlphabeticOrdering
    {
      get { return _acceptsAlphabeticOrdering; }
    }

    public override IVisitableDefinition Parent
    {
      get { return TargetClass; }
    }

    public int MixinIndex { get; internal set; }

    public UniqueDefinitionCollection<Type, ThisDependencyDefinition> ThisDependencies
    {
      get { return _thisDependencies; }
    }

    public UniqueDefinitionCollection<Type, BaseDependencyDefinition> BaseDependencies
    {
      get { return _baseDependencies; }
    }

    public UniqueDefinitionCollection<Type, MixinDependencyDefinition> MixinDependencies
    {
      get { return _mixinDependencies; }
    }

    public UniqueDefinitionCollection<Type, NonInterfaceIntroductionDefinition> NonInterfaceIntroductions
    {
      get { return _nonInterfaceIntroductions; }
    }

    public UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> InterfaceIntroductions
    {
      get { return _interfaceIntroductions; }
    }

    public IEnumerable<MemberDefinitionBase> GetAllOverrides ()
    {
      foreach (MemberDefinitionBase member in GetAllMembers ())
      {
        if (member.BaseAsMember != null)
          yield return member;
      }
    }

    protected override void ChildSpecificAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.Visit (this);

      _interfaceIntroductions.Accept (visitor);
      _nonInterfaceIntroductions.Accept (visitor);
      
      AttributeIntroductions.Accept (visitor);
      NonAttributeIntroductions.Accept (visitor);
      SuppressedAttributeIntroductions.Accept (visitor);

      _thisDependencies.Accept (visitor);
      _baseDependencies.Accept (visitor);
      _mixinDependencies.Accept (visitor);
    }

    public IEnumerable<DependencyDefinitionBase> GetOrderRelevantDependencies ()
    {
      foreach (var dependency in _baseDependencies)
        yield return dependency;
      foreach (var dependency in _mixinDependencies)
        yield return dependency;
    }

    public bool NeedsDerivedMixinType ()
    {
      return Type.IsAbstract || HasOverriddenMembers () || HasProtectedOverriders ();
    }

    public object GetConcreteMixinTypeCacheKey ()
    {
      // for each overridden member, find its topmost definition
      // these are used as the cache key (together with the mixin type), ie. we will always use the same concrete type given the same combination
      // of overriding methods for the same mixin.

      if (_concreteTypeIdentifier == null)
        _concreteTypeIdentifier = CalculateConcreteTypeIdentifier ();
      return _concreteTypeIdentifier;
    }

    private object CalculateConcreteTypeIdentifier ()
    {
      var topMostOverriders = new Set<MethodInfo> ();
      foreach (var member in GetAllMethods())
      {
        foreach (var overrider in member.Overrides)
          topMostOverriders.Add (overrider.MethodInfo.GetBaseDefinition ());
      }

      if (topMostOverriders.Count != 0)
        return Tuple.NewTuple (Type, new SetBasedCacheKey<MethodInfo> (topMostOverriders));
      else // if no overrides, use the mixin definition as a cache
        return this;
    }
  }
}

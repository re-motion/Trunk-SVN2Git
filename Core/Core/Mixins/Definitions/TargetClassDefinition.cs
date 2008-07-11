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
using Remotion.Mixins.Context;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{Type}")]
  public class TargetClassDefinition : ClassDefinitionBase, IAttributeIntroductionTargetDefinition
  {
    public readonly UniqueDefinitionCollection<Type, MixinDefinition> Mixins =
        new UniqueDefinitionCollection<Type, MixinDefinition> (delegate (MixinDefinition m) { return m.Type; });
    public readonly UniqueDefinitionCollection<Type, RequiredFaceTypeDefinition> RequiredFaceTypes =
        new UniqueDefinitionCollection<Type, RequiredFaceTypeDefinition> (delegate (RequiredFaceTypeDefinition t) { return t.Type; });
    public readonly UniqueDefinitionCollection<Type, RequiredBaseCallTypeDefinition> RequiredBaseCallTypes =
        new UniqueDefinitionCollection<Type, RequiredBaseCallTypeDefinition> (delegate (RequiredBaseCallTypeDefinition t) { return t.Type; });
    public readonly UniqueDefinitionCollection<Type, RequiredMixinTypeDefinition> RequiredMixinTypes =
        new UniqueDefinitionCollection<Type, RequiredMixinTypeDefinition> (delegate (RequiredMixinTypeDefinition t) { return t.Type; });
    public readonly UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> IntroducedInterfaces =
        new UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> (delegate (InterfaceIntroductionDefinition i) { return i.Type; });
    
    private readonly MultiDefinitionCollection<Type, AttributeIntroductionDefinition> _introducedAttributes =
        new MultiDefinitionCollection<Type, AttributeIntroductionDefinition> (delegate (AttributeIntroductionDefinition a) { return a.AttributeType; });
    private readonly MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> _suppressedIntroducedAttributes =
        new MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> (
          delegate (SuppressedAttributeIntroductionDefinition a) { return a.AttributeType; });

    private readonly MixinTypeInstantiator _mixinTypeInstantiator;
    private readonly ClassContext _configurationContext;

    public TargetClassDefinition (ClassContext configurationContext)
        : base (configurationContext.Type)
    {
      ArgumentUtility.CheckNotNull ("configurationContext", configurationContext);

      _configurationContext = configurationContext;
      _mixinTypeInstantiator = new MixinTypeInstantiator (configurationContext.Type);
    }

    public ClassContext ConfigurationContext
    {
      get { return _configurationContext; }
    }

    public MultiDefinitionCollection<Type, AttributeIntroductionDefinition> IntroducedAttributes
    {
      get { return _introducedAttributes; }
    }

    public MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> SuppressedIntroducedAttributes
    {
      get { return _suppressedIntroducedAttributes; }
    }

    internal MixinTypeInstantiator MixinTypeInstantiator
    {
      get { return _mixinTypeInstantiator; }
    }

    public bool IsInterface
    {
      get { return Type.IsInterface; }
    }

    public override IVisitableDefinition Parent
    {
      get { return null; }
    }

    protected override void ChildSpecificAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.Visit (this);
      
      Mixins.Accept (visitor);
      RequiredFaceTypes.Accept (visitor);
      RequiredBaseCallTypes.Accept (visitor);
      RequiredMixinTypes.Accept (visitor);
      IntroducedAttributes.Accept (visitor);
      SuppressedIntroducedAttributes.Accept (visitor);
    }

    public bool HasMixinWithConfiguredType(Type configuredType)
    {
      Type realType = _mixinTypeInstantiator.GetClosedMixinType (configuredType);
      return Mixins.ContainsKey (realType);
    }

    public MixinDefinition GetMixinByConfiguredType(Type configuredType)
    {
      Type realType = _mixinTypeInstantiator.GetClosedMixinType (configuredType);
      return Mixins[realType];
    }

    public IEnumerable<MethodDefinition> GetAllMixinMethods()
    {
      foreach (MixinDefinition mixin in Mixins)
        foreach (MethodDefinition method in mixin.Methods)
          yield return method;
    }

    public IEnumerable<PropertyDefinition> GetAllMixinProperties ()
    {
      foreach (MixinDefinition mixin in Mixins)
        foreach (PropertyDefinition property in mixin.Properties)
          yield return property;
    }

    public IEnumerable<EventDefinition> GetAllMixinEvents ()
    {
      foreach (MixinDefinition mixin in Mixins)
        foreach (EventDefinition eventDefinition in mixin.Events)
          yield return eventDefinition;
    }

    public bool IsAbstract
    {
      get { return Type.IsAbstract; }
    }
  }
}

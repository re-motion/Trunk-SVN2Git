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
  public class TargetClassDefinition : ClassDefinitionBase, IAttributeIntroductionTarget
  {
    private readonly UniqueDefinitionCollection<Type, MixinDefinition> _mixins =
        new UniqueDefinitionCollection<Type, MixinDefinition> (m => m.Type);
    private readonly UniqueDefinitionCollection<Type, RequiredFaceTypeDefinition> _requiredFaceTypes =
        new UniqueDefinitionCollection<Type, RequiredFaceTypeDefinition> (t => t.Type);
    private readonly UniqueDefinitionCollection<Type, RequiredBaseCallTypeDefinition> _requiredBaseCallTypes =
        new UniqueDefinitionCollection<Type, RequiredBaseCallTypeDefinition> (t => t.Type);
    private readonly UniqueDefinitionCollection<Type, RequiredMixinTypeDefinition> _requiredMixinTypes =
        new UniqueDefinitionCollection<Type, RequiredMixinTypeDefinition> (t => t.Type);
    
    private readonly UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> _receivedInterfaces =
        new UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> (i => i.InterfaceType);
    private readonly MultiDefinitionCollection<Type, AttributeIntroductionDefinition> _receivedAttributes;

    private readonly ClassContext _configurationContext;
    private readonly MixinTypeInstantiator _mixinTypeInstantiator;

    public TargetClassDefinition (ClassContext configurationContext)
        : base (configurationContext.Type)
    {
      ArgumentUtility.CheckNotNull ("configurationContext", configurationContext);

      _receivedAttributes = new MultiDefinitionCollection<Type, AttributeIntroductionDefinition> (a => a.AttributeType);

      _configurationContext = configurationContext;
      _mixinTypeInstantiator = new MixinTypeInstantiator (configurationContext.Type);
    }

    public MultiDefinitionCollection<Type, AttributeIntroductionDefinition> ReceivedAttributes
    {
      get { return _receivedAttributes; }
    }

    public ClassContext ConfigurationContext
    {
      get { return _configurationContext; }
    }

    internal MixinTypeInstantiator MixinTypeInstantiator
    {
      get { return _mixinTypeInstantiator; }
    }

    public bool IsInterface
    {
      get { return Type.IsInterface; }
    }

    public bool IsAbstract
    {
      get { return Type.IsAbstract; }
    }

    public override IVisitableDefinition Parent
    {
      get { return null; }
    }

    public UniqueDefinitionCollection<Type, MixinDefinition> Mixins
    {
      get { return _mixins; }
    }

    public UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> ReceivedInterfaces
    {
      get { return _receivedInterfaces; }
    }

    public UniqueDefinitionCollection<Type, RequiredMixinTypeDefinition> RequiredMixinTypes
    {
      get { return _requiredMixinTypes; }
    }

    public UniqueDefinitionCollection<Type, RequiredBaseCallTypeDefinition> RequiredBaseCallTypes
    {
      get { return _requiredBaseCallTypes; }
    }

    public UniqueDefinitionCollection<Type, RequiredFaceTypeDefinition> RequiredFaceTypes
    {
      get { return _requiredFaceTypes; }
    }

    protected override void ChildSpecificAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.Visit (this);
      
      _mixins.Accept (visitor);
      _requiredFaceTypes.Accept (visitor);
      _requiredBaseCallTypes.Accept (visitor);
      _requiredMixinTypes.Accept (visitor);
    }

    public bool HasMixinWithConfiguredType(Type configuredType)
    {
      Type realType = MixinTypeInstantiator.GetClosedMixinType (configuredType);
      return _mixins.ContainsKey (realType);
    }

    public MixinDefinition GetMixinByConfiguredType(Type configuredType)
    {
      Type realType = MixinTypeInstantiator.GetClosedMixinType (configuredType);
      return _mixins[realType];
    }

    public IEnumerable<MethodDefinition> GetAllMixinMethods()
    {
      foreach (MixinDefinition mixin in _mixins)
        foreach (MethodDefinition method in mixin.Methods)
          yield return method;
    }

    public IEnumerable<PropertyDefinition> GetAllMixinProperties ()
    {
      foreach (MixinDefinition mixin in _mixins)
        foreach (PropertyDefinition property in mixin.Properties)
          yield return property;
    }

    public IEnumerable<EventDefinition> GetAllMixinEvents ()
    {
      foreach (MixinDefinition mixin in _mixins)
        foreach (EventDefinition eventDefinition in mixin.Events)
          yield return eventDefinition;
    }
  }
}

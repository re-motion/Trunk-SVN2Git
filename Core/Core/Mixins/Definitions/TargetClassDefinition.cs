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
    public readonly UniqueDefinitionCollection<Type, MixinDefinition> Mixins =
        new UniqueDefinitionCollection<Type, MixinDefinition> (m => m.Type);
    public readonly UniqueDefinitionCollection<Type, RequiredFaceTypeDefinition> RequiredFaceTypes =
        new UniqueDefinitionCollection<Type, RequiredFaceTypeDefinition> (t => t.Type);
    public readonly UniqueDefinitionCollection<Type, RequiredBaseCallTypeDefinition> RequiredBaseCallTypes =
        new UniqueDefinitionCollection<Type, RequiredBaseCallTypeDefinition> (t => t.Type);
    public readonly UniqueDefinitionCollection<Type, RequiredMixinTypeDefinition> RequiredMixinTypes =
        new UniqueDefinitionCollection<Type, RequiredMixinTypeDefinition> (t => t.Type);
    public readonly UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> ReceivedInterfaces =
        new UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> (i => i.InterfaceType);

    public TargetClassDefinition (ClassContext configurationContext)
        : base (configurationContext.Type)
    {
      ArgumentUtility.CheckNotNull ("configurationContext", configurationContext);

      ReceivedAttributes = new MultiDefinitionCollection<Type, AttributeIntroductionDefinition> (a => a.AttributeType);

      ConfigurationContext = configurationContext;
      MixinTypeInstantiator = new MixinTypeInstantiator (configurationContext.Type);
    }

    public MultiDefinitionCollection<Type, AttributeIntroductionDefinition> ReceivedAttributes { get; private set; }

    public ClassContext ConfigurationContext { get; private set; }
    internal MixinTypeInstantiator MixinTypeInstantiator { get; private set; }

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

    protected override void ChildSpecificAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.Visit (this);
      
      Mixins.Accept (visitor);
      RequiredFaceTypes.Accept (visitor);
      RequiredBaseCallTypes.Accept (visitor);
      RequiredMixinTypes.Accept (visitor);
    }

    public bool HasMixinWithConfiguredType(Type configuredType)
    {
      Type realType = MixinTypeInstantiator.GetClosedMixinType (configuredType);
      return Mixins.ContainsKey (realType);
    }

    public MixinDefinition GetMixinByConfiguredType(Type configuredType)
    {
      Type realType = MixinTypeInstantiator.GetClosedMixinType (configuredType);
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
  }
}

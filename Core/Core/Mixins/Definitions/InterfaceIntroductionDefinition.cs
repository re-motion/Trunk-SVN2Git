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
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{_type.FullName} introduced via {_implementer.FullName}")]
  public class InterfaceIntroductionDefinition : IVisitableDefinition
  {
    public readonly UniqueDefinitionCollection<MethodInfo, MethodIntroductionDefinition> IntroducedMethods =
        new UniqueDefinitionCollection<MethodInfo, MethodIntroductionDefinition> (delegate (MethodIntroductionDefinition m) { return m.InterfaceMember; });
    public readonly UniqueDefinitionCollection<PropertyInfo, PropertyIntroductionDefinition> IntroducedProperties =
        new UniqueDefinitionCollection<PropertyInfo, PropertyIntroductionDefinition> (delegate (PropertyIntroductionDefinition m) { return m.InterfaceMember; });
    public readonly UniqueDefinitionCollection<EventInfo, EventIntroductionDefinition> IntroducedEvents =
        new UniqueDefinitionCollection<EventInfo, EventIntroductionDefinition> (delegate (EventIntroductionDefinition m) { return m.InterfaceMember; });

    private Type _type;
    private MixinDefinition _implementer;

    public InterfaceIntroductionDefinition (Type type, MixinDefinition implementer)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("implementer", implementer);

      _type = type;
      _implementer = implementer;
    }

    public Type Type
    {
      get { return _type; }
    }

    public MixinDefinition Implementer
    {
      get { return _implementer; }
    }

    public string FullName
    {
      get { return Type.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return Implementer; }
    }

    public TargetClassDefinition TargetClass
    {
      get { return Implementer.TargetClass; }
    }

    public IEnumerable<IMemberIntroductionDefinition> GetIntroducedMembers ()
    {
      foreach (MethodIntroductionDefinition introducedMethod in IntroducedMethods)
        yield return introducedMethod;
      foreach (PropertyIntroductionDefinition introducedProperty in IntroducedProperties)
        yield return introducedProperty;
      foreach (EventIntroductionDefinition introducedEvent in IntroducedEvents)
        yield return introducedEvent;
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
      IntroducedMethods.Accept (visitor);
      IntroducedProperties.Accept (visitor);
      IntroducedEvents.Accept (visitor);
    }
  }
}

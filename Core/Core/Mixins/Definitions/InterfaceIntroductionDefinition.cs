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
  [DebuggerDisplay ("{FullName} introduced via {Implementer.FullName}")]
  public class InterfaceIntroductionDefinition : IVisitableDefinition
  {
    private readonly UniqueDefinitionCollection<MethodInfo, MethodIntroductionDefinition> _introducedMethods =
        new UniqueDefinitionCollection<MethodInfo, MethodIntroductionDefinition> (m => m.InterfaceMember);
    private readonly UniqueDefinitionCollection<PropertyInfo, PropertyIntroductionDefinition> _introducedProperties =
        new UniqueDefinitionCollection<PropertyInfo, PropertyIntroductionDefinition> (m => m.InterfaceMember);
    private readonly UniqueDefinitionCollection<EventInfo, EventIntroductionDefinition> _introducedEvents =
        new UniqueDefinitionCollection<EventInfo, EventIntroductionDefinition> (m => m.InterfaceMember);

    public InterfaceIntroductionDefinition (Type type, MixinDefinition implementer)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("implementer", implementer);

      InterfaceType = type;
      Implementer = implementer;
    }

    public Type InterfaceType { get; private set; }
    public MixinDefinition Implementer { get; private set; }

    public string FullName
    {
      get { return InterfaceType.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return Implementer; }
    }

    public TargetClassDefinition TargetClass
    {
      get { return Implementer.TargetClass; }
    }

    public UniqueDefinitionCollection<EventInfo, EventIntroductionDefinition> IntroducedEvents
    {
      get { return _introducedEvents; }
    }

    public UniqueDefinitionCollection<PropertyInfo, PropertyIntroductionDefinition> IntroducedProperties
    {
      get { return _introducedProperties; }
    }

    public UniqueDefinitionCollection<MethodInfo, MethodIntroductionDefinition> IntroducedMethods
    {
      get { return _introducedMethods; }
    }

    public IEnumerable<IMemberIntroductionDefinition> GetIntroducedMembers ()
    {
      foreach (MethodIntroductionDefinition introducedMethod in _introducedMethods)
        yield return introducedMethod;
      foreach (PropertyIntroductionDefinition introducedProperty in _introducedProperties)
        yield return introducedProperty;
      foreach (EventIntroductionDefinition introducedEvent in _introducedEvents)
        yield return introducedEvent;
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
      _introducedMethods.Accept (visitor);
      _introducedProperties.Accept (visitor);
      _introducedEvents.Accept (visitor);
    }
  }
}

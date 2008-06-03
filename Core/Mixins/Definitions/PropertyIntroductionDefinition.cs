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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  public class PropertyIntroductionDefinition : MemberIntroductionDefinition<PropertyInfo, PropertyDefinition>
  {
    private bool _introducesGetMethod;
    private bool _introducesSetMethod;

    public PropertyIntroductionDefinition (InterfaceIntroductionDefinition declaringInterface, PropertyInfo interfaceMember, PropertyDefinition implementingMember)
        : base (declaringInterface, interfaceMember, implementingMember)
    {
      _introducesGetMethod = interfaceMember.GetGetMethod() != null;
      _introducesSetMethod = interfaceMember.GetSetMethod () != null;
    }

    public bool IntroducesGetMethod
    {
      get { return _introducesGetMethod; }
    }

    public bool IntroducesSetMethod
    {
      get { return _introducesSetMethod; }
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}

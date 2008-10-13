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
  public class MethodIntroductionDefinition : MemberIntroductionDefinitionBase<MethodInfo, MethodDefinition>
  {
    public MethodIntroductionDefinition (InterfaceIntroductionDefinition declaringInterface, MethodInfo interfaceMember, MethodDefinition implementingMember, MemberVisibility visibility)
        : base (declaringInterface, interfaceMember, implementingMember, visibility)
    {
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}

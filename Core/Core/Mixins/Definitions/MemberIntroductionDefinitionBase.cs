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
using System.Diagnostics;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{InterfaceMember}")]
  public abstract class MemberIntroductionDefinitionBase<TMemberInfo, TMemberDefinition>: IMemberIntroductionDefinition where TMemberInfo : MemberInfo
      where TMemberDefinition : MemberDefinitionBase
  {
    private readonly InterfaceIntroductionDefinition _declaringInterface;
    private readonly TMemberInfo _interfaceMember;
    private readonly TMemberDefinition _implementingMember;
    private readonly MemberVisibility _visibility;

    protected MemberIntroductionDefinitionBase (
        InterfaceIntroductionDefinition declaringInterface, TMemberInfo interfaceMember, TMemberDefinition implementingMember, MemberVisibility visibility)
    {
      ArgumentUtility.CheckNotNull ("interfaceMember", interfaceMember);
      ArgumentUtility.CheckNotNull ("declaringInterface", declaringInterface);
      ArgumentUtility.CheckNotNull ("implementingMember", implementingMember);

      _declaringInterface = declaringInterface;
      _implementingMember = implementingMember;
      _interfaceMember = interfaceMember;
      _visibility = visibility;
    }

    public InterfaceIntroductionDefinition DeclaringInterface
    {
      get { return _declaringInterface; }
    }

    public string Name
    {
      get { return InterfaceMember.Name; }
    }

    public TMemberInfo InterfaceMember
    {
      get { return _interfaceMember; }
    }

    public TMemberDefinition ImplementingMember
    {
      get { return _implementingMember; }
    }

    public string FullName
    {
      get { return DeclaringInterface.FullName + "." + InterfaceMember.Name; }
    }

    public IVisitableDefinition Parent
    {
      get { return DeclaringInterface; }
    }

    public MemberVisibility Visibility
    {
      get { return _visibility; }
    }

    public abstract void Accept (IDefinitionVisitor visitor);
  }
}

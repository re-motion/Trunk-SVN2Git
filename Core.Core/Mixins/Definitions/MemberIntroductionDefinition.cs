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
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{InterfaceMember}")]
  public abstract class MemberIntroductionDefinition<TMemberInfo, TMemberDefinition>: IVisitableDefinition
      where TMemberInfo : MemberInfo
      where TMemberDefinition : MemberDefinition
  {
    private InterfaceIntroductionDefinition _declaringInterface;
    private TMemberInfo _interfaceMember;
    private TMemberDefinition _implementingMember;

    public MemberIntroductionDefinition (
        InterfaceIntroductionDefinition declaringInterface, TMemberInfo interfaceMember, TMemberDefinition implementingMember)
    {
      ArgumentUtility.CheckNotNull ("interfaceMember", interfaceMember);
      ArgumentUtility.CheckNotNull ("declaringInterface", declaringInterface);
      ArgumentUtility.CheckNotNull ("implementingMember", implementingMember);

      _declaringInterface = declaringInterface;
      _implementingMember = implementingMember;
      _interfaceMember = interfaceMember;
    }

    public InterfaceIntroductionDefinition DeclaringInterface
    {
      get { return _declaringInterface; }
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

    public abstract void Accept (IDefinitionVisitor visitor);
  }
}

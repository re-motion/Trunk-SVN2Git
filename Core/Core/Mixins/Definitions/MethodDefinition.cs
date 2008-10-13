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
using Remotion.Mixins.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  public class MethodDefinition : MemberDefinitionBase
  {
    private static readonly SignatureChecker s_signatureChecker = new SignatureChecker ();

    private readonly UniqueDefinitionCollection<Type, MethodDefinition> _overrides =
        new UniqueDefinitionCollection<Type, MethodDefinition> (m => m.DeclaringClass.Type);

    private MethodDefinition _base;

    public MethodDefinition (MethodInfo memberInfo, ClassDefinitionBase declaringClass)
        : base (memberInfo, declaringClass)
    {
    }

    public MethodInfo MethodInfo
    {
      get { return (MethodInfo) MemberInfo; }
    }

    public override MemberDefinitionBase BaseAsMember
    {
      get { return _base; }
      protected internal set
      {
        if (value == null || value is MethodDefinition)
          _base = (MethodDefinition) value;
        else
          throw new ArgumentException ("Base must be MethodDefinition or null.", "value");
      }
    }

    public MethodDefinition Base
    {
      get { return _base; }
      set { BaseAsMember = value; }
    }

    public bool IsAbstract
    {
      get { return MethodInfo.IsAbstract; }
    }

    public new UniqueDefinitionCollection<Type, MethodDefinition> Overrides
    {
      get { return _overrides; }
    }

    protected override IDefinitionCollection<Type, MemberDefinitionBase> GetInternalOverridesWrapper()
    {
      return new CovariantDefinitionCollectionWrapper<Type, MethodDefinition, MemberDefinitionBase>(_overrides);
    }

    protected override bool IsSignatureCompatibleWith (MemberDefinitionBase overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);

      var overriderMethod = overrider as MethodDefinition;
      return overriderMethod != null && IsSignatureCompatibleWithMethod (overriderMethod);
    }

    private bool IsSignatureCompatibleWithMethod (MethodDefinition overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);
      return s_signatureChecker.MethodSignaturesMatch (MethodInfo, overrider.MethodInfo);
    }

    internal override void AddOverride (MemberDefinitionBase member)
    {
      ArgumentUtility.CheckNotNull ("member", member);

      var method = member as MethodDefinition;
      if (method == null)
      {
        string message = string.Format ("Member {0} cannot override method {1} - it is not a method.", member.FullName, FullName);
        throw new ArgumentException (message);
      }

      _overrides.Add (method);
    }

    protected override void ChildSpecificAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}

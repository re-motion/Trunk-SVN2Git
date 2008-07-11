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
  [DebuggerDisplay ("{MemberInfo}, DeclaringClass = {DeclaringClass.Type}")]
  public abstract class MemberDefinition : IAttributeIntroductionTargetDefinition, IVisitableDefinition
  {
    private readonly MemberInfo _memberInfo;
    private readonly ClassDefinitionBase _declaringClass;
    private IVisitableDefinition _parent;

    private readonly MultiDefinitionCollection<Type, AttributeDefinition> _customAttributes =
        new MultiDefinitionCollection<Type, AttributeDefinition> (delegate (AttributeDefinition a) { return a.AttributeType; });
    private readonly MultiDefinitionCollection<Type, AttributeIntroductionDefinition> _introducedAttributes =
        new MultiDefinitionCollection<Type, AttributeIntroductionDefinition> (delegate (AttributeIntroductionDefinition a) { return a.AttributeType; });
    private readonly MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> _suppressedIntroducedAttributes =
        new MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> (
        delegate (SuppressedAttributeIntroductionDefinition a) { return a.AttributeType; });


    private IDefinitionCollection<Type, MemberDefinition> _internalOverridesWrapper = null;

    public MemberDefinition (MemberInfo memberInfo, ClassDefinitionBase declaringClass)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);
      ArgumentUtility.CheckNotNull ("declaringClass", declaringClass);

      _memberInfo = memberInfo;
      _declaringClass = declaringClass;
      _parent = declaringClass;
    }

    public MemberInfo MemberInfo
    {
      get { return _memberInfo; }
    }

    public ClassDefinitionBase DeclaringClass
    {
      get { return _declaringClass; }
    }

    public MemberTypes MemberType
    {
      get { return MemberInfo.MemberType; }
    }

    public bool IsProperty
    {
      get { return MemberInfo.MemberType == MemberTypes.Property; }
    }

    public bool IsMethod
    {
      get { return MemberInfo.MemberType == MemberTypes.Method; }
    }

    public bool IsEvent
    {
      get { return MemberInfo.MemberType == MemberTypes.Event; }
    }

    public string Name
    {
      get { return MemberInfo.Name; }
    }

    public string FullName
    {
      get { return string.Format ("{0}.{1}", MemberInfo.DeclaringType.FullName, Name); }
    }

    public abstract MemberDefinition BaseAsMember { get; set; }

    public IVisitableDefinition Parent
    {
      get { return _parent; }
      internal set { _parent = ArgumentUtility.CheckNotNull ("value", value); }
    }

    public MultiDefinitionCollection<Type, AttributeDefinition> CustomAttributes
    {
      get { return _customAttributes; }
    }

    public ICustomAttributeProvider DeclaringEntity
    {
      get { return MemberInfo; }
    }

    public IDefinitionCollection<Type, MemberDefinition> Overrides
    {
      get
      {
        if (_internalOverridesWrapper == null)
        {
          _internalOverridesWrapper = GetInternalOverridesWrapper();
        }
        return _internalOverridesWrapper;
      }
    }

    public MultiDefinitionCollection<Type, AttributeIntroductionDefinition> IntroducedAttributes
    {
      get { return _introducedAttributes; }
    }

    public MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> SuppressedIntroducedAttributes
    {
      get { return _suppressedIntroducedAttributes; }
    }

    protected abstract IDefinitionCollection<Type, MemberDefinition> GetInternalOverridesWrapper();

    public virtual bool CanBeOverriddenBy (MemberDefinition overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);
      return MemberType == overrider.MemberType && IsSignatureCompatibleWith (overrider);
    }

    internal abstract void AddOverride (MemberDefinition member);

    protected abstract bool IsSignatureCompatibleWith (MemberDefinition overrider);

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      ChildSpecificAccept (visitor);

      _customAttributes.Accept (visitor);
      _introducedAttributes.Accept (visitor);

      Assertion.IsTrue (SuppressedIntroducedAttributes.Count == 0, "Must be updated once we support suppressing attributes on members");
    }

    protected abstract void ChildSpecificAccept (IDefinitionVisitor visitor);
  }
}

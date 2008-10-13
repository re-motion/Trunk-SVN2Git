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
  public class PropertyDefinition : MemberDefinitionBase
  {
    private static readonly SignatureChecker s_signatureChecker = new SignatureChecker();

    public new readonly UniqueDefinitionCollection<Type, PropertyDefinition> Overrides =
        new UniqueDefinitionCollection<Type, PropertyDefinition> (m => m.DeclaringClass.Type);

    private PropertyDefinition _base;
    private readonly MethodDefinition _getMethod;
    private readonly MethodDefinition _setMethod;

    public PropertyDefinition (PropertyInfo memberInfo, ClassDefinitionBase declaringClass, MethodDefinition getMethod, MethodDefinition setMethod)
        : base (memberInfo, declaringClass)
    {
      _getMethod = getMethod;
      _setMethod = setMethod;

      if (_getMethod != null)
        _getMethod.Parent = this;
      if (_setMethod != null)
        _setMethod.Parent = this;
    }

    public PropertyInfo PropertyInfo
    {
      get { return (PropertyInfo) MemberInfo; }
    }

    public MethodDefinition GetMethod
    {
      get { return _getMethod; }
    }

    public MethodDefinition SetMethod
    {
      get { return _setMethod; }
    }

    public override MemberDefinitionBase BaseAsMember
    {
      get { return _base; }
      protected internal set
      {
        if (value == null || value is PropertyDefinition)
        {
          _base = (PropertyDefinition) value;
          if (GetMethod != null)
            GetMethod.Base = _base == null ? null : _base.GetMethod;
          if (SetMethod != null)
            SetMethod.Base = _base == null ? null : _base.SetMethod;
        }
        else
          throw new ArgumentException ("Base must be PropertyDefinition or null.", "value");
      }
    }

    public PropertyDefinition Base
    {
      get { return _base; }
      set { BaseAsMember = value; }
    }

    protected override bool IsSignatureCompatibleWith (MemberDefinitionBase overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);

      var overriderProperty = overrider as PropertyDefinition;
      if (overriderProperty == null)
      {
        return false;
      }
      else
      {
        return IsSignatureCompatibleWithProperty (overriderProperty);
      }
    }

    private bool IsSignatureCompatibleWithProperty (PropertyDefinition overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);
      return s_signatureChecker.PropertySignaturesMatch (PropertyInfo, overrider.PropertyInfo);
    }

    internal override void AddOverride (MemberDefinitionBase member)
    {
      ArgumentUtility.CheckNotNull ("member", member);

      var overrider = member as PropertyDefinition;
      if (overrider == null)
      {
        string message = string.Format ("Member {0} cannot override property {1} - it is not a property.", member.FullName, FullName);
        throw new ArgumentException (message);
      }

      Overrides.Add (overrider);

      if (GetMethod != null && overrider.GetMethod != null)
        GetMethod.AddOverride (overrider.GetMethod);
      if (SetMethod != null && overrider.SetMethod != null)
        SetMethod.AddOverride (overrider.SetMethod);
    }

    protected override void ChildSpecificAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);

      if (GetMethod != null)
        GetMethod.Accept (visitor);
      if (SetMethod != null)
        SetMethod.Accept (visitor);
    }

    protected override IDefinitionCollection<Type, MemberDefinitionBase> GetInternalOverridesWrapper ()
    {
      return new CovariantDefinitionCollectionWrapper<Type, PropertyDefinition, MemberDefinitionBase> (Overrides);
    }
  }
}

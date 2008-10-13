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
  public class EventDefinition : MemberDefinitionBase
  {
    private static readonly SignatureChecker s_signatureChecker = new SignatureChecker();

    public new readonly UniqueDefinitionCollection<Type, EventDefinition> Overrides =
        new UniqueDefinitionCollection<Type, EventDefinition> (m => m.DeclaringClass.Type);

    private EventDefinition _base;
    private readonly MethodDefinition _addMethod;
    private readonly MethodDefinition _removeMethod;

    public EventDefinition (EventInfo memberInfo, ClassDefinitionBase declaringClass, MethodDefinition addMethod, MethodDefinition removeMethod)
        : base (memberInfo, declaringClass)
    {
      ArgumentUtility.CheckNotNull ("addMethod", addMethod);
      ArgumentUtility.CheckNotNull ("removeMethod", removeMethod);

      _addMethod = addMethod;
      _removeMethod = removeMethod;

      _addMethod.Parent = this;
      _removeMethod.Parent = this;
    }

    public EventInfo EventInfo
    {
      get { return (EventInfo) MemberInfo; }
    }

    public override MemberDefinitionBase BaseAsMember
    {
      get { return _base; }
      protected internal set
      {
        if (value == null || value is EventDefinition)
        {
          _base = (EventDefinition) value;
          AddMethod.Base = _base == null ? null : _base.AddMethod;
          RemoveMethod.Base = _base == null ? null : _base.RemoveMethod;
        }
        else
          throw new ArgumentException ("Base must be EventDefinition or null.", "value");
      }
    }

    public EventDefinition Base
    {
      get { return _base; }
      set { BaseAsMember = value; }
    }

    public MethodDefinition AddMethod
    {
      get { return _addMethod; }
    }

    public MethodDefinition RemoveMethod
    {
      get { return _removeMethod; }
    }

    protected override bool IsSignatureCompatibleWith (MemberDefinitionBase overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);

      var overriderEvent = overrider as EventDefinition;
      if (overriderEvent == null)
        return false;
      else
        return IsSignatureCompatibleWithEvent (overriderEvent);
    }

    private bool IsSignatureCompatibleWithEvent (EventDefinition overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);
      return s_signatureChecker.EventSignaturesMatch (EventInfo, overrider.EventInfo);
    }

    internal override void AddOverride (MemberDefinitionBase member)
    {
      ArgumentUtility.CheckNotNull ("member", member);

      var overrider = member as EventDefinition;
      if (overrider == null)
      {
        string message = string.Format ("Member {0} cannot override event {1} - it is not an event.", member.FullName, FullName);
        throw new ArgumentException (message);
      }

      Overrides.Add (overrider);

      AddMethod.AddOverride (overrider.AddMethod);
      RemoveMethod.AddOverride (overrider.RemoveMethod);
    }

    protected override void ChildSpecificAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);

      AddMethod.Accept (visitor);
      RemoveMethod.Accept (visitor);
    }

    protected override IDefinitionCollection<Type, MemberDefinitionBase> GetInternalOverridesWrapper ()
    {
      return new CovariantDefinitionCollectionWrapper<Type, EventDefinition, MemberDefinitionBase> (Overrides);
    }
  }
}

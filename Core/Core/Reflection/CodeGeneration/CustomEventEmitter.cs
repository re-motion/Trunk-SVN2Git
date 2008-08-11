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
using System.Reflection.Emit;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration
{
  public enum EventKind
  {
    Static,
    Instance
  }

  public class CustomEventEmitter : IAttributableEmitter
  {
    private readonly CustomClassEmitter _declaringType;
    private readonly EventBuilder _eventBuilder;

    private readonly string _name;
    private readonly EventKind _eventKind;
    private readonly Type _eventType;

    private CustomMethodEmitter _addMethod;
    private CustomMethodEmitter _removeMethod;

    public CustomEventEmitter (CustomClassEmitter declaringType, string name, EventKind eventKind, Type eventType, EventAttributes attributes)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("eventType", eventType);

      _declaringType = declaringType;
      _eventBuilder = declaringType.TypeBuilder.DefineEvent (name, attributes, eventType);
      _name = name;
      _eventKind = eventKind;
      _eventType = eventType;
      declaringType.RegisterEventEmitter (this);
    }

    public CustomMethodEmitter AddMethod
    {
      get
      {
        if (_addMethod == null)
          CreateAddMethod ();

        return _addMethod;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException ("value", "Event accessors cannot be set to null.");

        if (_addMethod != null)
          throw new InvalidOperationException ("Add methods can only be assigned once.");

        _addMethod = value;
        _eventBuilder.SetAddOnMethod (_addMethod.MethodBuilder);
      }
    }

    public CustomMethodEmitter RemoveMethod
    {
      get
      {
        if (_removeMethod == null)
          CreateRemoveMethod ();

        return _removeMethod;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException ("value", "Event accessors cannot be set to null.");

        if (_removeMethod != null)
          throw new InvalidOperationException ("Remove methods can only be assigned once.");

        _removeMethod = value;
        _eventBuilder.SetRemoveOnMethod (_removeMethod.MethodBuilder);
      }
    }

    public string Name
    {
      get { return _name; }
    }

    public Type EventType
    {
      get { return _eventType; }
    }

    public EventKind EventKind
    {
      get { return _eventKind; }
    }

    public CustomClassEmitter DeclaringType
    {
      get { return _declaringType; }
    }

    public EventBuilder EventBuilder
    {
      get { return _eventBuilder; }
    }

    private void CreateAddMethod ()
    {
      Assertion.IsNull (_addMethod);

      MethodAttributes flags = MethodAttributes.Public | MethodAttributes.SpecialName;
      if (EventKind == EventKind.Static)
        flags |= MethodAttributes.Static;
      CustomMethodEmitter method = _declaringType.CreateMethod ("add_" + Name, flags);
      method.SetParameterTypes (new Type[] { EventType });
      AddMethod = method;
    }

    private void CreateRemoveMethod ()
    {
      Assertion.IsNull (_removeMethod);

      MethodAttributes flags = MethodAttributes.Public | MethodAttributes.SpecialName;
      if (EventKind == EventKind.Static)
        flags |= MethodAttributes.Static;
      CustomMethodEmitter method = _declaringType.CreateMethod ("remove_" + Name, flags);
      method.SetParameterTypes (new Type[] { EventType });
      RemoveMethod = method;
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      _eventBuilder.SetCustomAttribute (customAttribute);
    }

    internal void EnsureValid ()
    {
      CustomMethodEmitter addMethod = AddMethod; // cause generation of default method if none has been assigned
      Assertion.IsNotNull (addMethod);

      CustomMethodEmitter removeMethod = RemoveMethod; // cause generation of default method if none has been assigned
      Assertion.IsNotNull (removeMethod);
    }
  }
}

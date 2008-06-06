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
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class PropertyReference : TypeReference
  {
    private readonly PropertyInfo _property;

    public PropertyReference (PropertyInfo property)
        : base (SelfReference.Self, property.PropertyType)
    {
      _property = property;
    }

    public PropertyReference(Reference owner, PropertyInfo property)
        : base (owner, property.PropertyType)
    {
      _property = property;
    }

    public PropertyInfo Reference
    {
      get { return _property; }
    }

    public override void LoadAddressOfReference (ILGenerator gen)
    {
      throw new NotSupportedException ("A property's address cannot be loaded.");
    }

    public override void LoadReference (ILGenerator gen)
    {
      MethodInfo getMethod = Reference.GetGetMethod(true);
      if (getMethod == null)
      {
        string message = string.Format("The property {0}.{1} cannot be loaded, it has no getter.", Reference.DeclaringType.FullName, Reference.Name);
        throw new InvalidOperationException (message);
      }
      if (getMethod.IsStatic)
      {
        gen.EmitCall (OpCodes.Call, getMethod, null);
      }
      else
      {
        gen.EmitCall (OpCodes.Callvirt, getMethod, null);
      }
    }

    public override void StoreReference (ILGenerator gen)
    {
      MethodInfo setMethod = Reference.GetSetMethod (true);
      if (setMethod == null)
      {
        string message = string.Format ("The property {0}.{1} cannot be stored, it has no setter.", Reference.DeclaringType.FullName, Reference.Name);
        throw new InvalidOperationException (message);
      }
      if (setMethod.IsStatic)
      {
        gen.EmitCall (OpCodes.Call, setMethod, null);
      }
      else
      {
        gen.EmitCall (OpCodes.Callvirt, setMethod, null);
      }
    }
  }
}

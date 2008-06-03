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
  public class FieldInfoReference : TypeReference
  {
    private readonly FieldInfo _field;

    public FieldInfoReference (Reference owner, FieldInfo field)
        : base (owner, field.FieldType)
    {
      _field = field;
    }

    public override void LoadAddressOfReference (ILGenerator gen)
    {
      if (IsStaticField)
        gen.Emit (OpCodes.Ldsflda, _field);
      else
        gen.Emit (OpCodes.Ldflda, _field);
    }

    public override void LoadReference (ILGenerator gen)
    {
      if (IsStaticField)
        gen.Emit (OpCodes.Ldsfld, _field);
      else
        gen.Emit (OpCodes.Ldfld, _field);
    }

    public override void StoreReference (ILGenerator gen)
    {
      if (IsStaticField)
        gen.Emit (OpCodes.Stsfld, _field);
      else
        gen.Emit (OpCodes.Stfld, _field);
    }

    private bool IsStaticField
    {
      get { return _field.IsStatic; }
    }
  }
}

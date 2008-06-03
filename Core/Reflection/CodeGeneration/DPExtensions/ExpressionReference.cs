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
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Generators.Emitters;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  // Converts an expression to a reference by saving it as a temporary local variable at time of emitting
  public class ExpressionReference : TypeReference
  {
    private readonly Expression _expression;
    private readonly MethodEmitter _methodEmitter;
    private readonly Type _referenceType;

    public ExpressionReference (Type referenceType, Expression expression, MethodEmitter methodEmitter) : base (referenceType)
    {
      _referenceType = referenceType;
      _expression = expression;
      _methodEmitter = methodEmitter;
    }

    public ExpressionReference (Type referenceType, Expression expression, CustomMethodEmitter methodEmitter)
      : this (referenceType, expression, methodEmitter.InnerEmitter)
    {
    }

    public override void LoadAddressOfReference (ILGenerator gen)
    {
      LocalReference local = CreateLocal (gen);
      local.LoadAddressOfReference (gen);
    }

    public override void LoadReference (ILGenerator gen)
    {
      LocalReference local = CreateLocal(gen);
      local.LoadReference (gen);
    }

    private LocalReference CreateLocal (ILGenerator gen)
    {
      LocalReference local = _methodEmitter.CodeBuilder.DeclareLocal (_referenceType);
      local.Generate (gen);
      new AssignStatement (local, _expression).Emit (_methodEmitter, gen);
      return local;
    }

    public override void StoreReference (ILGenerator gen)
    {
      throw new NotSupportedException ("Expressions cannot be assigned to.");
    }
  }
}

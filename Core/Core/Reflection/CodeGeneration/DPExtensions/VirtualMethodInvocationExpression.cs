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
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class VirtualMethodInvocationExpression : TypedMethodInvocationExpression
  {
    public VirtualMethodInvocationExpression (TypeReference owner, MethodInfo method, params Expression[] arguments)
        : base (owner, method, arguments)
    {
    }

    protected override void EmitCall (IMemberEmitter member, ILGenerator gen)
    {
      gen.Emit (OpCodes.Callvirt, Method);
    }
  }
}

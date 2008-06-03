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
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class SameConditionExpression : ConditionExpression
  {
    private Expression _left;
    private Expression _right;

    public SameConditionExpression(Expression left, Expression right)
    {
      _left = left;
      _right = right;
    }

    public override OpCode BranchIfTrue
    {
      get { return OpCodes.Beq; }
    }

    public override OpCode BranchIfFalse
    {
      get { return OpCodes.Bne_Un; }
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      _left.Emit (member, gen);
      _right.Emit (member, gen);
    }
  }
}

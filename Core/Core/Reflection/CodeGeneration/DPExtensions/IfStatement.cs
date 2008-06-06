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
  public class IfStatement : Statement
  {
    private ConditionExpression _condition;
    private Statement[] _thenStatements;

    public IfStatement (ConditionExpression condition, params Statement[] thenStatements)
    {
      _condition = condition;
      _thenStatements = thenStatements;
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      Label elseTarget = gen.DefineLabel ();
      _condition.Emit (member, gen);
      gen.Emit (_condition.BranchIfFalse, elseTarget);
      foreach (Statement s in _thenStatements)
        s.Emit (member, gen);
      gen.MarkLabel (elseTarget);
    }
  }
}

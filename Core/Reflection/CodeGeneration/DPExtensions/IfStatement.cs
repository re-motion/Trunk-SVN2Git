using System;
using System.Collections.Generic;
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

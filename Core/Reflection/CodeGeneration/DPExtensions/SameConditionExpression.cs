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

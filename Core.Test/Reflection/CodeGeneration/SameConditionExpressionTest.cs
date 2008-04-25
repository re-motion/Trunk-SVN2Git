using System;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class SameConditionExpressionTest : SnippetGenerationBaseTest
  {
    private class TestStatement : Statement
    {
      private readonly ConditionExpression _expression;

      public TestStatement (ConditionExpression expression)
      {
        _expression = expression;
      }

      public override void Emit (IMemberEmitter member, ILGenerator gen)
      {
        Label trueLabel = gen.DefineLabel ();
        Label falseLabel = gen.DefineLabel ();
        _expression.Emit (member, gen);
        gen.Emit (_expression.BranchIfTrue, trueLabel);
        _expression.Emit (member, gen);
        gen.Emit (_expression.BranchIfFalse, falseLabel);
        gen.Emit (OpCodes.Ldstr, "No label selected");
        gen.Emit (OpCodes.Ret);
        gen.MarkLabel (trueLabel);
        gen.Emit (OpCodes.Ldstr, "True");
        gen.Emit (OpCodes.Ret);
        gen.MarkLabel (falseLabel);
        gen.Emit (OpCodes.Ldstr, "False");
        gen.Emit (OpCodes.Ret);
      }
    }

    [Test]
    public void SameConditionTrue ()
    {
      CustomMethodEmitter methodEmitter = GetMethodEmitter (false);
      methodEmitter.SetReturnType (typeof (string));

      methodEmitter.AddStatement (new TestStatement (
          new SameConditionExpression (new TypeTokenExpression (typeof (object)), new TypeTokenExpression (typeof (object)))));

      Assert.AreEqual ("True", InvokeMethod());
    }

    [Test]
    public void SameConditionFalse ()
    {
      CustomMethodEmitter methodEmitter = GetMethodEmitter (false);
      methodEmitter.SetReturnType (typeof (string));

      methodEmitter.AddStatement (new TestStatement (
          new SameConditionExpression (new TypeTokenExpression (typeof (string)), new TypeTokenExpression (typeof (object)))));

      Assert.AreEqual ("False", InvokeMethod ());
    }
  }
}
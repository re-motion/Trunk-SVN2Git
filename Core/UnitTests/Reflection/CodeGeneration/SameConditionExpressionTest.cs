// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
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

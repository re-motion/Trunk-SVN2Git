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
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class IfStatementTest : SnippetGenerationBaseTest
  {
    [Test]
    public void IfWithTrueCondition ()
    {
      CustomMethodEmitter methodEmitter = GetMethodEmitter (false);
      methodEmitter.SetReturnType (typeof (string));
      methodEmitter.AddStatement (new IfStatement (new SameConditionExpression (NullExpression.Instance, NullExpression.Instance),
          new ReturnStatement (new ConstReference ("True"))));
      methodEmitter.AddStatement (new ReturnStatement (new ConstReference ("False")));

      Assert.AreEqual ("True", InvokeMethod());
    }

    [Test]
    public void FalseCondition ()
    {
      CustomMethodEmitter methodEmitter = GetMethodEmitter (false);
      methodEmitter.SetReturnType (typeof (string));
      methodEmitter.AddStatement (new IfStatement (new SameConditionExpression (NullExpression.Instance, new ConstReference ("5").ToExpression()),
          new ReturnStatement (new ConstReference ("True"))));
      methodEmitter.AddStatement (new ReturnStatement (new ConstReference ("False")));

      Assert.AreEqual ("False", InvokeMethod ());
    }
  }
}

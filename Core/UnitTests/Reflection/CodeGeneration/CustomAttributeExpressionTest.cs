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
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes;
using Remotion.Utilities;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class CustomAttributeExpressionTest : SnippetGenerationBaseTest
  {
    [Test]
    public void CustomAttributeExpression ()
    {
      CustomMethodEmitter methodEmitter = GetMethodEmitter(false)
          .SetReturnType (typeof (Tuple<SimpleAttribute, SimpleAttribute>));

      LocalReference attributeOwner = methodEmitter.DeclareLocal (typeof (Type));
      methodEmitter.AddStatement (new AssignStatement (attributeOwner, new TypeTokenExpression (typeof (ClassWithCustomAttribute))));

      ConstructorInfo tupleCtor =
          typeof (Tuple<SimpleAttribute, SimpleAttribute>).GetConstructor (new Type[] {typeof (SimpleAttribute), typeof (SimpleAttribute)});
      Expression tupleExpression = new NewInstanceExpression (tupleCtor,
          new CustomAttributeExpression (attributeOwner, typeof (SimpleAttribute), 0, true),
          new CustomAttributeExpression (attributeOwner, typeof (SimpleAttribute), 1, true));

      methodEmitter.AddStatement (new ReturnStatement (tupleExpression));

      object[] attributes = typeof (ClassWithCustomAttribute).GetCustomAttributes (typeof (SimpleAttribute), true);

      Tuple<SimpleAttribute, SimpleAttribute> attributeTuple = (Tuple<SimpleAttribute, SimpleAttribute>) InvokeMethod();
      Assert.That (new object[] { attributeTuple.A, attributeTuple.B }, Is.EquivalentTo (attributes));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = "Argument attributeOwner is a System.String, which cannot be assigned "
        + "to type System.Reflection.ICustomAttributeProvider.\r\nParameter name: attributeOwner")]
    public void CustomAttributeExpressionThrowsOnWrongReferenceType ()
    {
      new CustomAttributeExpression (new LocalReference (typeof (string)), typeof (SimpleAttribute), 0, true);
    }
  }
}

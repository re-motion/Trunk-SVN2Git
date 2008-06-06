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
using Remotion.Collections;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class TypedMethodInvocationExpressionTest : SnippetGenerationBaseTest
  {
    public class ReferenceType
    {
      public string Method ()
      {
        return "ReferenceTypeMethod";
      }

      public Tuple<int, string> MethodWithArgs (int i, string s)
      {
        return new Tuple<int, string> (i, s);
      }
    }

    public struct ValueType
    {
      public string Method ()
      {
        return "ValueTypeMethod";
      }
    }

    [Test]
    public void TypedMethodInvocationMethodProperty ()
    {
      SuppressAssemblySave ();

      CustomMethodEmitter method = GetMethodEmitter (false);
      method.SetReturnType (typeof (string));
      Expression newObject = new NewInstanceExpression (typeof (ReferenceType), Type.EmptyTypes);
      ExpressionReference newObjectReference = new ExpressionReference (typeof (ReferenceType), newObject, method);

      TypedMethodInvocationExpression expression =
          new TypedMethodInvocationExpression (newObjectReference, typeof (ReferenceType).GetMethod ("Method"));
      Assert.AreEqual (typeof (ReferenceType).GetMethod ("Method"), expression.Method);
    }

    [Test]
    public void TypedMethodInvocationOnReferenceType ()
    {
      CustomMethodEmitter method = GetMethodEmitter (false);
      method.SetReturnType (typeof (string));
      Expression newObject = new NewInstanceExpression (typeof (ReferenceType), Type.EmptyTypes);
      ExpressionReference newObjectReference = new ExpressionReference (typeof (ReferenceType), newObject, method);
      method.ImplementByReturning (new TypedMethodInvocationExpression (newObjectReference,
          typeof (ReferenceType).GetMethod ("Method")));

      Assert.AreEqual ("ReferenceTypeMethod", InvokeMethod ());
    }

    [Test]
    public void TypedMethodInvocationOnValueType ()
    {
      CustomMethodEmitter method = GetMethodEmitter (false);
      method.SetReturnType (typeof (string));
      Expression newObject = new InitObjectExpression (method, typeof (ValueType));
      ExpressionReference newObjectReference = new ExpressionReference (typeof (ValueType), newObject, method);
      method.ImplementByReturning (new TypedMethodInvocationExpression (newObjectReference,
          typeof (ValueType).GetMethod ("Method")));

      Assert.AreEqual ("ValueTypeMethod", InvokeMethod ());
    }

    [Test]
    public void TypedMethodInvocationWithComplexOwner ()
    {
      FieldReference fieldReference = ClassEmitter.CreateField ("CallTarget", typeof (ReferenceType));
      FieldInfoReference fieldInfoReference = new FieldInfoReference (SelfReference.Self, fieldReference.Reference);

      CustomMethodEmitter method = GetMethodEmitter (false);
      method.SetReturnType (typeof (string));

      method.AddStatement (new AssignStatement (fieldReference, new NewInstanceExpression (typeof (ReferenceType), Type.EmptyTypes)));

      method.ImplementByReturning (new TypedMethodInvocationExpression (fieldInfoReference, typeof (ReferenceType).GetMethod ("Method")));

      Assert.AreEqual ("ReferenceTypeMethod", InvokeMethod ());
    }

    [Test]
    public void TypedMethodInvocationWithArguments ()
    {
      FieldReference fieldReference = ClassEmitter.CreateField ("CallTarget", typeof (ReferenceType));
      FieldInfoReference fieldInfoReference = new FieldInfoReference (SelfReference.Self, fieldReference.Reference);

      CustomMethodEmitter method = GetMethodEmitter (false);
      method.SetReturnType (typeof (Tuple<int, string>));

      method.AddStatement (new AssignStatement (fieldReference, new NewInstanceExpression (typeof (ReferenceType), Type.EmptyTypes)));

      method.ImplementByReturning (new TypedMethodInvocationExpression (fieldInfoReference, typeof (ReferenceType).GetMethod ("MethodWithArgs"),
        new ConstReference (1).ToExpression(), new ConstReference ("2").ToExpression()));

      Assert.AreEqual (new Tuple<int, string> (1, "2"), InvokeMethod ());
    }
  }
}

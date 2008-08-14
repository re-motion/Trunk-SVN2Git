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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.Parsing;
using System.Collections.Generic;

namespace Remotion.Data.UnitTests.Linq.DataObjectModelTest
{
  [TestFixture]
  public class MethodCallTest
  {
    [Test]
    public void ToString_NullObject ()
    {
      MethodInfo method = ParserUtility.GetMethod (() => StaticMethodWithoutArguments());
      IEvaluation evaluationObject = null;
      List<IEvaluation> evaluationArguments = new List<IEvaluation>();
      MethodCall methodCall = new MethodCall (method, evaluationObject, evaluationArguments);
      Assert.That (methodCall.ToString(), Is.EqualTo("StaticMethodWithoutArguments()"));
    }

    [Test]
    public void ToString_NonNullObject ()
    {
      MethodInfo method = ParserUtility.GetMethod (() => InstanceMethodWithoutArguments ());
      IEvaluation evaluationObject = new Constant("abc");
      List<IEvaluation> evaluationArguments = new List<IEvaluation> ();
      MethodCall methodCall = new MethodCall (method, evaluationObject, evaluationArguments);
      Assert.That (methodCall.ToString (), Is.EqualTo ("abc.InstanceMethodWithoutArguments()"));
    }

    [Test]
    public void ToString_Arguments ()
    {
      MethodInfo method = ParserUtility.GetMethod (() => StaticMethodWithArguments (1, "2"));
      IEvaluation evaluationObject = null;
      List<IEvaluation> evaluationArguments = new List<IEvaluation> { new Constant(10), new Constant("Text") };
      MethodCall methodCall = new MethodCall (method, evaluationObject, evaluationArguments);
      Assert.That (methodCall.ToString (), Is.EqualTo ("StaticMethodWithArguments(10, Text)"));
    }

    [Test]
    public void Equals_True ()
    {
      MethodInfo method = ParserUtility.GetMethod (() => StaticMethodWithArguments (1, "2"));
      IEvaluation evaluationObject = new Constant ("abc");
      List<IEvaluation> evaluationArguments = new List<IEvaluation> { new Constant (10), new Constant ("Text") };
      MethodCall m1 = new MethodCall (method, evaluationObject, evaluationArguments);
      MethodCall m2 = new MethodCall (method, evaluationObject, evaluationArguments);

      Assert.That (m1, Is.EqualTo (m2));
    }

    [Test]
    public void Equals_False_Null ()
    {
      MethodInfo method = ParserUtility.GetMethod (() => StaticMethodWithArguments (1, "2"));
      IEvaluation evaluationObject = new Constant ("abc");
      List<IEvaluation> evaluationArguments = new List<IEvaluation> { new Constant (10), new Constant ("Text") };
      MethodCall m1 = new MethodCall (method, evaluationObject, evaluationArguments);

      Assert.That (m1, Is.Not.EqualTo (null));
    }

    [Test]
    public void Equals_False_ObjectDifferent ()
    {
      MethodInfo method = ParserUtility.GetMethod (() => StaticMethodWithArguments (1, "2"));
      IEvaluation evaluationObject1 = new Constant ("abc");
      IEvaluation evaluationObject2 = new Constant ("abcd");
      List<IEvaluation> evaluationArguments = new List<IEvaluation> { new Constant (10), new Constant ("Text") };
      MethodCall m1 = new MethodCall (method, evaluationObject1, evaluationArguments);
      MethodCall m2 = new MethodCall (method, evaluationObject2, evaluationArguments);

      Assert.That (m1, Is.Not.EqualTo (m2));
    }

    [Test]
    public void Equals_False_ArgumentsDifferent ()
    {
      MethodInfo method = ParserUtility.GetMethod (() => StaticMethodWithArguments (1, "2"));
      IEvaluation evaluationObject = new Constant ("abc");
      List<IEvaluation> evaluationArguments1 = new List<IEvaluation> { new Constant (10), new Constant ("Text") };
      List<IEvaluation> evaluationArguments2 = new List<IEvaluation> { new Constant (10), new Constant ("Text2") };
      MethodCall m1 = new MethodCall (method, evaluationObject, evaluationArguments1);
      MethodCall m2 = new MethodCall (method, evaluationObject, evaluationArguments2);

      Assert.That (m1, Is.Not.EqualTo (m2));
    }

    [Test]
    public void Equals_False_MethodDifferent ()
    {
      MethodInfo method1 = ParserUtility.GetMethod (() => StaticMethodWithArguments (1, "2"));
      MethodInfo method2 = ParserUtility.GetMethod (() => StaticMethodWithoutArguments ());
      IEvaluation evaluationObject = new Constant ("abc");
      List<IEvaluation> evaluationArguments = new List<IEvaluation> { new Constant (10), new Constant ("Text") };
      MethodCall m1 = new MethodCall (method1, evaluationObject, evaluationArguments);
      MethodCall m2 = new MethodCall (method2, evaluationObject, evaluationArguments);

      Assert.That (m1, Is.Not.EqualTo (m2));
    }

    [Test]
    public void GetHashCode_Equal ()
    {
      MethodInfo method = ParserUtility.GetMethod (() => StaticMethodWithArguments (1, "2"));
      IEvaluation evaluationObject = new Constant ("abc");
      List<IEvaluation> evaluationArguments = new List<IEvaluation> { new Constant (10), new Constant ("Text") };
      MethodCall m1 = new MethodCall (method, evaluationObject, evaluationArguments);
      MethodCall m2 = new MethodCall (method, evaluationObject, evaluationArguments);

      Assert.That (m1.GetHashCode (), Is.EqualTo (m2.GetHashCode ()));
    }

    public static int StaticMethodWithoutArguments () 
    {
      return 0;
    }

    public int InstanceMethodWithoutArguments () 
    {
      return 0; 
    }

    public static int StaticMethodWithArguments (int i, string j)
    {
      return 0;
    }
  }
}
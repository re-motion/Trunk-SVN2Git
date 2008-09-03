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
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics;

namespace Remotion.UnitTests.Diagnostics
{
  [TestFixture]
  public class ToTest
  {
    // TODO: Store & replace the ToTextProvider To.Text uses and restore it after the tests.

    public class Test
    {
      public Test ()
      {
        Name = "DefaultName";
        Int = 1234567;
      }

      public Test (string name, int i0)
      {
        Name = name;
        Int = i0;
        ListListString = new List<List<string>>();
      }

      public string Name { get; set; }
      public int Int { get; set; }
      public LinkedList<string> LinkedListString { get; set; }
      public List<List<string>> ListListString { get; set; }
      public Object[][][] Array3D { get; set; }
      public Object[,] RectangularArray2D { get; set; }
      public Object[,,] RectangularArray3D { get; set; }
    }

    public class Test2
    {
      public Test2 ()
      {
        Name = "DefaultName";
        Int = 1234567;
      }

      public Test2 (string name, int i0)
      {
        Name = name;
        Int = i0;
        ListListString = new List<List<string>>();
      }

      public string Name { get; set; }
      public int Int { get; set; }
      public LinkedList<string> LinkedListString { get; set; }
      public List<List<string>> ListListString { get; set; }
      public Object[][][] Array3D { get; set; }
      public Object[,] RectangularArray2D { get; set; }
      public Object[,,] RectangularArray3D { get; set; }
    }


    [Test]
    [Ignore]
    public void ObjectTest ()
    {
      Object o = 5711;
      Assert.That (To.Text (o), Is.EqualTo (o.ToString()));

      Object o2 = new object();
      Assert.That (To.Text (o2), Is.EqualTo (o2.ToString()));
    }

    [Test]
    public void FallbackToStringTest ()
    {
      //FallbackToStringTestSingleType ("abcd EFGH");
      FallbackToStringTestSingleType (87971132);
      //FallbackToStringTestSingleType (4786.5323);
      int i = 8723;
      FallbackToStringTestSingleType (i);
    }

    private void FallbackToStringTestSingleType<T> (T t)
    {
      Assert.That (To.Text (t), Is.EqualTo (t.ToString()));
    }

    private void RegisterHandlers ()
    {
      To.RegisterHandler<Int32> ((x, ttb) => ttb.sb ("[Int32: ", "", ",", "", "]").ts (x).se());
      To.RegisterHandler<Test> ((x, ttb) => ttb.sb ("<<Test: ", "", ";", "", ">>").m ("Name", x.Name).m ("Int", x.Int).se());
    }


    [Test]
    //[Ignore]
    public void RegisteredHandlerTest ()
    {
      To.ClearHandlers();
      RegisterHandlers();
      //int i = 34567; 
      //string toTextO = To.Text (i); 
      //Log ("toTextO=" + toTextO);
      //Assert.That (toTextO, Is.EqualTo (String.Format ("<<Object: {0}>>", i.ToString ())));

      var test = new ToTest.Test ("That's not my name", 179);
      string toTextTest = To.Text (test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("<<Test: Name=\"That's not my name\";Int=[Int32: 179]>>"));
    }

    [Test]
    public void NullTest ()
    {
      To.ClearHandlers();
      Object o = null;
      Assert.That (To.Text (o), Is.EqualTo ("null"));
    }

    [Test]
    public void IntToStringFallbackTest ()
    {
      To.ClearHandlers();
      int i = 908;
      Assert.That (To.Text (i), Is.EqualTo ("908"));
    }


    [Test]
    public void InitStandardHandlersTest ()
    {
      To.ClearHandlers();
      To.TextEnableAutomatics (true);
      Assert.That (To.Text ("Some text"), Is.EqualTo ("\"Some text\""));
      Assert.That (To.Text ('x'), Is.EqualTo ("'x'"));
    }


    //[Test]
    //public void ClearHandlersTest ()
    //{
    //  To.RegisterSpecificTypeHandler<Object> (x => "[ClearHandlersTest]" + x);
    //  Object o = new object ();
    //  string toTextTest = To.Text (o);
    //  Log ("[ClearHandlersTest] toTextTest=" + toTextTest);
    //  Assert.That (To.Text (o), Is.EqualTo ("[ClearHandlersTest]" + o));
    //  To.ClearSpecificTypeHandlers();
    //  Assert.That (To.Text (o), Is.EqualTo (o.ToString()));
    //}


    //[Test]
    //public void GetTypeHandlersTest ()
    //{
    //  var handlerMap = To.GetTypeHandlers();
    //  foreach (var pair in handlerMap)
    //  {
    //    Log (pair.Key + " " + pair.Value);
    //  }
    //  //Assert.That (To.Text ('x'), Is.EqualTo ("'x'"));
    //}

    [Test]
    public void LogStreamVariableTest ()
    {
      var arbitraryVariableName = "Me be a string...";
      var arbitraryVariableName2 = 123.456;
      var obj = new Object();
      LogStreamVariable (abrakadabra => arbitraryVariableName);
      LogStreamVariable (abrakadabra => arbitraryVariableName2);
      LogStreamVariable (abrakadabra => obj);
    }


    [Test]
    public void StreamVariableTest ()
    {
      var arbitraryVariableName = "Me be a string...";
      var arbitraryVariableName2 = 123.456;
      var obj = new Object ();
      Log (StreamVariable (lambdamagic => arbitraryVariableName2));
      Log (StreamVariable (abrakadabra => arbitraryVariableName));
      Log (StreamVariable (x => obj));
    }


    public static string RightUntilChar (string s, char separator)
    {
      int iSeparator = s.LastIndexOf (separator);
      if (iSeparator > 0)
      {
        return s.Substring (iSeparator + 1, s.Length - iSeparator - 1);
      }
      else
      {
        return s;
      }
    }


    private void LogStreamVariable<T> (Expression<Func<object, T>> expression)
    {
      Log ("\nStreamVariable:" + typeof (T));
      Log (expression.Body.ToString());
      Log (RightUntilChar (expression.Body.ToString (), '.'));
      Log (expression.NodeType.ToString ());
      Log (expression.Type.ToString ());
      Log (expression.Body.NodeType.ToString());
      //Log (expression.Body.ToString().);
      foreach (var parameter in expression.Parameters)
      {
        Log (parameter.Name);
      }
      //Log( ((T) ((ConstantExpression) expression.Body).Value).ToString());
      Log (expression.Body.ToString());
      //Log (expression.ToString());
      Log (expression.Compile().Invoke(null).ToString ());
    }

    private string StreamVariable<T> (Expression<Func<object, T>> expression)
    {
      Assert.That (expression.Parameters.Count == 1);
      var name = expression.Parameters[0].Name;
      Assert.That(name == "x" || name == "abrakadabra" || name == "lambdamagic");
      return RightUntilChar (expression.Body.ToString(), '.') + "=" + expression.Compile ().Invoke (null);
    }




    public static void Log (string s)
    {
      Console.WriteLine (s);
    }

    public static void LogVariables (string format, params object[] parameterArray)
    {
      Log (String.Format (format, parameterArray));
    }
  }
}
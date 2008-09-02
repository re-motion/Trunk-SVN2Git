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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Logging;
using Remotion.Diagnostics.ToText;
using Remotion.Diagnostics.ToText.Handlers;
using List = Remotion.Development.UnitTesting.ObjectMother.List;

namespace Remotion.UnitTests.Diagnostics
{
  [TestFixture]
  public class ToTextProviderHandlerTest
  {
    private readonly ISimpleLogger log = SimpleLogger.CreateForConsole (true);

    public class TestSimple
    {
      public TestSimple ()
      {
        Name = "ABC abc";
        Int = 54321;
      }

      public TestSimple (string name, int i)
      {
        Name = name;
        Int = i;
      }

      public string Name { get; set; }
      public int Int { get; set; }

      public override string ToString ()
      {
        return String.Format ("((TestSimple) Name:{0},Int:{1})", Name, Int);
      }
    }

    public class TestSimple2
    {
      public TestSimple2 ()
      {
        PubProp = "%public_property%";
        PrivateProp = "*private*";
      }

      public string PubProp { get; set; }
      private string PrivateProp { get; set; }

      public string pubField = "%public_field%";
      private string privateField = "*private_field*";
    }

    public interface ITestName
    {
      string Name { get; }
    }

    public interface ITestInt
    {
      int Int { get; }
    }

    public interface ITestListListString
    {
      List<List<string>> ListListString { get; }
    }


    public class Test : ITestName, ITestListListString, ITestInt
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
        ListListString = new List<List<string>> ();
      }

      public string Name { get; set; }
      public int Int { get; set; }
      public LinkedList<string> LinkedListString { get; set; }
      public List<List<string>> ListListString { get; set; }
      public Object[][][] Array3D { get; set; }
      public Object[,] RectangularArray2D { get; set; }
      public Object[, ,] RectangularArray3D { get; set; }

      private string _privateFieldString = "FieldString text";
      private List<List<string>> _privateFieldListList = List.New (List.New ("private", "field"), List.New ("list of", "list"));
    }

    public class TestChild : Test
    {
      public TestChild ()
      {
        Name = "Child Name";
        Int = 22222;
      }
    }

    public class TestChildChild : TestChild
    {
      public TestChildChild ()
      {
        Name = "CHILD CHILD NAME";
        Int = 333333333;
      }
    }


    public interface ITest2Name
    {
      string Name { get; }
    }

    public class Test2 : ITest2Name
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
        ListListString = new List<List<string>> ();
      }

      public string Name { get; set; }
      public int Int { get; set; }
      public LinkedList<string> LinkedListString { get; set; }
      public List<List<string>> ListListString { get; set; }
      public Object[][][] Array3D { get; set; }
      public Object[,] RectangularArray2D { get; set; }
      public Object[, ,] RectangularArray3D { get; set; }

      public override string ToString ()
      {
        return String.Format ("<Name->{0}><Int->{1}><ListListString->{2}>", Name, Int, ListListString);
      }
    }


    private static string ToText (ToTextProvider toText, object o)
    {
      var toTextBuilder = new ToTextBuilder (toText);
      toText.ToText (o, toTextBuilder);
      return toTextBuilder.CheckAndConvertToString ();
    }



    public void AssertToTextHandledStatus (IToTextProviderHandler handler, Object obj, bool handled)
    {
      var parameters = ToTextProviderTest.CreateToTextParameters (obj); // TODO: Derive tests from common base class instead
      parameters.Settings.UseAutomaticObjectToText = true;
      var feedback = new ToTextProviderHandlerFeedback ();
      handler.ToTextIfTypeMatches (parameters, feedback);
      Assert.That (feedback.Handled, Is.EqualTo (handled));
    }



    [Test]
    [Ignore]
    public void ToTextProviderTypeHandlerNoPrimitivesAndTypesTest ()
    {
      //ToTextProvider toText = CreateTextProvider ();

      //var toTextProviderAutomaticObjectToTextHandler = new ToTextProviderAutomaticObjectToTextHandler ();

      var handler = new ToTextProviderAutomaticObjectToTextHandler ();


      AssertToTextHandledStatus (handler, 123, false);
      AssertToTextHandledStatus (handler, 123.456, false);

      var testObj = new object();

      AssertToTextHandledStatus (handler, testObj.GetType (), false);
      AssertToTextHandledStatus (handler, testObj, true);


    }


    // Logging
    //private static readonly ILog s_log = LogManager.GetLogger (typeof(ToTextBuilderTest));
    //static ToTextProviderTest() { LogManager.InitializeConsole (); }
    //public static void Log (string s) { s_log.Info (s); }

    //public static void Log (string s) { System.Console.WriteLine (s); }
    public void Log (string s)
    {
      log.It (s);
    }

    public void LogVariables (params object[] parameterArray)
    {
      foreach (var obj in parameterArray)
      {
        log.Item (" " + obj);
      }
    }
  }
}
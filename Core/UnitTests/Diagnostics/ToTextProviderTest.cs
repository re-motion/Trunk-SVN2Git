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
using Remotion.Diagnostics;
using Remotion.Diagnostics.ToText;
using List = Remotion.Development.UnitTesting.ObjectMother.List;

namespace Remotion.UnitTests.Diagnostics
{
  [TestFixture]
  public class ToTextProviderTest
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
      string Name { get;  }
    }

    public interface ITestInt
    {
      int Int { get;  }
    }

    public interface ITestListListString
    {
      List<List<string>> ListListString { get;  }
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
        ListListString = new List<List<string>>();
      }

      public string Name { get; set; }
      public int Int { get; set; }
      public LinkedList<string> LinkedListString { get; set; }
      public List<List<string>> ListListString { get; set; }
      public Object[][][] Array3D { get; set; }
      public Object[,] RectangularArray2D { get; set; }
      public Object[,,] RectangularArray3D { get; set; }

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
        ListListString = new List<List<string>>();
      }

      public string Name { get; set; }
      public int Int { get; set; }
      public LinkedList<string> LinkedListString { get; set; }
      public List<List<string>> ListListString { get; set; }
      public Object[][][] Array3D { get; set; }
      public Object[,] RectangularArray2D { get; set; }
      public Object[,,] RectangularArray3D { get; set; }

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


    [Test]
    public void ObjectTest ()
    {
      ToTextProvider toText = CreateTextProvider();

      toText.Settings.UseAutomaticObjectToText = false;

      Object o = 5711;
      Assert.That (ToText (toText, o), Is.EqualTo (o.ToString()));

      Object o2 = new object();
      Assert.That (ToText (toText, o2), Is.EqualTo (o2.ToString()));
    }

    [Test]
    public void FallbackToStringTest ()
    {
      FallbackToStringTestSingleType (new object());
      FallbackToStringTestSingleType (new Test ("xxx", 123));

      var test2 = new Test2 ("aBc", 789);
      //log.It (test2);
      FallbackToStringTestSingleType (test2);
    }

    private void FallbackToStringTestSingleType<T> (T t)
    {
      ToTextProvider toText = CreateTextProvider();
      toText.Settings.UseAutomaticObjectToText = false;
      //log.It (t.ToString());
      Assert.That (ToText (toText, t), Is.EqualTo (t.ToString()));
    }

    private void RegisterHandlers ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterHandler<Object> ((x, ttb) => ttb.sf ("[Object: {0}]", x.ToString()));
      toText.RegisterHandler<Test> ((x, ttb) => ttb.sf ("[Test: {0};{1}]", x.Name, x.Int));
    }

    private void InitTestInstanceContainer (ToTextProviderTest.Test test)
    {
      test.ListListString.Add (new List<string>() { "Aaa", "Bbb", "Ccc", "Ddd" });
      test.ListListString.Add (new List<string>() { "R1 C0", "R1 C1" });
      test.ListListString.Add (new List<string>() { "R2 C0", "R2 C1", "R2 C2", "R2 C3", "R2 C4" });
      test.ListListString.Add (new List<string>() { "R3 C0", "R3 C1", "R3 C2" });
    }

    [Test]
    public void RegisteredHandlerTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterHandler<Object> ((x, ttb) => ttb.sf ("[Object: {0}]", x.ToString()));
      Object o = new object();
      string toTextO = ToText (toText, o);
      Log ("toTextO=" + toTextO);
      Assert.That (toTextO, Is.EqualTo (String.Format ("[Object: {0}]", o.ToString())));

      toText.RegisterHandler<ToTextProviderTest.Test> ((x, ttb) => ttb.sf ("[Test: {0};{1}]", x.Name, x.Int));
      var test = new ToTextProviderTest.Test ("That's not my name", 179);
      string toTextTest = ToText (toText, test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("[Test: That's not my name;179]"));
    }

    [Test]
    public void NullTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      Object o = null;
      Assert.That (ToText (toText, o), Is.EqualTo ("null"));
    }

    [Test]
    public void IntToStringFallbackTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      int i = 908;
      Assert.That (ToText (toText, i), Is.EqualTo ("908"));
    }


    [Test]
    public void StringHandlerTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterHandler<String> ((x, ttb) => ttb.s ("\"").ts (x).s ("\""));
      string toTextTest = ToText (toText, "Some text");
      Log ("[StringHandlerTest] toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("\"Some text\""));
    }

    [Test]
    public void CharHandlerTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterHandler<char> ((x, ttb) => ttb.s ("'").ts (x).s ("'"));
      string toTextTest = ToText (toText, 'x');
      Log ("[CharHandlerTest] toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("'x'"));
    }

    //[Test]
    //public void InitStandardHandlersTest ()
    //{
    //  ToTextProvider toText = GetTextProvider ();
    //  toText.RegisterStringHandlers ();
    //  Assert.That (ToText(toText,"Some text"), Is.EqualTo ("\"Some text\""));
    //  Assert.That (ToText(toText,'x'), Is.EqualTo ("'x'"));
    //}

    [Test]
    public void UseAutomaticObjectToTextTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      var testSimple = new TestSimple();

      toText.Settings.UseAutomaticObjectToText = true;
      var resultAutomaticObjectToText = ToText (toText, testSimple);
      Log (resultAutomaticObjectToText);

      Assert.That (resultAutomaticObjectToText, NUnit.Framework.SyntaxHelpers.Text.Contains ("ABC abc"));
      Assert.That (resultAutomaticObjectToText, NUnit.Framework.SyntaxHelpers.Text.Contains ("54321"));

      toText.Settings.UseAutomaticObjectToText = false;
      Assert.That (ToText (toText, testSimple), Is.EqualTo (testSimple.ToString()));
    }


    [Test]
    public void AutomaticObjectToTextConfigureTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      var testSimple2 = new TestSimple2();

      toText.Settings.UseAutomaticObjectToText = true;
      toText.Settings.SetAutomaticObjectToTextEmit (true, true, true, true);

      toText.Settings.SetAutomaticObjectToTextEmit (true, true, true, true);
      var resultAutomaticObjectToText = ToText (toText, testSimple2);
      Log (resultAutomaticObjectToText);
      Assert.That (
          resultAutomaticObjectToText,
          Is.EqualTo ("[TestSimple2  PubProp=%public_property%,pubField=%public_field%,PrivateProp=*private*,privateField=*private_field*]"));

      toText.Settings.SetAutomaticObjectToTextEmit (true, true, true, false);
      Assert.That (
          ToText (toText, testSimple2), Is.EqualTo ("[TestSimple2  PubProp=%public_property%,pubField=%public_field%,PrivateProp=*private*]"));

      toText.Settings.SetAutomaticObjectToTextEmit (true, true, false, true);
      Assert.That (
          ToText (toText, testSimple2), Is.EqualTo ("[TestSimple2  PubProp=%public_property%,pubField=%public_field%,privateField=*private_field*]"));

      toText.Settings.SetAutomaticObjectToTextEmit (true, false, true, true);
      Assert.That (
          ToText (toText, testSimple2), Is.EqualTo ("[TestSimple2  PubProp=%public_property%,PrivateProp=*private*,privateField=*private_field*]"));

      toText.Settings.SetAutomaticObjectToTextEmit (false, true, true, true);
      Assert.That (
          ToText (toText, testSimple2), Is.EqualTo ("[TestSimple2  pubField=%public_field%,PrivateProp=*private*,privateField=*private_field*]"));
    }


    [Test]
    public void ClearHandlersTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.Settings.UseAutomaticObjectToText = false;
      toText.RegisterHandler<Object> ((x, ttb) => ttb.s ("[ClearHandlersTest]").ts (x));
      var o = new object();
      string toTextTest = ToText (toText, o);
      Log ("[ClearHandlersTest] toTextTest=" + toTextTest);
      Assert.That (ToText (toText, o), Is.EqualTo ("[ClearHandlersTest]" + o));
      toText.ClearHandlers();
      Assert.That (ToText (toText, o), Is.EqualTo (o.ToString()));
    }

    [Test]
    public void StringNotEnumerableTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      string s = "Wow look at the size of that thing";
      string toTextTest = ToText (toText, s);
      const string toTextTestExpected = "Wow look at the size of that thing";
      Assert.That (toTextTest, Is.EqualTo (toTextTestExpected));
    }


    [Test]
    public void NullEnumerableTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      //toText.RegisterHandler<Test> ((x, ttb) => ttb.sf ("[Test: {0};{1};{2}]", ToText (toText, x.Name),ToText(toText,x.Int), ToText(toText,x.LinkedListString)));
      toText.RegisterHandler<Test> ((x, ttb) => ttb.s ("[Test: ").m (x.Name).semicolon.m (x.Int).semicolon.m (x.LinkedListString).s ("]"));
      var test = new Test ("That's not my name", 179);
      test.LinkedListString = null;
      string toTextTest = ToText (toText, test);
      const string toTextTestExpected = "[Test: That's not my name;179;null]";
      Assert.That (toTextTest, Is.EqualTo (toTextTestExpected));
    }

    [Test]
    public void EmptyEnumerableTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterHandler<ToTextProviderTest.Test> (
          (x, ttb) => ttb.sf ("[Test: {0};{1};{2}]", ToText (toText, x.Name), ToText (toText, x.Int), ToText (toText, x.ListListString)));
      var test = new Test ("That's not my name", 179);
      test.LinkedListString = new LinkedList<string>();
      string toTextTest = ToText (toText, test);
      const string toTextTestExpected = "[Test: That's not my name;179;{}]";
      Assert.That (toTextTest, Is.EqualTo (toTextTestExpected));
    }

    [Test]
    public void OneDimensionalEnumerableTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterHandler<ToTextProviderTest.Test> (
          (x, ttb) => ttb.sf ("[Test: {0};{1};{2}]", ToText (toText, x.Name), ToText (toText, x.Int), ToText (toText, x.LinkedListString)));
      var test = new ToTextProviderTest.Test ("That's not my name", 179);
      string[] linkedListInit = { "A1", "A2", "A3", "A4", "A5" };
      test.LinkedListString = new LinkedList<string> (linkedListInit);
      //LogVariables ("LinkedListString.Count={0}", test.LinkedListString.Count);
      string toTextTest = ToText (toText, test);
      const string toTextTestExpected = "[Test: That's not my name;179;{A1,A2,A3,A4,A5}]";
      Assert.That (toTextTest, Is.EqualTo (toTextTestExpected));
    }


    [Test]
    public void TwoDimensionalEnumerableTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterHandler<ToTextProviderTest.Test> (
          (x, ttb) => ttb.sf ("[Test: {0};{1};{2}]", ToText (toText, x.Name), ToText (toText, x.Int), ToText (toText, x.ListListString)));
      var test = new ToTextProviderTest.Test ("That's not my name", 179);
      test.ListListString = new List<List<string>>() { new List<string>() { "A1", "A2" }, new List<string>() { "B1", "B2", "B3" } };
      string toTextTest = ToText (toText, test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("[Test: That's not my name;179;{{A1,A2},{B1,B2,B3}}]"));
    }


    [Test]
    public void ThreeDimensionalEnumerableTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterHandler<ToTextProviderTest.Test> (
          (x, ttb) => ttb.sf ("[Test: {0};{1};{2}]", ToText (toText, x.Name), ToText (toText, x.Int), ToText (toText, x.Array3D)));
      var test = new ToTextProviderTest.Test ("That's not my name", 179);
      //Object[,] aaa = { { 1 } };
      test.Array3D = new Object[][][] { new Object[][] { new Object[] { 91, 82, 73, 64 } } };
      string toTextTest = ToText (toText, test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("[Test: That's not my name;179;{{{91,82,73,64}}}]"));
    }


    [Test]
    public void RectangularArrayTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterHandler<ToTextProviderTest.Test> (
          (x, ttb) => ttb.sf ("[Test: {0};{1};{2}]", ToText (toText, x.Name), ToText (toText, x.Int), ToText (toText, x.RectangularArray2D)));
      var test = new ToTextProviderTest.Test ("That's not my name", 179);
      test.RectangularArray2D = new Object[,] { { 1, 3, 5 }, { 7, 11, 13 } };
      string toTextTest = ToText (toText, test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("[Test: That's not my name;179;{{1,3,5},{7,11,13}}]"));
    }

    [Test]
    public void RectangularArrayTest2 ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.RegisterHandler<Test> (
          (x, ttb) => ttb.sf ("[Test: {0};{1};{2}]", ToText (toText, x.Name), ToText (toText, x.Int), ToText (toText, x.RectangularArray3D)));
      toText.RegisterHandler<Test2> (
          (x, ttb) => ttb.sf ("[Test2: {0};{1};{2}]", ToText (toText, x.Name), ToText (toText, x.Int), ToText (toText, x.RectangularArray2D)));
      var test = new Test ("That's not my name", 179);
      var at2 = new Test2[3,3,3];

      for (int i0 = 0; i0 < 2; i0++)
      {
        for (int i1 = 0; i1 < 2; i1++)
        {
          for (int i2 = 0; i2 < 2; i2++)
          {
            var test2 = new Test2 (String.Format ("{0}-{1}-{2}", i0, i1, i2), i0 ^ i1 ^ i2);
            test2.RectangularArray2D = new string[,] { { "A" + i0, "B" + 1 }, { "C" + i2, "D" } };
            at2[i0, i1, i2] = test2;
          }
        }
      }
      test.RectangularArray3D = new Object[,,]
                                {
                                    {
                                        { at2[0, 0, 0], at2[0, 0, 1] }, { at2[0, 1, 0], at2[0, 1, 1] },
                                    },
                                    {
                                        { at2[1, 0, 0], at2[1, 0, 1] }, { at2[1, 1, 0], at2[1, 1, 1] },
                                    },
                                };
      string toTextTest = ToText (toText, test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (
          toTextTest,
          Is.EqualTo (
              "[Test: That's not my name;179;{{{[Test2: 0-0-0;0;{{A0,B1},{C0,D}}],[Test2: 0-0-1;1;{{A0,B1},{C1,D}}]},{[Test2: 0-1-0;1;{{A0,B1},{C0,D}}],[Test2: 0-1-1;0;{{A0,B1},{C1,D}}]}},{{[Test2: 1-0-0;1;{{A1,B1},{C0,D}}],[Test2: 1-0-1;0;{{A1,B1},{C1,D}}]},{[Test2: 1-1-0;0;{{A1,B1},{C0,D}}],[Test2: 1-1-1;1;{{A1,B1},{C1,D}}]}}}]"));
    }


    [Test]
    public void AutomaticStringEnclosingTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      var test = "ABC";
      toText.Settings.UseAutomaticStringEnclosing = false;
      Assert.That (ToText (toText, test), Is.EqualTo ("ABC"));
      toText.Settings.UseAutomaticStringEnclosing = true;
      Assert.That (ToText (toText, test), Is.EqualTo ("\"ABC\""));
    }

    [Test]
    public void AutomaticCharEnclosingTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      var test = 'A';
      toText.Settings.UseAutomaticCharEnclosing = false;
      Assert.That (ToText (toText, test), Is.EqualTo ("A"));
      toText.Settings.UseAutomaticCharEnclosing = true;
      Assert.That (ToText (toText, test), Is.EqualTo ("'A'"));
    }


    private void ToTextProviderEnableAutomatics (ToTextProvider toText, bool enable)
    {
      toText.Settings.UseAutomaticObjectToText = enable;
      toText.Settings.UseAutomaticStringEnclosing = enable;
      toText.Settings.UseAutomaticCharEnclosing = enable;
    }

    [Test]
    public void AutomaticObjectToTextTest ()
    {
      ToTextProvider toText = CreateTextProvider();
      //toText.Settings.UseAutomaticObjectToText = true;
      ToTextProviderEnableAutomatics (toText, true);
      var test = new Test ("That's not my name", 179);
      test.Array3D = new Object[][][] { new Object[][] { new Object[] { 91, 82, 73, 64 } } };
      test.LinkedListString = new LinkedList<string>();
      string toTextTest = ToText (toText, test);
      //Log(toTextTest);
      log.It (toTextTest);
      //string toTextTestExpected = "[Test   Name=\"That's not my name\"  Int=179  LinkedListString={}  ListListString={}  Array3D={{{91,82,73,64}}}  RectangularArray2D=null  RectangularArray3D=null  _privateFieldString=\"FieldString text\"  _privateFieldListList={{\"private\",\"field\"},{\"list of\",\"list\"}} ]";
      string toTextTestExpected =
          "[Test  Name=\"That's not my name\",Int=179,LinkedListString={},ListListString={},Array3D={{{91,82,73,64}}},RectangularArray2D=null,RectangularArray3D=null,_privateFieldString=\"FieldString text\",_privateFieldListList={{\"private\",\"field\"},{\"list of\",\"list\"}}]";
      Assert.That (toTextTest, Is.EqualTo (toTextTestExpected));
    }


    private void TypeToTextTestDo ()
    {
      ToTextProvider toText = CreateTextProvider();
      toText.Settings.UseAutomaticObjectToText = true;
      var type = typeof (object).GetType();
      string result = toText.ToTextString (type);
    }

    //private delegate void TimesOutTestMethodDelegate();

    //private static bool TimesOut ()
    //{
    //  TimesOutTestMethodDelegate timeOutTestMethod = TypeToTextTestDo;
    //  //ThreadStart ts;

    //  ThreadStart threadDelegate = new ThreadStart (TypeToTextTestDo);

    //  ThreadRunner.RunTimesOutAfterSeconds (threadDelegate);
    //  return true;
    //}

    [Test]
    public void RuntimeTypeIsTypeTest ()
    {
      Assert.That (typeof (Type).IsAssignableFrom (typeof (object).GetType()), Is.True);
    }


    [Test]
    public void TypeToTextNoEndlessRecursionTest ()
    {
      bool timesOut = ThreadRunner.RunTimesOutAfterSeconds (TypeToTextTestDo, 0.1);
      Assert.That (timesOut, Is.False);
    }

    [Test]
    public void FloatingPointToTextTest ()
    {
      ToTextProvider toText = CreateTextProvider();

      double d = 1.2345678901234567890;
      Double D = d;

      float f = (float) d;
      Single F = f;

      Assert.That (toText.ToTextString (d), Is.EqualTo ("1.23456789012346"));
      Assert.That (toText.ToTextString (D), Is.EqualTo ("1.23456789012346"));
      Assert.That (toText.ToTextString (f), Is.EqualTo ("1.234568"));
      Assert.That (toText.ToTextString (F), Is.EqualTo ("1.234568"));
    }


    [Test]
    public void UseParentHandlerTest ()
    {
      ToTextProvider toText = CreateTextProvider();

      toText.Settings.UseParentHandlers = true;
      
      var testChildChild = new TestChildChild();

      toText.Settings.ParentHandlerSearchUpToRoot = false;
      toText.Settings.ParentHandlerSearchDepth = 2;
      toText.RegisterHandler<Test> ((o, ttb) => ttb.s ("Test"));
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo ("Test"));
      toText.Settings.ParentHandlerSearchDepth = 1;
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo (testChildChild.ToString()));

      toText.Settings.ParentHandlerSearchDepth = 1;
      toText.RegisterHandler<TestChild> ((o, ttb) => ttb.s ("TestChild"));
      toText.Settings.ParentHandlerSearchDepth = 0;
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo (testChildChild.ToString()));

      toText.Settings.ParentHandlerSearchDepth = 10;
      toText.RegisterHandler<TestChildChild> ((o, ttb) => ttb.s ("TestChildChild"));
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo ("TestChildChild"));


      toText.ClearHandlers();
      toText.RegisterHandler<Test> ((o, ttb) => ttb.s ("Test"));
      toText.Settings.ParentHandlerSearchDepth = 0;
      toText.Settings.ParentHandlerSearchUpToRoot = true;
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo ("Test"));
      toText.Settings.ParentHandlerSearchUpToRoot = false;
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo (testChildChild.ToString()));
    }

    [Test]
    public void UseParentHandlerSwitchTest ()
    {
      ToTextProvider toText = CreateTextProvider ();
      var testChild = new TestChild ();

      toText.RegisterHandler<Test> ((o, ttb) => ttb.s ("Test"));
      toText.Settings.UseParentHandlers = true;
      toText.Settings.ParentHandlerSearchUpToRoot = true;
      toText.Settings.ParentHandlerSearchDepth = 1000;
      Assert.That (toText.ToTextString (testChild), Is.EqualTo ("Test"));
      toText.Settings.UseParentHandlers = false;
      Assert.That (toText.ToTextString (testChild), Is.EqualTo (testChild.ToString ()));
    }


    [Test]
    public void InterfaceTest ()
    {
      ToTextProvider toText = CreateTextProvider ();
      toText.Settings.UseInterfaceHandlers = true;

      toText.RegisterInterfaceHandlerAppendLast<ITestName> ((o, ttb) => ttb.m ("Name", o.Name));

      Test testInterface = new Test ("�berbauer",3589);
      var result = toText.ToTextString (testInterface);
      Log (result);
      Assert.That (result, Is.EqualTo ("Name=�berbauer"));
    }

    [Test]
    public void InterfaceDisabledTest ()
    {
      ToTextProvider toText = CreateTextProvider ();
      toText.Settings.UseInterfaceHandlers = false;
      toText.RegisterInterfaceHandlerAppendLast<ITestName> ((o, ttb) => ttb.m ("Name", o.Name));

      Test testInterface = new Test ("�berbauer", 3589);
      var result = toText.ToTextString (testInterface);
      Log (result);
      Assert.That (result, Is.EqualTo (testInterface.ToString()));
    }

    [Test]
    public void InterfacePriorityTest ()
    {
      ToTextProvider toText = CreateTextProvider ();
      toText.Settings.UseInterfaceHandlers = true;

      toText.RegisterInterfaceHandlerAppendLast<ITestInt> ((o, ttb) => ttb.m ("Int", o.Int));
      toText.RegisterInterfaceHandlerAppendLast<ITestName> ((o, ttb) => ttb.m ("Name", o.Name));

      Test testInterface = new Test ("�berbauer", 3589);
      var result = toText.ToTextString (testInterface);
      Log (result);
      Assert.That (result, Is.EqualTo ("Int=3589"));
    }

    [Test]
    public void InterfacePriority2Test ()
    {
      ToTextProvider toText = CreateTextProvider ();
      toText.Settings.UseInterfaceHandlers = true;

      toText.RegisterInterfaceHandlerAppendLast<ITestInt> ((o, ttb) => ttb.m ("Int", o.Int));
      toText.RegisterInterfaceHandlerAppendLast<ITestName> ((o, ttb) => ttb.m ("Name", o.Name));
      toText.RegisterInterfaceHandlerAppendLast<ITest2Name> ((o, ttb) => ttb.m ("Name2", o.Name));
      toText.RegisterInterfaceHandlerAppendLast<ITestListListString> ((o, ttb) => ttb.m ("ListListString", o.ListListString));

      Test2 test2Interface = new Test2 ("�berbauer", 3589);
      var result = toText.ToTextString (test2Interface);
      Log (result);
      Assert.That (result, Is.EqualTo ("Name2=�berbauer"));
    }


    [Test]
    public void InterfaceAppendFirstPriorityTest ()
    {
      ToTextProvider toText = CreateTextProvider ();
      toText.Settings.UseInterfaceHandlers = true;

      toText.RegisterInterfaceHandlerAppendFirst<ITestName> ((o, ttb) => ttb.m ("Name", o.Name));
      toText.RegisterInterfaceHandlerAppendFirst<ITestInt> ((o, ttb) => ttb.m ("Int", o.Int));

      Test testInterface = new Test ("�berbauer", 3589);
      var result = toText.ToTextString (testInterface);
      Log (result);
      Assert.That (result, Is.EqualTo ("Int=3589"));
    }


    [Test]
    public void ToTextProviderNullHandlerTest ()
    {
      ToTextProvider toText = CreateTextProvider ();

      var handler = new ToTextProviderNullHandler();
      var parameters = CreateToTextParameters(null);
      var feedback = new ToTextProviderHandlerFeedback();
      handler.ToTextIfTypeMatches (parameters,feedback);
      Assert.That (feedback.Handled, Is.EqualTo (true));
      var result = parameters.ToTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("null"));
    }

    //[Test]
    //public void ToTextProviderRegisteredHandlerHandlerTest ()
    //{
    //  ToTextProvider toText = CreateTextProvider ();

    //  var handler = new ToTextProviderRegisteredHandlerWithBaseClassFallbackHandler ();
    //  handler.RegisterHandler<ToTextProviderTest.Test> ((x, ttb) => ttb.sf ("[Test: {0};{1}]", x.Name, x.Int));

    //  var test = new ToTextProviderTest.Test ("That's not my name", 179);

    //  var parameters = CreateToTextParameters (test);
    //  var feedback = new ToTextProviderHandlerFeedback ();
    //  handler.ToTextIfTypeMatches (parameters, feedback);
    //  Assert.That (feedback.Handled, Is.EqualTo (true));

    //  var result = parameters.ToTextBuilder.CheckAndConvertToString ();
    //  Log (result);
    //  Assert.That (result, Is.EqualTo ("[Test: That's not my name;179]"));
    //}

    private ToTextProvider CreateTextProvider ()
    {
      var toText = new ToTextProvider();
      toText.Settings.UseAutomaticObjectToText = false;
      toText.Settings.UseAutomaticStringEnclosing = false;
      toText.Settings.UseAutomaticCharEnclosing = false;
      toText.Settings.UseInterfaceHandlers = false;
      return toText;
    }

    private ToTextParameters CreateToTextParameters (object obj)
    {
      var toText = CreateTextProvider();
      var toTextBuilder = new ToTextBuilder (toText);
      Type type = (obj != null) ? obj.GetType() : null;
      var parameters = new ToTextParameters () { Object = obj, Type = type, ToTextBuilder = toTextBuilder };
      //LogVariables (parameters.Object, parameters.Type, parameters.ToTextProvider, parameters.ToTextBuilder);
      return parameters;
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
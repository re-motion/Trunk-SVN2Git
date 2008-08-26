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
using Remotion.Diagnostic;
using Remotion.Logging;
    //using NUnit.Framework.Constraints;

namespace Remotion.UnitTests.Diagnostic
{
  [TestFixture]
  public class ToTextProviderTest
  {
    private ISimpleLogger log = SimpleLogger.Create (true);

    public class TestSimple
    {
      public TestSimple ()
      {
        Name = "ABC abc";
        Int = 54321;
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

    public interface ITest
    {
    }

    public class Test : ITest
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
      private List<List<string>> _privateFieldListList = New.List (New.List ("private", "field"), New.List ("list of", "list"));
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

      public override string ToString ()
      {
        return String.Format ("<Name->{0}><Int->{1}><ListListString->{2}>", Name, Int, ListListString);
      }
    }


    private static string ToText (ToTextProvider toTextProvider, object o)
    {
      var toTextBuilder = new ToTextBuilder (toTextProvider);
      toTextProvider.ToText (o, toTextBuilder);
      return toTextBuilder.ToString();
    }


    [Test]
    public void ObjectTest ()
    {
      ToTextProvider toText = GetTextProvider();

      toText.UseAutomaticObjectToText = false;

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
      ToTextProvider toText = GetTextProvider();
      toText.UseAutomaticObjectToText = false;
      //log.It (t.ToString());
      Assert.That (ToText (toText, t), Is.EqualTo (t.ToString()));
    }

    private void RegisterHandlers ()
    {
      ToTextProvider toText = GetTextProvider();
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
      ToTextProvider toText = GetTextProvider();
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
      ToTextProvider toText = GetTextProvider();
      Object o = null;
      Assert.That (ToText (toText, o), Is.EqualTo ("null"));
    }

    [Test]
    public void IntToStringFallbackTest ()
    {
      ToTextProvider toText = GetTextProvider();
      int i = 908;
      Assert.That (ToText (toText, i), Is.EqualTo ("908"));
    }


    [Test]
    public void StringHandlerTest ()
    {
      ToTextProvider toText = GetTextProvider();
      toText.RegisterHandler<String> ((x, ttb) => ttb.s ("\"").ts (x).s ("\""));
      string toTextTest = ToText (toText, "Some text");
      Log ("[StringHandlerTest] toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("\"Some text\""));
    }

    [Test]
    public void CharHandlerTest ()
    {
      ToTextProvider toText = GetTextProvider();
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
      ToTextProvider toText = GetTextProvider();
      var testSimple = new TestSimple();

      toText.UseAutomaticObjectToText = true;
      var resultAutomaticObjectToText = ToText (toText, testSimple);
      Log (resultAutomaticObjectToText);

      Assert.That (resultAutomaticObjectToText, NUnit.Framework.SyntaxHelpers.Text.Contains ("ABC abc"));
      Assert.That (resultAutomaticObjectToText, NUnit.Framework.SyntaxHelpers.Text.Contains ("54321"));

      toText.UseAutomaticObjectToText = false;
      Assert.That (ToText (toText, testSimple), Is.EqualTo (testSimple.ToString()));
    }


    [Test]
    public void AutomaticObjectToTextConfigureTest ()
    {
      ToTextProvider toText = GetTextProvider();
      var testSimple2 = new TestSimple2();

      toText.UseAutomaticObjectToText = true;
      toText.SetAutomaticObjectToTextEmit (true, true, true, true);

      toText.SetAutomaticObjectToTextEmit (true, true, true, true);
      var resultAutomaticObjectToText = ToText (toText, testSimple2);
      Log (resultAutomaticObjectToText);
      Assert.That (
          resultAutomaticObjectToText,
          Is.EqualTo ("[TestSimple2  PubProp=%public_property%,pubField=%public_field%,PrivateProp=*private*,privateField=*private_field*]"));

      toText.SetAutomaticObjectToTextEmit (true, true, true, false);
      Assert.That (
          ToText (toText, testSimple2), Is.EqualTo ("[TestSimple2  PubProp=%public_property%,pubField=%public_field%,PrivateProp=*private*]"));

      toText.SetAutomaticObjectToTextEmit (true, true, false, true);
      Assert.That (
          ToText (toText, testSimple2), Is.EqualTo ("[TestSimple2  PubProp=%public_property%,pubField=%public_field%,privateField=*private_field*]"));

      toText.SetAutomaticObjectToTextEmit (true, false, true, true);
      Assert.That (
          ToText (toText, testSimple2), Is.EqualTo ("[TestSimple2  PubProp=%public_property%,PrivateProp=*private*,privateField=*private_field*]"));

      toText.SetAutomaticObjectToTextEmit (false, true, true, true);
      Assert.That (
          ToText (toText, testSimple2), Is.EqualTo ("[TestSimple2  pubField=%public_field%,PrivateProp=*private*,privateField=*private_field*]"));
    }


    [Test]
    public void ClearHandlersTest ()
    {
      ToTextProvider toText = GetTextProvider();
      toText.UseAutomaticObjectToText = false;
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
      ToTextProvider toText = GetTextProvider();
      string s = "Wow look at the size of that thing";
      string toTextTest = ToText (toText, s);
      const string toTextTestExpected = "Wow look at the size of that thing";
      Assert.That (toTextTest, Is.EqualTo (toTextTestExpected));
    }


    [Test]
    public void NullEnumerableTest ()
    {
      ToTextProvider toText = GetTextProvider();
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
      ToTextProvider toText = GetTextProvider();
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
      ToTextProvider toText = GetTextProvider();
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
      ToTextProvider toText = GetTextProvider();
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
      ToTextProvider toText = GetTextProvider();
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
      ToTextProvider toText = GetTextProvider();
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
      ToTextProvider toText = GetTextProvider();
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
      ToTextProvider toText = GetTextProvider();
      var test = "ABC";
      toText.UseAutomaticStringEnclosing = false;
      Assert.That (ToText (toText, test), Is.EqualTo ("ABC"));
      toText.UseAutomaticStringEnclosing = true;
      Assert.That (ToText (toText, test), Is.EqualTo ("\"ABC\""));
    }

    [Test]
    public void AutomaticCharEnclosingTest ()
    {
      ToTextProvider toText = GetTextProvider();
      var test = 'A';
      toText.UseAutomaticCharEnclosing = false;
      Assert.That (ToText (toText, test), Is.EqualTo ("A"));
      toText.UseAutomaticCharEnclosing = true;
      Assert.That (ToText (toText, test), Is.EqualTo ("'A'"));
    }


    private void ToTextProviderEnableAutomatics (ToTextProvider toTextProvider, bool enable)
    {
      toTextProvider.UseAutomaticObjectToText = enable;
      toTextProvider.UseAutomaticStringEnclosing = enable;
      toTextProvider.UseAutomaticCharEnclosing = enable;
    }

    [Test]
    public void AutomaticObjectToTextTest ()
    {
      ToTextProvider toText = GetTextProvider();
      //toText.UseAutomaticObjectToText = true;
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


    private static void TypeToTextTestDo ()
    {
      ToTextProvider toText = GetTextProvider();
      toText.UseAutomaticObjectToText = true;
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
      ToTextProvider toText = GetTextProvider();

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
      ToTextProvider toText = GetTextProvider();
      var testChildChild = new TestChildChild();

      toText.ParentHandlerSearchUpToRoot = false;
      toText.ParentHandlerSearchDepth = 2;
      toText.RegisterHandler<Test> ((o, ttb) => ttb.s ("Test"));
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo ("Test"));
      toText.ParentHandlerSearchDepth = 1;
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo (testChildChild.ToString()));

      toText.ParentHandlerSearchDepth = 1;
      toText.RegisterHandler<TestChild> ((o, ttb) => ttb.s ("TestChild"));
      toText.ParentHandlerSearchDepth = 0;
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo (testChildChild.ToString()));

      toText.ParentHandlerSearchDepth = 10;
      toText.RegisterHandler<TestChildChild> ((o, ttb) => ttb.s ("TestChildChild"));
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo ("TestChildChild"));

      toText.ClearHandlers();
      toText.RegisterHandler<Test> ((o, ttb) => ttb.s ("Test"));
      toText.ParentHandlerSearchDepth = 0;
      toText.ParentHandlerSearchUpToRoot = true;
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo ("Test"));
      toText.ParentHandlerSearchUpToRoot = false;
      Assert.That (toText.ToTextString (testChildChild), Is.EqualTo (testChildChild.ToString()));
    }


    //[Test]
    //public void InterfaceTest ()
    //{
    //  ToTextProvider toText = GetTextProvider ();
    //  ITest testInterface = new Test();
    //  var result = toText.ToTextString (testInterface);
    //  Log (result);
    //  Assert.That (result, Is.EqualTo ("[ITest]"));
    //}


    public static ToTextProvider GetTextProvider ()
    {
      var toTextProvider = new ToTextProvider();
      toTextProvider.UseAutomaticObjectToText = false;
      toTextProvider.UseAutomaticStringEnclosing = false;
      toTextProvider.UseAutomaticCharEnclosing = false;
      return toTextProvider;
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
  }
}
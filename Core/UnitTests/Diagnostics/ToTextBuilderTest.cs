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
using Remotion.Development.UnitTesting.Logging;
using Remotion.Diagnostics.ToText;
using List = Remotion.Development.UnitTesting.ObjectMother.List;


namespace Remotion.UnitTests.Diagnostics
{
  [NUnit.Framework.TestFixture]
  public class ToTextBuilderTest
  {
    private ISimpleLogger log = SimpleLogger.CreateForConsole (true);

    [Test]
    public void StringTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.s ("ABC defg");
      var result = toTextBuilder.CheckAndConvertToString();
      Assert.That (result, Is.EqualTo ("ABC defg"));
    }

    //[Test]
    //public void ToTextStringTest ()
    //{
    //  var toTextBuilder = CreateTextBuilder();
    //  toTextBuilder.ToTextString ("XyZ !");
    //  var result = toTextBuilder.CheckAndConvertToString();
    //  Assert.That (result, Is.EqualTo ("XyZ !"));
    //}

    [Test]
    public void sTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.s ("A");
      var result = toTextBuilder.CheckAndConvertToString();
      Assert.That (result, Is.EqualTo ("A"));
    }

    [Test]
    public void nlTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.nl().s ("");
      var result = toTextBuilder.CheckAndConvertToString();
      Assert.That (result, Is.EqualTo (System.Environment.NewLine));
    }

    //[Test]
    //public void spaceTest ()
    //{
    //  var toTextBuilder = CreateTextBuilder();
    //  toTextBuilder.space().s ("");
    //  var result = toTextBuilder.CheckAndConvertToString();
    //  Assert.That (result, Is.EqualTo (" "));
    //}

    //[Test]
    //public void tabTest ()
    //{
    //  var toTextBuilder = CreateTextBuilder();
    //  toTextBuilder.tab().s ("");
    //  var result = toTextBuilder.CheckAndConvertToString();
    //  Assert.That (result, Is.EqualTo ("\t"));
    //}


    //[Test]
    //public void SeperatorTest ()
    //{
    //  var toTextBuilder = CreateTextBuilder();
    //  toTextBuilder.seperator.comma.colon.semicolon.s ("");
    //  var result = toTextBuilder.CheckAndConvertToString();
    //  Assert.That (result, Is.EqualTo (",,:;"));
    //}


    [Test]
    public void StringConcatenationTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.s ("A").s ("B").s ("C");
      var result = toTextBuilder.CheckAndConvertToString();
      Assert.That (result, Is.EqualTo ("ABC"));
    }

    [Test]
    public void StringFormattedTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      int i = 123;
      double f = 456.789;
      string s = "Text";
      toTextBuilder.sf ("[{0},{1},{2}]", i, f, s);
      var result = toTextBuilder.CheckAndConvertToString();
      Assert.That (result, Is.EqualTo ("[123,456,789,Text]"));
    }


    //[Test]
    //public void MultiTest ()
    //{
    //  int i = 987654321;
    //  double f = 3.14;
    //  string s = "Text";

    //  var toTextBuilder = CreateTextBuilder();
    //  toTextBuilder.s ("START-").nl().tab().sf ("[{0};{1};{2}]", i, f, s).space().s ("-END");
    //  var result = toTextBuilder.CheckAndConvertToString();
    //  Log (result);
    //  Assert.That (result, Is.EqualTo ("START-" + Environment.NewLine + "\t[987654321;3,14;Text] -END"));
    //}


    [Test]
    public void tsTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var o = new Object();
      toTextBuilder.ts (o);
      var result = toTextBuilder.CheckAndConvertToString();
      Assert.That (result, Is.EqualTo (o.ToString()));
    }


    [Test]
    public void ToTextTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var o = new Object();
      toTextBuilder.e (o);
      var result = toTextBuilder.CheckAndConvertToString();
      //Log (result);
      Assert.That (result, Is.EqualTo (o.ToString()));
    }

    [Test]
    public void MemberTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var o = new Object();
      toTextBuilder.e (o);
      var result = toTextBuilder.CheckAndConvertToString();
      Assert.That (result, Is.EqualTo (o.ToString()));
    }

    [Test]
    public void MemberTest2 ()
    {
      var toTextBuilder = CreateTextBuilder();
      //var list = new List<int> () { 5, 3, 1 };
      var myList = List.New (5, 3, 1);
      toTextBuilder.e ("myList", myList);
      var result = toTextBuilder.CheckAndConvertToString();
      //Assert.That (result, Is.EqualTo ("myList:{5,3,1}"));
      Assert.That (result, Is.EqualTo ("myList={5,3,1}"));
    }

    [Test]
    public void MemberVariableTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      var theAnswer = 42;
      toTextBuilder.e (x => theAnswer);
      var result = toTextBuilder.CheckAndConvertToString ();
      Assert.That (result, Is.EqualTo ("theAnswer=42"));
    }

    [Test]
    public void MemberVariableTest2 ()
    {
      var toTextBuilder = CreateTextBuilder ();
      var myList = List.New (5, 3, 1);
      toTextBuilder.e (x => myList);
      var result = toTextBuilder.CheckAndConvertToString ();
      Assert.That (result, Is.EqualTo ("myList={5,3,1}"));
    }

    //[Test]
    //[ExpectedException ("Remotion.Utilities.AssertionException")]
    //public void MemberVariableWrongParameterNameTest ()
    //{
    //  var toTextBuilder = CreateTextBuilder ();
    //  var myList = List.New (5, 3, 1);
    //  toTextBuilder.e (n => myList);
    //  var result = toTextBuilder.CheckAndConvertToString ();
    //  Assert.That (result, Is.EqualTo ("myList={5,3,1}"));
    //}

    [Test]
    public void MemberInSequenceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var myList = List.New (5, 3, 1);
      toTextBuilder.sb().e ("Abra").e ("myList", myList).e ("myList", myList).e ("Kadabra").se();
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("(Abra,myList={5,3,1},myList={5,3,1},Kadabra)"));
    }

    [Test]
    public void AppendCollectionTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = List.New (5, 3, 1, 11, 13, 17);
      toTextBuilder.AppendEnumerable (list);
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{5,3,1,11,13,17}"));
    }

    [Test]
    public void AppendNestedCollectionTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = List.New (List.New (5, 3, 1), List.New (11, 13, 17));
      toTextBuilder.AppendEnumerable (list);
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{5,3,1},{11,13,17}}"));
    }

    [Test]
    public void AppendNestedCollectionStringMemberTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      var list = List.New (List.New ("5", "3", "1"), List.New ("11", "13", "17"));
      toTextBuilder.AppendEnumerable (list);
      var result = toTextBuilder.CheckAndConvertToString ();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{5,3,1},{11,13,17}}"));
    }


    [Test]
    public void AppendNestedCollectionTest2 ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = List.New (List.New (5, 3, 1), List.New (11, 13, 17), List.New (19, 23, 29));
      toTextBuilder.AppendEnumerable (list);
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{5,3,1},{11,13,17},{19,23,29}}"));
    }

    [Test]
    public void AppendNestedCollectionTest3 ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = List.New (List.New (List.New (5, 3, 1), List.New (11, 13, 17)), List.New (List.New (19, 23, 29), List.New (31, 37, 41)));
      toTextBuilder.AppendEnumerable (list);
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{{5,3,1},{11,13,17}},{{19,23,29},{31,37,41}}}"));
    }

    [Test]
    public void AppendNestedCollectionTest4 ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = List.New (
          List.New (List.New (1, 2, 3), List.New (4, 5, 6), List.New (7, 8, 9)),
          List.New (List.New (10, 11, 12), List.New (13, 14, 15), List.New (16, 17, 18)),
          List.New (List.New (19, 20, 21), List.New (22, 23, 24), List.New (25, 26, 27))
          );
      toTextBuilder.AppendEnumerable (list);
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{{1,2,3},{4,5,6},{7,8,9}},{{10,11,12},{13,14,15},{16,17,18}},{{19,20,21},{22,23,24},{25,26,27}}}"));
    }

    [Test]
    public void collectionTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = List.New (List.New (List.New ("A", "B", "C")));
      toTextBuilder.collection (list);
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{{A,B,C}}}"));
    }



    [Test]
    public void AppendArray3DTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var array = new[] {
          new[] {new[] {1, 3}, new[] {5, 7}},
          new[] {new[] {11, 13}, new[] {17, 19}},
          new[] {new[] {23, 29}, new[] {31, 37}}};
      toTextBuilder.AppendArray (array);
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{{1,3},{5,7}},{{11,13},{17,19}},{{23,29},{31,37}}}"));
    }

    [Test]
    public void AppendRectangularArrayTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      int[,,] array = { { { 1, 3 }, { 5, 7 } }, { { 11, 13 }, { 17, 19 } }, { { 23, 29 }, { 31, 37 } } };
      toTextBuilder.AppendArray (array);
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{{1,3},{5,7}},{{11,13},{17,19}},{{23,29},{31,37}}}"));
    }

    [Test]
    public void AppendRectangularArrayTagTest ()
    {
      int[,,] array = { { { 1, 3 }, { 5, 7 } }, { { 11, 13 }, { 17, 19 } }, { { 23, 29 }, { 31, 37 } } };

      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.Settings.ArrayPrefix = "{";
      //toTextBuilder.ArrayFirstElementPrefix = "(";
      toTextBuilder.Settings.ArrayFirstElementPrefix = "(";
      toTextBuilder.Settings.ArrayOtherElementPrefix = ",(";
      toTextBuilder.Settings.ArrayElementPostfix = ")";
      toTextBuilder.Settings.ArrayPostfix = "}";

      toTextBuilder.AppendArray (array);
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{({({(1),(3)}),({(5),(7)})}),({({(11),(13)}),({(17),(19)})}),({({(23),(29)}),({(31),(37)})})}"));
    }


    [Test]
    public void collectionTest2 ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = List.New (List.New (List.New ("A", "B", "C")));
      toTextBuilder.collection (list);
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{{A,B,C}}}"));
    }


    //[Test]
    //public void EnumerableTagTest ()
    //{
    //  var toTextBuilder = CreateTextBuilder();
    //  var list = List.New (List.New (5, 3, 1), List.New (11, 13, 17));
    //  toTextBuilder.EnumerableBegin = "(";
    //  toTextBuilder.EnumerableSeparator = ";";
    //  toTextBuilder.EnumerableEnd = ")";

    //  toTextBuilder.AppendEnumerable (list);
    //  var result = toTextBuilder.CheckAndConvertToString();
    //  Log (result);
    //  Assert.That (result, Is.EqualTo ("((5;3;1);(11;13;17))"));
    //}


    //[Test]
    //public void ArrayTagTest ()
    //{
    //  var toTextBuilder = CreateTextBuilder();
    //  var array = new[] {
    //      new[] {new[] {1, 3}, new[] {5, 7}},
    //      new[] {new[] {11, 13}, new[] {17, 19}},
    //      new[] {new[] {23, 29}, new[] {31, 37}}};
    //  toTextBuilder.ArrayBegin = "<{[";
    //  toTextBuilder.ArraySeparator = "�|�";
    //  toTextBuilder.ArrayEnd = "]}>";

    //  toTextBuilder.AppendArray (array);
    //  var result = toTextBuilder.CheckAndConvertToString();
    //  Log (result);
    //  Assert.That (
    //      result, Is.EqualTo ("<{[<{[<{[1�|�3]}>�|�<{[5�|�7]}>]}>�|�<{[<{[11�|�13]}>�|�<{[17�|�19]}>]}>�|�<{[<{[23�|�29]}>�|�<{[31�|�37]}>]}>]}>"));
    //}

    [Test]
    public void ArrayTagTest2 ()
    {
      var toTextBuilder = CreateTextBuilder();
      var array = new[] {new[] {new[] {1, 3, 5}, new[] {5, 7, 11}}};
      toTextBuilder.Settings.ArrayPrefix = "<";
      toTextBuilder.Settings.ArrayFirstElementPrefix = "[";
      toTextBuilder.Settings.ArrayOtherElementPrefix = ";[";
      toTextBuilder.Settings.ArrayElementPostfix = "]";
      toTextBuilder.Settings.ArrayPostfix = ">";

      toTextBuilder.AppendArray (array);
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("<[<[<[1];[3];[5]>];[<[5];[7];[11]>]>]>"));
    }


    [Test]
    public void SinglelineMultilineTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.UseMultiLine = false;
      toTextBuilder.s ("Hello").nl().s (" world");
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("Hello world"));

      toTextBuilder.UseMultiLine = true;
      toTextBuilder.s (" here comes the").nl().s ("newline");
      var result2 = toTextBuilder.CheckAndConvertToString();
      Log (result2);
      Assert.That (result2, Is.EqualTo ("Hello world here comes the" + Environment.NewLine + "newline"));
    }

    [Test]
    public void EnabledTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.s ("test");
      Assert.That (toTextBuilder.CheckAndConvertToString(), Is.EqualTo ("test"));
      toTextBuilder.Enabled = false;
      toTextBuilder.s ("test");
      Assert.That (toTextBuilder.CheckAndConvertToString(), Is.EqualTo ("test"));
      toTextBuilder.Enabled = true;
      toTextBuilder.s ("test");
      Assert.That (toTextBuilder.CheckAndConvertToString(), Is.EqualTo ("testtest"));
    }

    [Test]
    public void OutputComplexityTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.OutputComplex();
      Assert.That (toTextBuilder.OutputComplexity, Is.EqualTo (ToTextBuilder.ToTextBuilderOutputComplexityLevel.Complex));
    }

    [Test]
    public void ComplexityFilteringSettingTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      Assert.That (toTextBuilder.Enabled, Is.EqualTo (true));
      toTextBuilder.OutputSkeleton();
      toTextBuilder.WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilder.ToTextBuilderOutputComplexityLevel.Basic);
      Assert.That (toTextBuilder.Enabled, Is.EqualTo (false));

      toTextBuilder.OutputBasic();
      toTextBuilder.WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilder.ToTextBuilderOutputComplexityLevel.Basic);
      Assert.That (toTextBuilder.Enabled, Is.EqualTo (true));
    }


    public ToTextBuilder AllFilterLevelsFilteredOutput (ToTextBuilder toTextBuilder)
    {
      if (toTextBuilder == null)
        toTextBuilder = CreateTextBuilder();
      toTextBuilder.cBasic.s ("b").cComplex.s ("c").cFull.s ("f").cMedium.s ("m").cSkeleton.s ("s");
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      return toTextBuilder;
    }


    [Test]
    public void ComplexityFilteringTest ()
    {
      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.OutputDisable();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).CheckAndConvertToString ();
        Assert.That (result, Is.EqualTo (""));
      }

      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.OutputSkeleton();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).CheckAndConvertToString ();
        Assert.That (result, Is.EqualTo ("s"));
      }

      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.OutputBasic();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).CheckAndConvertToString ();
        Assert.That (result, Is.EqualTo ("bs"));
      }

      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.OutputMedium();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).CheckAndConvertToString ();
        Assert.That (result, Is.EqualTo ("bms"));
      }

      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.OutputComplex();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).CheckAndConvertToString ();
        Assert.That (result, Is.EqualTo ("bcms"));
      }

      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.OutputFull();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).CheckAndConvertToString ();
        Assert.That (result, Is.EqualTo ("bcfms"));
      }
    }


    [Test]
    public void AppendObjectTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var o = new Object();
      Assert.That (toTextBuilder.LowLevelWrite (o).CheckAndConvertToString (), Is.EqualTo (o.ToString ()));
    }


    [Test]
    public void beginInstanceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var obj = new object();
      toTextBuilder.beginInstance (obj.GetType()).endInstance();
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("[Object]")); // NUnit.Framework.SyntaxHelpers.Text.Contains("[ToTextBuilder"));
    }

    [Test]
    public void SequenceStateTest ()
    {
      var sequenceState = new SequenceStateHolder ("Name","Start", "FirstPrefix", "OtherPrefix", "Postfix", "End");
      Assert.That (sequenceState.Counter, Is.EqualTo (0));
      Assert.That (sequenceState.Name, Is.EqualTo ("Name"));
      Assert.That (sequenceState.SequencePrefix, Is.EqualTo ("Start"));
      Assert.That (sequenceState.FirstElementPrefix, Is.EqualTo ("FirstPrefix"));
      Assert.That (sequenceState.OtherElementPrefix, Is.EqualTo ("OtherPrefix"));
      Assert.That (sequenceState.ElementPostfix, Is.EqualTo ("Postfix"));
      Assert.That (sequenceState.SequencePostfix, Is.EqualTo ("End"));
    }

    [Test]
    public void SequenceShorthandTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb ("<{[", "", ",", "", "]}>").e ("hello").e ("world").e (1).e (2).e (3).se();
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("<{[hello,world,1,2,3]}>"));
    }

    [Test]
    public void SequenceTest1 ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb ("<", "(", ",(", ")", ">").e ("hello").e ("world").e (1).e (2).e (3).se();
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("<(hello),(world),(1),(2),(3)>"));
    }


    [Test]
    public void BraketTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb ("", "|", "|", ">", "").elementsNumbered ("a", 1, 5).se();
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("|a1>|a2>|a3>|a4>|a5>"));
    }


    [Test]
    public void SequenceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.WriteSequenceBegin ("", "", "", ",", "", "");
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (0));
      toTextBuilder.e ("1");
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (1));
      toTextBuilder.e (2);
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (2));
      toTextBuilder.e ("drei");
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (3));
      toTextBuilder.WriteSequenceEnd();
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("1,2,drei"));
    }

    [Test]
    public void SequenceCounterUpdateTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var dreiString = "drei";
      toTextBuilder.WriteSequenceBegin ("", "", "", ",", "", "");
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (0));
      toTextBuilder.e ("1");
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (1));
      toTextBuilder.e (2);
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (2));
      toTextBuilder.e (dreiString);
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (3));
      toTextBuilder.WriteSequenceEnd();
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("1,2,drei"));
    }

    [Test]
    public void NestedSequencesTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb ("[", "", ",", "", "]").e ("hello").e (toTextBuilder.SequenceState.Counter);
      toTextBuilder.sb ("<", "(", ";(", ")", ">").e ("hello").e ("world").e (toTextBuilder.SequenceState.Counter).e (
          toTextBuilder.SequenceState.Counter).e (toTextBuilder.SequenceState.Counter).se();
      toTextBuilder.e ("world").e (toTextBuilder.SequenceState.Counter).e (toTextBuilder.SequenceState.Counter).se();
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("[hello,1,<(hello);(world);(2);(3);(4)>,world,4,5]"));
    }

    [Test]
    public void IsInSequenceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb ("[", "", ",", "", "]");
      Assert.That (toTextBuilder.IsInSequence, Is.True);
      toTextBuilder.sb ("[", "", ",", "", "]");
      Assert.That (toTextBuilder.IsInSequence, Is.True);
      toTextBuilder.se();
      Assert.That (toTextBuilder.IsInSequence, Is.True);
      toTextBuilder.se();
      Assert.That (toTextBuilder.IsInSequence, Is.False);
    }

    [Test]
    public void AppendSequenceElementsTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb ("[", "", ",", "", "]").WriteSequenceElements ("a", 2, "b", 3, "c").se();
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("[a,2,b,3,c]"));
    }

    [Test]
    public void SimpleSequenceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb ("<", ">").e ("ABC").e (toTextBuilder.SequenceState.Counter).e ("DEFG").se();
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("<ABC,1,DEFG>"));
    }

    [Test]
    public void SimpleSequenceTest2 ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb ("<", ";", ">").e ("ABC").e (toTextBuilder.SequenceState.Counter).e ("DEFG").se();
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("<ABC;1;DEFG>"));
    }

    [Test]
    public void StandardSequenceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb().e ("ABC").e (toTextBuilder.SequenceState.Counter).e ("DEFG").se();
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("(ABC,1,DEFG)"));
    }

    [Test]
    public void elementsTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb ("[", "", ",", "", "]").elements ("a", 2, "b", 3, "c").se();
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("[a,2,b,3,c]"));
    }


    [Test]
    public void NestedSequencesWitMembersTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      //toTextBuilder.ToTextProvider.UseAutomaticObjectToText = true;
      var simpleTest = new ToTextProviderTest.TestSimple();
      //var test = new ToTextProviderTest.Test ("Test with class", 99999);
      var simpleTest2 = new ToTextProviderTest.TestSimple ("simple Test", 987654321);
      toTextBuilder.sb ("[", "", ",", "", "]").e ("hello").e (toTextBuilder.SequenceState.Counter);
      toTextBuilder.sb ("<", "(", ";(", ")", ">").e ("a variable").e ("simpleTest", simpleTest).e ("was here and").e ("simpleTest2", simpleTest2).e ("here").e (
          toTextBuilder.SequenceState.Counter).se();
      toTextBuilder.e ("world").e (toTextBuilder.SequenceState.Counter).e (toTextBuilder.SequenceState.Counter).se();
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (
          result,
          Is.EqualTo (
              "[hello,1,<(a variable);(simpleTest=((TestSimple) Name:ABC abc,Int:54321));(was here and);(simpleTest2=((TestSimple) Name:simple Test,Int:987654321));(here);(5)>,world,4,5]"
      ));
    }

    [Test]
    public void SequencesWithAppendToTextTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      //toTextBuilder.ToTextProvider.UseAutomaticObjectToText = true;
      toTextBuilder.sb ("[", "", ",", "", "]").e ("ABC").e (toTextBuilder.SequenceState.Counter).e ("DEFG").se();
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("[ABC,1,DEFG]"));
    }

    [Test]
    public void NestedSequencesWithAppendToTextTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      //toTextBuilder.ToTextProvider.UseAutomaticObjectToText = true;
      var simpleTest = new ToTextProviderTest.TestSimple();
      var simpleTest2 = new ToTextProviderTest.TestSimple ("simple Test",987654321);
      //var test = new ToTextProviderTest.Test ("Test with class", 99999);
      toTextBuilder.sb ("[", "", ",", "", "]").e ("hello").e (toTextBuilder.SequenceState.Counter);
      toTextBuilder.sb ("<", "(", ";(", ")", ">").e ("a variable").e ("simpleTest", simpleTest).e ("was here and").e ("simpleTest2", simpleTest2).e ("here").e (
          toTextBuilder.SequenceState.Counter).se();
      toTextBuilder.e ("world").e (toTextBuilder.SequenceState.Counter).e (toTextBuilder.SequenceState.Counter).se();
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result,
          Is.EqualTo (
              "[hello,1,<(a variable);(simpleTest=((TestSimple) Name:ABC abc,Int:54321));(was here and);(simpleTest2=((TestSimple) Name:simple Test,Int:987654321));(here);(5)>,world,4,5]"
      ));
    }

    [Test]
    [ExpectedException (typeof (Remotion.Utilities.AssertionException), ExpectedMessage = "Assertion failed: Expression evaluates to true.")]
    public void NotInSequenceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb ("[", "", ",", "", "]").e ("hello").e (toTextBuilder.SequenceState.Counter); // missing se()
      var result = toTextBuilder.CheckAndConvertToString();
    }

    private void EscapeString(string s, ToTextBuilder toTextBuilder)
    {
      var mapping = new Dictionary<char, string> () { { '"', "\\\"" }, { '\n', "\\n" }, { '\r', "\\r" }, { '\t', "\\t" }, { '\\', "\\\\" }, {'\b',"\\b"}, {'\v',"\\v"}, {'\f',"\\f"} };
      toTextBuilder.WriteRawElementBegin();
      foreach (char c in s)
      {
        string mappedString;
        mapping.TryGetValue (c, out mappedString);
        if (mappedString == null)
        {
          toTextBuilder.WriteRawChar (c);
        }
        else
        {
          toTextBuilder.WriteRawString (mappedString);
        }
      }
      toTextBuilder.WriteRawElementEnd ();
    }

    [Test]
    public void AutoEscapeStringBaseTest ()
    {
      var testString = "abcdEFG\t\n\"\\ HIJklmn \t\t\n\n\"\"\\\\ \r \b\v\f";
      var toTextBuilder = CreateTextBuilder ();
      EscapeString (testString, toTextBuilder);
      var result = toTextBuilder.CheckAndConvertToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("abcdEFG\\t\\n\\\"\\\\ HIJklmn \\t\\t\\n\\n\\\"\\\"\\\\\\\\ \\r \\b\\v\\f"));
    }

    [Test]
    public void EscapeStringTest ()
    {
      var testString = "abcdEFG\t\n\"\\ HIJklmn \t\t\n\n\"\"\\\\ \r \b\v\f";
      var toTextBuilder = CreateTextBuilder ();
      toTextBuilder.sEsc (testString);
      var result = toTextBuilder.CheckAndConvertToString ();
      Log (result);
      Assert.That (result, Is.EqualTo ("abcdEFG\\t\\n\\\"\\\\ HIJklmn \\t\\t\\n\\n\\\"\\\"\\\\\\\\ \\r \\b\\v\\f"));
    }



    public static ToTextBuilder CreateTextBuilder ()
    {
      var toTextProvider = new ToTextProvider();
      toTextProvider.Settings.UseAutomaticObjectToText = false;
      toTextProvider.Settings.UseAutomaticStringEnclosing = false;
      toTextProvider.Settings.UseAutomaticCharEnclosing = false;
      return new ToTextBuilder (toTextProvider);
    }


    // Logging
    //private static readonly ILog s_log = LogManager.GetLogger (typeof (ToTextBuilderTest));
    //static ToTextBuilderTest () { LogManager.InitializeConsole (); }
    //public static void Log (string s) { s_log.Info (s); }
    //public static void Log (string s) { System.Console.WriteLine(s); }
    public void Log (string s)
    {
      log.It (s);
    }
  }
}
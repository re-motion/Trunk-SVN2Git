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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.Logging;
using Remotion.Development.UnitTesting.ObjectMother;
using Remotion.Diagnostic;

using List = Remotion.Development.UnitTesting.ObjectMother.List;


namespace Remotion.UnitTests.Diagnostic
{
  [NUnit.Framework.TestFixture]
  public class ToTextBuilderTest
  {
    private ISimpleLogger log = SimpleLogger.Create (true);

    [Test]
    public void StringTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.s ("ABC defg");
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("ABC defg"));
    }

    [Test]
    public void ToTextStringTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.ToTextString ("XyZ !");
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("XyZ !"));
    }

    [Test]
    public void sTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.s ("A");
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("A"));
    }

    [Test]
    public void nlTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.nl.s ("");
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo (System.Environment.NewLine));
    }

    [Test]
    public void spaceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.space.s ("");
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo (" "));
    }

    [Test]
    public void tabTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.tab.s ("");
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("\t"));
    }


    [Test]
    public void SeperatorTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.seperator.comma.colon.semicolon.s ("");
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo (",,:;"));
    }


    [Test]
    public void StringConcatenationTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.s ("A").s ("B").s ("C");
      var result = toTextBuilder.ToString();
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
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("[123,456,789,Text]"));
    }


    [Test]
    public void MultiTest ()
    {
      int i = 987654321;
      double f = 3.14;
      string s = "Text";

      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.s ("START-").nl.tab.sf ("[{0};{1};{2}]", i, f, s).space.s ("-END");
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("START-" + Environment.NewLine + "\t[987654321;3,14;Text] -END"));
    }


    [Test]
    public void tsTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var o = new Object();
      toTextBuilder.ts (o);
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo (o.ToString()));
    }


    [Test]
    public void ToTextTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var o = new Object();
      toTextBuilder.tt (o);
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo (o.ToString()));
    }

    [Test]
    public void MemberTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var o = new Object();
      toTextBuilder.m (o);
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo (o.ToString()));
    }

    [Test]
    public void MemberTest2 ()
    {
      var toTextBuilder = CreateTextBuilder();
      //var list = new List<int> () { 5, 3, 1 };
      var myList = List.New (5, 3, 1);
      toTextBuilder.m ("myList", myList);
      var result = toTextBuilder.ToString();
      //Assert.That (result, Is.EqualTo ("myList:{5,3,1}"));
      Assert.That (result, Is.EqualTo ("myList={5,3,1}"));
    }

    [Test]
    public void MemberInSequenceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var myList = List.New (5, 3, 1);
      toTextBuilder.sb().tt ("Abra").m ("myList", myList).m ("myList", myList).tt ("Kadabra").se();
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("(Abra,myList={5,3,1},myList={5,3,1},Kadabra)"));
    }

    [Test]
    public void AppendCollectionTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = List.New (5, 3, 1, 11, 13, 17);
      toTextBuilder.AppendEnumerable (list);
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{5,3,1,11,13,17}"));
    }

    [Test]
    public void AppendNestedCollectionTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = List.New (List.New (5, 3, 1), List.New (11, 13, 17));
      toTextBuilder.AppendEnumerable (list);
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{5,3,1},{11,13,17}}"));
    }

    [Test]
    public void AppendNestedCollectionTest2 ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = List.New (List.New (5, 3, 1), List.New (11, 13, 17), List.New (19, 23, 29));
      toTextBuilder.AppendEnumerable (list);
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{5,3,1},{11,13,17},{19,23,29}}"));
    }

    [Test]
    public void AppendNestedCollectionTest3 ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = List.New (List.New (List.New (5, 3, 1), List.New (11, 13, 17)), List.New (List.New (19, 23, 29), List.New (31, 37, 41)));
      toTextBuilder.AppendEnumerable (list);
      var result = toTextBuilder.ToString();
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
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{{1,2,3},{4,5,6},{7,8,9}},{{10,11,12},{13,14,15},{16,17,18}},{{19,20,21},{22,23,24},{25,26,27}}}"));
    }

    [Test]
    public void collectionTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = List.New (List.New (List.New ("A", "B", "C")));
      toTextBuilder.collection (list);
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{{A,B,C}}}"));
    }

    [Test]
    public void AppendArray2DTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      //var array = new int[][] {new int[] {1,2},new int[] {3,4},new int[] {5,6}};
      var array = new[] {new[] {1, 2}, new[] {3, 4}, new[] {5, 6}};
      toTextBuilder.AppendArray (array);
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{1,2},{3,4},{5,6}}"));
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
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{{1,3},{5,7}},{{11,13},{17,19}},{{23,29},{31,37}}}"));
    }

    [Test]
    public void AppendRectangularArrayTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      int[,,] array = { { { 1, 3 }, { 5, 7 } }, { { 11, 13 }, { 17, 19 } }, { { 23, 29 }, { 31, 37 } } };
      toTextBuilder.AppendArray (array);
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{{1,3},{5,7}},{{11,13},{17,19}},{{23,29},{31,37}}}"));
    }

    [Test]
    public void AppendRectangularArrayTagTest ()
    {
      int[,,] array = { { { 1, 3 }, { 5, 7 } }, { { 11, 13 }, { 17, 19 } }, { { 23, 29 }, { 31, 37 } } };

      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.ArrayPrefix = "{";
      //toTextBuilder.ArrayFirstElementPrefix = "(";
      toTextBuilder.ArrayFirstElementPrefix = "(";
      toTextBuilder.ArrayOtherElementPrefix = ",(";
      toTextBuilder.ArrayElementPostfix = ")";
      toTextBuilder.ArrayPostfix = "}";

      toTextBuilder.AppendArray (array);
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{({({(1),(3)}),({(5),(7)})}),({({(11),(13)}),({(17),(19)})}),({({(23),(29)}),({(31),(37)})})}"));
    }


    [Test]
    public void collectionTest2 ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = List.New (List.New (List.New ("A", "B", "C")));
      toTextBuilder.collection (list);
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{{A,B,C}}}"));
    }


    [Test]
    public void EnumerableTagTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = List.New (List.New (5, 3, 1), List.New (11, 13, 17));
      toTextBuilder.EnumerableBegin = "(";
      toTextBuilder.EnumerableSeparator = ";";
      toTextBuilder.EnumerableEnd = ")";

      toTextBuilder.AppendEnumerable (list);
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("((5;3;1);(11;13;17))"));
    }


    [Test]
    public void ArrayTagTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var array = new[] {
          new[] {new[] {1, 3}, new[] {5, 7}},
          new[] {new[] {11, 13}, new[] {17, 19}},
          new[] {new[] {23, 29}, new[] {31, 37}}};
      toTextBuilder.ArrayBegin = "<{[";
      toTextBuilder.ArraySeparator = "§|§";
      toTextBuilder.ArrayEnd = "]}>";

      toTextBuilder.AppendArray (array);
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (
          result, Is.EqualTo ("<{[<{[<{[1§|§3]}>§|§<{[5§|§7]}>]}>§|§<{[<{[11§|§13]}>§|§<{[17§|§19]}>]}>§|§<{[<{[23§|§29]}>§|§<{[31§|§37]}>]}>]}>"));
    }

    [Test]
    public void ArrayTagTest2 ()
    {
      var toTextBuilder = CreateTextBuilder();
      var array = new[] {new[] {new[] {1, 3, 5}, new[] {5, 7, 11}}};
      toTextBuilder.ArrayPrefix = "<";
      toTextBuilder.ArrayFirstElementPrefix = "[";
      toTextBuilder.ArrayOtherElementPrefix = ";[";
      toTextBuilder.ArrayElementPostfix = "]";
      toTextBuilder.ArrayPostfix = ">";

      toTextBuilder.AppendArray (array);
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("<[<[<[1];[3];[5]>];[<[5];[7];[11]>]>]>"));
    }


    [Test]
    public void SinglelineMultilineTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.UseMultiLine = false;
      toTextBuilder.s ("Hello").nl.s (" world");
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("Hello world"));

      toTextBuilder.UseMultiLine = true;
      toTextBuilder.s (" here comes the").nl.s ("newline");
      var result2 = toTextBuilder.ToString();
      Log (result2);
      Assert.That (result2, Is.EqualTo ("Hello world here comes the" + Environment.NewLine + "newline"));
    }

    [Test]
    public void EnabledTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.s ("test");
      Assert.That (toTextBuilder.ToString(), Is.EqualTo ("test"));
      toTextBuilder.Enabled = false;
      toTextBuilder.s ("test");
      Assert.That (toTextBuilder.ToString(), Is.EqualTo ("test"));
      toTextBuilder.Enabled = true;
      toTextBuilder.s ("test");
      Assert.That (toTextBuilder.ToString(), Is.EqualTo ("testtest"));
    }

    [Test]
    public void OutputComplexityTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.OutputComplex();
      Assert.That (toTextBuilder.OutputComplexity, Is.EqualTo (ToTextBuilder.OutputComplexityLevel.Complex));
    }

    [Test]
    public void ComplexityFilteringSettingTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      Assert.That (toTextBuilder.Enabled, Is.EqualTo (true));
      toTextBuilder.OutputSkeleton();
      toTextBuilder.AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilder.OutputComplexityLevel.Basic);
      Assert.That (toTextBuilder.Enabled, Is.EqualTo (false));

      toTextBuilder.OutputBasic();
      toTextBuilder.AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilder.OutputComplexityLevel.Basic);
      Assert.That (toTextBuilder.Enabled, Is.EqualTo (true));
    }


    public ToTextBuilder AllFilterLevelsFilteredOutput (ToTextBuilder toTextBuilder)
    {
      if (toTextBuilder == null)
        toTextBuilder = CreateTextBuilder();
      toTextBuilder.cBasic.s ("b").comma.cComplex.s ("c").comma.cFull.s ("f").comma.cMedium.s ("m").comma.cSkeleton.s ("s");
      var result = toTextBuilder.ToString();
      Log (result);
      return toTextBuilder;
    }


    [Test]
    public void ComplexityFilteringTest ()
    {
      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.OutputDisable();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString();
        Assert.That (result, Is.EqualTo (""));
      }

      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.OutputSkeleton();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString();
        Assert.That (result, Is.EqualTo ("s"));
      }

      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.OutputBasic();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString();
        Assert.That (result, Is.EqualTo ("b,s"));
      }

      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.OutputMedium();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString();
        Assert.That (result, Is.EqualTo ("b,m,s"));
      }

      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.OutputComplex();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString();
        Assert.That (result, Is.EqualTo ("b,c,m,s"));
      }

      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.OutputFull();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString();
        Assert.That (result, Is.EqualTo ("b,c,f,m,s"));
      }
    }


    [Test]
    public void AppendObjectTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var o = new Object();
      Assert.That (toTextBuilder.Append (o).ToString(), Is.EqualTo (o.ToString()));
    }


    [Test]
    public void beginInstanceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var obj = new object();
      toTextBuilder.beginInstance (obj.GetType()).endInstance();
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("[Object]")); // NUnit.Framework.SyntaxHelpers.Text.Contains("[ToTextBuilder"));
    }

    //[Test]
    //public void endInstanceTest ()
    //{
    //  var toTextBuilder = CreateTextBuilder ();
    //  toTextBuilder.endInstance ();
    //  var result = toTextBuilder.ToString ();
    //  Log (result);
    //  Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("]"));
    //}


    [Test]
    public void SequenceStateTest ()
    {
      var sequenceState = new ToTextBuilder.SequenceStateHolder ("Start", "FirstPrefix", "OtherPrefix", "Postfix", "End");
      Assert.That (sequenceState.Counter, Is.EqualTo (0));
      Assert.That (sequenceState.SequencePrefix, Is.EqualTo ("Start"));
      Assert.That (sequenceState.FirstElementPrefix, Is.EqualTo ("FirstPrefix"));
      Assert.That (sequenceState.OtherElementPrefix, Is.EqualTo ("OtherPrefix"));
      Assert.That (sequenceState.ElementPostfix, Is.EqualTo ("Postfix"));
      Assert.That (sequenceState.SequencePostfix, Is.EqualTo ("End"));
    }

    //[Test]
    //public void SequenceStackSequenceBeginTest ()
    //{
    //  var toTextBuilder = CreateTextBuilder ();
    //  toTextBuilder.AppendSequenceBegin ("", "", ",", "", "");
    //  Assert.That (toTextBuilder._sequenceStack.Count, Is.EqualTo (1));
    //}

    //[Test]
    //public void SequenceStackSequenceEndTest ()
    //{
    //  var toTextBuilder = CreateTextBuilder ();
    //  toTextBuilder.AppendSequenceBegin ("", "", ",", "", "");
    //  Assert.That (toTextBuilder._sequenceStack.Count, Is.EqualTo (1));
    //  toTextBuilder.AppendSequenceEnd ();
    //  Assert.That (toTextBuilder._sequenceStack.Count, Is.EqualTo (0));
    //}

    [Test]
    public void SequenceShorthandTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb ("<{[", "", ",", "", "]}>").e ("hello").e ("world").e (1).e (2).e (3).se();
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("<{[hello,world,1,2,3]}>"));
    }

    [Test]
    public void SequenceTest1 ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb ("<", "(", ",(", ")", ">").e ("hello").e ("world").e (1).e (2).e (3).se();
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("<(hello),(world),(1),(2),(3)>"));
    }


    [Test]
    public void BraketTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb ("", "|", "|", ">", "").elementsNumbered ("a", 1, 5).se();
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("|a1>|a2>|a3>|a4>|a5>"));
    }


    [Test]
    public void SequenceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.AppendSequenceBegin ("", "", ",", "", "");
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (0));
      toTextBuilder.e ("1");
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (1));
      toTextBuilder.e (2);
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (2));
      toTextBuilder.e ("drei");
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (3));
      toTextBuilder.AppendSequenceEnd();
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("1,2,drei"));
    }

    [Test]
    public void SequenceCounterUpdateTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var dreiString = "drei";
      toTextBuilder.AppendSequenceBegin ("", "", ",", "", "");
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (0));
      toTextBuilder.tt ("1");
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (1));
      toTextBuilder.e (2);
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (2));
      toTextBuilder.m (dreiString);
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (3));
      toTextBuilder.AppendSequenceEnd();
      var result = toTextBuilder.ToString();
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
      var result = toTextBuilder.ToString();
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
      toTextBuilder.sb ("[", "", ",", "", "]").AppendSequenceElements ("a", 2, "b", 3, "c").se();
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("[a,2,b,3,c]"));
    }

    [Test]
    public void SimpleSequenceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb ("<", ">").tt ("ABC").tt (toTextBuilder.SequenceState.Counter).tt ("DEFG").se();
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("<ABC,1,DEFG>"));
    }

    [Test]
    public void SimpleSequenceTest2 ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb ("<", ";", ">").tt ("ABC").tt (toTextBuilder.SequenceState.Counter).tt ("DEFG").se();
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("<ABC;1;DEFG>"));
    }

    [Test]
    public void StandardSequenceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb().tt ("ABC").tt (toTextBuilder.SequenceState.Counter).tt ("DEFG").se();
      var result = toTextBuilder.ToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("(ABC,1,DEFG)"));
    }

    [Test]
    public void elementsTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb ("[", "", ",", "", "]").elements ("a", 2, "b", 3, "c").se();
      var result = toTextBuilder.ToString();
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
      toTextBuilder.sb ("<", "(", ";(", ")", ">").e ("a variable").m ("simpleTest", simpleTest).e ("was here and").m ("simpleTest2", simpleTest2).e ("here").e (
          toTextBuilder.SequenceState.Counter).se();
      toTextBuilder.e ("world").e (toTextBuilder.SequenceState.Counter).e (toTextBuilder.SequenceState.Counter).se();
      var result = toTextBuilder.ToString();
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
      toTextBuilder.sb ("[", "", ",", "", "]").tt ("ABC").tt (toTextBuilder.SequenceState.Counter).tt ("DEFG").se();
      var result = toTextBuilder.ToString();
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
      toTextBuilder.sb ("[", "", ",", "", "]").tt ("hello").tt (toTextBuilder.SequenceState.Counter);
      toTextBuilder.sb ("<", "(", ";(", ")", ">").tt ("a variable").m ("simpleTest", simpleTest).tt ("was here and").m ("simpleTest2", simpleTest2).tt ("here").tt (
          toTextBuilder.SequenceState.Counter).se();
      toTextBuilder.tt ("world").tt (toTextBuilder.SequenceState.Counter).tt (toTextBuilder.SequenceState.Counter).se();
      var result = toTextBuilder.ToString();
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
      toTextBuilder.sb ("[", "", ",", "", "]").tt ("hello").tt (toTextBuilder.SequenceState.Counter); // missing se()
      var result = toTextBuilder.ToString();
    }


    public static ToTextBuilder CreateTextBuilder ()
    {
      var toTextProvider = new ToTextProvider();
      toTextProvider.UseAutomaticObjectToText = false;
      toTextProvider.UseAutomaticStringEnclosing = false;
      toTextProvider.UseAutomaticCharEnclosing = false;
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
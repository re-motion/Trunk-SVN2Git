// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.Logging;
using Remotion.Development.UnitTesting.ObjectMother;
using Remotion.Diagnostics.ToText;
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.Reflection;
using Remotion.UnitTests.Diagnostics.TestDomain;
using Remotion.Utilities.PropertyRestorer;


namespace Remotion.UnitTests.Diagnostics
{
  [TestFixture]
  public class ToTextBuilderTest
  {
    private ISimpleLogger log = SimpleLogger.CreateForConsole (false);

    [Test]
    public void StringTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.s ("ABC defg");
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("ABC defg"));
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
      toTextBuilder.nl().s ("");
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo (System.Environment.NewLine));
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
    public void ToTextTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var o = new Object();
      toTextBuilder.e (o);
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo (o.ToString()));
    }

    [Test]
    public void MemberTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var o = new Object();
      toTextBuilder.e (o);
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo (o.ToString()));
    }

    [Test]
    public void MemberTest2 ()
    {
      var toTextBuilder = CreateTextBuilder();
      var myList = ListMother.New (5, 3, 1);
      toTextBuilder.e ("myList", myList);
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("myList={5,3,1}"));
    }

    [Test]
    public void MemberVariableTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      var theAnswer = 42;
      toTextBuilder.e (() => theAnswer);
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo ("theAnswer=42"));
    }

    [Test]
    public void MemberVariableTest2 ()
    {
      var toTextBuilder = CreateTextBuilder ();
      var myList = ListMother.New (5, 3, 1);
      toTextBuilder.e (() => myList);
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo ("myList={5,3,1}"));
    }




    [Test]
    public void MemberInSequenceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var myList = ListMother.New (5, 3, 1);
      toTextBuilder.sb().e ("Abra").e ("myList", myList).e ("myList", myList).e ("Kadabra").se();
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("(Abra,myList={5,3,1},myList={5,3,1},Kadabra)"));
    }

    [Test]
    public void AppendCollectionTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = ListMother.New (5, 3, 1, 11, 13, 17);
      toTextBuilder.WriteEnumerable (list);
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("{5,3,1,11,13,17}"));
    }

    [Test]
    public void WriteDictionaryTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var dictionary = DictionaryMother.New ("a",11,"b",22,"C",33);
      toTextBuilder.WriteDictionary (dictionary);
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("{a:11,b:22,C:33}"));
    }



    [Test]
    public void AppendNestedCollectionTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = ListMother.New (ListMother.New (5, 3, 1), ListMother.New (11, 13, 17));
      toTextBuilder.WriteEnumerable (list);
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("{{5,3,1},{11,13,17}}"));
    }

    [Test]
    public void AppendNestedCollectionStringMemberTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      var list = ListMother.New (ListMother.New ("5", "3", "1"), ListMother.New ("11", "13", "17"));
      toTextBuilder.WriteEnumerable (list);
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo ("{{5,3,1},{11,13,17}}"));
    }


    [Test]
    public void AppendNestedCollectionTest2 ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = ListMother.New (ListMother.New (5, 3, 1), ListMother.New (11, 13, 17), ListMother.New (19, 23, 29));
      toTextBuilder.WriteEnumerable (list);
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("{{5,3,1},{11,13,17},{19,23,29}}"));
    }

    [Test]
    public void AppendNestedCollectionTest3 ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = ListMother.New (ListMother.New (ListMother.New (5, 3, 1), ListMother.New (11, 13, 17)), ListMother.New (ListMother.New (19, 23, 29), ListMother.New (31, 37, 41)));
      toTextBuilder.WriteEnumerable (list);
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("{{{5,3,1},{11,13,17}},{{19,23,29},{31,37,41}}}"));
    }

    [Test]
    public void AppendNestedCollectionTest4 ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = ListMother.New (
          ListMother.New (ListMother.New (1, 2, 3), ListMother.New (4, 5, 6), ListMother.New (7, 8, 9)),
          ListMother.New (ListMother.New (10, 11, 12), ListMother.New (13, 14, 15), ListMother.New (16, 17, 18)),
          ListMother.New (ListMother.New (19, 20, 21), ListMother.New (22, 23, 24), ListMother.New (25, 26, 27))
          );
      toTextBuilder.WriteEnumerable (list);
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("{{{1,2,3},{4,5,6},{7,8,9}},{{10,11,12},{13,14,15},{16,17,18}},{{19,20,21},{22,23,24},{25,26,27}}}"));
    }

    [Test]
    public void collectionTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = ListMother.New (ListMother.New (ListMother.New ("A", "B", "C")));
      toTextBuilder.enumerable (list);
      var result = toTextBuilder.ToString();
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
      toTextBuilder.WriteArray (array);
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("{{{1,3},{5,7}},{{11,13},{17,19}},{{23,29},{31,37}}}"));
    }

    [Test]
    public void AppendRectangularArrayTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      int[,,] array = { { { 1, 3 }, { 5, 7 } }, { { 11, 13 }, { 17, 19 } }, { { 23, 29 }, { 31, 37 } } };
      toTextBuilder.WriteArray (array);
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("{{{1,3},{5,7}},{{11,13},{17,19}},{{23,29},{31,37}}}"));
    }

    [Test]
    public void AppendRectangularArrayTagTest ()
    {
      int[,,] array = { { { 1, 3 }, { 5, 7 } }, { { 11, 13 }, { 17, 19 } }, { { 23, 29 }, { 31, 37 } } };

      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.Settings.ArrayPrefix = "{";
      //toTextBuilder.
      //toTextBuilder.ArrayElementPrefix = "(";
      toTextBuilder.Settings.ArrayElementPrefix = "(";
      toTextBuilder.Settings.ArrayElementPostfix = ")";
      toTextBuilder.Settings.ArraySeparator = ",";
      toTextBuilder.Settings.ArrayPostfix = "}";

      toTextBuilder.WriteArray (array);
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("{({({(1),(3)}),({(5),(7)})}),({({(11),(13)}),({(17),(19)})}),({({(23),(29)}),({(31),(37)})})}"));
    }


    [Test]
    public void collectionTest2 ()
    {
      var toTextBuilder = CreateTextBuilder();
      var list = ListMother.New (ListMother.New (ListMother.New ("A", "B", "C")));
      toTextBuilder.enumerable (list);
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("{{{A,B,C}}}"));
    }



    [Test]
    public void ArrayTagTest2 ()
    {
      var toTextBuilder = CreateTextBuilder();
      var array = new[] {new[] {new[] {1, 3, 5}, new[] {5, 7, 11}}};
      toTextBuilder.Settings.ArrayPrefix = "<";
      toTextBuilder.Settings.ArrayElementPrefix = "[";
      toTextBuilder.Settings.ArrayElementPostfix = "]";
      toTextBuilder.Settings.ArraySeparator = ";";
      toTextBuilder.Settings.ArrayPostfix = ">";

      toTextBuilder.WriteArray (array);
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("<[<[<[1];[3];[5]>];[<[5];[7];[11]>]>]>"));
    }


    [Test]
    public void SinglelineMultilineTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.AllowNewline = false;
      toTextBuilder.s ("Hello").nl().s (" world");
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("Hello world"));

      toTextBuilder.AllowNewline = true;
      toTextBuilder.s (" here comes the").nl().s ("newline");
      var result2 = toTextBuilder.ToString();
      Assert.That (result2, Is.EqualTo ("Hello world here comes the" + Environment.NewLine + "newline"));
    }

    [Test]
    public void MultipleMultilineTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      toTextBuilder.AllowNewline = true;

      toTextBuilder.nl (3);
      string result = toTextBuilder.ToString ();
      string sExpected = ((Func<string, string>) (s => s + s + s)) (Environment.NewLine);
      To.Console.sEsc (result).s (",").sEsc (sExpected);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains (sExpected));
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
      toTextBuilder.SetOutputComplexityToComplex();
      Assert.That (toTextBuilder.OutputComplexity, Is.EqualTo (ToTextBuilder.ToTextBuilderOutputComplexityLevel.Complex));
      toTextBuilder.SetOutputComplexityToBasic ();
      Assert.That (toTextBuilder.OutputComplexity, Is.EqualTo (ToTextBuilder.ToTextBuilderOutputComplexityLevel.Basic));
    }

    [Test]
    public void ComplexityFilteringSettingTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      Assert.That (toTextBuilder.Enabled, Is.EqualTo (true));
      toTextBuilder.SetOutputComplexityToSkeleton();
      toTextBuilder.WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilder.ToTextBuilderOutputComplexityLevel.Basic);
      Assert.That (toTextBuilder.Enabled, Is.EqualTo (false));

      toTextBuilder.SetOutputComplexityToBasic();
      toTextBuilder.WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilder.ToTextBuilderOutputComplexityLevel.Basic);
      Assert.That (toTextBuilder.Enabled, Is.EqualTo (true));
    }


    public ToTextBuilder AllFilterLevelsFilteredOutput (ToTextBuilder toTextBuilder)
    {
      if (toTextBuilder == null)
        toTextBuilder = CreateTextBuilder();
      toTextBuilder.writeIfBasicOrHigher.s ("b").writeIfComplexOrHigher.s ("c").writeIfFull.s ("f").writeIfMediumOrHigher.s ("m").writeIfSkeletonOrHigher.s ("s");
      var result = toTextBuilder.ToString();
      return toTextBuilder;
    }


    [Test]
    public void ComplexityFilteringTest ()
    {
      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.SetOutputComplexityToDisable();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString ();
        Assert.That (result, Is.EqualTo (""));
      }

      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.SetOutputComplexityToSkeleton();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString ();
        Assert.That (result, Is.EqualTo ("s"));
      }

      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.SetOutputComplexityToBasic();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString ();
        Assert.That (result, Is.EqualTo ("bs"));
      }

      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.SetOutputComplexityToMedium();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString ();
        Assert.That (result, Is.EqualTo ("bms"));
      }

      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.SetOutputComplexityToComplex();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString ();
        Assert.That (result, Is.EqualTo ("bcms"));
      }

      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.SetOutputComplexityToFull();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString ();
        Assert.That (result, Is.EqualTo ("bcfms"));
      }
    }

    public ToTextBuilder SequenceAllFilterLevelsFilteredOutput (ToTextBuilder toTextBuilder)
    {
      if (toTextBuilder == null)
        toTextBuilder = CreateTextBuilder ();
      toTextBuilder.sb().e ("before");
      toTextBuilder.sb().e ("start");
      toTextBuilder.writeIfMediumOrHigher.e ("m").writeIfSkeletonOrHigher.e ("s");
      toTextBuilder.writeIfFull.sb ().e ("1").e ("2").e ("3").se ();
      toTextBuilder.writeIfBasicOrHigher.e ("b").writeIfComplexOrHigher.e ("c").writeIfFull.e ("f");
      toTextBuilder.e ("end").se ();
      toTextBuilder.e ("after").se ();

      var result = toTextBuilder.ToString ();
      return toTextBuilder;
    }

    [Test]
    public void SequenceComplexityFilteringTest ()
    {
      {
        var toTextBuilder = CreateTextBuilder();
        toTextBuilder.SetOutputComplexityToMedium ();
        var result = SequenceAllFilterLevelsFilteredOutput (toTextBuilder).ToString ();
        Assert.That (result, Is.EqualTo ("(before,(start,m,s,b))"));
      }
    }

    [Test]
    public void AppendObjectTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var o = new Object();
      Assert.That (toTextBuilder.WriteRawElementBegin().WriteRaw (o).WriteRawElementEnd().ToString (), Is.EqualTo (o.ToString ()));
    }


    [Test]
    public void beginInstanceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var obj = new object();
      toTextBuilder.WriteInstanceBegin (obj.GetType (),null).WriteSequenceEnd ();
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("[Object]")); // NUnit.Framework.SyntaxHelpers.Text.Contains("[ToTextBuilder"));
    }

    [Test]
    public void ibGenericTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      toTextBuilder.ib<Test>().ie();
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo ("[Test]")); // NUnit.Framework.SyntaxHelpers.Text.Contains("[ToTextBuilder"));
    }

    [Test]
    public void ibShortName ()
    {
      var toTextBuilder = CreateTextBuilder ();
      toTextBuilder.ib<Test> ("ShortName").ie ();
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo ("[ShortName]")); // NUnit.Framework.SyntaxHelpers.Text.Contains("[ToTextBuilder"));
    }


    [Test]
    public void SequenceStateTest ()
    {
      var sequenceState = new SequenceStateHolder ("Name", "Start", "Prefix", "Postfix", "Separator", "Close",true);
      Assert.That (sequenceState.Counter, Is.EqualTo (0));
      Assert.That (sequenceState.Name, Is.EqualTo ("Name"));
      Assert.That (sequenceState.SequencePrefix, Is.EqualTo ("Start"));
      Assert.That (sequenceState.ElementPrefix, Is.EqualTo ("Prefix"));
      Assert.That (sequenceState.ElementPostfix, Is.EqualTo ("Postfix"));
      Assert.That (sequenceState.Separator, Is.EqualTo ("Separator"));
      Assert.That (sequenceState.SequencePostfix, Is.EqualTo ("Close"));
      Assert.That (sequenceState.SequenceStartWritten, Is.EqualTo (true));
    }

    [Test]
    public void SequenceShorthandTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sbLiteral ("<{[", ",", "]}>").e ("hello").e ("world").e (1).e (2).e (3).se();
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("<{[hello,world,1,2,3]}>"));
    }

    [Test]
    public void SequenceTest1 ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sbLiteral ("<", "(", ")", ",", ">").e ("hello").e ("world").e (1).e (2).e (3).se ();
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("<(hello),(world),(1),(2),(3)>"));
    }


    [Test]
    public void BraketTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sbLiteral ("", "|", ">", "", "").elementsNumbered ("a", 1, 5).se();
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("|a1>|a2>|a3>|a4>|a5>"));
    }


    [Test]
    public void SequenceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.WriteSequenceLiteralBegin ("", "", "", "", ",", "");
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (0));
      toTextBuilder.e ("1");
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (1));
      toTextBuilder.e (2);
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (2));
      toTextBuilder.e ("drei");
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (3));
      toTextBuilder.WriteSequenceEnd();
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("1,2,drei"));
    }

    [Test]
    public void SequenceCounterUpdateTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var dreiString = "drei";
      toTextBuilder.WriteSequenceLiteralBegin ("", "", "", "", ",", "");
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (0));
      toTextBuilder.e ("1");
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (1));
      toTextBuilder.e (2);
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (2));
      toTextBuilder.e (dreiString);
      Assert.That (toTextBuilder.SequenceState.Counter, Is.EqualTo (3));
      toTextBuilder.WriteSequenceEnd();
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("1,2,drei"));
    }

    [Test]
    public void NestedSequencesTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sbLiteral ("[", "", "", ",", "]").e ("hello").e (toTextBuilder.SequenceState.Counter);
      toTextBuilder.sbLiteral ("<", "(", ")", ";", ">").e ("hello").e ("world").e (toTextBuilder.SequenceState.Counter).e (
          toTextBuilder.SequenceState.Counter).e (toTextBuilder.SequenceState.Counter).se();
      toTextBuilder.e ("world").e (toTextBuilder.SequenceState.Counter).e (toTextBuilder.SequenceState.Counter).se();
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("[hello,1,<(hello);(world);(2);(3);(4)>,world,4,5]"));
    }

    [Test]
    public void IsInSequenceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sbLiteral ("[", "", "", ",", "]");
      Assert.That (toTextBuilder.IsInSequence, Is.True);
      toTextBuilder.sbLiteral ("[", "", "", ",", "]");
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
      toTextBuilder.sbLiteral ("[", "", "", ",", "]").WriteSequenceElements ("a", 2, "b", 3, "c").se();
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("[a,2,b,3,c]"));
    }

    [Test]
    public void SimpleSequenceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sbLiteral ("<", ">").e ("ABC").e (toTextBuilder.SequenceState.Counter).e ("DEFG").se();
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("<ABC,1,DEFG>"));
    }

    [Test]
    public void SimpleSequenceTest2 ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sbLiteral ("<", ";", ">").e ("ABC").e (toTextBuilder.SequenceState.Counter).e ("DEFG").se();
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("<ABC;1;DEFG>"));
    }

    [Test]
    public void StandardSequenceTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sb().e ("ABC").e (toTextBuilder.SequenceState.Counter).e ("DEFG").se();
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("(ABC,1,DEFG)"));
    }

    [Test]
    public void elementsTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      toTextBuilder.sbLiteral ("[", "", "", ",", "]").elements ("a", 2, "b", 3, "c").se();
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("[a,2,b,3,c]"));
    }

    [Test]
    public void sequenceTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      toTextBuilder.sequence("a", 2, "b", 3, "c");
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo ("(a,2,b,3,c)"));
    }


    [Test]
    public void NestedSequencesWitMembersTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var simpleTest = new TestSimple();
      var simpleTest2 = new TestSimple ("simple Test", 987654321);
      toTextBuilder.sbLiteral ("[", "", "", ",", "]").e ("hello").e (toTextBuilder.SequenceState.Counter);
      toTextBuilder.sbLiteral ("<", "(", ")", ";", ">").e ("a variable").e ("simpleTest", simpleTest).e ("was here and").e ("simpleTest2", simpleTest2).e ("here").e (
          toTextBuilder.SequenceState.Counter).se();
      toTextBuilder.e ("world").e (toTextBuilder.SequenceState.Counter).e (toTextBuilder.SequenceState.Counter).se();
      var result = toTextBuilder.ToString();
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
      toTextBuilder.sbLiteral ("[", "", "", ",", "]").e ("ABC").e (toTextBuilder.SequenceState.Counter).e ("DEFG").se();
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("[ABC,1,DEFG]"));
    }

    [Test]
    public void NestedSequencesWithAppendToTextTest ()
    {
      var toTextBuilder = CreateTextBuilder();
      var simpleTest = new TestSimple();
      var simpleTest2 = new TestSimple ("simple Test",987654321);
      toTextBuilder.sbLiteral ("[", "", "", ",", "]").e ("hello").e (toTextBuilder.SequenceState.Counter);
      toTextBuilder.sbLiteral ("<", "(", ")", ";", ">").e ("a variable").e ("simpleTest", simpleTest).e ("was here and").e ("simpleTest2", simpleTest2).e ("here").e (
          toTextBuilder.SequenceState.Counter).se();
      toTextBuilder.e ("world").e (toTextBuilder.SequenceState.Counter).e (toTextBuilder.SequenceState.Counter).se();
      var result = toTextBuilder.ToString();
      Assert.That (result,
          Is.EqualTo (
              "[hello,1,<(a variable);(simpleTest=((TestSimple) Name:ABC abc,Int:54321));(was here and);(simpleTest2=((TestSimple) Name:simple Test,Int:987654321));(here);(5)>,world,4,5]"
      ));
    }

    //[Test]
    //[ExpectedException (typeof (Remotion.Utilities.AssertionException), ExpectedMessage = "Assertion failed: Expression evaluates to true.")]
    //public void NotInSequenceTest ()
    //{
    //  var toTextBuilder = CreateTextBuilder();
    //  toTextBuilder.sbLiteral ("[", "", "", ",", "]").e ("hello").e (toTextBuilder.SequenceState.Counter); // missing se()
    //  var result = toTextBuilder.ToString();
    //}

    private void EscapeString(string s, ToTextBuilder toTextBuilder)
    {
      var mapping = new Dictionary<char, string> { { '"', "\\\"" }, { '\n', "\\n" }, { '\r', "\\r" }, { '\t', "\\t" }, { '\\', "\\\\" }, {'\b',"\\b"}, {'\v',"\\v"}, {'\f',"\\f"} };
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
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo ("abcdEFG\\t\\n\\\"\\\\ HIJklmn \\t\\t\\n\\n\\\"\\\"\\\\\\\\ \\r \\b\\v\\f"));
    }

    [Test]
    public void EscapeStringTest ()
    {
      var testString = "abcdEFG\t\n\"\\ HIJklmn \t\t\n\n\"\"\\\\ \r \b\v\f";
      var toTextBuilder = CreateTextBuilder ();
      toTextBuilder.sEsc (testString);
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo ("abcdEFG\\t\\n\\\"\\\\ HIJklmn \\t\\t\\n\\n\\\"\\\"\\\\\\\\ \\r \\b\\v\\f"));
    }


    [Test]
    public void ElementOutsideSequenceTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      var someVar = 345.789;
      toTextBuilder.e("xyz").e("abc");
      toTextBuilder.e (() => someVar);
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo ("xyzabcsomeVar=345.789"));
    }

    public class TestTypeHandler : ToTextSpecificTypeHandler<Test>
    {
      public override void ToText (Test accessControlEntry, IToTextBuilder ttb)
      {
        ttb.ib<Test> ().s ("handled by TestTypeHandler").ie ();
      }
    }

    [Test]
    [Ignore]
    public void ToTextSpecificHandlerMapTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      toTextBuilder.ToTextProvider.Settings.UseAutomaticObjectToText = true;
      toTextBuilder.ToTextProvider.Settings.EmitPublicFields = true;
      toTextBuilder.ToTextProvider.Settings.EmitPublicProperties = true;
      toTextBuilder.ToTextProvider.Settings.EmitPrivateFields = true;
      toTextBuilder.ToTextProvider.Settings.EmitPrivateProperties = true;
      var typeHandlerMap = new ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> ();
      typeHandlerMap.Add (typeof (Test), new TestTypeHandler());
      toTextBuilder.e (To.ToTextProvider.typeHandlerMap);
      var result = toTextBuilder.ToString ();
    }



    // Timings: WriteElementBasicPerformanceTest = 270ms, WriteElementFastPerformanceTest = 1100ms
    // Basically same timings for one-char-variable-name holding 3 char string.
    private const int _nrWriteElementPerformanceTestLoops = 100000;
    private const string _myVeryLongVariableName = "ABCDEFGHIJKLMNOPQRST";

    [Test]
    [Explicit ("performance test")]
    public void WriteElementPerformanceTest ()
    {
      var toTextBuilder = CreateTextBuilder ();

      Stopwatch stopwatch = new Stopwatch ();
      stopwatch.Start ();
      for (int i = 0; i < _nrWriteElementPerformanceTestLoops; ++i)
      {
        toTextBuilder.WriteElement ("_myVeryLongVariableName", _myVeryLongVariableName);
      }
      stopwatch.Stop ();
      double milliSeconds = stopwatch.ElapsedMilliseconds;
      To.Console.e (() => milliSeconds);
    }

    [Test]
    [Explicit ("performance test")]
    public void WriteElementWithLambdaExpressionPerformanceTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      Stopwatch stopwatch = new Stopwatch ();
      stopwatch.Start ();
      for (int i = 0; i < _nrWriteElementPerformanceTestLoops; ++i)
      {
        toTextBuilder.WriteElement (() => _myVeryLongVariableName);
      }
      stopwatch.Stop ();
      double milliSeconds = stopwatch.ElapsedMilliseconds;
      To.Console.e (() => milliSeconds);
    }


    [Test]
    [ExpectedException (typeof (System.ObjectDisposedException))]
    public void AlreadyClosedTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      toTextBuilder.Close();
      // ToTextBuilder is disposed => Write attempt throws
      toTextBuilder.s ("Sorry, we're alredy closed !");
    }


    [Test]
    [ExpectedException (typeof (System.ObjectDisposedException))]
    public void DisposableTest ()
    {
      var toTextProvider = new ToTextProvider ();
      var stringWriter = new StringWriter();
      using (var toTextBuilder = new ToTextBuilder (toTextProvider, stringWriter))
      {
        toTextBuilder.s ("Using ToTextBuilder");
      }
      Assert.That (stringWriter.ToString(), Is.EqualTo ("Using ToTextBuilder"));
      // StringWriter should be closed when ToTextBuilder is disposed => Write attempt throws
      stringWriter.Write ("Sorry, we're alredy closed !");
    }


    [Test]
    public void WriteIfNotNull ()
    {
      var toTextBuilder = CreateTextBuilder ();
      Object nullVar = null;
      var someVar = "text";
      toTextBuilder.eIfNotNull (nullVar).eIfNotNull (someVar).e (nullVar);
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo ("textnull"));
    }

    [Test]
    public void WriteIfNotNullWithName ()
    {
      var toTextBuilder = CreateTextBuilder ();
      Object nullVar = null;
      var someVar = "text";
      toTextBuilder.eIfNotNull ("nullVar_NotEmitted", nullVar).eIfNotNull ("someVar", someVar).e ("nullVar", nullVar);
      var result = toTextBuilder.ToString ();
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.DoesNotContain ("NotEmitted"));
    }

    [Test]
    public void WriteIfNotEqualWithName ()
    {
      var toTextBuilder = CreateTextBuilder ();
      Object nullVar = null;
      String s = "ABC";
      bool testBool = false;
      toTextBuilder.sb ().eIfNotEqualTo ("testBoolFalse_NotEmitted", testBool, false).eIfNotEqualTo ("testBoolFalse", testBool, true);
      toTextBuilder.eIfNotEqualTo ("s_NotEmitted", s, "ABC").eIfNotEqualTo ("s", s, "XYZ");
      toTextBuilder.eIfNotEqualTo ("nullVar_NotEmitted", nullVar, null).eIfNotEqualTo ("nullVar", nullVar, s).se();
      var result = toTextBuilder.ToString ();
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.DoesNotContain ("NotEmitted"));
    }

    [Test]
    public void IndentTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      toTextBuilder.IndentationString = "  ";
      toTextBuilder.s ("line0").indent ().nl ().s ("line1").nl ().s ("line2").indent ().nl ().s ("line3").unindent ().nl ().s ("line4").unindent ().nl ().s ("line5");
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo (
@"line0
  line1
  line2
    line3
  line4
line5"));
    }

    [Test]
    [ExpectedException (typeof (Remotion.Utilities.AssertionException), ExpectedMessage = "unindent called without pairing call to indent.")]
    public void IndentErrorTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      toTextBuilder.unindent ();
    }

    [Test]
    public void IndentationStringTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      toTextBuilder.IndentationString = ">}|";
      toTextBuilder.s ("line0").indent ().nl ().s ("line1").nl ().s ("line2").indent ().nl ().s ("line3").unindent ().nl ().s ("line4").unindent ().nl ().s ("line5");
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo (
@"line0
>}|line1
>}|line2
>}|>}|line3
>}|line4
line5"));
    }



    [Test]
    public void ToStringTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      toTextBuilder.s ("test");
      string result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo ("test"));
    }


    [Test]
    public void AutoIndentSequencesOffTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      string result = WriteNestedSequences(toTextBuilder).ToString();
      var resultExpected = @"(1,xyz,(2,3,(ABC{9,8,7,6,5,4,3,2,1},{a,b,c,d,e,f,g},{9,8,7,6,5,4,3,2,1}),xyz,{a,b,c,d,e,f,g},{a,b,c,d,e,f,g}),4,System.Object)";
      Assert.That (result, Is.EqualTo (resultExpected));
    }

    [Test]
    public void AutoIndentSequencesOnTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      toTextBuilder.Settings.AutoIndentSequences = true;
      string result = WriteNestedSequences (toTextBuilder).ToString ();
      var resultExpected = @"
(1,xyz,
  (2,3,
    (ABC
      {9,8,7,6,5,4,3,2,1},
      {a,b,c,d,e,f,g},
      {9,8,7,6,5,4,3,2,1}),xyz,
    {a,b,c,d,e,f,g},
    {a,b,c,d,e,f,g}),4,System.Object)";
      Assert.That (result, Is.EqualTo (resultExpected));
    }

    [Test]
    public void AutoIndentSequencesOnOffTest ()
    {
      var toTextBuilder = CreateTextBuilder ();
      
      toTextBuilder.Settings.AutoIndentSequences = true;
      toTextBuilder.sb().e(1).e(2);
      toTextBuilder.sb ().e (3).e(4);
      toTextBuilder.sb().e ("a").e("b").se ();
      toTextBuilder.sb ().e ("a").e ("b").se ();
      toTextBuilder.e (5).e (6).se ();
      toTextBuilder.Settings.AutoIndentSequences = false;
      toTextBuilder.sb ().e ("not").e ("indented").e("sequence").se ();
      toTextBuilder.nl().e ("not a sequence");
      toTextBuilder.nl ().e ("also not a sequence");
      toTextBuilder.Settings.AutoIndentSequences = true;
      toTextBuilder.sb ().e ("a").e ("b").se ();
      toTextBuilder.se ();
      toTextBuilder.sb().e ("xyz").e("abc").se();

      var result = toTextBuilder.ToString();
      var resultExpected = @"
(1,2,
  (3,4,
    (a,b),
    (a,b),5,6),(not,indented,sequence),
  not a sequence,
  also not a sequence,
  (a,b))
(xyz,abc)";
      Assert.That (result, Is.EqualTo (resultExpected));
    }


    [Test]
    public void AutoIndentSequencesPropertyRestorerTest ()
    {
      var toTextBuilder = CreateTextBuilder ();

      toTextBuilder.Settings.AutoIndentSequences = true;

      using (PropertyRestorerMother.New (toTextBuilder.Settings, x => x.AutoIndentSequences))
      {
        toTextBuilder.Settings.AutoIndentSequences = true;
        toTextBuilder.sb().e (1).e (2);
        toTextBuilder.sb().e (3).e (4);
        toTextBuilder.sb().e ("a").e ("b").se();
        toTextBuilder.sb().e ("a").e ("b").se();
        toTextBuilder.e (5).e (6).se();
        toTextBuilder.Settings.AutoIndentSequences = false;
        toTextBuilder.sb().e ("not").e ("indented").e ("sequence").se();
        toTextBuilder.nl().e ("not a sequence");
        toTextBuilder.nl().e ("also not a sequence");
        toTextBuilder.Settings.AutoIndentSequences = true;
        toTextBuilder.sb().e ("a").e ("b").se();
        toTextBuilder.se();
        toTextBuilder.sb().e ("xyz").e ("abc").se();
        toTextBuilder.Settings.AutoIndentSequences = false;
        Assert.That (toTextBuilder.Settings.AutoIndentSequences,Is.False);
      }

      Assert.That (toTextBuilder.Settings.AutoIndentSequences, Is.True);

      var result = toTextBuilder.ToString ();
      Log (result);
      var resultExpected = @"
(1,2,
  (3,4,
    (a,b),
    (a,b),5,6),(not,indented,sequence),
  not a sequence,
  also not a sequence,
  (a,b))
(xyz,abc)";
      Assert.That (result, Is.EqualTo (resultExpected));
    }



    private ToTextBuilder WriteNestedSequences (ToTextBuilder toTextBuilder)
    {
      var numbersList = ListMother.New (9, 8, 7, 6, 5, 4, 3, 2, 1);
      var stringList = ListMother.New ("a", "b", "c", "d", "e", "f", "g");
      toTextBuilder.sb ().e (1).e ("xyz");
      toTextBuilder.sb().e (2).e (3);
      toTextBuilder.sb ().s ("ABC").e (numbersList).e (stringList).e (numbersList).se ();
      toTextBuilder.e ("xyz");
      toTextBuilder.e (stringList).e (stringList).se ();
      toTextBuilder.e(4).e (new object ()).se ();
      return toTextBuilder;
    }


    public static ToTextBuilder CreateTextBuilder ()
    {
      var toTextProvider = new ToTextProvider();
      toTextProvider.Settings.UseAutomaticObjectToText = false;
      toTextProvider.Settings.UseAutomaticStringEnclosing = false;
      toTextProvider.Settings.UseAutomaticCharEnclosing = false;
      return new ToTextBuilder (toTextProvider);
    }


    public void Log (string s)
    {
      log.It (s);
    }
  }
}

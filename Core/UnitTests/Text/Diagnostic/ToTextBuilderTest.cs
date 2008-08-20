using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Logging;
using Remotion.Text.Diagnostic;

namespace Remotion.UnitTests.Text.Diagnostic
{
  [NUnit.Framework.TestFixture]
  public class ToTextBuilderTest
  {

    [Test]
    public void StringTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      toTextBuilder.s ("ABC defg");
      var result = toTextBuilder.ToString();
      Assert.That (result, Is.EqualTo("ABC defg"));
    }

    [Test]
    public void ToTextStringTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      toTextBuilder.ToTextString ("XyZ !");
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo ("XyZ !"));
    }

    [Test]
    public void sTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      toTextBuilder.s ("A");
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo ("A"));
    }

    [Test]
    public void nlTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      toTextBuilder.nl.s("");
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo (System.Environment.NewLine));
    }

    [Test]
    public void spaceTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      toTextBuilder.space.s ("");
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo (" "));
    }

    [Test]
    public void tabTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      toTextBuilder.tab.s ("");
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo ("\t"));
    }


    [Test]
    public void SeperatorTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      toTextBuilder.seperator.comma.colon.semicolon.s ("");
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo (",,:;"));
    }


    [Test]
    public void StringConcatenationTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      toTextBuilder.s("A").s("B").s("C");
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo ("ABC"));
    }

    [Test]
    public void StringFormattedTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      int i = 123;
      double f = 456.789;
      string s = "Text";
      toTextBuilder.sf ("[{0},{1},{2}]",i,f,s);
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo ("[123,456,789,Text]"));
    }


    [Test]
    public void MultiTest ()
    {
      int i = 987654321;
      double f = 3.14;
      string s = "Text";

      var toTextBuilder = GetTextBuilder ();
      toTextBuilder.s("START-").nl.tab.sf ("[{0};{1};{2}]", i, f, s).space.s("-END");
      var result = toTextBuilder.ToString ();
      s_log.Info(result);
      Assert.That (result, Is.EqualTo ("START-" + Environment.NewLine + "\t[987654321;3,14;Text] -END"));
    }


    [Test]
    public void tsTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      var o = new Object();
      toTextBuilder.ts (o);
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo (o.ToString()));
    }


    [Test]
    public void ToTextTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      var o = new Object ();
      toTextBuilder.tt (o);
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo (o.ToString ()));
    }

    [Test]
    public void MemberTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      var o = new Object ();
      toTextBuilder.m (o);
      var result = toTextBuilder.ToString ();
      Assert.That (result, Is.EqualTo (o.ToString ()));
    }

    [Test]
    public void MemberTest2 ()
    {
      var toTextBuilder = GetTextBuilder ();
      //var list = new List<int> () { 5, 3, 1 };
      var myList = New.List (5, 3, 1);
      toTextBuilder.m ("myList", myList);
      var result = toTextBuilder.ToString ();
      //Assert.That (result, Is.EqualTo ("myList:{5,3,1}"));
      Assert.That (result, Is.EqualTo (" myList={5,3,1} "));
    }

    [Test]
    public void AppendCollectionTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      var list = New.List (New.List (5, 3, 1), New.List(11,13,17));
      toTextBuilder.AppendEnumerable (list);
      var result = toTextBuilder.ToString ();
      Log(result);
      Assert.That (result, Is.EqualTo ("{{5,3,1},{11,13,17}}"));
    }

    [Test]
    public void collectionTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      var list = New.List(New.List(New.List("A", "B", "C")));
      toTextBuilder.collection (list);
      var result = toTextBuilder.ToString ();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{{A,B,C}}}"));
    }

    [Test]
    public void AppendArray2DTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      //var array = new int[][] {new int[] {1,2},new int[] {3,4},new int[] {5,6}};
      var array = New.Array (New.Array(1,2),New.Array(3,4),New.Array(5,6));
      toTextBuilder.AppendArray (array);
      var result = toTextBuilder.ToString ();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{1,2},{3,4},{5,6}}"));
    }

    [Test]
    public void AppendArray3DTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      var array = New.Array (New.Array (New.Array (1, 3), New.Array (5, 7)), New.Array (New.Array (11, 13), New.Array (17, 19)), New.Array (New.Array (23, 29), New.Array (31, 37)));
      toTextBuilder.AppendArray (array);
      var result = toTextBuilder.ToString ();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{{1,3},{5,7}},{{11,13},{17,19}},{{23,29},{31,37}}}"));
    }


    [Test]
    public void arrayTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      var list = New.List (New.List (New.List ("A", "B", "C")));
      toTextBuilder.collection (list);
      var result = toTextBuilder.ToString ();
      Log (result);
      Assert.That (result, Is.EqualTo ("{{{A,B,C}}}"));
    }


    [Test]
    public void EnumerableTagTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      var list = New.List (New.List (5, 3, 1), New.List (11, 13, 17));
      toTextBuilder.EnumerableBegin = "(";
      toTextBuilder.EnumerableSeparator = ";";
      toTextBuilder.EnumerableEnd = ")";
      toTextBuilder.ArrayBegin = "X";
      toTextBuilder.ArraySeparator = "Y";
      toTextBuilder.ArrayEnd = "Z";
      
      toTextBuilder.AppendEnumerable (list);
      var result = toTextBuilder.ToString ();
      Log (result);
      Assert.That (result, Is.EqualTo ("((5;3;1);(11;13;17))"));
    }

    [Test]
    public void ArrayTagTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      var array = New.Array (New.Array (New.Array (1, 3), New.Array (5, 7)), New.Array (New.Array (11, 13), New.Array (17, 19)), New.Array (New.Array (23, 29), New.Array (31, 37)));
      toTextBuilder.ArrayBegin = "<{[";
      toTextBuilder.ArraySeparator = "§|§";
      toTextBuilder.ArrayEnd = "]}>";
      toTextBuilder.EnumerableBegin = "X";
      toTextBuilder.EnumerableSeparator = "Y";
      toTextBuilder.EnumerableEnd = "Z";

      toTextBuilder.AppendArray (array);
      var result = toTextBuilder.ToString ();
      Log (result);
      Assert.That (result, Is.EqualTo ("<{[<{[<{[1§|§3]}>§|§<{[5§|§7]}>]}>§|§<{[<{[11§|§13]}>§|§<{[17§|§19]}>]}>§|§<{[<{[23§|§29]}>§|§<{[31§|§37]}>]}>]}>"));
    }



    [Test] public void SinglelineMultilineTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      toTextBuilder.UseMultiLine = false;
      toTextBuilder.s("Hello").nl.s (" world");
      var result = toTextBuilder.ToString ();
      Log(result);
      Assert.That (result, Is.EqualTo ("Hello world"));

      toTextBuilder.UseMultiLine = true;
      toTextBuilder.s (" here comes the").nl.s ("newline");
      var result2 = toTextBuilder.ToString ();
      Log (result2);
      Assert.That (result2, Is.EqualTo ("Hello world here comes the" + Environment.NewLine + "newline"));
    }

    [Test]
    public void EnabledTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      toTextBuilder.s ("test");
      Assert.That (toTextBuilder.ToString (), Is.EqualTo ("test"));
      toTextBuilder.Enabled = false;
      toTextBuilder.s ("test");
      Assert.That (toTextBuilder.ToString (), Is.EqualTo ("test"));
      toTextBuilder.Enabled = true;
      toTextBuilder.s ("test");
      Assert.That (toTextBuilder.ToString (), Is.EqualTo ("testtest"));
    }

    [Test]
    public void OutputComplexityTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      toTextBuilder.OutputComplex();
      Assert.That (toTextBuilder.OutputComplexity, Is.EqualTo (ToTextBuilder.OutputComplexityLevel.Complex));
    }

    [Test]
    public void ComplexityFilteringSettingTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      Assert.That (toTextBuilder.Enabled, Is.EqualTo (true));
      toTextBuilder.OutputSkeleton();
      toTextBuilder.AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo(ToTextBuilder.OutputComplexityLevel.Basic);
      Assert.That (toTextBuilder.Enabled, Is.EqualTo (false));

      toTextBuilder.OutputBasic();
      toTextBuilder.AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilder.OutputComplexityLevel.Basic);
      Assert.That (toTextBuilder.Enabled, Is.EqualTo (true));
    }


    public ToTextBuilder AllFilterLevelsFilteredOutput (ToTextBuilder toTextBuilder)
    {
      if (toTextBuilder == null)
      {
        toTextBuilder = GetTextBuilder ();
      }
      toTextBuilder.cBasic.s ("b").comma.cComplex.s ("c").comma.cFull.s ("f").comma.cMedium.s ("m").comma.cSkeleton.s ("s");
      var result = toTextBuilder.ToString ();
      Log (result);
      return toTextBuilder;
    }


    [Test]
    public void ComplexityFilteringTest ()
    {
      {
        var toTextBuilder = GetTextBuilder ();
        toTextBuilder.OutputDisable();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString ();
        Assert.That (result, Is.EqualTo (""));
      }

      {
        var toTextBuilder = GetTextBuilder ();
        toTextBuilder.OutputSkeleton ();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString ();
        Assert.That (result, Is.EqualTo ("s"));
      }

      {
        var toTextBuilder = GetTextBuilder ();
        toTextBuilder.OutputBasic(); 
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString ();
        Assert.That (result, Is.EqualTo ("b,s"));
      }

      {
        var toTextBuilder = GetTextBuilder ();
        toTextBuilder.OutputMedium(); 
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString ();
        Assert.That (result, Is.EqualTo ("b,m,s"));
      }

      {
        var toTextBuilder = GetTextBuilder ();
        toTextBuilder.OutputComplex();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString ();
        Assert.That (result, Is.EqualTo ("b,c,m,s"));
      }

      {
        var toTextBuilder = GetTextBuilder ();
        toTextBuilder.OutputFull();
        var result = AllFilterLevelsFilteredOutput (toTextBuilder).ToString ();
        Assert.That (result, Is.EqualTo ("b,c,f,m,s"));
      }
    }


    [Test]
    public void AppendObjectTest ()
    {
      var toTextBuilder = GetTextBuilder ();
      var o = new Object();
      Assert.That (toTextBuilder.Append (o).ToString (), Is.EqualTo (o.ToString()));
    }


    public static ToTextBuilder GetTextBuilder ()
    {
      var toTextProvider = new ToTextProvider();
      toTextProvider.UseAutomaticObjectToText = false;
      toTextProvider.UseAutomaticStringEnclosing = false;
      toTextProvider.UseAutomaticCharEnclosing = false;
      return new ToTextBuilder (toTextProvider);
    }



    // Logging
    private static readonly ILog s_log = LogManager.GetLogger (typeof(ToTextBuilderTest));
    static ToTextBuilderTest() { LogManager.InitializeConsole (); }
    public static void Log (string s) { s_log.Info (s); }

  }

}